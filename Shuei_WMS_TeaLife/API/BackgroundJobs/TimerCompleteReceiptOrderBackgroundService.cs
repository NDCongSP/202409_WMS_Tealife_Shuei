using Application.Services.Inbound;
using Infrastructure.Extensions;

namespace API.BackgroundJobs
{
    public class TimerCompleteReceiptOrderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PeriodicTimer _timer;

        public TimerCompleteReceiptOrderBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _timer = new PeriodicTimer(TimeSpan.FromMinutes(GlobalVariable.BackgroundJobInterval));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(stoppingToken))
                {
                    var now = DateTime.Now;

                    var nextRun = now.AddMinutes(GlobalVariable.BackgroundJobInterval);

                    LogHelpers.LogFile("COMPLETE_RECEIPT", $"START COMPLETE RECEIPT ORDER CHECK: {now:dd/MM/yyyy HH:mm:ss}");
                    Console.WriteLine($"START COMPLETE RECEIPT ORDER CHECK: {now:dd/MM/yyyy HH:mm:ss}");

                    //var result = await CompleteReceiptOrder();

                    //LogHelpers.LogFile("COMPLETE_RECEIPT", $"COMPLETE RECEIPT ORDER RESULT: {(result ? "SUCCESS" : "FAILED")}");
                    //LogHelpers.LogFile("COMPLETE_RECEIPT", $"NEXT CHECK TIME: {nextRun:dd/MM/yyyy HH:mm:ss}");
                    //LogHelpers.LogFile("COMPLETE_RECEIPT", "----------------------------");

                    //Console.WriteLine($"COMPLETE RECEIPT ORDER RESULT: {(result ? "SUCCESS" : "FAILED")}");
                    //Console.WriteLine($"NEXT CHECK TIME: {nextRun:dd/MM/yyyy HH:mm:ss}");
                    //Console.WriteLine("----------------------------");
                }
            }
            catch (Exception ex)
            {
                LogHelpers.LogFile("COMPLETE_RECEIPT", $"Background service error: {ex.Message}");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            LogHelpers.LogFile("COMPLETE_RECEIPT", $"FINISH COMPLETE_RECEIPT");
            await base.StopAsync(cancellationToken);
        }

        //private async Task<bool> CompleteReceiptOrder()
        //{
        //    try
        //    {
        //        using (var scope = _serviceProvider.CreateScope())
        //        {
        //            var service = scope.ServiceProvider.GetRequiredService<IWarehouseReceiptOrder>();
                    //return await service.CompleteAndPutAwayReceipts();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelpers.LogFile("COMPLETE_RECEIPT", $"ERROR: {ex.Message}");
        //        return false;
        //    }
        //}
    }
}
