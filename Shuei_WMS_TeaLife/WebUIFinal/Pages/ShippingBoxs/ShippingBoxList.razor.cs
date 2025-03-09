using Application.DTOs.Response;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using RestEase;
using ShippingBoxModel = Application.DTOs.ShippingBoxDTO;

namespace WebUIFinal.Pages.ShippingBoxs
{
    public partial class ShippingBoxList
    {
        List<ShippingBoxModel> _shippingBoxes = new();
        RadzenDataGrid<ShippingBoxModel> _profileGrid;
        bool _showPagerSummary = true;
        IList<ShippingBoxModel> selectedShippingBoxes = [];
        bool allowRowSelectOnRowClick = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await RefreshDataAsync();
            Constants.PagingSummaryFormat = _CLoc["DisplayPage"] + " {0} " + _CLoc["Of"] + " {1} <b>(" + _CLoc["Total"] + " {2} " + _CLoc["Records"] + ")</b>";
        }

        async Task DeleteItemAsync(ShippingBoxModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm(_localizer["Are you sure you want to delete shipping box:"] + $" {model.BoxName}?", _localizer["Delete shipping box"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _shippingBoxServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizer["Success"],
                        Detail = _localizer["Delete shipping box"] + $" {model.BoxName} " + _localizer["successfully"],
                        Duration = 5000
                    });

                    await RefreshDataAsync();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = res.Messages.ToString(),
                        Duration = 5000
                    });
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }

        void EditItemAsync(Guid shippingBoxId) => _navigation.NavigateTo($"/addshippingbox/Edit|" + shippingBoxId);

        void AddNewItemAsync() => _navigation.NavigateTo($"/addshippingbox/Create");

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _shippingBoxServices.GetAllWithCarrierAsync();

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _shippingBoxes = res.Data.ToList();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);

                return;
            }
        }

        async void PrintQrLabels()
        {
            if (selectedShippingBoxes == null || !selectedShippingBoxes.Any())
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Warning, _localizerNotification["Warning"]
                    , _localizerNotification["Please select at least one item to print labels."]);

                return;
            }

            List<ShippingBoxModel> _dataMaster = new List<ShippingBoxModel>();

            if (selectedShippingBoxes.Count > 0)
            {
                _dataMaster.AddRange(selectedShippingBoxes);
            }

            //foreach (var item in selectedShippingBoxes)
            //{
            //    var shippingBoxPrint = await _warehousePutAwayServices.GetPutAwayAsync(item.PutAwayNo);
            //    if (shippingBoxPrint.Succeeded)
            //    {
            //        int index = 0;
            //        foreach (var line in shippingBoxPrint.Data.WarehousePutAwayLines)
            //        {
            //            _dataMaster.Add(line);
            //        }
            //    }
            //}

            await _localStorage.SetItemAsync("selectedShippingBoxes", _dataMaster);
            await _jsRuntime.InvokeVoidAsync("openTab", "/PrintShippingBoxes");
        }
    }
}
