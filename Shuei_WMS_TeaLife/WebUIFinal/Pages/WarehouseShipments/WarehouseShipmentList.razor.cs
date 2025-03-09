using Application.DTOs.Request;
using Application.Extentions.Pagings;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

namespace WebUIFinal.Pages.WarehouseShipments
{
    public partial class WarehouseShipmentList
    {
        private PageList<WarehouseShipmentDto> _warehouseShipments;
        private RadzenDataGrid<WarehouseShipmentDto> _warehouseShipmentGrid;
        private bool _showPagerSummary = true;
        private WarehouseShipmentSearchModel _searchModel = new WarehouseShipmentSearchModel() { Type = EnumWarehouseTransType.Shipment, EstimateShipDateFrom = DateOnly.FromDateTime(DateTime.Today) };
        private int _count, _pageNumber = 1, _pageSize = 30;
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
        private List<CompanyTenant> _tenants;
        private List<Bin> _bins;
        private bool disabledCreatePicking = true;
        private async Task GetMasterDataAsync()
        {
            try
            {
                var locationResult = await _locationServices.GetAllAsync();
                var tenantResult = await _companiesServices.GetAllAsync();
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
        private async Task Clear()
        {
            _searchModel = new WarehouseShipmentSearchModel() { Type = EnumWarehouseTransType.Shipment };
            await RefreshDataAsync();
        }
        void EditItemAsync(Guid shipmentId) => _navigation.NavigateTo("/addwarehouseshipment/Edit | " + shipmentId);

        void AddNewItemAsync() => _navigation.NavigateTo("/addwarehouseshipment/Create");

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

        async Task CreatePickingAsync()
        {
            if (_selectedShipment.Count == 0)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _commonLocalizer["Warning"],
                    Detail = _shipmentLocalizer["PleaseSelectShipment"],
                    Duration = 5000
                });
                return;
            }
            if(!_selectedShipment.Any(x => x.Status == EnumShipmentOrderStatus.Open))
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _commonLocalizer["Warning"],
                    Detail = _shipmentLocalizer["ShipmentStatusOpen"],
                    Duration = 5000
                });
                return;
            }
            var d = new SubmitCompletedShipmentDto
            {
                Id = _selectedShipment.Select(x => x.Id).ToList()
            };

            var res = await _dialogService.OpenAsync<DialogCreatePicking>($"{_commonLocalizer["Detail.View"]} {_shipmentLocalizer["Picking"]}",
               new Dictionary<string, object>() { { "_model", d }, { "VisibleBtnSubmit", true } },
               new DialogOptions()
               {
                   Width = "800",
                   Height = "400",
                   Resizable = true,
                   Draggable = true,
                   CloseDialogOnOverlayClick = true
               });
            if (res != null)
            {
                _selectedShipment = new List<WarehouseShipmentDto>();
                await RefreshDataAsync();
            }
        }

        async void PrintDeliveryNote()
        {
            if (_selectedShipment.Count == 0)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _commonLocalizer["Warning"],
                    Detail = _shipmentLocalizer["PleaseSelectShipment"],
                    Duration = 5000
                });
                return;
            }
            var _dataTransfer = _selectedShipment.Select(s => s.Id).ToList();
            await _localStorage.SetItemAsync("DeliveryNoteTransfer", _dataTransfer);
            await _jsRuntime.InvokeVoidAsync("openTab", "/deliverynote");
        }

        async void DLPickup()
        {
            if (_selectedShipment.Count == 0)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _commonLocalizer["Warning"],
                    Detail = _shipmentLocalizer["PleaseSelectShipment"],
                    Duration = 5000
                });
                return;
            }
            var _dataTransfer = _selectedShipment.Select(s => s.Id).ToList();
            var result = await _warehouseShipmentServices.DHLPickup(_dataTransfer);
            if (result.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = _commonLocalizer["Success"],
                    Detail = _shipmentLocalizer["DHLPickupSuccessfully"],
                    Duration = 4000
                });
            }
            else
            {
                NotifyError(result.Messages);
            }
        }

        async void CompletedMultipleShipment()
        {
            if (_selectedShipment.Count == 0)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _commonLocalizer["Warning"],
                    Detail = _shipmentLocalizer["PleaseSelectShipment"],
                    Duration = 5000
                });
                return;
            }
            if (!_selectedShipment.Any(x => x.Status == EnumShipmentOrderStatus.Packed))
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _commonLocalizer["Warning"],
                    Detail = _shipmentLocalizer["ShipmentStatusPacked"],
                    Duration = 5000
                });
                return;
            }
            var shipmentNos = _selectedShipment.Select(s => s.ShipmentNo).ToList();
            var result = await _warehouseShipmentServices.CompleteShipmentsAsync(shipmentNos);
            if (result.Succeeded)
            {
                await RefreshDataAsync();
                if(_selectedShipment.Count(x => x.Status == EnumShipmentOrderStatus.Packed || x.Status == EnumShipmentOrderStatus.Completed) < _selectedShipment.Count())
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _commonLocalizer["Success"],
                        Detail = _shipmentLocalizer["CompletedShipmentSuccessfullyWithNote"],
                        Duration = 4000
                    });
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _commonLocalizer["Success"],
                        Detail = _shipmentLocalizer["CompletedShipmentSuccessfully"],
                        Duration = 4000
                    });
                }
                _selectedShipment = new List<WarehouseShipmentDto>();
                StateHasChanged();
            }
            else
            {
                string errors = string.Join(',', result.Messages);
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = errors,
                });
            }
        }

        async void GetLaterPayment()
        {
            if (_selectedShipment.Count == 0)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = _shipmentLocalizer["PleaseSelectShipment"],
                    Duration = 5000
                });
                return;
            }

            string trackingNos = String.Join('-', _selectedShipment.Select(x => x.TrackingNo).ToList());
            //string trackingNos = "RN073814037JP-RN073800832JP-RN073760585JP";

            var requestData = new LaterPayRequest()
            {
                laterPayNumbers = trackingNos
            };

            var r = await _commonServices.GetLaterPaymentAsync(requestData);

            if (!r.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = r.Messages.FirstOrDefault(),
                    Duration = 5000
                });
                return;
            }

            Console.WriteLine(r);

            await _localStorage.SetItemAsync("base64PdfString", r.Data);
            await _jsRuntime.InvokeVoidAsync("openTab", "/showpdffrombase64");
        }

        async void DownloadCSV()
        {
            var r = await _commonServices.GetDataForExportCsvAsync();

            if (!r.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = r.Messages.FirstOrDefault(),
                    Duration = 5000
                });
                return;
            }

            try
            {
                byte[] data = Convert.FromBase64String(r.Data);
                await _jsRuntime.InvokeVoidAsync("downloadFileFromBytes", data, $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}_export.csv", "text/csv");
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error, 
                    Summary = _commonLocalizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }

        async Task ImportCSV()
        {
            await _dialogService.OpenAsync<DialogUploadCSVFileJQJP>(_shipmentLocalizer["Import CSV"],
                new Dictionary<string, object>(),
                new DialogOptions()
                {
                    Width = "500px",
                    Height = "400px",
                    CloseDialogOnEsc = true,
                    CloseDialogOnOverlayClick = true,
                    ShowClose = false,
                    Draggable = true
                });
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
                    Detail = _shipmentLocalizer[item],
                    Duration = 4000
                });
            }
        }
        void ShowTooltip(string text, ElementReference elementReference, TooltipOptions options = null) => tooltipService.Open(elementReference, text, options);
    }

}
