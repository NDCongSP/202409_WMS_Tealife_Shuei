using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Application.DTOs.Request.StockTake;
using Microsoft.JSInterop;
using Application.DTOs.Transfer;
using static Application.Extentions.ApiRoutes;

namespace WebUIFinal.Pages.StockTakeRecording
{
    public partial class StockTakeRecording
    {
        List<InventStockTakeRecordingDTO> _dataGrid = null;
        RadzenDataGrid<InventStockTakeRecordingDTO> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        bool allowRowSelectOnRowClick = false;

        StockTakeRecordingSearchRequestDto _searchModel = new StockTakeRecordingSearchRequestDto();

        IList<InventStockTakeRecordingDTO> _gridSelected = [];
        bool _disable = false;

        EnumInvenTransferStatus? _selectStatus;

        List<LocationDisplayDto> _locations = [];
        LocationDisplayDto _locationSelected;
        List<CompanyDisplayDto> _tenants = [];
        CompanyDisplayDto _tenantSelected;
        List<Bin> _bins = [];
        Bin _binSelected;
        DateOnly? _transactionDateFrom, _transactionDateTo;
        IList<InventStockTakeRecordingDTO> selectedItems = [];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";
            await Task.WhenAll(GetLocationAsync(), GetTenantAsync(), RefreshDataAsync(_searchModel));
            _selectStatus = null;
            CheckPermission();
        }
        private async Task RefreshDataAsync(StockTakeRecordingSearchRequestDto model)
        {
            try
            {
                var res = await _inventStockTakeRecordingServices.GetStockTakeRecordingAsync(model);

                if (!res.Succeeded)
                {
                    NotifyError(_localizer[res.Messages.FirstOrDefault()]);
                    return;
                }

                _dataGrid = null;
                _dataGrid = new List<InventStockTakeRecordingDTO>();
                _dataGrid = res.Data.ToList();

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotifyError(_localizer[ex.Message]);
                return;
            }
        }
        private async Task ClearFilter()
        {
            _binSelected = null;
            _locationSelected = null;
            _selectStatus = null;
            _searchModel = null;
            _transactionDateFrom = null;
            _transactionDateTo = null;
            _searchModel = new StockTakeRecordingSearchRequestDto();
            await RefreshDataAsync(_searchModel);
        }
        void OpenAsync(InventStockTakeRecordingDTO model)
        {
            _navigation.NavigateTo($"/stocktakerecordingdetail/{"~" + model.Id}");
        }
        private async void OnSearch(StockTakeRecordingSearchRequestDto arg)
        {
            var r = _gridSelected;
            arg.Location = _locationSelected?.Id;
            arg.Tenant = _tenantSelected?.Id;
            arg.Bin = _binSelected?.BinCode;
            arg.TransactionDateFrom = _transactionDateFrom;
            arg.TransactionDateTo = _transactionDateTo;
            arg.Status = _selectStatus;
            await RefreshDataAsync(arg);
        }
        private async Task GetBin()
        {
            if (_locationSelected == null) return;
            if (Guid.TryParse(_locationSelected.Id, out var locationGuid))
            {
                var binResponse = await _binServices.GetByLocationId(locationGuid);
                if (binResponse.Succeeded)
                {
                    _bins = binResponse.Data.ToList();
                }
                NotifyError(_localizer[binResponse.Messages.FirstOrDefault()]);
                return;
            }
        }
        private async Task GetLocationAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded)
            {
                _locations.AddRange(data.Data.Select(l => new LocationDisplayDto { Id = l.Id.ToString(), LocationName = l.LocationName }));
            }
        }
        private async Task GetTenantAsync()
        {
            var data = await _companiesServices.GetAllAsync();
            if (data.Succeeded)
            {
                _tenants.AddRange(data.Data.Select(t => new CompanyDisplayDto { Id = t.AuthPTenantId, CompanyName = t.FullName }));
            }
        }
        private async Task PrintSelectedLabels()
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                NotifyError(_localizer["Please select at least one item to print labels"]);
                return;
            }

            var _dataTrasfers = new List<StockTakeRecordingSlipInfos>();
            foreach (var item in selectedItems)
            {
                // Lấy chi tiết của từng item đã chọn
                var res = await _inventStockTakeRecordingServices.GetByIdDTOAsync(item.Id);
                if (!res.Succeeded)
                {
                    NotifyError(_localizer[res.Messages.FirstOrDefault()]);
                    return;
                }
                var _dataTrasfer = new StockTakeRecordingSlipInfos(item, res.Data.InventStockTakeRecordingLineDtos);
                _dataTrasfers.Add(_dataTrasfer);
            }
            await _localStorage.SetItemAsync("StockTakeRecordingSlipInfosTransfer", _dataTrasfers);
            await JSRuntime.InvokeVoidAsync("openTab", "/printstocktakerecordingslip");
        }
        private List<EnumDisplay<EnumInvenTransferStatus>> GetDisplayStatus()
        {
            return Enum.GetValues(typeof(EnumInvenTransferStatus))
               .Cast<EnumInvenTransferStatus>()
               .Select(status => new EnumDisplay<EnumInvenTransferStatus>
               {
                   Value = status,
                   DisplayValue = GetValueLocalizedStatus(status)
               })
               .ToList();
        }
        private string GetValueLocalizedStatus(EnumInvenTransferStatus? enumStatus)
        {
            if (enumStatus.HasValue)
            {
                return _localizerEnum[enumStatus.Value.ToString()];
            }
            else
            {
                return "Status not specified";
            }
        }
        private void CheckPermission()
        {
            if (!GlobalVariable.AuthenticationStateTask.HasPermission("Edit") && !GlobalVariable.AuthenticationStateTask.HasPermission("StocktakeEdit") &&
                !GlobalVariable.AuthenticationStateTask.HasPermission("View") && !GlobalVariable.AuthenticationStateTask.HasPermission("StocktakeView"))
            {
                NotifyError("You do not have permission to access this functionality");
                _navigation.NavigateTo("/");
            }
        }
        // Cac ham thong bao
        #region NOTIFY
        private async Task<bool> ConfirmAction(string message, string title)
        {
            return await _dialogService.Confirm(title, message, new ConfirmOptions
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true
            }) ?? false;
        }
        private void NotifyError(string message)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _localizerCommon["Error"],
                Detail = message,
                Duration = 5000
            });
        }
        private void NotifySuccess(string message)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = _localizerCommon["Success"],
                Detail = message,
                Duration = 5000
            });
        }
        private void NotifyWarning(string message)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = _localizerCommon["Warning"],
                Detail = message,
                Duration = 10000
            });
        }
        #endregion
    }
}
