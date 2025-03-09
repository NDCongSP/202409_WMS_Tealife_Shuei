using Application.DTOs.Request.Account;
using Application.Extentions;
using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbelt.Blazor;

namespace Application.Services.Authen.UI
{
    public class HttpInterceptorManager : IHttpInterceptorManager
    {
        private readonly NotificationService _notify;
        private readonly NavigationManager _navigationManager;
        private readonly HttpClientInterceptor _httpInterceptor;
        private readonly IAuthServices _authService;

        public HttpInterceptorManager(NotificationService notify, NavigationManager navigationManager
            , HttpClientInterceptor httpInterceptor, IAuthServices authService)
        {
            _notify = notify;
            _navigationManager = navigationManager;
            _httpInterceptor = httpInterceptor;
            _authService = authService;
        }

        public void DisposeEvent()
        {
            _httpInterceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
        }

        public async Task InterceptBeforeHttpAsync(object sender, Toolbelt.Blazor.HttpClientInterceptorEventArgs args)
        {
            var absPath = args.Request.RequestUri.AbsolutePath.ToLower();
            if (!absPath.Contains("login") && !absPath.Contains("refresh-token"))
            {
                try
                {
                    //var result = await _authService.RefreshTokenAsync();
                    //if (result != null && result.Flag == true)
                    //{                        
                    //    args.Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);                        
                    //}
                    //else
                    //{
                    //    throw new UnauthorizedAccessException(result?.Message);
                    //}
                }
                catch (Exception ex)
                {                    
                    args.Cancel = true;
                    _notify.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = ex.Message ?? "Your session was expired",
                        Duration = 5000
                    });
                    await _authService.LogoutAsync();
                    throw;
                }
            }
        }

        public void RegisterEvent()
        {
            //_httpInterceptor.BeforeSendAsync += InterceptBeforeHttpAsync;
            //_httpInterceptor.AfterSendAsync += _httpInterceptor_AfterSendAsync;
        }

        private Task _httpInterceptor_AfterSendAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
