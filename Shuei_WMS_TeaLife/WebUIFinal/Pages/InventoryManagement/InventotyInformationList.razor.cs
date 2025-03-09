using Microsoft.JSInterop;
using QRCoder.Core;
using System.Text.Json;
using WebUIFinal.Core.Dto;
using WebUIFinal.TemplateHtmlPrintLabel;
using InventoryHistoryModel = Application.DTOs.InventoryHistoryDto;



namespace WebUIFinal.Pages.InventoryManagement
{
    public partial class InventotyInformationList
    {
        IEnumerable<int> _pageSizeOptions = new int[] { 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        DateTime? value;
        List<InventoryHistoryDto> _dataGrid = new();
        RadzenDataGrid<InventoryHistoryDto>? _inventoryHistoryGrid;

        private List<InventoryHistoryDto> _filteredModel = new List<InventoryHistoryDto>(); // Added this line
        bool allowRowSelectOnRowClick = true;

        private List<TenantDto> tenants = new();

        List<LocationDisplayDto> locations = new();
        List<BinDisplayDto> bins = new();
        List<CategoryDisplayDto> categories = new();
        List<SupplierDisplayDto> suppliers = new();
        IList<InventoryHistoryDto> selectedInventoryHistoryList = [];
        string _id = string.Empty;

        bool _visibaleProgressBar = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            await RefreshDataAsync();
            await GetBinFilterListAsync();
            await GetSupplierFilterListAsync();
            await GetLocationFilterListAsync();
            await GetCategoryFilterListAsync();
            await GetTenantsAsync();

            _inventoryHistoryItems = new List<InventoryHistoryDto>(_dataGrid);
        }

        private async Task GetBinFilterListAsync()
        {
            var data = await _binServices.GetAllAsync();
            if (data.Succeeded) bins.AddRange(data.Data.Select(_ => new BinDisplayDto { Id = _.Id.ToString(), BinCode = _.BinCode }));
        }
        private async Task GetTenantsAsync()
        {
            var data = await _companiesServices.GetAllAsync();

            if (data.Succeeded)
            {
                tenants.Clear();
                tenants.AddRange(data.Data.Select(_ => new TenantDto { Id = _.AuthPTenantId.ToString(), TenantName = _.FullName }));
            }
        }
        private async Task GetCategoryFilterListAsync()
        {
            var data = await _productServices.GetProductCategoriesAsync();
            if (data.Succeeded) categories.AddRange(data.Data.Select(_ => new CategoryDisplayDto { Id = _.Id.ToString(), CategoryName = _.CategoryName }));
        }

        private async Task GetSupplierFilterListAsync()
        {
            var data = await _suppliersServices.GetAllAsync();
            if (data.Succeeded) suppliers.AddRange(data.Data.Select(_ => new SupplierDisplayDto { Id = _.Id.ToString(), SupplierName = _.SupplierName }));
        }

        private async Task GetLocationFilterListAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
        }
        async Task UpdateListBin()
        {
            var data = await _binServices.GetAllAsync();
            bins.Clear();
            if (data.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(_searchModel.Location))
                {
                    data.Data = data.Data.Where(x => x.LocationId.ToString() == _searchModel.Location).ToList();
                }
                bins.AddRange(data.Data.Select(_ => new BinDisplayDto { Id = _.Id.ToString(), BinCode = _.BinCode }));
            }
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _warehouseTranServices.GetProductInventoryInformationAsync(_searchModel);

                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = res.Messages.ToString(),
                    });
                    return;
                }

                _dataGrid = res.Data.ToList();
                _inventoryHistoryItems = _dataGrid;
                StateHasChanged();
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
                return;
            }
        }

        private string QueryString(InventoryHistoryDto data)
        {
            var queryParameters = new Dictionary<string, string>();
            if (data.LocationId != null)
            {
                queryParameters["Location"] = data.LocationId.ToString();
            }
            if (!string.IsNullOrEmpty(data.BinCode)) 
            {
                queryParameters["Bin"] = data.BinId.ToString();
            }
            if (!string.IsNullOrEmpty(data.Lot)) 
            {
                queryParameters["Lot"] = data.Lot;
            }
            if (data.ExpirationDate != null) 
            {
                queryParameters["LotExpiredDate"] = data.ExpirationDate.Value.ToString("yyyy-MM-dd");
            }

            return queryParameters.Any()
            ? "?" + string.Join("&", queryParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"))
            : string.Empty;
        }

    }
}
