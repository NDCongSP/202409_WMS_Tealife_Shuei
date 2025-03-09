using Application.DTOs;
using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Application.Extentions;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Application.Services.Authen.UI
{
    public class AuthServices : IAuthServices
    {
        readonly HttpClient _httpClient;
        readonly ILocalStorageService _localStorage;
        readonly ApiAuthenticationStateProvider _authStateProvider;
        readonly NavigationManager _navigationManager;

        public AuthServices(HttpClient httpClient, ILocalStorageService localStorage, ApiAuthenticationStateProvider authStateProvider, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
            _navigationManager = navigationManager;
        }


        #region httpClient call API        
        public async Task<GeneralResponse> AssignUserRoleAsync(AssignUserRoleRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.AssignUserRole}", model);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response;
        }

        public async Task<GeneralResponse> ChangePassAsync(ChangePassRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.ChangePassword}", model);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response!;
        }

        public async Task<GeneralResponse> ChangeUserRoleAsync(AssignUserRoleRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.ChangeUserRole}", model);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response;
        }

        public async Task<GeneralResponse> CreateAccountAsync(CreateAccountRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.CreateAccount}", model);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response;
        }

        public async Task<GeneralResponse> CreateSuperAdminAsync()
        {
            var result = await _httpClient.PostAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.CreateSuperAdminAccount}", null);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response;
        }

        public async Task<GeneralResponse> CreateRoleAsysnc(CreateRoleRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.CreateRole}", model);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response;
        }

        public async Task<GeneralResponse> DeleteUserAsync(UpdateDeleteRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.DeleteUser}", model);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response;
        }

        public async Task<List<GetRoleResponseDTO>> GetRolesAsync()
        {
            var result = await _httpClient.GetAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.RoleList}");

            if (!result.IsSuccessStatusCode)
                return new List<GetRoleResponseDTO>();

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<List<GetRoleResponseDTO>>(content);

            return response;
        }

        public async Task<List<GetUserWithRoleResponseDTO>> GetUsersWithRolesAsync()
        {
            var result = await _httpClient.GetAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.UserWithRole}");

            if (!result.IsSuccessStatusCode)
                return new List<GetUserWithRoleResponseDTO>();

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<List<GetUserWithRoleResponseDTO>>(content);
            return response;
        }

        public async Task<LoginResponse> LoginAccountAsync(LoginRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.Login}", model);

            if (!result.IsSuccessStatusCode)
                return new LoginResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(content);

            //Set local storage
            await _authStateProvider.CacheAuthTokensAsync(loginResponse.Token, loginResponse.RefreshToken, string.Empty);
            ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated();
            //Gán token này mặc đinh vào header của tất cả các request của httpClient có tên là Bearer
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

            return loginResponse;
        }

        public async Task LogoutAsync()
        {
            await _authStateProvider.ClearCacheAsync();
            ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;

            _navigationManager.NavigateTo("/login");
        }

        public async Task<LoginResponse> RefreshTokenAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(ConstantExtention.StorageConst.AuthToken);
            var refreshToken = await _localStorage.GetItemAsync<string>(ConstantExtention.StorageConst.RefreshToken);
            var model = new RefreshTokenRequestDTO()
            {
                Token = token,
                RefreshToken = refreshToken,
            };

            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.RefreshToken}", model);

            if (!result.IsSuccessStatusCode)
                return new LoginResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(content);

            //Set local storage
            await _authStateProvider.CacheAuthTokensAsync(loginResponse.Token, loginResponse.RefreshToken, string.Empty);
            ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated();
            //Gán token này mặc đinh vào header của tất cả các request của httpClient có tên là Bearer
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

            return loginResponse;
        }

        public async Task<GeneralResponse> UpdateRoleAsync(UpdateDeleteRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.UpdateRole}", model);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response;
        }

        public async Task<GeneralResponse> DeleteUserRoleAsync(AssignUserRoleRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.DeleteUserRole}", model);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response;
        }

        public async Task<GeneralResponse> UpdateUserInfoAsync(UpdateUserInfoRequestDTO model)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.UpdateUserInfo}", model);

                if (!result.IsSuccessStatusCode)
                    return new GeneralResponse()
                    {
                        Flag = false,
                        Message = await result.Content.ReadAsStringAsync()
                    };

                var responseData = await result.Content.ReadFromJsonAsync<GeneralResponse>();

                //var content = await result.Content.ReadAsStringAsync();
                //var responseData = JsonConvert.DeserializeObject<GeneralResponse>(content);

                return responseData;
            }
            catch (HttpRequestException httpEx)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{httpEx.Message}{Environment.NewLine}{httpEx.InnerException}"
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}"
                };
            }
        }

        public async Task<GetUserWithRoleResponseDTO> UserGetById(string id)
        {
            var result = await _httpClient.GetAsync($"{ApiRoutes.Identity.BasePath}/identity/{id}");

            if (!result.IsSuccessStatusCode)
                return null;

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GetUserWithRoleResponseDTO>(content);

            return response;
        }

        public async Task<GeneralResponse> DeleteRoleAsync(UpdateDeleteRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.DeleteRole}", model);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response;
        }

        public async Task<GetUserWithRoleResponseDTO> UserGetByEmailAsync(string email)
        {
            var result = await _httpClient.GetAsync($"{ApiRoutes.Identity.BasePath}/identity/UserGetByEmail/{email}");

            if (!result.IsSuccessStatusCode)
                return null;

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GetUserWithRoleResponseDTO>(content);

            return response;
        }

        public async Task<GetRoleResponseDTO> RoleGetById(string id)
        {
            try
            {
                var result = await _httpClient.GetAsync($"{ApiRoutes.Identity.BasePath}/identity/Role/{id}");

                if (!result.IsSuccessStatusCode)
                    return null;

                var content = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<GetRoleResponseDTO>(content);

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetReportBase64(string id)
        {
            try
            {
                var result = await _httpClient.GetAsync($"api/account/identity/GetReportBase64/{id}");

                if (!result.IsSuccessStatusCode)
                    return null;

                var response = await result.Content.ReadAsStringAsync();

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GeneratePdf()
        {
            try
            {
                var result = await _httpClient.GetAsync($"api/account/identity/GeneratePdf");

                if (!result.IsSuccessStatusCode)
                    return null;

                var response = await result.Content.ReadAsStringAsync();

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<LabelInfoDto>> GetLabelByIdAsync(string id)
        {
            try
            {
                var result = await _httpClient.GetAsync($"{ApiRoutes.Identity.BasePath}/identity/GetLabelById/{id}");

                if (!result.IsSuccessStatusCode)
                    return null;

                var content = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<List<LabelInfoDto>>(content);

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<LabelInfoDto>> GetLabelsAllAsync()
        {
            try
            {
                var result = await _httpClient.GetAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.GetLabelsAll}");

                if (!result.IsSuccessStatusCode)
                    return null;

                var content = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<List<LabelInfoDto>>(content);

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<LoginResponse> LoginAccountHTAsync(LoginRequestDTO model)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.Login}", model);

                if (!result.IsSuccessStatusCode)
                    return new LoginResponse()
                    {
                        Flag = false,
                        Message = await result.Content.ReadAsStringAsync()
                    };

                var content = await result.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(content);

                //Set local storage
                await _authStateProvider.CacheAuthTokensAsync(loginResponse.Token, loginResponse.RefreshToken, string.Empty);
                ((ApiAuthenticationStateProvider)_authStateProvider).MarkUserAsAuthenticated();
                //Gán token này mặc đinh vào header của tất cả các request của httpClient có tên là Bearer
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

                return loginResponse;
            }
            catch (Exception ex)
            {
                return new LoginResponse()
                {
                    Flag = false,
                    Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}"
                };
            }
        }

        public async Task<GeneralResponse> UpdateRoleDTOAsync(CreateRoleRequestDTO model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{ApiRoutes.Identity.BasePath}/{ApiRoutes.Identity.UpdateRoleDTO}", model);

            if (!result.IsSuccessStatusCode)
                return new GeneralResponse()
                {
                    Flag = false,
                    Message = await result.Content.ReadAsStringAsync()
                };

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<GeneralResponse>(content);

            return response;
        }
        #endregion
    }

    public class HttpResponse
    {
        public string Error { get; set; }
    }
}
