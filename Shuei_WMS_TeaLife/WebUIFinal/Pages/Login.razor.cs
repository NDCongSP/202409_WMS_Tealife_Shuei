using Application.DTOs.Request.Account;
using Application.DTOs.Response;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Radzen;
using System.Security.Claims;

namespace WebUIFinal.Pages
{
    [AllowAnonymous]
    public partial class Login
    {
        LoginResponse login;
        private string token;

        LoginRequestDTO _loginRequest = new LoginRequestDTO();

        bool password = true;
        void TogglePassword()
        {
            password = !password;
        }

        bool _remember = false;

        private AuthenticationState? authState;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        async void Submit(LoginRequestDTO arg)
        {
            //Console.WriteLine($"Username: {arg.EmailAddress} and password: {arg.Password}");
            try
            {
                var result = await _authenServices.LoginAccountAsync(arg);

                //Fail
                if (!result.Flag)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(result.Message)?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                token = result.Token;
                login = result;

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Login OK."]);

                //await InvokeAsync(StateHasChanged);
                //StateHasChanged();

                var claimsIdentity = new ClaimsIdentity(JwtHelper.GetClaimsFromJwt(token), "jwt");
                var user = new ClaimsPrincipal(claimsIdentity);

                //_httpInterceptorManager.RegisterEvent();
                GlobalVariable.UserAuthorizationInfo.UserName = user.Identity.Name;
                GlobalVariable.UserAuthorizationInfo.FullName = user.FindFirst("FullName").Value;
                GlobalVariable.UserAuthorizationInfo.EmailName = user.FindFirst(ClaimTypes.Email).Value;
                GlobalVariable.UserAuthorizationInfo.UserId = user.FindFirst("UserId").Value;

                var permission = user.FindFirst("RoleToPermission").Value;
                var permissionList = JsonConvert.DeserializeObject<List<RoleToPermission>>(permission);

                var perrr = user.HasClaim("Permission", "Warehouse Staff");

                var claimRole = user.FindAll(ClaimTypes.Role)?.ToList();

                foreach (var item in claimRole)
                {
                    var per = permissionList.Where(x => x.RoleName == item.Value).ToList();

                    GlobalVariable.UserAuthorizationInfo.Roles.Add(new Roles()
                    {
                        Name = item.Value,
                        Permissions = per
                    });
                }

                //if (GlobalVariable.UserAuthorizationInfo.Roles.FirstOrDefault().Name == ConstantExtention.Roles.WarehouseAdmin)
                //    _navigation.NavigateTo("/userlist");
                //else if (GlobalVariable.UserAuthorizationInfo.Roles.FirstOrDefault().Name == ConstantExtention.Roles.WarehouseStaff)
                //    _navigation.NavigateTo("/");
                //else 
                //    _navigation.NavigateTo("/numbersequence");

                authState = await _authStateProvider.GetAuthenticationStateAsync();

                //truyen authState
                GlobalVariable.AuthenticationStateTask = authState;

                if (GlobalVariable.AuthenticationStateTask.HasRole(ConstantExtention.Roles.WarehouseAdmin))
                    _navigation.NavigateTo("/userlist");
                else if (GlobalVariable.AuthenticationStateTask.HasRole(ConstantExtention.Roles.WarehouseStaff))
                    _navigation.NavigateTo("/");
                else
                    _navigation.NavigateTo("/numbersequence");
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = $"Login fail: {ex.Message}",
                    Duration = 5000
                });
            }
        }

        void Cancel()
        {

        }

        void OnRegister(string name)
        {
            Console.WriteLine($"{name} -> Register");
        }

        void OnResetPassword(string value, string name)
        {
            Console.WriteLine($"{name} -> ResetPassword for user: {value}");
        }
    }
}
