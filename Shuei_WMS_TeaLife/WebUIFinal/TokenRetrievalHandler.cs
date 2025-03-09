using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Polly;
using System.Net.Http.Headers;

namespace WebUIFinal
{
    public class TokenRetrievalHandler : DelegatingHandler
    {
        private readonly IAccessTokenProviderAccessor _tokenProviderAccessor;
        private readonly NavigationManager _navigation;

        public TokenRetrievalHandler(IAccessTokenProviderAccessor tokenProviderAccessor, NavigationManager navigation)
        {
            _tokenProviderAccessor = tokenProviderAccessor;
            _navigation = navigation;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var context = request.GetPolicyExecutionContext();
                if (context.Count == 0)
                {

                    var accessTokenResult = await _tokenProviderAccessor.TokenProvider.RequestAccessToken();
                    if (accessTokenResult.TryGetToken(out AccessToken accessToken) && !string.IsNullOrWhiteSpace(accessToken.Value))
                    {
                        // request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);

                        context = new Context("TokenRetrieval", new Dictionary<string, object> { { "TokenKey", accessToken } });
                        request.SetPolicyExecutionContext(context);
                    }


                }

                if (context["TokenKey"] is AccessToken token)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                }
            }
            catch (Exception ex) { }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
