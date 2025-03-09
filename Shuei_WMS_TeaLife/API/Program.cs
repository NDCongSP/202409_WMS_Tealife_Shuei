using API.BackgroundJobs;
using Infrastructure.Data;
using Infrastructure.IoC.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

GlobalVariable.BackgroundJobInterval = int.TryParse(config["BackgroundJobInterval"], out int value) ? value : 10;

//var p = int.TryParse(config["ApiPort"], out int value) ? value : 9500;
//set cho 
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.Listen(System.Net.IPAddress.Any, Convert.ToInt32(config["ApiPort"]));//http
//    //options.ListenAnyIP(Convert.ToInt32(config["ApiPort"]), listenOption => listenOption.UseHttps);//https
//});//set port

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.CustomOperationIds(desc =>
    {
        if (desc.ActionDescriptor is ControllerActionDescriptor descriptor)
        {
            return $"{descriptor.ControllerName}_{descriptor.ActionName}";
        }
        return desc.GroupName;
    });
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WMS API",
        Version = "v1",
        Description = "This is API description.",
        Contact = new OpenApiContact
        {
            Name = "Shuei Info Tech",
            Email = "your.email@example.com",
        }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Description = "Input your Bearer token to access this API",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "Bearer",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    });
});

builder.Services.AddInfrastructureService(config);

// Đăng ký BackgroundService
builder.Services.AddHostedService<TimedBackgroundService>();
//if (Convert.ToInt32(config["Enviroment"]) == 3)
//{
//builder.Services.AddHostedService<TimerGenerateReceiptIntrustionBackgroundService>();
//builder.Services.AddHostedService<TimerGenerateShipmentByOrderBackgroundService>();
//builder.Services.AddHostedService<TimerGeneratePickingByShipmentBackgroundService>();

//builder.Services.AddHostedService<PutawayBackgroundService>();
//builder.Services.AddHostedService<TimerCompleteReceiptOrderBackgroundService>();
//builder.Services.AddHostedService<TimerCompletePickingBackgroundService>();
//}

var app = builder.Build();

// Seeding the database when the application starts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    // Call the seeding method
    await DbInitializer.InitializeAsync(context);
}

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WMS API V1");
        //c.RoutePrefix = string.Empty; // Truy cập Swagger tại đường dẫn gốc ("/")
    });
}

//var useHttps = config.GetSection("UseHttps").Get<bool>();
//if (useHttps)
//    app.UseHttpsRedirection();//use https

app.UseCors("WebUI");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
