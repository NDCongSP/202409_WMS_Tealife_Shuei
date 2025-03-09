using Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using QRCoder.Core;
using System.Text.Json;
using WebUIFinal.Core.Dto;
using WebUIFinal.TemplateHtmlPrintLabel;
using static Application.Extentions.ApiRoutes;
using InventoryHistoryModel = Application.DTOs.InventoryHistoryDetailsDto;


namespace WebUIFinal.Pages.InventoryManagement
{
    public partial class InventotyInformationListDetails
    {
        [Parameter] public string ProductCode { get; set; } = string.Empty;
        [Parameter] public string LocationName { get; set; } = string.Empty;
        [Parameter] public string TenantName { get; set; } = string.Empty;
        [Parameter] public string BinName { get; set; } = string.Empty; 
        [Parameter] public string LotExpiredDate { get; set; } = string.Empty; 
        [Parameter] public string LotName { get; set; } = string.Empty; 


        IEnumerable<int> _pageSizeOptions = new int[] { 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        DateTime? value;
        List<InventoryHistoryDetailsDto> _dataGrid = new();
        RadzenDataGrid<InventoryHistoryDetailsDto>? _inventoryHistoryGrid;

        private List<InventoryHistoryDetailsDto> _filteredModel = new List<InventoryHistoryDetailsDto>(); // Added this line
        bool allowRowSelectOnRowClick = true;

        List<LocationDisplayDto> locations = new();
        List<BinDisplayDto> bins = new();
        List<TransTypeDisplayDto> transTypes = new();
        List<SupplierDisplayDto> suppliers = new();

        private List<TenantDto> tenants = new();

        IList<InventoryHistoryDetailsDto> selectedInventoryHistoryList = [];


        string _id = string.Empty;

        bool _visibaleProgressBar = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            // Get LocationName from query parameters
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Location",
     out var locationName))
            {
                LocationName = locationName;
                _searchModel.Location = LocationName;
            }
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Tenant",
     out var tenantName))
            {
                TenantName = tenantName;
                _searchModel.Tenant = TenantName;
            }
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Bin",
     out var binName))
            {
                BinName = binName;
                _searchModel.Bin = BinName;
            }
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("LotExpiredDate", out var lotExpiredDateString)
     && DateOnly.TryParse(lotExpiredDateString, out var lotExpiredDate))
            {
                _searchModel.LotExpiredDate = lotExpiredDate;
            }
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("Lot",
     out var lotName))
            {
                _searchModel.Lot = lotName;
            }


            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            _searchModel.ProductCode = ProductCode;
            await GetBinFilterListAsync();
            await GetSupplierFilterListAsync();
            await GetLocationFilterListAsync();
            await GetTranstypeFilterListAsync();
            await GetTenantsAsync();
            await RefreshDataAsync();
            _inventoryHistoryItems = new List<InventoryHistoryDetailsDto>(_dataGrid);
        }

        private async Task GetTenantsAsync()
        {
            // var data = await _tenantsServices.GetAllAsync();
            var data = await _companiesServices.GetAllAsync();

            if (data.Succeeded)
            {
                tenants.Clear();
                tenants.AddRange(data.Data.Select(_ => new TenantDto { Id = _.AuthPTenantId.ToString(), TenantName = _.FullName }));
            }
        }

        private async Task GetBinFilterListAsync()
        {
            var data = await _binServices.GetAllAsync();
            if (data.Succeeded) bins.AddRange(data.Data.Select(_ => new BinDisplayDto { Id = _.Id.ToString(), BinCode = _.BinCode }));
        }

        private async Task GetTranstypeFilterListAsync()
        {
            var enumValues = Enum.GetValues(typeof(EnumWarehouseTransType))
                .Cast<EnumWarehouseTransType>()
                .Select(e => new
                {
                    Id = (int)e,
                    Value = e.ToString()
                })
                .ToList();
            transTypes.AddRange(enumValues.Select(_ => new TransTypeDisplayDto { Id = _.Id.ToString(), TransType = _localizer[_.Value] }));
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
                var res = await _warehouseTranServices.GetProductInventoryInformationDetailsAsync(
                    _searchModel);

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
    }
}
