using System.Collections.Generic;
using System.Net.NetworkInformation;
using SupplierModel = FBT.ShareModels.Entities.Supplier;

namespace WebUIFinal.Pages.InventoryManagement
{
    public partial class InventoryBundleList
    {
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        private List<InventBundleDTO> bundle = new();

        List<LocationDisplayDto> locations = new();
        List<BinDisplayDto> bins = new();

        private RadzenDataGrid<InventBundleDTO> _bundleGrid;
        public string? TransNo { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                await RefreshDataAsync();
                await GetLocationFilterListAsync();
                await GetBinFilterListAsync();
                _pagingSummaryFormat = _CLoc["DisplayPage"] + " {0} " + _CLoc["Of"] + " {1} <b>(" + _CLoc["Total"] + " {2} " + _CLoc["Records"] + ")</b>";
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], e.Message);
            }
            finally
            {
                await _bundleGrid.RefreshDataAsync();
            }

        }
        private void EditItemAsync(string TransNo) => _navigation.NavigateTo($"/bundle-details/{_CLoc["Detail.Edit"]}-{_localizer["WarehouseAdjustment"]}/{TransNo}");

        private void AddNewItemAsync() => _navigation.NavigateTo($"/bundle-details/{_CLoc["Detail.Create"]} {_localizer["WarehouseAdjustment"]}");
        
        private async Task GetLocationFilterListAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
        }

        private async Task GetBinFilterListAsync()
        {
            var data = await _binServices.GetAllAsync();
            if (data.Succeeded) bins.AddRange(data.Data.Select(_ => new BinDisplayDto { Id = _.Id.ToString(), BinCode = _.BinCode }));
        }
        private async Task UpdateListBin()
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

        private async Task RefreshDataAsync()
        {
            try
            {
                var res = await _inventBundleServices.GetAllDTOAsync();

                if (!res.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], res.Messages.ToString());
                    return;
                }

                bundle.AddRange(res.Data);
                _filteredModel = bundle;
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }
        }

        private void ShowNotification(NotificationSeverity severity, string summary, string detail, int duration = 5000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }

        private List<EnumDisplay<EnumStatusBundle>> GetDisplayAdjustmentStatus()
        {
            return Enum.GetValues(typeof(EnumStatusBundle)).Cast<EnumStatusBundle>().Select(_ => new EnumDisplay<EnumStatusBundle>
            {
                Value = _,
                DisplayValue = _localizer[_.ToString()]
            }).ToList();
        }

        private void GetPlannedShpimentBundleProducts() => _navigation.NavigateTo($"/inventory-preset-list");

    }
}