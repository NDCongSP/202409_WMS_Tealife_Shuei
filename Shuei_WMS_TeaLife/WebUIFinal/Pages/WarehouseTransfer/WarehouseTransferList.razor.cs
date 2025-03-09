using System.Net.NetworkInformation;
using SupplierModel = FBT.ShareModels.Entities.Supplier;

namespace WebUIFinal.Pages.WarehouseTransfer
{
    public partial class WarehouseTransferList
    {
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        private List<InventTransfersDTO> transfer = new();
        private List<Location> locations = new();
        private RadzenDataGrid<InventTransfersDTO> _transferGrid;
        public string? TransferNo { get; set; }

        List<CompanyTenant> _tenants = [];
        CompanyTenant _selectTenant;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                await RefreshDataAsync();
                await GetLocationAsync();
                await GetTenantsAsync();
                _pagingSummaryFormat = _CLoc["DisplayPage"] + " {0} " + _CLoc["Of"] + " {1} <b>(" + _CLoc["Total"] + " {2} " + _CLoc["Records"] + ")</b>";
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], e.Message);
            }
            finally
            {
                await _transferGrid.RefreshDataAsync();
            }
        }

        private async Task GetTenantsAsync()
        {
            var data = await _companiesServices.GetAllAsync();

            if (!data.Succeeded)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], data.Messages.FirstOrDefault());
                return;
            }
            _tenants.AddRange(data.Data);
        }

        private void EditItemAsync(string TransferNo) => _navigation.NavigateTo($"/addtransfer/{_CLoc["Detail.Edit"]} {_localizer["WarehouseTransfer"]}|{TransferNo}");

        private void AddNewItemAsync() => _navigation.NavigateTo($"/addtransfer/{_CLoc["Detail.Create"]} {_localizer["WarehouseTransfer"]}");
        private async Task GetLocationAsync() => locations = (await _locationServices.GetAllAsync()).Data.ToList();

        private async Task RefreshDataAsync()
        {
            try
            {
                var res = await _warehouseTransferService.GetAllDTO();

                if (!res.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], res.Messages.ToString());
                    return;
                }

                transfer.AddRange(res.Data);
                _filteredModel = transfer;
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
        private string GetLocalizedStatus(EnumInvenTransferStatus status)
        {
            return status switch
            {
                EnumInvenTransferStatus.InProcess => _localizer["InProcess"],
                EnumInvenTransferStatus.Completed => _localizer["Completed"],
                _ => status.ToString(),
            };
        }
        private List<EnumDisplay<EnumInvenTransferStatus>> GetDisplayTransferStatus()
        {
            return Enum.GetValues(typeof(EnumInvenTransferStatus)).Cast<EnumInvenTransferStatus>().Select(_ => new EnumDisplay<EnumInvenTransferStatus>
            {
                Value = _,
                DisplayValue = GetValueTransferStatus(_)
            }).ToList();
        }

        private string GetValueTransferStatus(EnumInvenTransferStatus TransferStatus) => TransferStatus switch
        {

            EnumInvenTransferStatus.InProcess => _localizer["InProcess"],
            EnumInvenTransferStatus.Completed => _localizer["Completed"],
            _ => throw new ArgumentException("Invalid value for TransferStatus", nameof(TransferStatus))
        };

    }
}