using Application.DTOs.Response;
using Application.Extentions;
using Blazored.LocalStorage;
using Blazored.SessionStorage;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;


namespace Application.Services.Authen.UI
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider, IAccessTokenProvider
    {
        //kiểm soát truy cập đồng thời vào tài nguyên trong các tình huống cần giới hạn số lượng tác vụ hoặc luồng có thể chạy cùng lúc,
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        readonly HttpClient _httpClient;
        readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;

        #region Public methods
        public ApiAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage, ISessionStorageService sessionStorage = null)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
        }

        private bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.ValidTo < DateTime.UtcNow;
        }

        /// <summary>
        /// Get state and claim.
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Kiểm tra cả localStorage và sessionStorage để lấy mã thông báo
            //var cachedToken = await _localStorage.GetItemAsync<string>(ConstantExtention.StorageConst.AuthToken)
            //            ?? await _sessionStorage.GetItemAsync<string>(ConstantExtention.StorageConst.AuthToken);

            var cachedToken = await GetCachedAuthTokenAsync();

            if (string.IsNullOrWhiteSpace(cachedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // Giải mã mã thông báo và thiết lập danh tính của người dùng
            //identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

            // Generate claimsIdentity from cached token
            var claimsIdentity = new ClaimsIdentity(JwtHelper.GetClaimsFromJwt(cachedToken), "jwt");

            var user = new ClaimsPrincipal(claimsIdentity);

            return new AuthenticationState(user);
        }

        public void MarkUserAsAuthenticated()
        {
            var authState = GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(authState);

            //// Lưu mã thông báo vào nơi lưu trữ thích hợp
            //if (rememberMe)
            //{
            //    _localStorage.SetItemAsync(AuthTokenKey, token);
            //}
            //else
            //{
            //    _sessionStorage.SetItemAsync(AuthTokenKey, token);
            //}
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
        public async ValueTask CacheAuthTokensAsync(string token, string refreshToken, string sessionId)
        {
            await _localStorage.SetItemAsync(ConstantExtention.StorageConst.AuthToken, token);
            await _localStorage.SetItemAsync(ConstantExtention.StorageConst.RefreshToken, refreshToken);
        }

        public async ValueTask CachePermissionsAsync(ICollection<string> permissions) =>
           await _localStorage.SetItemAsync(ConstantExtention.StorageConst.RemeberMe, permissions);

        public async Task ClearCacheAsync()
        {
            await _localStorage.RemoveItemAsync(ConstantExtention.StorageConst.AuthToken);
            await _localStorage.RemoveItemAsync(ConstantExtention.StorageConst.RefreshToken);
            await _localStorage.RemoveItemAsync(ConstantExtention.StorageConst.RemeberMe);
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken()
        {
            var authState = await GetAuthenticationStateAsync();
            if (authState.User.Identity?.IsAuthenticated is not true)
            {
                return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "/login");
            }

            // We make sure the access token is only refreshed by one thread at a time. The other ones have to wait.
            await _semaphore.WaitAsync();
            try
            {
                string token = await GetCachedAuthTokenAsync();


                // Check if token needs to be refreshed (when its expiration time is less than 1 minute away)
                //var expTime = GetExpiration(authState.User);

                //var diff = expTime - DateTime.UtcNow;
                //if (diff.TotalSeconds <= 5)
                //{
                //    //string refreshToken = await GetCachedRefreshTokenAsync();
                //    //(bool succeeded, var response) = await TryRefreshTokenAsync(new RefreshTokenRequest { Token = token, RefreshToken = refreshToken });
                //    //if (!succeeded)
                //    //{
                //    //    return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, _authSettings.Value.LoginUrl);
                //    //}

                //    //token = response?.Token;
                //}
                //else if (diff.TotalSeconds < 0)
                //{
                //    return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, new AccessToken() { Value = null }, "/login");
                //}

                return new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken() { Value = token }, string.Empty);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options) =>
           RequestAccessToken();


        #endregion

        #region Private Methods
        private async ValueTask<string> GetCachedAuthTokenAsync() =>
          await _localStorage.GetItemAsync<string>(ConstantExtention.StorageConst.AuthToken);

        private async ValueTask<string> GetCachedRefreshTokenAsync() =>
           await _localStorage.GetItemAsync<string>(ConstantExtention.StorageConst.RefreshToken);

        private async ValueTask<ICollection<string>> GetCachedPermissionsAsync() =>
          await _localStorage.GetItemAsync<ICollection<string>>(ConstantExtention.StorageConst.RemeberMe);

        public static DateTimeOffset GetExpiration(ClaimsPrincipal principal)
        {
            return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(principal.FindFirst("exp")?.Value));
        }
        #endregion
    }
}
