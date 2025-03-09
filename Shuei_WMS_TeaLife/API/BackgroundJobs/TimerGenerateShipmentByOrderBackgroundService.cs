//using Application.Services.Outbound;
//using Infrastructure.Extensions;

//namespace API.BackgroundJobs
//{
//    public class TimerGenerateShipmentByOrderBackgroundService : BackgroundService
//    {
//        private readonly IServiceProvider _serviceProvider;

//        public TimerGenerateShipmentByOrderBackgroundService(
//            IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                var now = DateTime.Now;
//                //var nextRun = now.AddSeconds(30);
//                //var nextRun = new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0);
//                var nextRun = now.Hour == 23
//                                ? new DateTime(now.Year, now.Month, now.Day + 1, 0, 0, 0)
//                                : new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0);
//                var delay = nextRun - now;
//                LogHelpers.LogFile("GENERATE_SHIPMENT", $"NEXT TIME CHECK: {nextRun.ToString("dd/MM/yyyy HH:mm:ss")}");
//                LogHelpers.LogFile("GENERATE_SHIPMENT", "----------------------------");

//                Console.WriteLine($"NEXT TIME CHECK: {nextRun.ToString("dd/MM/yyyy HH:mm:ss")}");
//                Console.WriteLine("----------------------------");
//                await Task.Delay(delay, stoppingToken);

//                Console.WriteLine("GENERATE_SHIPMENT", $"START CHECK ORDER DISPATCHES");
//                LogHelpers.LogFile("GENERATE_SHIPMENT", $"START CHECK ORDER DISPATCHES");
//                var checkGenerateShipment = await GenerateShipmentFromOrder();

//                Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] CHECK ORDER DISPATCHES RESULT: SUCCESS {checkGenerateShipment.Item1}/{checkGenerateShipment.Item2}");
//                LogHelpers.LogFile("GENERATE_SHIPMENT", $"CHECK ORDER DISPATCHES RESULT: SUCCESS {checkGenerateShipment.Item1}/{checkGenerateShipment.Item2}");
//                foreach (var log in checkGenerateShipment.Item3)
//                {
//                    Console.WriteLine($"Dispatch ID {log.Item1}: {log.Item2}");
//                    LogHelpers.LogFile("GENERATE_SHIPMENT", $"Dispatch ID {log.Item1}: {log.Item2}");
//                }
//                Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] FINISH CHECK ORDER DISPATCHES");
//                LogHelpers.LogFile("GENERATE_SHIPMENT", $"FINISH CHECK ORDER DISPATCHES");
//            }
//        }

//        public override Task StopAsync(CancellationToken cancellationToken)
//        {
//            LogHelpers.LogFile("GENERATE_SHIPMENT", $"FINISH CHECK ORDER DISPATCHES");
//            return base.StopAsync(cancellationToken);
//        }

//        public async Task<Tuple<int, int, List<Tuple<string, string>>>> GenerateShipmentFromOrder()
//        {
//            try
//            {
//                var Configuration = new ConfigurationBuilder()
//                    .SetBasePath(Directory.GetCurrentDirectory())
//                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                    .Build();
//                string domain = Configuration.GetValue<string>("ApiDomainConfig");
//                var client = new HttpClient();
//                var request = new HttpRequestMessage(HttpMethod.Post, $"{domain}/api/WarehouseShipment/generate-shipment-from-order");
//                var response = await client.SendAsync(request);
//                response.EnsureSuccessStatusCode();
//                if (response.IsSuccessStatusCode)
//                {
//                    var result = await response.Content.ReadFromJsonAsync<Tuple<int, int, List<Tuple<string, string>>>>();
//                    return result;
//                }
//                else
//                {
//                    // Handle error response
//                    LogHelpers.LogFile("GENERATE_SHIPMENT", $"ERROR: {response.ReasonPhrase}");
//                    return new Tuple<int, int, List<Tuple<string, string>>>(0, 0, new List<Tuple<string, string>>());
//                }
//                using (var scope = _serviceProvider.CreateScope())
//                {
//                    var service = scope.ServiceProvider.GetRequiredService<IWarehouseShipment>();
//                    return await service.GenerateShipmentFromOrder();
//                }
//            }
//            catch (Exception ex)
//            {
//                LogHelpers.LogFile("GENERATE_SHIPMENT", $"ERROR: {ex.Message}");
//                return new Tuple<int, int, List<Tuple<string, string>>>(0, 0, new List<Tuple<string, string>>());
//            }
//        }
//    }
//}
