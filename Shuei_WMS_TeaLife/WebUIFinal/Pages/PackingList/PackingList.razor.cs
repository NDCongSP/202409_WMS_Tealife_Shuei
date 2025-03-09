using Application.DTOs.Request.shipment;
using Application.DTOs.Response;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using RestEase;
using System.Reflection;

namespace WebUIFinal.Pages.PackingList
{
    public partial class PackingList
    {
        List<WarehousePackingListDto> _dataGrid = null;
        RadzenDataGrid<WarehousePackingListDto> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        bool allowRowSelectOnRowClick = false;

        PackingListSearchRequestDto _searchModel = new PackingListSearchRequestDto();

        IList<WarehousePackingListDto> _gridSelected = [];
        bool _disable = false;

        DateOnly _from, _to;
        //DateOnly value = DateOnly.FromDateTime(DateTime.Now);

        EnumPackingListStatus _selectStatus;

        List<Location> _locations = [];
        Location _locationSelect;
        List<Bin> _bins = [];
        Bin _binSelect;

        //check permission
        [CascadingParameter] public static Task<AuthenticationState> AuthenticationStateTaskAsync { get; set; }
        private bool _hasPermission; 

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _hasPermission = GlobalVariable.AuthenticationStateTask.HasPermission("Edit");
            var hasPermission = GlobalVariable.AuthenticationStateTask.HasPermission("PackingEdit");

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            _selectStatus = EnumPackingListStatus.All;

            var locationResponse = await _locationServices.GetAllAsync();
            if (!locationResponse.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(locationResponse.Messages.FirstOrDefault())?.Errors.First();

                NotificationHelper.ShowNotification(_notificationService
                , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }
            _locations = locationResponse.Data.ToList();

            RefreshDataAsync(new PackingListSearchRequestDto());
        }

        async Task OpenAsync(WarehousePackingListDto model)
        {
            //var m = model;

            //_MasterTransferToDetails.TransferToPackingDetail = model;
            //_navigation.NavigateTo("/packinglistDetail");

            _MasterTransferToDetails.ShipmentNo = model.ShipmentNo;
            _navigation.NavigateTo("/packing-list-detail/{View}");
        }

        async Task RefreshDataAsync(PackingListSearchRequestDto model)
        {
            try
            {
                var res = await _packingListServices.GetDataMasterAsync(model);

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _dataGrid = null;
                _dataGrid = new List<WarehousePackingListDto>();
                _dataGrid = res.Data.OrderByDescending(x=>x.ShipmentNo).ToList();

                //await _profileGrid.RefreshDataAsync();

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }

        async Task ClearFilter()
        {
            _binSelect = null;
            _locationSelect = null;
            _selectStatus = EnumPackingListStatus.All;
            _searchModel = null;
            _searchModel = new PackingListSearchRequestDto();
            RefreshDataAsync(_searchModel);
        }

        async void Submit(PackingListSearchRequestDto arg)
        {
            var r = _gridSelected;
            //var m = _searchModel;
            arg.DeliveryLocation = _locationSelect?.LocationName;
            arg.OutgoingBin = _binSelect?.BinCode;
            arg.ScheduledShipDateFrom = _from.ToString("yyyy-MM-dd") == "0001-01-01" ? null : _from.ToString("yyyy-MM-dd");
            arg.ScheduledShipDateTo = _to.ToString("yyyy-MM-dd") == "0001-01-01" ? null : _to.ToString("yyyy-MM-dd");
            arg.Status = _selectStatus;
            RefreshDataAsync(arg);
        }

        async Task GetBin()
        {
            if (_locationSelect == null) return;
            var binResponse = await _binServices.GetByLocationId(_locationSelect.Id);

            if (!binResponse.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(binResponse.Messages.FirstOrDefault())?.Errors.First();

                NotificationHelper.ShowNotification(_notificationService
                , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }
            _bins = binResponse.Data.ToList();
        }

        private List<EnumDisplay<EnumPackingListStatus>> GetDisplayStatus()
        {
            return Enum.GetValues(typeof(EnumPackingListStatus)).Cast<EnumPackingListStatus>().Select(_ => new EnumDisplay<EnumPackingListStatus>
            {
                Value = _,
                DisplayValue = GetValueLocalizedStatus(_)
            }).ToList();
        }

        private string GetValueLocalizedStatus(EnumPackingListStatus enumStatus)
        {
            return _localizerEnum[enumStatus.ToString()];
        }

        async Task GoToPacking()
        {
            _MasterTransferToDetails.ShipmentNo = string.Empty;
            _navigation.NavigateTo("/packing-list-detail/{Edit}");
        }
    }
}
