using Application.DTOs.Response;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RestEase;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WebUIFinal.Pages.Supplier
{
    public partial class SupplierMaster
    {


        RadzenDataGrid<SupplierTenantDTO> _profileGrid;
        List<SupplierTenantDTO> supplier = new();

        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        private List<CompanyTenant> tenants = new List<CompanyTenant>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await RefreshDataAsync();
            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";
            await GetTenantsAsync();
            _filteredModel = new List<SupplierTenantDTO>(supplier);

        }
        async Task OpenAsync(SupplierTenantDTO model)
        {
            //await _localStorage.SetItemAsync("SupplierDetailTransfer", model);
            _MasterTransferToDetails.SupplierInfo = model;
            _navigation.NavigateTo($"/detailsupplier/{_localizerCommon["Detail.Edit"]}");
        }
        private async Task GetTenantsAsync()
        {
            try
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

                tenants = res.Data;

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService
                       , NotificationSeverity.Error
                       , _localizerNotification["Error"], $"{ex.Message}|{ex.InnerException}");
                return;
            }
        }
        async Task CreateAsync() => _navigation.NavigateTo($"/detailsupplier/{_localizerCommon["Detail.Create"]}");
        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _suppliersServices.GetSupplierWithTenantAsync();

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);
                }

                supplier = res.Data.ToList();
                _filteredModel = supplier;
                StateHasChanged();
            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService
                       , NotificationSeverity.Error
                       , _localizerNotification["Error"], $"{ex.Message}|{ex.InnerException}");
                return;
            }
        }
    }
}