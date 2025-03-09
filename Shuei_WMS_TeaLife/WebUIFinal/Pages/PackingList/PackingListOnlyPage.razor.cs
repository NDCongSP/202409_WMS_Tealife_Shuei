using Application.DTOs.Request.shipment;
using Microsoft.AspNetCore.Components;
using Application.Models;
using Microsoft.JSInterop;
using Radzen;
using System.Security.Policy;
using static Application.Extentions.ApiRoutes;
using Microsoft.AspNetCore.Components.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Application.DTOs.Response;
using Newtonsoft.Json;
using System.ComponentModel.Design;

namespace WebUIFinal.Pages.PackingList
{
    public partial class PackingListOnlyPage
    {
        [Parameter]
        public string Action { get; set; } = "View";

        WarehousePackingListDto _packingData = new WarehousePackingListDto() { PackingStatus = EnumPackingStatus.Beginning };

        List<PackingListGenerateRow> _dataGrid = null;
        RadzenDataGrid<PackingListGenerateRow> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        bool _allowRowSelectOnRowClick = false;
        bool _manualEntry = false;
        string _productScan = string.Empty;
        string _shipmentNo = string.Empty;

        List<FBT.ShareModels.WMS.ShippingBox> _shippingBox = [];
        FBT.ShareModels.WMS.ShippingBox _selectShippingBox = new FBT.ShareModels.WMS.ShippingBox();

        IList<PackingListModel> _gridSelected = [];

        bool _disable = true;

        private RadzenDropDownDataGrid<FBT.ShareModels.WMS.ShippingBox> _dropdownDataGrid; // Adjust the type to your data type
        private string _searchText = string.Empty;
        private string popupID = "popup-boxdropdown"; // Use a unique ID to identify the popup

        List<FBT.ShareModels.WMS.ShippingCarrier> _shippingCarrier = new List<FBT.ShareModels.WMS.ShippingCarrier>();

        string _styleTxtShipmentNo = "background-color: var(--rz-base-light)";
        string _styleTxtProductCode = "background-color: var(--rz-base-light)";
        string _styleSelectBox = "background-color: var(--rz-base-light)";
        string _styleBtnComplete = "background-color: var(--rz-success)";

        bool _checkboxValue = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            if (!string.IsNullOrEmpty(_MasterTransferToDetails.ShipmentNo)) await RefreshDataAsync();

            var resShCarrier = await _shippingCarrierServices.GetAllAsync();

            if (!resShCarrier.Succeeded) return;

            _shippingCarrier = resShCarrier.Data;
        }

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
                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txtOrderNo");
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var response = await _packingListServices.GetDataMasterByShipmentNoAsync(_MasterTransferToDetails.ShipmentNo);

                if (!response.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _packingData = response.Data;
                _packingData.PackingStatus = EnumPackingStatus.Scanning;

                _dataGrid = new List<PackingListGenerateRow>();
                _dataGrid = _packingData.GenerateDetails.OrderBy(x => x.Scanned).ThenBy(x => x.ProductCode).ToList();

                #region Get ShippingBox theo shipmentNo
                if (!string.IsNullOrEmpty(_packingData.ShippingBoxesName))
                {
                    var data = await _shippingBoxServices.GetByShippingCarrierCodeAsync(_packingData.ShippingCarrierCode);

                    if (!data.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.First();

                        NotificationHelper.ShowNotification(_notificationService
                        , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                        , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }
                    _shippingBox = data.Data;

                    _selectShippingBox = _shippingBox.FirstOrDefault(_ => _.Id == _packingData.ShippingBoxesId);

                    #endregion
                }

                StateHasChanged();

            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }

        async Task DeleteAsync()
        {

        }

        async Task CompleteAsync()
        {
            if (!string.IsNullOrEmpty(_packingData.InternalRemarks) && !_checkboxValue)
            {
                var mess = $"{_localizer["Please check the comments."]}";
                await _dialogService.OpenAsync<AlertWarning>($"{_localizerCommon["Warning"]}",
                       new Dictionary<string, object>() { { "WarningMessage", mess } },
                       new DialogOptions()
                       {
                           Width = "800",
                           Height = "200",
                           Resizable = true,
                           Draggable = true,
                           CloseDialogOnOverlayClick = true,
                           Style = "background-color: rgb(255, 200, 40)",
                       });

                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_labScanProduct");
                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");
                return;
            }

            var confirm = await _dialogService.Confirm($"{_localizer["Do you complete the packing?"]}"
               , $"{_localizer["Shipment No."]}: {_packingData.ShipmentNo}", new ConfirmOptions()
               {
                   OkButtonText = _localizerCommon["Yes"],
                   CancelButtonText = _localizerCommon["No"],
                   AutoFocusFirstElement = true,
               });

            if (confirm == null || confirm == false) return;

            //DateTime? dateTimeValue = someObject.Date;
            //DateOnly? dateOnlyValue = dateTimeValue.HasValue ? DateOnly.FromDateTime(dateTimeValue.Value) : (DateOnly?)null;

            if (_selectShippingBox is null)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Warning, _localizerNotification["Warning"]
                    , _localizerNotification["The box is not selected."]);

                return;
            }
            var shippingBoxId = _selectShippingBox.Id;
            var shippingBoxName = _selectShippingBox.BoxName;

            List<PackingListUpdatePackedQtyRequestDto> updateModel = new List<PackingListUpdatePackedQtyRequestDto>();

            foreach (var item in _packingData.ShipmentLines)
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
                        ShippingBoxesId = shippingBoxId,
                        ShippingBoxesName = shippingBoxName,
                    });
            }

            if (updateModel.Count == 0) return;

            var responseUpdate = await _packingListServices.CompletePackingAsync(updateModel);

            if (!responseUpdate.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseUpdate.Messages.FirstOrDefault())?.Errors.First();

                NotificationHelper.ShowNotification(_notificationService
                , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }

            NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);

            _packingData = new WarehousePackingListDto() { PackingStatus = EnumPackingStatus.Beginning };
            _dataGrid = new List<PackingListGenerateRow>();

            //Clear Search text
            _dropdownDataGrid.Reload();
            _dropdownDataGrid.Reset();
            _shippingBox = new List<FBT.ShareModels.WMS.ShippingBox>();
            _selectShippingBox = new FBT.ShareModels.WMS.ShippingBox();

            StateHasChanged();

            await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");
            await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txtOrderNo");
        }

        async Task PrintLabel(string filePath)
        {
            //var dataPrint = await _authenServices.GetReportBase64(_id);
            //filePath = "C:\\20240904_ShueiCongData\\TestIn.pdf";
            var response = await _packingListServices.GetPdfAsBase64Async(filePath);

            if (!response.Succeeded)
            {
                _packingData.PackingStatus = EnumPackingStatus.PrintedLabel;

                var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.First();

                NotificationHelper.ShowNotification(_notificationService
                , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                StateHasChanged();
                return;
            }

            var dataPrint = response.Data;

            //chon may in theo carrier

            var carrierInfo = _shippingCarrier.FirstOrDefault(_ => _.ShippingCarrierCode == _packingData.ShippingCarrierCode);
            Console.WriteLine(carrierInfo.PrinterName);
            await CallApiPost(new PrintData()
            {
                printerName = carrierInfo.PrinterName,
                printData = dataPrint
            });

            //cap nhat trang thai da in packing label xong.
            _packingData.PackingStatus = EnumPackingStatus.Finish;
            //await _jsRuntime.InvokeVoidAsync("FocusElementText", "_btnComplete");
        }

        async Task SaveAsync()
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}"
                , $"{_localizer["Shipment No."]}: {_packingData.ShipmentNo}", new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

            if (confirm == null || confirm == false) return;

            List<PackingListUpdatePackedQtyRequestDto> updateModel = new List<PackingListUpdatePackedQtyRequestDto>();

            foreach (var item in _packingData.ShipmentLines)
            {
                if (item.PackedQty > 0)
                    updateModel.Add(new PackingListUpdatePackedQtyRequestDto()
                    {
                        Id = item.Id,
                        PackedQty = item.PackedQty,
                        StatusShipment = EnumShipmentOrderStatus.Packing,
                        //StatusIssueWhTran=EnumStatusIssue.Packing,
                        ShipmentNo = item.ShipmentNo,
                        ShippingBoxesId = _selectShippingBox?.Id,
                        ShippingBoxesName = _selectShippingBox?.BoxName
                    });
            }

            if (updateModel.Count == 0) return;

            var responseUpdate = await _packingListServices.UpdatePackedQtyAsync(updateModel);

            if (!responseUpdate.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseUpdate.Messages.FirstOrDefault())?.Errors.First();

                NotificationHelper.ShowNotification(_notificationService
                , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }

            NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
        }

        async Task CancelAsync()
        {
            _packingData = new WarehousePackingListDto() { PackingStatus = EnumPackingStatus.Beginning };
            _dataGrid = new List<PackingListGenerateRow>();

            //bat laij popup panel cho DropdownDatagrid.
            //Clear Search text
            _dropdownDataGrid.Reload();
            _dropdownDataGrid.Reset();
            _shippingBox = new List<FBT.ShareModels.WMS.ShippingBox>();
            _selectShippingBox = new FBT.ShareModels.WMS.ShippingBox();
            await _jsRuntime.InvokeVoidAsync("ClosePopupDropDownGrid", popupID, "hidden");

            StateHasChanged();

            await _jsRuntime.InvokeVoidAsync("FocusElementText", "_labScanProduct");
            await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txtOrderNo");
        }

        async Task OnChange(string value)
        {
            string lotNo = string.Empty;

            if (!value.Contains(":"))
            {
                var mess = $"{_localizer["Scan product label only."]}";
                var mess1 = $"{_localizer["(The format seems to be different)"]}";
                await _dialogService.OpenAsync<AlertWarning>($"{_localizerCommon["Warning"]}",
                       new Dictionary<string, object>() { { "WarningMessage", mess }, { "WarningMessage1", mess1 } },
                       new DialogOptions()
                       {
                           Width = "800",
                           Height = "200",
                           Resizable = true,
                           Draggable = true,
                           CloseDialogOnOverlayClick = true,
                           Style = "background-color: rgb(255, 200, 40)",
                       });

                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_labScanProduct");
                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");
                return;
            }

            var charCount = value.GroupBy(c => c)
                             .ToDictionary(g => g.Key, g => g.Count());
            var d = charCount.FirstOrDefault(x => x.Key.ToString() == ":").Value;

            if (d > 2)
            {
                _productScan = value.Split(":")[0];
                lotNo = value.Split(":")[2];
            }
            else
            {
                _productScan = value.Split(":")[0];
                lotNo = value.Split(":")[1];
            }

            var productInfo = _packingData.ShipmentLines.FirstOrDefault(x => x.ProductCode == _productScan);

            if (productInfo == null)
            {
                var mess = $"{_productScan}{Environment.NewLine}{_localizer["do not exist in the shipment"]}";
                var mess1 = $"{Environment.NewLine}{_localizer["(Please check)"]}";

                await _dialogService.OpenAsync<AlertWarning>($"{_localizerCommon["Warning"]}",
                      new Dictionary<string, object>() { { "WarningMessage", mess }, { "WarningMessage1", mess1 } },
                      new DialogOptions()
                      {
                          Width = "800",
                          Height = "200",
                          Resizable = true,
                          Draggable = true,
                          CloseDialogOnOverlayClick = true,
                          Style = "background-color: rgb(255, 200, 40)",
                      });

                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_labScanProduct");
                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");

                return;
            }

            if (productInfo.LotNo != lotNo && !string.IsNullOrEmpty(productInfo.LotNo))
            {
                var mess = $"{_localizer["The lot num does not match."]}";

                await _dialogService.OpenAsync<AlertWarning>($"{_localizerCommon["Warning"]}",
                      new Dictionary<string, object>() { { "WarningMessage", mess } },
                      new DialogOptions()
                      {
                          Width = "800",
                          Height = "200",
                          Resizable = true,
                          Draggable = true,
                          CloseDialogOnOverlayClick = true,
                          Style = "background-color: rgb(255, 200, 40)",
                      });

                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_labScanProduct");
                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");

                return;
            }

            if (productInfo.PackedQty + 1 > productInfo.ShipmentQty)
            {
                //var mess = $"{_productScan} {_localizer["is fully packed"]}.{Environment.NewLine} {_localizer["Shipment Order Qty"]}:{productInfo.ShipmentQty}. {_localizer["Packed Qty"]}: {productInfo.PackedQty}";
                var mess = $"{_productScan} {_localizer["is fully packed"]}";
                var mess1 = $"{Environment.NewLine}{_localizer["(Please check)"]}";

                await _dialogService.OpenAsync<AlertWarning>($"{_localizerCommon["Warning"]}",
                      new Dictionary<string, object>() { { "WarningMessage", mess }, { "WarningMessage1", mess1 } },
                      new DialogOptions()
                      {
                          Width = "800",
                          Height = "200",
                          Resizable = true,
                          Draggable = true,
                          CloseDialogOnOverlayClick = true,
                          Style = "background-color: rgb(255, 200, 40)",
                      });

                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_labScanProduct");
                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");

                return;
            }

            productInfo.PackedQty += 1;

            var selectProduct = _packingData.GenerateDetails.Where(x => x.ProductCode == _productScan);
            foreach (var item in selectProduct)
            {
                if (item.Scanned == "0")
                {
                    item.Scanned = "1";
                    break;
                }
            }

            _dataGrid = new List<PackingListGenerateRow>();
            //_dataGrid = _packingData.GenerateDetails.OrderBy(x => x.Scanned).ThenBy(x => x.ProductCode).ToList();
            _dataGrid = _packingData.GenerateDetails.OrderBy(x => x.ProductCode).ToList();

            #region Check done scan product
            int checkDone = 0;
            foreach (var item in _packingData.ShipmentLines)
            {
                if (item.Remaining == 0) checkDone += 1;
            }

            if (checkDone == _packingData.ShipmentLines.Count)
            {
                _packingData.PackingStatus = EnumPackingStatus.PrintedLabel;

                await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");
                //bat laij popup panel cho DropdownDatagrid.

                await _jsRuntime.InvokeVoidAsync("ClosePopupDropDownGrid", popupID, "visible");
                //goi method chuyen focus den dropDownDataGrid.
                await _dropdownDataGrid.FocusAsync();

                return;
            }
            #endregion
            //StateHasChanged();

            await _jsRuntime.InvokeVoidAsync("FocusElementText", "_labScanProduct");
            await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");
        }

        async Task OnChangeOrderNo(string value)
        {
            _shipmentNo = value;
            Console.WriteLine($"SHIPMENT NO:{_shipmentNo}");

            var response = await _packingListServices.GetDataMasterByShipmentNoAsync(_shipmentNo);

            if (!response.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.First();

                NotificationHelper.ShowNotification(_notificationService
                , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }

            if (response.Data.StatusOfShipment == EnumPackingListStatus.Packed)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Warning
                    , _localizerNotification["Warning"], _localizerNotification["Shipment Completed"]);
                return;
            }

            _packingData = response.Data;
            _packingData.PackingStatus = EnumPackingStatus.Scanning;

            _dataGrid = new List<PackingListGenerateRow>();
            _dataGrid = _packingData.GenerateDetails.OrderBy(x => x.Scanned).ThenBy(x => x.ProductCode).ToList();

            #region Get ShippingBox theo shipmentNo
            var data = await _shippingBoxServices.GetByShippingCarrierCodeAsync(_packingData.ShippingCarrierCode);

            if (!data.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.First();

                NotificationHelper.ShowNotification(_notificationService
                , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }
            _shippingBox = data.Data;

            if (!string.IsNullOrEmpty(_packingData.ShippingBoxesName))
            {
                _selectShippingBox = _shippingBox.FirstOrDefault(_ => _.Id == _packingData.ShippingBoxesId);
                //da chon box in label thi chuyen sang trang thai finish de conplete packing
                _packingData.PackingStatus = EnumPackingStatus.Finish;
            }
            else//neu chua co chon Box, thi phai xet lai xem da quet xong product hay chua de chuyen trang thai packing thanh PrintedLabel de cho chon box in tem
            {
                #region Check done scan product
                int checkDone = 0;
                foreach (var item in _packingData.ShipmentLines)
                {
                    if (item.Remaining == 0) checkDone += 1;
                }

                if (checkDone == _packingData.ShipmentLines.Count)
                {
                    _packingData.PackingStatus = EnumPackingStatus.PrintedLabel;

                    await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");

                    //bat laij popup panel cho DropdownDatagrid.
                    await _jsRuntime.InvokeVoidAsync("ClosePopupDropDownGrid", popupID, "visible");
                }
                #endregion
            }
            #endregion

            StateHasChanged();

            await _jsRuntime.InvokeVoidAsync("FocusElementText", "_txt");
        }
        void OnFocusTxtShipmentNo(FocusEventArgs e)
        {
            _styleTxtShipmentNo = $"background-color: var(--rz-success-lighter)";
        }
        void OnBlurTxtShipmentNo(FocusEventArgs e)
        {
            _styleTxtShipmentNo = $"background-color: var(--rz-white)";
        }

        void OnFocusTxtProductCode(FocusEventArgs e)
        {
            _styleTxtProductCode = $"background-color: var(--rz-success-lighter)";
        }
        void OnBlurTxtProductCode(FocusEventArgs e)
        {
            _styleTxtProductCode = $"background-color: var(--rz-white)";
        }

        void OnFocusSelectBox(FocusEventArgs e)
        {
            _styleSelectBox = $"background-color: var(--rz-success-lighter)";
        }
        void OnBlurSelectBox(FocusEventArgs e)
        {
            _styleSelectBox = $"background-color: var(--rz-danger)";
        }

        void OnFocusBtnComplete(FocusEventArgs e)
        {
            _styleBtnComplete = $"background-color: var(--rz-success-lighter)";
        }
        void OnBlurBtnComplete(FocusEventArgs e)
        {
            _styleBtnComplete = $"background-color: var(--rz-base-light)";
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

        void RowRender(RowRenderEventArgs<PackingListGenerateRow> args)
        {
            args.Attributes.Add("style", $"font-weight: {(args.Data.Scanned == "0" ? "bold" : "normal")};");
        }

        void CellRender(DataGridCellRenderEventArgs<PackingListGenerateRow> args)
        {
            //args.Attributes.Add("style", $"background-color: {(args.Data.ShipmentQty.Substring(0, 2) == "1/" && args.Data.Scanned == "0" ? "var(--rz-success-lighter)" : args.Data.Scanned == "0" ? "var(--rz-base-background-color)" : "var(--rz-base-500)")};");
            args.Attributes.Add("style", $"background-color: {(args.Data.Index == 0 && args.Data.Scanned == "0" ? "var(--rz-success-lighter)" : args.Data.Scanned == "0" ? "var(--rz-base-background-color)" : "var(--rz-base-500)")};");
        }

        private async Task CallApiPost(PrintData model)
        {
            // var printerName = "Microsoft Print to PDF";
            // var printerName = "DocuCentre-V 4070";
            //var printerAddress = "localhost";
            //var printerName = "Canon iR-ADV 4525/4535 UFR II";
            //var printData = "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m6CGTkQMNUdN8RGRa6ET0ZWbm2j1kZhroZPZx3aeHjqZXYTiSuhkdfEO0ZVvh06KA1UMrXbNdUQVQwiUVDv5G2Lof378f49flhsKZW5kc3RyZWFtCmVuZG9iago3IDAgb2JqCjw8Ci9YT2JqZWN0IDw8Ci9JbWFnZTEgNCAwIFIKPj4KPj4KZW5kb2JqCjMgMCBvYmoKPDwKL0NvbnRlbnRzIFsgNSAwIFIgNiAwIFIgXQovQ3JvcEJveCBbIDAuMCAwLjAgNTk1LjMyMDAxIDg0MS45MjAwNCBdCi9NZWRpYUJveCBbIDAuMCAwLjAgNTk1LjMyMDAxIDg0MS45MjAwNCBdCi9QYXJlbnQgMiAwIFIKL1Jlc291cmNlcyA3IDAgUgovUm90YXRlIDAKL1R5cGUgL1BhZ2UKPj4KZW5kb2JqCjIgMCBvYmoKPDwKL0NvdW50IDEKL0tpZHMgWyAzIDAgUiBdCi9UeXBlIC9QYWdlcwo+PgplbmRvYmoKMSAwIG9iago8PAovUGFnZXMgMiAwIFIKL1R5cGUgL0NhdGFsb2cKPj4KZW5kb2JqCjggMCBvYmoKPDwKL0F1dGhvciAoZGV2ZWxvcDEpCi9DcmVhdGlvbkRhdGUgKEQ6MjAyNDEyMTgxNjAzMTArMDAnMDAnKQovTW9kRGF0ZSAoRDoyMDI0MTIxODE2MDMxMCswMCcwMCcpCi9Qcm9kdWNlciAoTWljcm9zb2Z0OiBQcmludCBUbyBQREYpCi9UaXRsZSAoRkJULVdNUykKPj4KZW5kb2JqCnhyZWYKMCA5DQowMDAwMDAwMDAwIDY1NTM1IGYNCjAwMDAwNDgwODMgMDAwMDAgbg0KMDAwMDA0ODAyNCAwMDAwMCBuDQowMDAwMDQ3ODQxIDAwMDAwIG4NCjAwMDAwMDAwMDkgMDAwMDAgbg0KMDAwMDAwMjUzNCAwMDAwMCBuDQowMDAwMDMwNzg4IDAwMDAwIG4NCjAwMDAwNDc3OTEgMDAwMDAgbg0KMDAwMDA0ODEzMiAwMDAwMCBuDQp0cmFpbGVyCjw8Ci9JbmZvIDggMCBSCi9Sb290IDEgMCBSCi9TaXplIDkKPj4Kc3RhcnR4cmVmCjQ4MzAxCiUlRU9GCg==";

            await _jsRuntime.InvokeVoidAsync("callApiPost", model.printerName, model.printData);
        }

        private async Task OnSearchTextChanged(string text)
        {
            string boxName = string.Empty, boxType = string.Empty;
            List<FBT.ShareModels.WMS.ShippingBox> listBoxFilter = new List<FBT.ShareModels.WMS.ShippingBox>();

            if (text.Contains(":"))
            {
                boxName = text.Split(":")[0];
                boxType = text.Split(":")[1];

                // Example: Filter the data source
                listBoxFilter = _shippingBox.Where(item => item.BoxName.Contains(boxName) && item.BoxType == boxType).ToList();
            }
            else
            {
                boxName = text;

                // Example: Filter the data source
                listBoxFilter = _shippingBox.Where(item => item.BoxName.Contains(boxName)).ToList();
            }

            // Perform filtering or other actions based on the search text
            Console.WriteLine($"Search text changed: {_searchText}");

            //_dropdownDataGrid.SelectItem(_selectShippingBox);

            // Close the popup by calling Radzen.closePopup
            //await _jsRuntime.InvokeVoidAsync("Radzen.closePopup", popupId1);
            if (listBoxFilter == null || listBoxFilter.Count != 1) return;

            _selectShippingBox = listBoxFilter.FirstOrDefault();

            await _jsRuntime.InvokeVoidAsync("ClosePopupDropDownGrid", popupID, "hidden");

            // Refresh the dropdown data (optional)
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// DropdownDataGrid OnSelectedItemChanged.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task OnSelectedItemChanged(FBT.ShareModels.WMS.ShippingBox arg)
        {
            Console.WriteLine($"OnSelectedItemChanged: {arg.BoxName}");
            //goi API lay file ve in ra va complete packing

            if (string.IsNullOrEmpty(arg.BoxName) || arg is null || string.IsNullOrEmpty(arg.BoxName) || _packingData.PackingStatus != EnumPackingStatus.PrintedLabel) return;

            _selectShippingBox = arg;

            if (_packingData.ShippingCarrierCode != "JP-YP")
            {
                if (string.IsNullOrEmpty(_packingData.LabelFilePath))
                {
                    NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Warning
                        , _localizerNotification["Warning"], _localizerNotification[$"{_localizer["Tracking Code"]} {_packingData.TrackingNo} {_localizerNotification["don't have LabelFilePath in OrderDispatches table."]}"]);

                    return;
                }

                //var confirm = await _dialogService.Confirm($"{_localizer["Are you sure you have chosen the right box type and want to print delivery notes?"]}"
                //   , $"{_localizerShippingBox["BoxName"]}/{_localizerShippingBox["BoxType"]}: {_selectShippingBox.BoxName}/{_selectShippingBox.BoxType}", new ConfirmOptions()
                //   {
                //       OkButtonText = _localizerCommon["Yes"],
                //       CancelButtonText = _localizerCommon["No"],
                //       AutoFocusFirstElement = true,
                //   });

                //if (confirm == null || confirm == false)
                //{
                //    _packingData.PackingStatus = EnumPackingStatus.PrintedLabel;
                //    return;
                //}

                //await PrintLabel("C:\\20240904_ShueiCongData\\TestIn.pdf");
                await PrintLabel(_packingData.LabelFilePath);
            }
            else
            {
                //cap nhat trang thai da in packing label xong.
                _packingData.PackingStatus = EnumPackingStatus.Finish;
            }

            //await CompleteAsync();
        }
    }
}
