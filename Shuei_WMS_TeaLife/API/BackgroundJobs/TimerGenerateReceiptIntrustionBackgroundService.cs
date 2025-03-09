//using Application.Services.Inbound;
//using Application.Services.Outbound;
//using Infrastructure.Data;
//using Infrastructure.Extensions;
//using Microsoft.CodeAnalysis;
//using Microsoft.EntityFrameworkCore;
//using MoreLinq;

//namespace API.BackgroundJobs
//{
//    public class TimerGenerateReceiptIntrustionBackgroundService : BackgroundService
//    {
//        private readonly IServiceProvider _serviceProvider;
//        public TimerGenerateReceiptIntrustionBackgroundService(IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//        }
//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {

//            while (!stoppingToken.IsCancellationRequested)
//            {
//                var now = DateTime.Now;
//                var nextRun = GetNextRunTime(now);
//                var delay = nextRun - now;
//                LogHelpers.LogFile("GENERATE_PLAN", $"NEXT TIME CHECK: {nextRun.ToString("dd/MM/yyyy HH:ss")}");
//                LogHelpers.LogFile("GENERATE_PLAN", "----------------------------");
//                await Task.Delay(delay, stoppingToken);
//                LogHelpers.LogFile("GENERATE_PLAN", $"START CHECK RECEIPT PLAN");
//                var checkCreateReceiptPlan = await GenerateReceiptFromReceiptPlan();
//                LogHelpers.LogFile("GENERATE_PLAN", $"CHECK RECEIPT PLAN RESULT: SUCCESS {checkCreateReceiptPlan.Item1}/{checkCreateReceiptPlan.Item2}");
//                LogHelpers.LogFile("GENERATE_PLAN", $"FINISH CHECK RECEIPT PLAN");
//            }
//        }

//        private DateTime GetNextRunTime(DateTime currentTime)
//        {
//            var targetHours = new[] { 0, 6, 12, 18 };

//            var nextHour = targetHours.FirstOrDefault(h => h > currentTime.Hour);
//            if (nextHour == 0) // If no hours left today, get first hour tomorrow
//            {
//                return currentTime.Date.AddDays(1).AddHours(targetHours[0]);
//            }

//            return currentTime.Date.AddHours(nextHour);
//        }

//        public override Task StopAsync(CancellationToken cancellationToken)
//        {
//            LogHelpers.LogFile("GENERATE_PLAN", $"FINISH CHECK RECEIPT PLAN");
//            return base.StopAsync(cancellationToken);
//        }

//        public async Task<Tuple<int, int>> GenerateReceiptFromReceiptPlan()
//        {
//            try
//            {
//                var Configuration = new ConfigurationBuilder()
//                    .SetBasePath(Directory.GetCurrentDirectory())
//                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                    .Build();
//                string domain = Configuration.GetValue<string>("ApiDomainConfig");
//                var request = new HttpRequestMessage(HttpMethod.Post, $"{domain}/api/WarehouseReceiptOrder/generate-receipt-from-receipt-plan");
//                var client = new HttpClient();
//                var response = await client.SendAsync(request);
//                response.EnsureSuccessStatusCode();
//                if (response.IsSuccessStatusCode)
//                {
//                    var result = await response.Content.ReadFromJsonAsync<Tuple<int, int>>();
//                    return result;
//                }
//                else
//                {
//                    // Handle error response
//                    LogHelpers.LogFile("GENERATE_PLAN", $"ERROR: {response.ReasonPhrase}");
//                    return new Tuple<int, int>(0, 0);
//                }
//                using (var scope = _serviceProvider.CreateScope())
//                {
//                    var service = scope.ServiceProvider.GetRequiredService<IWarehouseReceiptOrder>();
//                    return await service.GenerateReceiptFromReceiptPlan();
//                }
//            }
//            catch (Exception ex)
//            {
//                LogHelpers.LogFile("GENERATE_PLAN", $"ERROR: {ex.Message}");
//                return new Tuple<int, int>(0, 0);
//            }
//        }
//    }
//}
