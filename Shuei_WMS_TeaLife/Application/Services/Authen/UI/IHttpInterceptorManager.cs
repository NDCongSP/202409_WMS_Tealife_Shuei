using Toolbelt.Blazor;

namespace Application.Services.Authen.UI
{
    public interface IHttpInterceptorManager
    {
        void RegisterEvent();
        Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs args);
        void DisposeEvent();
    }
}
