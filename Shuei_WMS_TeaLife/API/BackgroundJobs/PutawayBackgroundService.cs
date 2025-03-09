using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Services.Inbound;
using Infrastructure.Data;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Infrastructure.Extensions;
using Mapster;
using Application.Extentions;
using DocumentFormat.OpenXml.Spreadsheet;

namespace API.BackgroundJobs
{
    public class PutawayBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PeriodicTimer _timer;

        public PutawayBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _timer = new PeriodicTimer(TimeSpan.FromMinutes(GlobalVariable.BackgroundJobInterval));
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"FINISH COMPLETE_PUTAWAYS");
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(stoppingToken))
                {
                    var now = DateTime.Now;

                    var nextRun = now.AddMinutes(GlobalVariable.BackgroundJobInterval);

                    LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"START COMPLETE PUTAWAYS CHECK: {now:dd/MM/yyyy HH:mm:ss}");
                    Console.WriteLine($"START COMPLETE PUTAWAYS CHECK: {now:dd/MM/yyyy HH:mm:ss}");

                    await ProcessPutawaysAsync();

                    LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"COMPLETE PUTAWAYS RESULT: {("SUCCESS", "FAILED")}");
                    LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"NEXT CHECK TIME: {nextRun:dd/MM/yyyy HH:mm:ss}");
                    LogHelpers.LogFile("COMPLETE_PUTAWAYS", "----------------------------");

                    Console.WriteLine($"COMPLETE PUTAWAYS RESULT: {("SUCCESS", "FAILED")}");
                    Console.WriteLine($"NEXT CHECK TIME: {nextRun:dd/MM/yyyy HH:mm:ss}");
                    Console.WriteLine("----------------------------");
                }
            }
            catch (Exception ex)
            {
                LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"Background service error: {ex.Message}");
                throw;
            }
        }

        private async Task ProcessPutawaysAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var putAwayService = scope.ServiceProvider.GetRequiredService<IWarehousePutAway>();

                var putaways = await dbContext.WarehousePutAways
                    .Where(p => p.HHTStatus == EnumHHTStatus.Done && p.Status != EnumPutAwayStatus.Completed)
                    .ToListAsync();



                foreach (var putaway in putaways)
                {

                    using var transaction = await dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var putawayDto = putaway.Adapt<WarehousePutAwayDto>();
                        putawayDto.WarehousePutAwayLines = (
                            from putawayLines in dbContext.WarehousePutAwayLines.Where(_ => _.PutAwayNo == putaway.PutAwayNo)
                            join product in dbContext.Products on putawayLines.ProductCode equals product.ProductCode into products
                            from product in products.DefaultIfEmpty()
                            select new WarehousePutAwayLineDto
                            {
                                Id = putawayLines.Id,
                                PutAwayNo = putawayLines.PutAwayNo,
                                TenantId = putawayLines.TenantId,
                                ProductCode = putawayLines.ProductCode,
                                UnitId = product.UnitId,
                                JournalQty = putawayLines.JournalQty,
                                TransQty = putawayLines.TransQty,
                                Bin = putawayLines.Bin,
                                LotNo = putawayLines.LotNo,
                                ExpirationDate = putawayLines.ExpirationDate,
                                ReceiptLineId = putawayLines.ReceiptLineId,
                                CreateAt = putawayLines.CreateAt,
                                CreateOperatorId = putawayLines.CreateOperatorId,
                                UpdateAt = putawayLines.UpdateAt,
                                UpdateOperatorId = putawayLines.UpdateOperatorId

                            }).ToList();
                        // Call SyncHTData
                        var syncResult = await SyncHTData(putAwayService, putawayDto);
                        if (!syncResult.Succeeded)
                        {
                            LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"Failed to sync HT data for putaway {putaway.PutAwayNo}: {string.Join(", ", syncResult.Messages)}");
                            continue;
                        }
                        await dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                       
                        // Call AdjustActionPutAway
                        var adjustResult = await AdjustActionPutAway(putAwayService, putawayDto);
                        if (!adjustResult.Succeeded)
                        {
                            LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"Failed to adjust putaway {putaway.PutAwayNo}: {string.Join(", ", adjustResult.Messages)}");
                            continue;
                        }
                        await dbContext.WarehousePutAwayStagings.Where(_ => _.PutAwayNo == putaway.PutAwayNo).ExecuteDeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"Transaction error: {ex.Message}");
                    }


                }
                   
                   
               
            }
        }

        private async Task<Result> SyncHTData(IWarehousePutAway putAwayService, WarehousePutAwayDto putawayDto)
        {
            // Implement the logic for synchronizing HT data
            return await putAwayService.SyncHTData(putawayDto);
        }

        private async Task<Result> AdjustActionPutAway(IWarehousePutAway putAwayService, WarehousePutAwayDto putawayDto)
        {
            // Implement the logic for adjusting the putaway
            return await putAwayService.AdjustActionPutAway(putawayDto);
        }
    }
}