using Application.DTOs.Request.Products;
using Application.DTOs.Response;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ProductModel = FBT.ShareModels.Entities.Product;

namespace WebUIFinal.Pages.Product
{
    public partial class ProductList
    {
        List<ProductsDTO> _dataGrid = null;
        RadzenDataGrid<ProductsDTO> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        bool allowRowSelectOnRowClick = false;
        private CompanyTenant _selectedTenant;
        private List<CompanyTenant> _tenants = [];

        ProductSearchRequestDTO _searchModel = new ProductSearchRequestDTO();

        EnumProductStatus _selectStatus;
        List<ProductCategory> _productCategories = [];
        ProductCategory _selectCatetgory = null;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Constants.PagingSummaryFormat = _localizer["DisplayPage"] + " {0} " + _localizer["Of"] + " {1} <b>(" + _localizer["Total"] + " {2} " + _localizer["Records"] + ")</b>";

            await RefreshDataAsync(new ProductSearchRequestDTO());
        }

        async Task DeleteItemAsync(ProductModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Delete"] + _localizer["Product"] + $": {model.ProductName}?", _localizer["Delete"] + " " + _localizer["Product.Name"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _productServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizer["Success"],
                        Detail = $"Delete product {model.ProductName} successfully.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/productList");
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = res.Messages.ToString(),
                        Duration = 5000
                    });
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }

        void EditItemAsync(int productId) => _navigation.NavigateTo($"/addproduct/{_localizer["Detail.Edit"]} {_localizer["Product"]}|" + productId);

        void AddNewItemAsync() => _navigation.NavigateTo($"/addproduct/{_localizer["Detail.Create"]} {_localizer["Product"]}");

        void NavigateDetailPage(int productId) => _navigation.NavigateTo($"/addproduct/{_localizer["Detail.View"]} {_localizer["Product"]}|{productId}");

        private async Task GetTenantsAsync()
        {
            var res = await _companiesServices.GetAllAsync();

            if (!res.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
               , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
               , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }
            _tenants.AddRange(res.Data);
        }
        async Task RefreshDataAsync(ProductSearchRequestDTO model)
        {
            try
            {
                var data = await _productCategoryServices.GetAllAsync();
                if (!data.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(data.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                   , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                   , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }
                _productCategories = data.Data;

                var res = await _productServices.GetAllDtoAsync(model);

                await GetTenantsAsync();

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                   , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                   , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _dataGrid = res.Data.ToList();

                StateHasChanged();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], $"{ex.Message}|{ex.InnerException}");
            }
        }

        async void Submit(ProductSearchRequestDTO arg)
        {
            arg.CategoryId = _selectCatetgory is null ? 0 : _selectCatetgory.Id;
            arg.TenantId = _selectedTenant is null ? 0 : _selectedTenant.AuthPTenantId;
            //arg.ProductStatus = _selectStatus;

            RefreshDataAsync(arg);
        }
        async Task ClearFilter()
        {
            _selectedTenant = null;
            _selectCatetgory = null;
            _selectStatus = EnumProductStatus.All;
            _searchModel = null;
            _searchModel = new ProductSearchRequestDTO();
            RefreshDataAsync(_searchModel);
        }
    }
}