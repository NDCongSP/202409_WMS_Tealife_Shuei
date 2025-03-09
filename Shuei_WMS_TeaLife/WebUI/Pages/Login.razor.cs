using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Radzen;

namespace WebUI.Pages
{
    [AllowAnonymous]
    public partial class Login
    {
        LoginResponse login;
        private string token;

        protected string _svg { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            #region Get String from SVG image
            //using var stream = this.GetType().Assembly.GetManifestResourceStream("WebUI.Resources.logoTeaLife.svg");

            //stream.Seek(0, SeekOrigin.Begin);
            //StreamReader reader = new StreamReader(stream);
            //_svg = reader.ReadToEnd();
            #endregion
        }

        async void OnLogin(LoginArgs args)
        {
            Console.WriteLine($"Username: {args.Username} and password: {args.Password}");
            try
            {
                var result = await _authenServices.LoginAccountAsync(new LoginRequestDTO()
                {
                    EmailAddress = args.Username,
                    Password = args.Password
                });

                //Fail
                if (!result.Flag)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Login fail",
                        Duration = 2000
                    });
                    return;
                }

                token = result.Token;
                login = result;

                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Successfull",
                    Detail = "Login OK",
                    Duration = 2000
                });
                //await InvokeAsync(StateHasChanged);
                //StateHasChanged();

                _navigation.NavigateTo("/user");
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = $"Login fail: {ex.Message}",
                    Duration = 2000
                });
            }
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
