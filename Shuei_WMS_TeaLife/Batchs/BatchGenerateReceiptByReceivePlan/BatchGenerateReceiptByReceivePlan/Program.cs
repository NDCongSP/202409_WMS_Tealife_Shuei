using BatchGenerateReceiptByReceivePlan.Helpers;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

class Program
{
    private static string domain = string.Empty;
    private static string userName = string.Empty;
    private static string password = string.Empty;
    private static string tenantIds = string.Empty;
    static async Task Main(string[] args)
    {
        // Build the configuration
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        domain = configuration["ApiDomainConfig"] ?? "http://133.167.47.242:9500"; // Set domain from configuration
        userName = configuration["UserName"];
        password = configuration["Password"];
        //tenantIds = configuration["TenantIds"];
        if (args.Length > 0)
        {
            string input = args[0].Trim();
            if (input.StartsWith("tenants:"))
            {
                tenantIds = input.Replace(" ", "").Substring(8);
            }
        }
        Console.WriteLine("Apply for tenants: " + tenantIds);

        Console.WriteLine("-----------------");
        var checkCreateReceiptPlan = await GenerateReceiptFromReceiptPlan();

        Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] CHECK RECEIPT PLAN RESULT: SUCCESS {checkCreateReceiptPlan.Item1}/{checkCreateReceiptPlan.Item2}");
        LogHelpers.LogFile("GENERATE_PLAN", $"CHECK RECEIPT PLAN RESULT: SUCCESS {checkCreateReceiptPlan.Item1}/{checkCreateReceiptPlan.Item2}");

        Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] FINISH CHECK RECEIPT PLAN");
        LogHelpers.LogFile("GENERATE_PLAN", $"FINISH CHECK RECEIPT PLAN");

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }

    static async Task<Tuple<int, int>> GenerateReceiptFromReceiptPlan()
    {
        try
        {
            var client = new HttpClient();
            var token = await GetAuthentication();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{domain}/api/WarehouseReceiptOrder/generate-receipt-from-receipt-plan/{tenantIds}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Tuple<int, int>>();
                return result;
            }
            else
            {
                // Handle error response
                LogHelpers.LogFile("GENERATE_PLAN", $"ERROR: {response.ReasonPhrase}");
                return new Tuple<int, int>(0, 0);
            }
        }
        catch (Exception ex)
        {
            LogHelpers.LogFile("GENERATE_PLAN", $"ERROR: {ex.Message}");
            return new Tuple<int, int>(0, 0);
        }
    }

    public static async Task<string> GetAuthentication()
    {
        var client = new HttpClient();
        var requestUri = $"{domain}/api/Account/identity/loginasync";

        var requestBody = new
        {
            emailAddress = userName,
            password = password,
            remember = false
        };

        var response = await client.PostAsJsonAsync(requestUri, requestBody);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        if (result.flag == true)
        {
            return result.token;
        }
        else
        {
            LogHelpers.LogFile("AUTHENTICATION", $"ERROR: {result.message}");
            return null;
        }
    }
    public class AuthenticationResponse
    {
        public bool flag { get; set; }
        public string message { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }
        public string expiration { get; set; }
    }
}
