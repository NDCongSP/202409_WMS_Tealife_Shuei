

using Application.DTOs.Response;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using RestEase;
using System.Net.Mail;

namespace WebUIFinal.Pages.ProductCategoryPage
{
    public partial class ProductCategoryDetail
    {
        [Parameter] public string Title { get; set; }

        private bool isDisabled = false;
        private ProductCategory _model = new ProductCategory();
        private List<string> _status = new List<string>();
        private EnumStatus _selectStatus;

        bool _visibleBtnSubmit = true, _disable = false;
        string _id = string.Empty;
        bool _disibleBtnDelete = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (Title.Contains(_localizerCommon["Detail.Create"])) _visibleBtnSubmit = false;

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

                    var res = await _productCategoryServices.GetByIdAsync(int.Parse(_id));

                    if (res.Succeeded)
                    {
                        _model = res.Data;

                        var checkIsUse = await _productServices.GetByCatetgoryAsync(_model.Id);

                        if (checkIsUse.Succeeded)
                        {
                            if (checkIsUse.Data != null) _disibleBtnDelete = true;
                            else _disibleBtnDelete = false;
                        }
                        else
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Error,
                                Summary = _localizerCommon["Error"],
                                Detail = checkIsUse.Messages.FirstOrDefault(),
                                Duration = 5000
                            });
                            return;
                        }
                    }
                    else
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = _localizerCommon["Error"],
                            Detail = res.Messages.FirstOrDefault(),
                            Duration = 5000
                        });
                        return;
                    }

                    //_selectStatus = Status.Activated.ToString() == _model.Status ? Status.Activated : Status.Inactivated;
                }

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }
        }


        async void Submit(ProductCategory arg)
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}: {arg.CategoryName}?", $"{_localizerCommon["Save"]} {_localizer["Product Category"]}", new ConfirmOptions()
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Status = _selectStatus;

            if (Title.Contains(_localizerCommon["Detail.Create"]))//Add
            {
                var res = await _productCategoryServices.InsertAsync(_model);
                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
            }
            else if (Title.Contains(_localizerCommon["Detail.Edit"])) //update
            {
                var res = await _productCategoryServices.UpdateAsync(_model);
                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
            }
        }

        async Task DeleteItemAsync(ProductCategory model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}?", $"{_localizer["Product Category"]}: {model.CategoryName}", new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _productCategoryServices.DeleteAsync(model);

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);

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
