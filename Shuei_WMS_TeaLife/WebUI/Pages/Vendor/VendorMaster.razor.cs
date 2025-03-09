using WebUI.Core;
using VendorEntity = Domain.Entity.Commons.Vendor;

namespace WebUI.Pages.Vendor
{
    public partial class VendorMaster
    {
        private List<VendorEntity> _vendors = new List<VendorEntity>();
        protected override async Task OnInitializedAsync()
        {
            var result = await _vendor.GetAllAsync();

            _vendors = result.Data.ToList();
        }

        async Task DeleteItemAsync(VendorEntity vendor) => await _vendor.DeleteAsync(vendor);

        void NavigateDetailPage(int vendorId) => _navigation.NavigateTo($"{Constants.Routes.Vendor}/{vendorId}");
    }
}
