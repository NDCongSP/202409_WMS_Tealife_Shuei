using System.Collections.Generic;
using System.Net.NetworkInformation;
using SupplierModel = FBT.ShareModels.Entities.Supplier;

namespace WebUIFinal.Pages.InventoryManagement
{
    public partial class InventoryAdjustmentList
    {
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        private List<InventAdjustmentDTO> adjustment = new();

        List<LocationDisplayDto> locations = new();
        List<BinDisplayDto> bins = new();

        private RadzenDataGrid<InventAdjustmentDTO> _adjustmentGrid;
        public string? AdjustmentNo { get; set; }
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
                await _adjustmentGrid.RefreshDataAsync();
            }

        }
        private void EditItemAsync(string AdjustmentNo) => _navigation.NavigateTo($"/adjustment-details/{_CLoc["Detail.Edit"]}-{_localizer["WarehouseAdjustment"]}/{AdjustmentNo}");

        private void AddNewItemAsync() => _navigation.NavigateTo($"/adjustment-details/{_CLoc["Detail.Create"]} {_localizer["WarehouseAdjustment"]}");
        
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
                var res = await _inventAdjustmentServices.GetAllDTOAsync();

                if (!res.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], res.Messages.ToString());
                    return;
                }

                adjustment.AddRange(res.Data);
                _filteredModel = adjustment;
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
        private string GetLocalizedStatus(EnumInventoryAdjustmentStatus status)
        {
            return status switch
            {
                EnumInventoryAdjustmentStatus.InProcess => _localizer["InProcess"],
                EnumInventoryAdjustmentStatus.Completed => _localizer["Completed"],
                _ => status.ToString(),
            };
        }
        private List<EnumDisplay<EnumInventoryAdjustmentStatus>> GetDisplayAdjustmentStatus()
        {
            return Enum.GetValues(typeof(EnumInventoryAdjustmentStatus)).Cast<EnumInventoryAdjustmentStatus>().Select(_ => new EnumDisplay<EnumInventoryAdjustmentStatus>
            {
                Value = _,
                DisplayValue = GetValueAdjustmentStatus(_)
            }).ToList();
        }

        private string GetValueAdjustmentStatus(EnumInventoryAdjustmentStatus AdjustmentStatus) => AdjustmentStatus switch
        {

            EnumInventoryAdjustmentStatus.InProcess => _localizer["InProcess"],
            EnumInventoryAdjustmentStatus.Completed => _localizer["Completed"],
            _ => throw new ArgumentException("Invalid value for AdjustmentStatus", nameof(AdjustmentStatus))
        };

    }
}