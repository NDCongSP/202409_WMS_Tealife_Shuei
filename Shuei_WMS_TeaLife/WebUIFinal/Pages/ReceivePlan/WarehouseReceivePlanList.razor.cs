using Application.DTOs.Request;
using Application.Extentions.Pagings;
namespace WebUIFinal.Pages.ReceivePlan
{
    public partial class WarehouseReceivePlanList
    {
        private PageList<ArrivalInstructionDto> _dataGrid;
        private ReceivePlanSearchModel _searchModel = new ReceivePlanSearchModel();
        RadzenDataGrid<ArrivalInstructionDto>? _receivePlanGrid;
        private int _pageNumber = 1;
        private int _pageSize = 10;

        #region MASTER DATA
        private List<FBT.ShareModels.Entities.Supplier> suppliers = new();
        private async Task GetSupplierAsync() => suppliers = (await _suppliersServices.GetAllAsync()).Data.ToList();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Constants.PagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            await RefreshDataAsync();
            await GetSupplierAsync();
        }

        private void Clear()
        {
            _searchModel = new ReceivePlanSearchModel();
            _receivePlanGrid.Reload();
        }

        async Task LoadData(LoadDataArgs args)
        {
            _pageNumber = (int)((args.Skip / args.Top) + 1);
            _pageSize = (int)args.Top;
            await RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var model = new QueryModel<ReceivePlanSearchModel>
                {
                    Entity = _searchModel,
                    PageNumber = _pageNumber,
                    PageSize = _pageSize
                };
                var result = await _warehouseReceivePlan.SearchArrivalInstructions(model);
                if (result.Succeeded)
                {
                    _dataGrid = result.Data;
                    StateHasChanged();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = result.ToString(),
                    });
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }

        async Task CreateReceiptIntrustrion(int receiptPlanId)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.CreateReceiptIntrustion"]}?", $"{_localizerCommon["Confirm"]}",
                new ConfirmOptions { OkButtonText = _localizerCommon["Yes"], CancelButtonText = _localizerCommon["No"], AutoFocusFirstElement = true }) ?? false;
                if (confirm == true)
                {
                    var result = await _warehouseReceiptOrderService.CreateReceiptFromReceiptPlan(receiptPlanId);
                    if (result.Succeeded)
                    {
                        await RefreshDataAsync();

                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = _localizerCommon["Success"],
                            Detail = result.ToString(),
                            Duration = 5000
                        });
                    }
                    else
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = _localizerCommon["Error"],
                            Detail = result.ToString(),
                            Duration = 5000
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }
    }
}
