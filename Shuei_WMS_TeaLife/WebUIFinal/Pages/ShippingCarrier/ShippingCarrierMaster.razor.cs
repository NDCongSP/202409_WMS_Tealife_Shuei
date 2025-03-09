using Application.DTOs.Response;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using RestEase;
using WebUIFinal.Core;
using ShippingCarrierEntity = FBT.ShareModels.WMS.ShippingCarrier;

namespace WebUIFinal.Pages.ShippingCarrier
{
    public partial class ShippingCarrierMaster
    {
        List<ShippingCarrierEntity>? _dataGrid = null;
        RadzenDataGrid<ShippingCarrierEntity>? _profileGrid;

        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        

        protected override async Task OnInitializedAsync()
        {
            Constants.PagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";
            await base.OnInitializedAsync();
            RefreshDataAsync();
        }

        async Task DeleteItemAsync(ShippingCarrierEntity model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]} {_localizer["ShippingCarrier"]}:{model.Id}?", $"{_localizerCommon["Delete"]} {_localizer["ShippingCarrier"]}", new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true, 
                });

                if (confirm == null || confirm == false) return;

                var res = await _shippingCarrierServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = _localizerCommon["Delete"] + $" {model.ShippingCarrierName} " + _localizerCommon["Success"],
                        Duration = 5000
                    });

                    RefreshDataAsync();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
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
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }
        }

        async Task ViewItemAsync(ShippingCarrierEntity model)
        {
            _navigation.NavigateTo($"/detailshippingcarrier/{_localizerCommon["Detail.View"]} {_localizer["ShippingCarrier"]}|{model.Id}");
        }

        async Task EditItemAsync(ShippingCarrierEntity model)
        {
            _navigation.NavigateTo($"/detailshippingcarrier/{_localizerCommon["Detail.Edit"]} {_localizer["ShippingCarrier"]}|{model.Id}");
        }

        async Task AddNewItemAsync()
        {
            _navigation.NavigateTo($"/detailshippingcarrier/{_localizerCommon["Detail.Create"]} {_localizer["ShippingCarrier"]}");
        }

        async void RefreshDataAsync()
        {
            try
            {
                var res = await _shippingCarrierServices.GetAllAsync();

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _dataGrid = null;
                _dataGrid = new List<ShippingCarrierEntity>();
                _dataGrid = res.Data.ToList();

                StateHasChanged();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }
    }
}
