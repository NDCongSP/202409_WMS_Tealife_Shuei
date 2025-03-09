using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RestEase;
using WebUIFinal.Pages.StockTake;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DeviceEntity = FBT.ShareModels.WMS.Device;

namespace WebUIFinal.Pages.Device
{
    public partial class DetailDevice
    {
        [Parameter] public string Title { get; set; }

        public string _deviceName { get; set; }
        private DeviceEntity _model = new DeviceEntity();
        private EnumStatus selectStatus;
        private List<UserDto> _users = new();
        private bool _visibled;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await Task.WhenAll(RefreshDataAsync(), GetUserAsync());
            CheckPermission();
        }
        private async Task RefreshDataAsync()
        {
            if (Title != null && Title.Contains("~"))
            {
                _deviceName = Title[1..];
                var res = await _deviceServices.GetByNameAsync(_deviceName);
                if (!res.Succeeded)
                {
                    //NotifyError(_localizerCommon["Detail model is null"]);

                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    //_navigation.NavigateTo("/devicelist");
                    return;
                }

                _model = res?.Data;
                if (_model != null)
                {
                    selectStatus = _model.Status;
                    StateHasChanged();
                }
                else
                {
                    NotificationHelper.ShowNotification(_notificationService
                      ,  NotificationSeverity.Warning
                      , _localizerNotification["Warning"], _localizerNotification["Empty."]);
                    _navigation.NavigateTo("/devicelist");
                }
            }
        }
        private async Task GetUserAsync()
        {
            var data = await _userToTenantServices.GetUsersAsync();
            if (data.Count > 0) _users.AddRange(data);
        }
        private async Task Submit(DeviceEntity arg)
        {
            //if (!await ConfirmAction(_localizerCommon["Save"], $"{_localizerCommon["Confirmation.Save"]}")) return;

            Result<DeviceEntity> res = new Result<DeviceEntity>();
            // Kiểm tra chế độ (tạo mới hoặc chỉnh sửa)
            if (!Title.Contains("~"))
            {
                res = await _deviceServices.InsertAsync(arg);
                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
                _navigation.NavigateTo($"/devicedetail/{"~" + _model.Name}");
                await RefreshDataAsync();
            }
            else
            {
                res = await _deviceServices.UpdateAsync(arg);
                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
                await RefreshDataAsync();
            }
        }
        private async Task Delete()
        {
            try
            {                
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}?", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _deviceServices.DeleteAsync(_model);

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
                await Task.Delay(1000);
                _navigation.NavigateTo("/devicelist");
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }
        private void CheckPermission()
        {
            if (GlobalVariable.AuthenticationStateTask.HasPermission("Edit"))
            {
                _visibled = true;
            }
            else if (GlobalVariable.AuthenticationStateTask.HasPermission("View"))
            {
                _visibled = false;
            }
        }
    }
}
