using Application.Services.Authen.UI;
using Polly;
using System.Net;

namespace WebUIFinal
{
    public static class RetryRefreshTokenHandler
    {
        public static IAsyncPolicy<HttpResponseMessage> GetTokenRefresher(IServiceProvider provider, HttpRequestMessage request)
        {
            return Policy<HttpResponseMessage>
                .HandleResult(response => response.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(async (_, __) =>
                {
                    var authService = provider.GetRequiredService<IAuthServices>();
                    await authService.RefreshTokenAsync();
                    await Task.Delay(10);
                    request.SetPolicyExecutionContext(new Context());
                });
        }
    }
}
