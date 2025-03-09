using Microsoft.JSInterop;
using Application.DTOs.Transfer;
using Application.DTOs.Request;
using ProductEntity = FBT.ShareModels.Entities.Product;

namespace WebUIFinal.Pages.StockTake
{
    public partial class StockTake
    {       
        private List<InventStockTakeDto> _dataGrid = null;
        private RadzenDataGrid<InventStockTakeDto> _profileGrid;
        private IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        private bool _showPagerSummary = true;
        private string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        private bool allowRowSelectOnRowClick = false;
        private InventStockTakeSearchModel _searchModel = new InventStockTakeSearchModel();
        private IList<InventStockTakeDto> _gridSelected = [];
        private bool _disable = false;
        private EnumInventStockTakeStatus? _selectStatus;
        private List<Location> _locations = [];
        private Location _locationSelect;
        private List<ProductEntity> _products = [];
        private ProductEntity _productSelect;
        private List<CompanyTenant> _tenants = [];
        private CompanyTenant _tenantSelect;
        private DateOnly? _transactionDateFrom, _transactionDateTo;
        private IList<InventStockTakeDto> selectedItems = [];
        private bool _visibled;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";
            await Task.WhenAll(GetLocation(), GetProduct(), GetTenant(), RefreshDataAsync(_searchModel));
            _selectStatus = null;
            CheckPermission();
        }
        private async Task RefreshDataAsync(InventStockTakeSearchModel model)
        {
            try
            {
                var res = await _inventStockTakeServices.GetStockTakeAsync(model);
                if (!res.Succeeded)
                {
                    NotifyError(_localizer[res.Messages.FirstOrDefault()]);
                }
                _dataGrid = new List<InventStockTakeDto>();
                _dataGrid = res.Data.ToList();
                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotifyError(_localizer[ex.Message]);
            }
        }
        private async Task ClearFilter()
        {
            _productSelect = null;
            _locationSelect = null;
            _selectStatus = null;
            _searchModel = null;
            _transactionDateFrom = null;
            _transactionDateTo = null;
            _searchModel = new InventStockTakeSearchModel();
            await RefreshDataAsync(_searchModel);
        }
        private void OpenAsync(InventStockTakeDto model)
        {
            _navigation.NavigateTo($"/stocktakedetail/{"~" + model.StockTakeNo}");
        }
        private void CreateAsync()
        {
            _navigation.NavigateTo($"/stocktakedetail/{_localizerCommon["Detail.Create"]}");
        }
        private async void OnSearch(InventStockTakeSearchModel arg)
        {
            var r = _gridSelected;
            arg.Location = (_locationSelect?.Id).ToString();
            arg.Tenant = _tenantSelect?.AuthPTenantId;
            arg.ProductCode = _productSelect?.ProductCode;
            arg.StockTakeFrom = _transactionDateFrom;
            arg.StockTakeTo = _transactionDateTo;
            arg.Status = _selectStatus;
            await RefreshDataAsync(arg);
        }
        private async Task PrintSelectedLabels()
        {
            if (selectedItems == null || !selectedItems.Any())
            {
                NotifyWarning(_localizer["Please select at least one item to print labels."]);
            }

            var _dataTrasfers = new List<StockTakeSlipInfos>();
            foreach (var item in selectedItems)
            {
                // Lấy chi tiết của từng item đã chọn
                var res = await _inventStockTakeServices.GetStockTakeLineByIdAsync(item.StockTakeNo);
                if (!res.Succeeded)
                {
                    NotifyError(_localizer[res.Messages.FirstOrDefault()]);
                }
                var _dataTrasfer = new StockTakeSlipInfos (item, res.Data.ToList());
                _dataTrasfers.Add(_dataTrasfer);
            }
            await _localStorage.SetItemAsync("StockTakeSlipInfosTransfer", _dataTrasfers);
            await JSRuntime.InvokeVoidAsync("openTab", "/printstocktakeslip");
        }
        private async Task GetProduct()
        {
            var productResponse = await _productServices.GetAllAsync();

            if (!productResponse.Succeeded)
            {
                NotifyError(_localizer[productResponse.Messages.FirstOrDefault()]);
                return;
            }
            _products = productResponse.Data.ToList();
        }
        private async Task GetLocation()
        {
            var locationResponse = await _locationServices.GetAllAsync();
            if (!locationResponse.Succeeded)
            {
                NotifyError(_localizer[locationResponse.Messages.FirstOrDefault()]);
                return;
            }
            _locations = locationResponse.Data.ToList();
        }
        private async Task GetTenant()
        {
            var companyResponse = await _companiesServices.GetAllAsync();
            if (!companyResponse.Succeeded)
            {
                NotifyError(_localizer[companyResponse.Messages.FirstOrDefault()]);
                return;
            }
            _tenants = companyResponse.Data.ToList();
        }
        private List<EnumDisplay<EnumInventStockTakeStatus>> GetDisplayStatus()
        {
            return Enum.GetValues(typeof(EnumInventStockTakeStatus))
               .Cast<EnumInventStockTakeStatus>()
               .Select(status => new EnumDisplay<EnumInventStockTakeStatus>
               {
                   Value = status,
                   DisplayValue = GetValueLocalizedStatus(status)
               }) 
               .ToList();
        }
        private string GetValueLocalizedStatus(EnumInventStockTakeStatus? enumStatus)
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
            var hasEditPermission = GlobalVariable.AuthenticationStateTask.HasPermission("Edit");
            var hasStocktakeEditPermission = GlobalVariable.AuthenticationStateTask.HasPermission("StocktakeEdit");
            var hasViewPermission = GlobalVariable.AuthenticationStateTask.HasPermission("View");
            var hasStocktakeViewPermission = GlobalVariable.AuthenticationStateTask.HasPermission("StocktakeView");

            if (hasEditPermission || hasStocktakeEditPermission)
            {
                _visibled = true;
            }
            else if (hasViewPermission || hasStocktakeViewPermission)
            {
                _visibled = false;
            }
            else
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
