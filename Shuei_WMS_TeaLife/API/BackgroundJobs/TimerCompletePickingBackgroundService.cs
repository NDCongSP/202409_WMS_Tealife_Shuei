using Application.Services.Outbound;
using Infrastructure.Extensions;

namespace API.BackgroundJobs
{
    public class TimerCompletePickingBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PeriodicTimer _timer;

        public TimerCompletePickingBackgroundService(IServiceProvider serviceProvider)
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

                    LogHelpers.LogFile("COMPLETE_PICKING", $"START COMPLETE PICKING CHECK: {now:dd/MM/yyyy HH:mm:ss}");
                    Console.WriteLine($"START COMPLETE PICKING CHECK: {now:dd/MM/yyyy HH:mm:ss}");

                    var result = await CompletedPickings();

                    LogHelpers.LogFile("COMPLETE_PICKING", $"COMPLETE PICKING RESULT: {(result ? "SUCCESS" : "FAILED")}");
                    LogHelpers.LogFile("COMPLETE_PICKING", $"NEXT CHECK TIME: {nextRun:dd/MM/yyyy HH:mm:ss}");
                    LogHelpers.LogFile("COMPLETE_PICKING", "----------------------------");

                    Console.WriteLine($"COMPLETE PICKING RESULT: {(result ? "SUCCESS" : "FAILED")}");
                    Console.WriteLine($"NEXT CHECK TIME: {nextRun:dd/MM/yyyy HH:mm:ss}");
                    Console.WriteLine("----------------------------");
                }
            }
            catch (Exception ex)
            {
                LogHelpers.LogFile("COMPLETE_PICKING", $"Background service error: {ex.Message}");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            LogHelpers.LogFile("COMPLETE_PICKING", $"FINISH COMPLETE_PICKING");
            await base.StopAsync(cancellationToken);
        }

        private async Task<bool> CompletedPickings()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IWarehousePickingList>();
                    var result = await service.AutoCompletePickingsAsync();
                    if (!result.Succeeded)
                    {
                        LogHelpers.LogFile("COMPLETE_PICKING", $"Failed: {string.Join(',', result.Messages)}");
                    }
                    return result.Succeeded;
                }
            }
            catch (Exception ex)
            {
                LogHelpers.LogFile("COMPLETE_PICKING", $"ERROR: {ex.Message}");
                return false;
            }
        }
    }
}
