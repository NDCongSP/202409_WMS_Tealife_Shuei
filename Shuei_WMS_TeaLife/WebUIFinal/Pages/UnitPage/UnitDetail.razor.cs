

using Application.DTOs.Response;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Radzen;
using RestEase;

namespace WebUIFinal.Pages.UnitPage
{
    public partial class UnitDetail
    {
        [Parameter] public string Title { get; set; }

        private bool isDisabled = false;
        private Unit _model = new Unit();
        private List<string> _status = new List<string>();
        private EnumStatus _selectStatus;

        bool _visibleBtnSubmit = true, _disable = false;
        string _id = string.Empty;

        bool _disibleBtnDelete = true;//dung de On/Off cho nut xoa

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (Title.Contains($"{_localizerCommon["Detail.Create"]}")) _visibleBtnSubmit = false;

            _selectStatus = EnumStatus.Activated;
            await RefreshDataAsync();
        }
        async Task RefreshDataAsync()
        {
            try
            {
                //_selectStatus = Status.Activated;

                if (Title.Contains("|"))
                {
                    var arr = Title.Split('|');
                    Title = arr[0];
                    _id = arr[1];

                    var res = await _unitsService.GetByIdAsync(int.Parse(_id));

                    //var checlIsUse=await _productServices.get
                    if (!res.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                     , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                     , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    _model = res.Data;

                    var checkIsUse = await _productServices.GetByUnitAsync(_model.Id);

                    if (!checkIsUse.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(checkIsUse.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                     , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                     , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    if (checkIsUse.Data != null) _disibleBtnDelete = true;
                    else _disibleBtnDelete = false;

                    //_selectStatus = Status.Activated.ToString() == _model.Status ? Status.Activated : Status.Inactivated;
                }

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }


        async void Submit(Unit arg)
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}?", $"{_localizer["Unit"]}: {arg.UnitName}", new ConfirmOptions()
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Status = _selectStatus;

            var loal = _localizerCommon["Detail.Create"];

            if (Title.Contains(_localizerCommon["Detail.Create"]))//Add
            {
                var res = await _unitsService.InsertAsync(_model);
                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
            }
            else if (Title.Contains(_localizerCommon["Detail.Edit"]))//update
            {
                var res = await _unitsService.UpdateAsync(_model);
                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
            }
        }

        async Task DeleteItemAsync(Unit model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}?", $"{_localizer["Unit"]}: {model.UnitName}", new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _unitsService.DeleteAsync(model);

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
                StateHasChanged();

                await RefreshDataAsync();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);

                return;
            }
        }
    }
}
