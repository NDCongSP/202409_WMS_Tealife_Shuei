using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;

namespace WebUIFinal.Pages.Components
{
    public partial class ChangePass
    {
        [Parameter] public string Id { get; set; } = string.Empty;

        private ChangePassRequestDTO _model=new ChangePassRequestDTO();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                _model.Id = GlobalVariable.UserAuthorizationInfo.UserId;
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }
        }
        async void Submit(ChangePassRequestDTO arg)
        {
            var confirm = await _dialogService.Confirm(_localizer["Do you want to change password?"], _localizer["Change Password"], new ConfirmOptions()
            {
                OkButtonText = _localizer["Yes"],
                CancelButtonText = _localizer["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            
            var response = await _authenServices.ChangePassAsync(_model);

            if (!response.Flag)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Message)?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }

            NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);

            _dialogService.Close("Success");
        }
    }
}
