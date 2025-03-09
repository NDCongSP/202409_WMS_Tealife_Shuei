
using ClosedXML.Parser;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.BackgroundJobs
{
    public class TimedBackgroundService : BackgroundService
    {
        private readonly ILogger<TimedBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public TimedBackgroundService(ILogger<TimedBackgroundService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background task is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    // Thực hiện tác vụ với DbContext hoặc các service khác

                    var res = await dbContext.Units.FirstOrDefaultAsync();
                    if (res == null) return;
                    Console.WriteLine("Background task is running at: {0} | Connect DbContext {1}", DateTimeOffset.Now,res.UnitName);
                }

                await Task.Delay(10000, stoppingToken); // Chạy mỗi 10 giây
            }

            _logger.LogInformation("Background service is stopping.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background service is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
