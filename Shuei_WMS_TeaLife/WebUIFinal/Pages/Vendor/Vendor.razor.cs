using Radzen;
using Radzen.Blazor;
using VendorEntity = FBT.ShareModels.Entities.Vendor;

namespace WebUIFinal.Pages.Vendor
{
    public partial class Vendor
    {
        private List<VendorEntity> _vendors = new List<VendorEntity>();
        RadzenDataGrid<VendorEntity> _profileVendorGrid;
        bool _showPagerSummary = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                var result = await _vendorsServices.GetAllAsync();

                if (result != null)
                {
                    _vendors.AddRange(result.Data);
                    _filteredModel = _vendors;
                }

                _filteredModel = new List<VendorEntity>(_vendors);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            finally
            {
                await RefreshDataAsync();
            }
        }

        async Task AddNewItemAsync() => _navigation.NavigateTo($"/addvendor/Create Vendor");

        async Task EditItemAsync(int vendorId) => _navigation.NavigateTo($"/addvendor/Edit Vendor|{vendorId}");

        async Task DeleteItemAsync(VendorEntity vendor)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete vendor: {vendor.VendorName}?", "Delete vendor", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _vendorsServices.DeleteAsync(vendor);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                         Summary = _localizerCommon["Success"],
                        Detail = res.Messages.ToString(),
                        Duration = 5000
                    });

                    StateHasChanged();
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

                await RefreshDataAsync();
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

        void NavigateDetailPage(int vendorId) => _navigation.NavigateTo($"/addvendor/Vendor Detail|{vendorId}");

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _vendorsServices.GetAllAsync();
                _vendors = null;
                _vendors = new();

                if (res != null)
                    _vendors.AddRange(res.Data);

                //await _profileGrid.RefreshDataAsync();

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