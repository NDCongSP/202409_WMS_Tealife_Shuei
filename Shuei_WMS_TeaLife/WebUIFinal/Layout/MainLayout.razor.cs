using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Security.Claims;
using WebUIFinal.Pages.Components;

namespace WebUIFinal.Layout
{
    public partial class MainLayout
    {
        //[Inject] IHttpInterceptorManager _httpInterceptorManager { get; set; }

        private ClaimsPrincipal? user;
        bool _sidebarExpanded = true;
        string _linkChangePass = string.Empty;
        private AuthenticationState? authState;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                //if (string.IsNullOrEmpty(GlobalVariable.UserAuthorizationInfo.UserName))
                {
                    authState = await _authStateProvider.GetAuthenticationStateAsync();

                    //truyen authState
                    GlobalVariable.AuthenticationStateTask = authState;

                    //_httpInterceptorManager.RegisterEvent();
                    GlobalVariable.UserAuthorizationInfo.UserName = authState?.User?.Identity.Name;
                    GlobalVariable.UserAuthorizationInfo.FullName = authState?.User?.FindFirst("FullName").Value;
                    GlobalVariable.UserAuthorizationInfo.EmailName = authState?.User?.FindFirst(ClaimTypes.Email).Value;
                    GlobalVariable.UserAuthorizationInfo.UserId = authState?.User?.FindFirst("UserId").Value;

                    var permission = authState?.User?.FindFirst("RoleToPermission").Value;
                    var permissionList = JsonConvert.DeserializeObject<List<RoleToPermission>>(permission);

                    var claimRole = user?.FindAll(ClaimTypes.Role)?.ToList();

                    foreach (var item in claimRole)
                    {
                        var per = permissionList.Where(x => x.RoleName == item.Value).ToList();

                        GlobalVariable.UserAuthorizationInfo.Roles.Add(new Roles()
                        {
                            Name = item.Value,
                            Permissions = per
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {

            }

            base.OnAfterRender(firstRender);
        }

        void OnParentClicked(RadzenProfileMenuItem args)
        {
            var t = _localizer["Profile.Logout"];
            if (args.Text == _localizer["Profile.Logout"])
            {
                GlobalVariable.AuthenticationStateTask = null;
                GlobalVariable.UserAuthorizationInfo = null;
                GlobalVariable.UserAuthorizationInfo = new UserAuthorizationInfo();
                _authenServices.LogoutAsync();
            }
        }

        private void OnClick(string text)
        {
            _notificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Info, Summary = "Button Clicked", Detail = text });
        }

        async void ChangePassClick()
        {
            var res = await _dialogService.OpenAsync<ChangePass>("Change Password",
                    new Dictionary<string, object>() { { "Id", GlobalVariable.UserAuthorizationInfo.UserId } },
                    new DialogOptions()
                    {
                        Width = "600px",
                        Height = "400px",
                        Resizable = true,
                        Draggable = true,
                        ShowClose = false,
                        CloseDialogOnOverlayClick = true
                    });

            if (res == "Success")
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Change password",
                    Detail = "Successfull",
                    Duration = 5000
                });
            }
        }

        public void Dispose()
        {
        }
    }
}
