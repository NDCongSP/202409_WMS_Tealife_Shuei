using Application.DTOs.Request.shipment;
using Microsoft.AspNetCore.Components;
using Application.Models;
using Microsoft.JSInterop;

namespace WebUIFinal.Pages.PackingList
{
    public partial class PackingListDetail
    {
        List<PackingListModel> _dataGrid = null;
        RadzenDataGrid<PackingListModel> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        bool _allowRowSelectOnRowClick = false;
        bool _manualEntry = false;
        string _productScan = string.Empty;

        IList<PackingListModel> _gridSelected = [];

        bool _disable = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            await RefreshDataAsync();
        }

        void ShowTooltip(ElementReference elementReference, TooltipOptions options = null) => _tooltipService.Open(elementReference, $"{_localizerCommon["Print"]}: {_localizer[_manualEntry.ToString()]}", options);

        async Task OpenAsync(WarehousePackingListDto model)
        {
            var m = model;

            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Info,
                Summary = "Go to packing",
                Detail = $"Shipping No:{model.ShipmentNo}",
                Duration = 5000
            });
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");

            if (!editorFocused && editor != null)
            {
                editorFocused = true;

                try
                {
                    await editor.FocusAsync();
                }
                catch
                {
                    //
                }
            }
        }

        async Task RefreshDataAsync()
        {
            try
            {
                if (_MasterTransferToDetails.TransferToPackingDetail == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = "Detail model is null",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/packinglist");
                    return;
                }

                _dataGrid = null;
                _dataGrid = new List<PackingListModel>();
                _dataGrid = _MasterTransferToDetails.TransferToPackingDetail.ShipmentLines;

                //await _profileGrid.RefreshDataAsync();

                StateHasChanged();
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

        async Task DeleteAsync()
        {

        }

        async Task CompleteAsync()
        {
            var confirm = await _dialogService.Confirm($"{_localizer["Do you complete the packing?"]}"
               , $"{_localizer["Shipment No."]}: {_MasterTransferToDetails.TransferToPackingDetail.ShipmentNo}", new ConfirmOptions()
               {
                   OkButtonText = _localizerCommon["Yes"],
                   CancelButtonText = _localizerCommon["No"],
                   AutoFocusFirstElement = true,
               });

            if (confirm == null || confirm == false) return;

            //DateTime? dateTimeValue = someObject.Date;
            //DateOnly? dateOnlyValue = dateTimeValue.HasValue ? DateOnly.FromDateTime(dateTimeValue.Value) : (DateOnly?)null;

            List<PackingListUpdatePackedQtyRequestDto> updateModel = new List<PackingListUpdatePackedQtyRequestDto>();

            foreach (var item in _MasterTransferToDetails.TransferToPackingDetail.ShipmentLines)
            {
                if (item.PackedQty > 0)
                    updateModel.Add(new PackingListUpdatePackedQtyRequestDto()
                    {
                        Id = item.Id,
                        PackedQty = item.PackedQty,
                        StatusShipment = EnumShipmentOrderStatus.Packed,
                        StatusIssueWhTran = EnumStatusIssue.Delivered,
                        PackedDate = DateOnly.FromDateTime(DateTime.Now),
                        ShipmentNo = item.ShipmentNo,
                    });
            }

            if (updateModel.Count == 0) return;

            var responseUpdate = await _packingListServices.CompletePackingAsync(updateModel);

            if (responseUpdate.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                     Summary = _localizerCommon["Success"],
                    Detail = responseUpdate.Messages.FirstOrDefault(),
                    Duration = 5000
                });
            }
            else
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = responseUpdate.Messages.FirstOrDefault(),
                    Duration = 5000
                });
            }
        }

        async Task SaveAsync()
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}"
                , $"{_localizer["Shipment No."]}: {_MasterTransferToDetails.TransferToPackingDetail.ShipmentNo}", new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

            if (confirm == null || confirm == false) return;

            List<PackingListUpdatePackedQtyRequestDto> updateModel = new List<PackingListUpdatePackedQtyRequestDto>();

            foreach (var item in _MasterTransferToDetails.TransferToPackingDetail.ShipmentLines)
            {
                if (item.PackedQty > 0)
                    updateModel.Add(new PackingListUpdatePackedQtyRequestDto()
                    {
                        Id = item.Id,
                        PackedQty = item.PackedQty,
                        StatusShipment = EnumShipmentOrderStatus.Packing,
                        //StatusIssueWhTran=EnumStatusIssue.Packing,
                        ShipmentNo = item.ShipmentNo,
                    });
            }

            if (updateModel.Count == 0) return;

            var responseUpdate = await _packingListServices.UpdatePackedQtyAsync(updateModel);

            if (responseUpdate.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                     Summary = _localizerCommon["Success"],
                    Detail = responseUpdate.Messages.FirstOrDefault(),
                    Duration = 5000
                });
            }
            else
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = responseUpdate.Messages.FirstOrDefault(),
                    Duration = 5000
                });
            }
        }

        async Task CancelAsync() => _navigation.NavigateTo("/packinglist");

        async Task OnChange(string value)
        {
            _productScan = value;
            Console.WriteLine($"Manucl entry:{_productScan}");

            var productInfo = _MasterTransferToDetails.TransferToPackingDetail.ShipmentLines.FirstOrDefault(x => x.ProductCode == _productScan);

            if (productInfo != null)
            {
                if (productInfo.PackedQty + 1 > productInfo.ShipmentQty)
                {
                    _dialogService.Alert($"{_productScan} {_localizer["is fully packed"]}.{Environment.NewLine} {_localizer["Shipment Order Qty"]}:{productInfo.ShipmentQty}. {_localizer["Packed Qty"]}: {productInfo.PackedQty + 1}"
                                        , _localizerCommon["Warning"]
                                        , new AlertOptions()
                                        {
                                            OkButtonText = _localizerCommon["Yes"],
                                            CloseDialogOnOverlayClick = true,
                                            CloseDialogOnEsc = true,
                                            ShowClose = false,
                                            CssClass = "background-color:yellow",
                                            Style = "background-color: rgb(255, 158, 0)",
                                        });

                    await _jsRuntime.InvokeVoidAsync("FocusElementText", "_btn");
                    await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");
                    return;
                }
                productInfo.PackedQty += 1;
                //productInfo.Remaining = productInfo.ShipmentQty - productInfo.PackedQty;
            }
            else
            {
                _dialogService.Alert($"{_productScan}{Environment.NewLine}{_localizer["do not exist in the shipment"]}"
                                        , _localizerCommon["Warning"]
                                        , new AlertOptions()
                                        {
                                            OkButtonText = _localizerCommon["Yes"],
                                            CloseDialogOnOverlayClick = true,
                                            CloseDialogOnEsc = true,
                                            ShowClose = false,
                                            CssClass= "background-color:yellow",
                                            Style = "background-color: rgb(255, 158, 0)",
                                        });
                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_btn");
                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");
                return;
            }

            // await RefreshDataAsync();

            await _jsRuntime.InvokeVoidAsync("FocusElementText", "_btn");
            await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");
        }

        private async Task OnClick(bool value)
        {
            //_manualEntry = value;
            //Console.WriteLine($"Manucl entry:{_manualEntry}");

            //await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");
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
    }
}
