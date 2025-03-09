using Application.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RestEase.HttpClientFactory;
using System.Security;
using UITestAPI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var config = builder.Configuration;

builder.Services.AddHttpClient("BlazorJWTApp.ServerAPI", client => client.BaseAddress = new Uri(config["AppSettings:ApiBaseUrl"]!))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<AuthorizationMessageHandler>()
        .ConfigureHandler(authorizedUrls: new[] {config["AppSettings:ApiBaseUrl"]! }));

// Register services for authentication and authorization
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorJWTApp.ServerAPI"));
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

//builder.Services.AddHttpClient("API")
//    .ConfigureHttpClient(x => x.BaseAddress = new Uri(config["AppSettings:ApiBaseUrl"]!))
//    //.AddHttpMessageHandler(sp =>
//    //{
//    //    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
//    //    handler.ConfigureHandler(["/login"]);
//    //    return handler;
//    //})
//    .UseWithRestEaseClient<IVendors>();


await builder.Build().RunAsync();
