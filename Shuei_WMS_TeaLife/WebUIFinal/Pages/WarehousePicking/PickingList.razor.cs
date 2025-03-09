using Application.DTOs.Request.Picking;
using WebUIFinal.Core.Dto;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using RestEase;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using Application.DTOs.Request;

namespace WebUIFinal.Pages.WarehousePicking
{
    public partial class PickingList
    {
        

        List<WarehousePickingDTO> _dataGrid = null;
        RadzenDataGrid<WarehousePickingDTO> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        private int _count, _pageNumber = 1, _pageSize = 20;
        bool allowRowSelectOnRowClick = false;
        private List<WarehousePickingDTO> _selectedPicking = new List<WarehousePickingDTO>();

        PickingListSearchRequestDto _searchModel = new PickingListSearchRequestDto();

        IList<WarehousePickingDTO> _gridSelected = [];
        bool _disable = false;

        EnumShipmentOrderStatus? _selectStatus;

        List<Location> _locations = [];
        Location _locationSelect;
        List<Bin> _bins = [];
        Bin _binSelect;
        DateOnly? _planShipDateFrom, _planShipDateTo;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            _selectStatus = null;

            var locationResponse = await _locationServices.GetAllAsync();
            if (!locationResponse.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Get location error"],
                    Detail = locationResponse.Messages.FirstOrDefault(),
                    Duration = 5000
                });
                return;
            }
            _locations = locationResponse.Data.ToList();

            await RefreshDataAsync(_searchModel);
        }
        async Task LoadData(LoadDataArgs args)
        {
            _pageNumber = (int)((args.Skip / args.Top) + 1);
            _pageSize = (int)args.Top;
            _selectedPicking = new List<WarehousePickingDTO>();
            await RefreshDataAsync(_searchModel);
        }
        async Task RefreshDataAsync(PickingListSearchRequestDto model)
        {
            try
            {
                var queryModel = new QueryModel<PickingListSearchRequestDto>
                {
                    PageNumber = _pageNumber,
                    PageSize = _pageSize,
                    Entity = model
                };
                var res = await _warehousePickingListServices.GetWarehousePickingDTOAsync(queryModel);

                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizer["GetPickingSuccess"],
                        Duration = 5000
                    });
                    _selectedPicking = new List<WarehousePickingDTO>();
                    return;
                }

                _dataGrid = null;
                _dataGrid = new List<WarehousePickingDTO>();
                _dataGrid = res.Data.ToList();

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = _localizer["Get Picking Failed"],
                    Duration = 5000
                });
                return;
            }
        }

        async Task ClearFilter()
        {
            _binSelect = null;
            _locationSelect = null;
            _selectStatus = null;
            _searchModel = null;
            _planShipDateFrom = null;
            _planShipDateTo = null;
            _searchModel = new PickingListSearchRequestDto();
            await RefreshDataAsync(_searchModel);
        }

        async Task OpenAsync(WarehousePickingDTO model)
        {
            await _localStorage.SetItemAsync("PickingDetailTransfer", model);
            _navigation.NavigateTo("/pickingdetail");
        }
        private async Task OpenShipmentDetail(string shipmentNo)
        {
            if (!string.IsNullOrEmpty(shipmentNo))
            {
                var response = await _warehouseShipmentServices.GetByMasterCodeAsync(shipmentNo);
                if (!response.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizer["Could not found shipment no"],
                        Duration = 5000
                    });
                    return;
                }
                WarehouseShipment res = response.Data;
                var shipmentId = res.Id;
                var shipmentType = response.Data.ShipmentType;
                if (shipmentType == EnumWarehouseTransType.Shipment)
                {
                    _navigation.NavigateTo($"/addwarehouseshipment/Edit|{shipmentId}");
                }
                else if (shipmentType == EnumWarehouseTransType.Movement)
                {
                    _navigation.NavigateTo($"/addwarehousemovement/Edit|{shipmentId}");
                }
            }
        }
        async void OnSearch(PickingListSearchRequestDto arg)
        {
            var r = _gridSelected;
            arg.Location = _locationSelect?.LocationName;
            arg.Bin = _binSelect?.BinCode;
            arg.PlanShipDateFrom = _planShipDateFrom;
            arg.PlanShipDateTo = _planShipDateTo;
            arg.Status = _selectStatus;
            await RefreshDataAsync(arg);
        }

        async Task GetBin()
        {
            if (_locationSelect == null) return;
            var binResponse = await _binServices.GetByLocationId(_locationSelect.Id);

            if (!binResponse.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Get bin error"],
                    Detail = binResponse.Messages.FirstOrDefault(),
                    Duration = 5000
                });
                return;
            }
            _bins = binResponse.Data.ToList();
        }

        private List<EnumDisplay<EnumShipmentOrderStatus>> GetDisplayStatus()
        {
            return Enum.GetValues(typeof(EnumShipmentOrderStatus))
               .Cast<EnumShipmentOrderStatus>()
               .Where(status => status == EnumShipmentOrderStatus.Picking || status == EnumShipmentOrderStatus.Picked)
               .Select(status => new EnumDisplay<EnumShipmentOrderStatus>
               {
                   Value = status,
                   DisplayValue = GetValueLocalizedStatus(status)
               })
               .ToList();
        }

        private string GetValueLocalizedStatus(EnumShipmentOrderStatus? enumStatus)
        {
            return _localizerEnum[enumStatus.ToString()];
        }

        void OnRowSelect(object isAdd, WarehousePickingDTO d)
        {
            _selectedPicking = _selectedPicking == null ? new List<WarehousePickingDTO>() : _selectedPicking;
            if ((bool)isAdd == true)
            {
                _selectedPicking.Add(d);
            }
            else
            {
                _selectedPicking = _selectedPicking.Except([d]).ToList();
            }
        }

        async void PrintCoverSheetNDeliveryNote()
        {
            if (_selectedPicking.Count == 0)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = _localizer["PleaseSelectPicking"],
                    Duration = 5000
                });
                return;
            }
            var ids = _selectedPicking.Where(x => !string.IsNullOrEmpty(x.ShipmentNo)).Select(s => s.PickNo).ToList();
            var data = await _warehousePickingListServices.GetDataCoverSheetNDeliveryNote(ids);
            await _localStorage.SetItemAsync("CoverSheetNDeliveryNotes", data);
            await JSRuntime.InvokeVoidAsync("openTab", "/coversheetNdeliverynote");
        }

        async void CompletedMultiplePicking()
        {
            if (_selectedPicking.Count == 0)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = _localizer["PleaseSelectPicking"],
                    Duration = 5000
                });
                return;
            }
            var pickingNos = _selectedPicking.Select(s => s.PickNo).ToList();
            var result = await _warehousePickingListServices.CompletePickingsAsync(pickingNos);
            if (result.Succeeded)
            {
                await RefreshDataAsync(_searchModel);
                _selectedPicking = new List<WarehousePickingDTO>();
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = _localizerCommon["Success"],
                    Detail = _localizer["CompletedPickingSuccessfully"],
                    Duration = 4000
                });
                StateHasChanged();
            }
            else
            {
                string errors = string.Join(',', result.Messages);
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = errors,
                });
            }
        }
    }
}
