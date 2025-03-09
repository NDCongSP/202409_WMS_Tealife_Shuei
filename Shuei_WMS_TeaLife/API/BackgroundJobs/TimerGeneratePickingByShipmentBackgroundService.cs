using Application.Services.Inbound;
using Application.Services.Outbound;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

namespace API.BackgroundJobs
{
    public class TimerGeneratePickingByShipmentBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TimerGeneratePickingByShipmentBackgroundService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                //var nextRun = now.AddSeconds(30);
                //var nextRun = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0);
                var nextRun = now.AddMinutes(5);
                var delay = nextRun - now;
                LogHelpers.LogFile("GENERATE_PICKING", $"NEXT TIME CHECK: {nextRun.ToString("dd/MM/yyyy HH:mm:ss")}");
                LogHelpers.LogFile("GENERATE_PICKING", "----------------------------");

                Console.WriteLine($"NEXT TIME CHECK: {nextRun.ToString("dd/MM/yyyy HH:mm:ss")}");
                Console.WriteLine("----------------------------");
                await Task.Delay(delay, stoppingToken);

                Console.WriteLine("GENERATE_PICKING", $"START CHECK GENERATE PICKING");
                LogHelpers.LogFile("GENERATE_PICKING", $"START CHECK GENERATE PICKING");
                //var checkGenerateShipment = await GeneratePickingByShipment();

                //Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] CHECK GENERATE PICKING RESULT: SUCCESS {checkGenerateShipment.Item2}/{checkGenerateShipment.Item3}");
                //LogHelpers.LogFile("GENERATE_PICKING", $"CHECK GENERATE PICKING RESULT: SUCCESS {checkGenerateShipment.Item2}/{checkGenerateShipment.Item3}");
                
                //Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] FINISH CHECK GENERATE PICKING");
                //LogHelpers.LogFile("GENERATE_PICKING", $"FINISH CHECK GENERATE PICKING");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            LogHelpers.LogFile("GENERATE_PICKING", $"FINISH CHECK GENERATE PICKING");
            return base.StopAsync(cancellationToken);
        }

        //public async Task<Tuple<bool, int, int>> GeneratePickingByShipment()
        //{
        //    try
        //    {
        //        using (var scope = _serviceProvider.CreateScope())
        //        {
        //            var service = scope.ServiceProvider.GetRequiredService<IWarehouseShipment>();
        //            return await service.CreateAutoPickingAsync("Auto-Created");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelpers.LogFile("GENERATE_PICKING", $"ERROR: {ex.Message}");
        //        return new Tuple<bool, int, int>(false, 0, 0);
        //    }
        //}
    }
}
