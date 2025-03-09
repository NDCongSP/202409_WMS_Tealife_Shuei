using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using RestEase.HttpClientFactory;
using System.Globalization;
using WebUI;
using RestEase;

using Application.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Application.Services.Authen.UI;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Application.Services.Authen;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var config = builder.Configuration;

#region BreadCumb initial
//GlobalVariable.BreadCrumbDataMaster.Add(new BreadCrumbModel()
//{
//    Path = "User Manager",
//    Text= "Users List|"
//});
#endregion

builder.Services.AddRadzenComponents();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthServices, AuthServices>();

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>()
    .AddScoped(sp => (ApiAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>())
    .AddScoped(sp => (IAccessTokenProvider)sp.GetRequiredService<AuthenticationStateProvider>())
    .AddScoped<IAccessTokenProviderAccessor, AccessTokenProviderAccessor>()
    .AddScoped<AuthenticationHeaderHandler>();

builder.Services.AddScoped<IHttpInterceptorManager, HttpInterceptorManager>();

builder.Services.AddAuthorizationCore(b =>
{
    b.AddPolicy("Admin", p =>
    {
        p.RequireRole(["Admin","SuperAdmmin"]);
        p.RequireClaim("ABC", "1");
    });
});

builder.Services.AddCascadingAuthenticationState();

//Register client and services use RestEase library
// Register the RestEase client
builder.Services.AddHttpClient("API")
    .ConfigureHttpClient(x => x.BaseAddress = new Uri(config["AppSettings:ApiBaseUrl"]!))
    //.AddHttpMessageHandler(sp =>
    //{
    //    var handler = sp.GetRequiredService<AuthorizationMessageHandler>();
    //    handler.ConfigureHandler(["/login"]);
    //    return handler;
    //})
    .AddHttpMessageHandler<AuthenticationHeaderHandler>()
    .UseWithRestEaseClient<IProducts>()
    .UseWithRestEaseClient<IPermissions>()
    .UseWithRestEaseClient<IPermissionTenant>()
    .UseWithRestEaseClient<IRoleToPermissions>()
    .UseWithRestEaseClient<IRoleToPermissionTenant>()
    .UseWithRestEaseClient<IVendors>();

builder.Services.AddScoped<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

await builder.Build().RunAsync();
