using GeneratePicking.Helpers;
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
            .SetBasePath(AppContext.BaseDirectory) // Đặt thư mục cơ sở
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Thêm file JSON
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
        Console.WriteLine("GENERATE_PICKING", $"START CHECK GENERATE PICKING");
        Console.WriteLine($"DOMAIN: {domain}");
        LogHelpers.LogFile("GENERATE_PICKING", $"START CHECK GENERATE PICKING");
        var checkGeneratePicking = await GeneratePickingByShipment();

        Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] CHECK GENERATE PICKING RESULT: SUCCESS {checkGeneratePicking.Item2}/{checkGeneratePicking.Item3}");
        LogHelpers.LogFile("GENERATE_PICKING", $"CHECK GENERATE PICKING RESULT: SUCCESS {checkGeneratePicking.Item2}/{checkGeneratePicking.Item3}");

        Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] FINISH CHECK GENERATE PICKING");
        LogHelpers.LogFile("GENERATE_PICKING", $"FINISH CHECK GENERATE PICKING");
        Console.WriteLine();
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }

    public static async Task<Tuple<bool, int, int>> GeneratePickingByShipment()
    {
        try
        {
            var client = new HttpClient();
            var token = await GetAuthentication();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{domain}/api/WarehouseShipment/AutoCreatePickingAsync/Batch-Script/{tenantIds}");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Tuple<bool, int, int>>();
                return result;
            }
            else
            {
                // Handle error response
                LogHelpers.LogFile("GENERATE_PICKING", $"ERROR: {response.ReasonPhrase}");
                return new Tuple<bool, int, int>(false, 0, 0);
            }
        }
        catch (Exception ex)
        {
            LogHelpers.LogFile("GENERATE_PICKING", $"ERROR: {ex.Message}");
        }
        return new Tuple<bool, int, int>(false, 0, 0);
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
