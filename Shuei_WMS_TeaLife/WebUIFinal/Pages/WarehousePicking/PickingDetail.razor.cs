using Application.DTOs.Transfer;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebUIFinal.Pages.WarehousePicking
{
    public partial class PickingDetail
    {
        
        WarehousePickingDTO _model { get; set; }
        RadzenDataGrid<WarehousePickingLineDTO> _profileGrid;
        RadzenDataGrid<WarehousePickingShipmentDTO> _profileGridShipment;

        List<WarehousePickingLineDTO> _dataGrid = null;
        List<WarehousePickingShipmentDTO> _dataGridShipment = null;

        string pickNo = null;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        bool _allowRowSelectOnRowClick = false;
        bool _manualEntry = false;

        bool _disableEdit = false;
        bool _visibleEdit = true;

        private WarehousePickingLineDTO selectedProduct = new WarehousePickingLineDTO() { ProductCode = "default"};
        private List<WarehousePickingLineDTO> pickingDetailsBefore = new List<WarehousePickingLineDTO>();
        private List<WarehousePickingLineDTO> pickingDetailsToUpdate = new List<WarehousePickingLineDTO>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            await RefreshDataAsync();
        }

        void ShowTooltip(ElementReference elementReference, TooltipOptions options = null) => _tooltipService.Open(elementReference, $"{_localizer["Manual Entry"]}: {_localizer[_manualEntry.ToString()]}", options);
        async Task OpenAsync(WarehousePickingDTO model)
        {
            var m = model;

            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Info,
                Summary = _localizer["Go to packing"],
                Detail = $"Shipping No:{model.ShipmentNo}",
                Duration = 5000
            });
        }
        async Task RefreshDataAsync()
        {
            try
            {
                _model = await _localStorage.GetItemAsync<WarehousePickingDTO>("PickingDetailTransfer");
                if (_model == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizer["Detail model is null"],
                        Duration = 5000
                    });
                    _navigation.NavigateTo("/pickinglist");
                    return;
                }
                pickNo = _model.PickNo;
                if (_model.Status != EnumShipmentOrderStatus.Picking)
                {
                    _disableEdit = true;
                    _visibleEdit = false;
                }
                var res = await _warehousePickingLineServices.GetPickingLineDTOAsync(pickNo);
                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizer["Get Picking line successfully"],
                        Duration = 5000
                    });
                    return;
                }
                _dataGrid = null;
                _dataGrid = new List<WarehousePickingLineDTO>();
                _dataGrid = res.Data.ToList();

                var shipments = await _warehousePickingLineServices.GetShipmentsByPickAsync(pickNo);
                if (!shipments.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizer["Get Picking line failed"],
                        Duration = 5000
                    });
                    return;
                }

                _dataGridShipment = null;
                _dataGridShipment = new List<WarehousePickingShipmentDTO>();
                _dataGridShipment = shipments.Data.ToList();

                StateHasChanged();
                await LoadPickingDetailsAsync();
                Remaining();
            }
            catch (UnauthorizedAccessException) { }
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
        async Task CompleteAsync()
        {
            if (pickingDetailsToUpdate.Any())
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _localizerCommon["Warning"],
                    Detail = _localizer["MustKeep"],
                    Duration = 5000
                });
                return;
            }
            string messengerconfirm = $"{_localizerCommon["Complete"]}: {pickNo}";
            int i = 0;
            foreach (var pickDetail in _dataGrid)
            {
                if (pickDetail.Remaining != 0)
                {
                    i ++;
                }
            }
            if (i > 0) messengerconfirm += _localizer["InsufficientQuantity"];
            try
            {
                var confirm = await _dialogService.Confirm(messengerconfirm + "?", _localizerCommon["Complete"], new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _warehousePickingListServices.CompletePickingAsync(pickNo);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = $"{pickNo}" + _localizer["completed"],
                        Duration = 5000
                    });
                    await _localStorage.RemoveItemAsync("PickingDetail");
                    await Task.Delay(1000);
                    _navigation.NavigateTo("/pickinglist", true);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = $"{pickNo}" + _localizer["completion failed"],
                    Duration = 5000
                });
            }
        }
        async Task HTSync() 
        {
            var result = await _warehousePickingListServices.SyncToHTTmpAsync(_dataGrid);
            _dataGrid = result.Data;
            foreach (var row in _dataGrid)
            {
                pickingDetailsToUpdate.Add(row);
                await _profileGrid.UpdateRow(row);
            }
            Remaining();
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = _localizer["Sync Completed"],
                Detail = _localizer["The synchronization has been successfully completed"],
                Duration = 5000
            });
        }
        async Task PrintPickSlip()
        {
            string barcodeBase64s = "";
            var result = await _warehousePickingListServices.GetBarcodeDataAsync(_model.PickNo);
            if (result != "")
            {
                barcodeBase64s = result;
            }
            else
            {
                Console.WriteLine("Error");
            }
            var _dataTransfer = new List<PickingSlipInfos>
            {
                new PickingSlipInfos(_model, _dataGridShipment, _dataGrid, barcodeBase64s)
            };

            await _localStorage.SetItemAsync("PickingSlipInfosTransfer", _dataTransfer);

            // Điều hướng mà không cần truyền dữ liệu qua URL
            await JSRuntime.InvokeVoidAsync("openTab", "/printpickslip");
        }
        async Task DeleteAsync()
        {
            try
            {
                var confirm = await _dialogService.Confirm(
                                    $"{_localizer["PickingNo"]}: {pickNo}?<br/>{_localizer["DeleteNoti"]}<br/>{_localizer["DeleteWarning"]}",
                                    _localizerCommon["Delete"],
                                    new ConfirmOptions()
                                    {
                                        OkButtonText = _localizerCommon["Yes"],
                                        CancelButtonText = _localizerCommon["No"],
                                        AutoFocusFirstElement = true
                                    });

                if (confirm == null || confirm == false) return;

                var res = await _warehousePickingListServices.DeletePickingAsync(pickNo);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = _localizer["successfully"],
                        Duration = 5000
                    });

                    await Task.Delay(1000);
                    _navigation.NavigateTo("/pickinglist", true);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = _localizer["Failed to delete"] + $"{pickNo}.",
                    Duration = 5000
                });
            }
        }
        async Task Cancel() {
            pickingDetailsBefore.Clear();
            pickingDetailsToUpdate.Clear();
            _navigation.NavigateTo("/pickinglist");

        }
        async Task EditRow(WarehousePickingLineDTO detail)
        {
            if (!CheckSelectedProduct())
            {
                await _profileGrid.EditRow(detail);
                selectedProduct = detail;
            }
        }
        async Task SaveRow(WarehousePickingLineDTO detail)
        {
            if (string.IsNullOrEmpty(detail.ProductCode))
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = _localizer["ProductIsRequired"],
                    Duration = 4000
                });
                return;
            }
            var existingDetail = pickingDetailsToUpdate.FirstOrDefault(d => d.Id == detail.Id);

            if (existingDetail != null)
            {
                int index = pickingDetailsToUpdate.IndexOf(existingDetail);
                pickingDetailsToUpdate[index] = detail;
            }
            else
            {
                pickingDetailsToUpdate.Add(detail);
            }
            selectedProduct = null;
            await _profileGrid.UpdateRow(detail);
            Remaining();
        }
        void CancelEdit(WarehousePickingLineDTO detail)
        {
            selectedProduct = null;
            _profileGrid.CancelEditRow(detail);
        }
        private async Task KeepAsync()
        {
            if (!CheckSelectedProduct())
            {
                var changedDetails = pickingDetailsToUpdate.Where(updatedDetail =>
                {
                    var originalDetail = pickingDetailsBefore
                        .FirstOrDefault(beforeDetail => beforeDetail.ProductCode == updatedDetail.ProductCode);
                    return originalDetail == null || !AreDetailsEqual(originalDetail, updatedDetail);
                }).ToList();

                // Check the count of changedDetails
                if (changedDetails.Count <= 0)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Info,
                        Summary = _localizer["Notification"],
                        Detail = _localizer["No changes detected"],
                        Duration = 5000
                    });
                    return; // Exit the method if there are no changes
                }

                // Call UpdateWarehousePickingLinesAsync and check the result
                var updateResult = await _warehousePickingLineServices.UpdateWarehousePickingLinesAsync(changedDetails);

                if (updateResult.Succeeded)
                {
                    pickingDetailsToUpdate.Clear();
                    await RefreshDataAsync();
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = _localizer["successfully"],
                        Duration = 5000
                    });
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Failure"],
                        Detail = updateResult.Messages.FirstOrDefault() ?? _localizer["ErrorUpdate"],
                        Duration = 5000
                    });
                }
            }
        }
        bool AreDetailsEqual(WarehousePickingLineDTO detail1, WarehousePickingLineDTO detail2)
        {
            return detail1.ActualQty == detail2.ActualQty && detail1.Lot == detail2.Lot && detail1.Bin == detail2.Bin;
        }
        private List<EnumDisplay<EnumShipmentOrderStatus>> GetDisplayStatus()
        {
            return Enum.GetValues(typeof(EnumShipmentOrderStatus)).Cast<EnumShipmentOrderStatus>().Select(_ => new EnumDisplay<EnumShipmentOrderStatus>
            {
                Value = _,
                DisplayValue = GetValueLocalizedStatus(_)
            }).ToList();
        }
        private string GetValueLocalizedStatus(EnumShipmentOrderStatus enumStatus)
        {
            return _localizerEnum[enumStatus.ToString()];
        }
        private async Task LoadPickingDetailsAsync()
        {
            var res = await _warehousePickingLineServices.GetPickingLineDTOAsync(pickNo);

            if (!res.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = res.Messages.FirstOrDefault(),
                    Duration = 5000
                });
                return;
            }
            pickingDetailsBefore = res.Data.ToList();
        }
        private bool CheckSelectedProduct()
        {
            if (selectedProduct != null && selectedProduct.ProductCode != "default")
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _localizerCommon["Warning"],
                    Detail = _localizer["MustComplete"],
                    Duration = 5000
                });
                return true;
            }
            return false;
        }
        private async void Remaining()
        {
            // Kiểm tra nếu _dataGrid không null hoặc trống
            if (_dataGrid != null && _dataGrid.Any())
            {
                foreach (var d in _dataGrid)
                {
                    // Kiểm tra giá trị hợp lệ cho PickQty và ActualQty
                    if (d.PickQty != null && d.ActualQty != null)
                    {
                        d.Remaining = d.PickQty - d.ActualQty;
                    }
                    else
                    {
                        d.Remaining = 0; // Hoặc giá trị mặc định nào đó
                    }
                }
                await _profileGrid.Reload();
            }
        }
        async void PrintCoverSheetNDeliveryNote()
        {
            var data = await _warehousePickingListServices.GetDataCoverSheetNDeliveryNote(new List<string> { _model.PickNo });
            await _localStorage.SetItemAsync("CoverSheetNDeliveryNotes", data);
            await JSRuntime.InvokeVoidAsync("openTab", "/coversheetNdeliverynote");
        }
    }
}
