using Application.DTOs.Request;
using Application.Extentions.Pagings;

namespace WebUIFinal.Pages.Movements
{
    public partial class WarehouseMovementList
    {
        private PageList<WarehouseShipmentDto> _warehouseShipments;
        private RadzenDataGrid<WarehouseShipmentDto> _warehouseShipmentGrid;
        private bool _showPagerSummary = true;
        private WarehouseShipmentSearchModel _searchModel = new WarehouseShipmentSearchModel() { Type = EnumWarehouseTransType.Movement };
        private int _count, _pageNumber = 1, _pageSize = 5;
        private IList<WarehouseShipmentDto> _selectedShipment = new List<WarehouseShipmentDto>();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await GetMasterDataAsync();
            await RefreshDataAsync();
            Constants.PagingSummaryFormat = _commonLocalizer["DisplayPage"] + " {0} " + _commonLocalizer["Of"] + " {1} <b>(" + _commonLocalizer["Total"] + " {2} " + _commonLocalizer["Records"] + ")</b>";
        }

        #region MASTER DATA
        private List<Location> _locations;
        private List<TenantAuth> _tenants;
        private List<Bin> _bins;
        private bool disabledCreatePicking = true;
        private async Task GetMasterDataAsync()
        {
            try
            {
                var locationResult = await _locationServices.GetAllAsync();
                var tenantResult = await _tenantsServices.GetAllAsync();
                var binResult = await _binServices.GetAllAsync();

                _locations = locationResult.Data;
                _tenants = tenantResult.Data;
                _bins = binResult.Data;
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = _commonLocalizer["FailedToLoadMasterData"] + ex.Message,
                    Duration = 5000
                });
            }
        }
        #endregion

        #region BUSSINESS

        void EditItemAsync(Guid shipmentId) => _navigation.NavigateTo("/addwarehousemovement/Edit | " + shipmentId);

        void AddNewItemAsync() => _navigation.NavigateTo("/addwarehousemovement/Create");

        async Task LoadData(LoadDataArgs args)
        {
            _pageNumber = (int)((args.Skip / args.Top) + 1);
            _pageSize = (int)args.Top;
            await RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var model = new QueryModel<WarehouseShipmentSearchModel>
                {
                    Entity = _searchModel,
                    PageNumber = _pageNumber,
                    PageSize = _pageSize
                };
                var result = await _warehouseShipmentServices.SearchWhShipments(model);
                if (result.Succeeded)
                {
                    _warehouseShipments = result.Data;
                }
                else
                {
                    NotifyError(result.Messages);
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }
        #endregion

        void NotifyError(List<string> errors)
        {
            foreach (var item in errors)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = _movementLocalizer[item],
                    Duration = 4000
                });
            }
        }
    }
}
