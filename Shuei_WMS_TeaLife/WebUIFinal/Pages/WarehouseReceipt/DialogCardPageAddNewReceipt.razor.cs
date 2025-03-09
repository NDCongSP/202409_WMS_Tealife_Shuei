using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using Polly.Caching;
using System.Reflection.Metadata;
using System.Transactions;
using SupplierModel = FBT.ShareModels.Entities.Supplier;

namespace WebUIFinal.Pages.WarehouseReceipt
{
    public partial class DialogCardPageAddNewReceipt
    {
        [Parameter] public string Title { get; set; }
        public string? ReceiptNo { get; set; }

        //variables
        private bool isDisabled = false;
        private bool isDisabledEditLine = false;
        private bool _showPagerSummary = true;
        private bool allowRowSelectOnRowClick = true;
        private bool _visibleBtnSubmit = true;
        private EnumReceiptOrderStatus selectedReceiptStatus = EnumReceiptOrderStatus.Draft;
        private string pagingSummaryFormat;
        private DataGridEditMode editMode = DataGridEditMode.Single;
        private bool lineIsDisable = false;

        //Master
        private WarehouseReceiptOrderDto warehouseReceiptOrder = new();
        private List<CompanyTenant> tenants = new();
        private List<LocationDisplayDto> locations = new();
        private List<WarehouseReceiptOrderLineDto> warehouseReceiptOrderLines = [];
        private List<SupplierModel> suppliers = new();
        private RadzenDataGrid<WarehouseReceiptOrderLineDto>? _receiptLineProfileGrid;
        private IList<WarehouseReceiptOrderLineDto> selectedReceiptOrderLines = [];
        private List<UserDto> users = new();

        //Line
        private List<WarehouseReceiptOrderLineDto> receiptLinesToInsert = new List<WarehouseReceiptOrderLineDto>();
        private List<WarehouseReceiptOrderLineDto> receiptLinesToUpdate = new List<WarehouseReceiptOrderLineDto>();
        private IEnumerable<ProductDto> _autocompleteProducts;
        private RadzenAutoComplete _autocompleteProduct;
        private ProductDto selectedProduct = new();
        List<Bin> bins = new List<Bin>();
        Dictionary<string, ProductDto> productDict = new Dictionary<string, ProductDto>();
        int currentTenantId;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();

                pagingSummaryFormat = $"{_CLoc["DisplayPage"]} {{0}} {_CLoc["Of"]} {{1}} <b>({_CLoc["Total"]} {{2}} {_CLoc["Records"]})</b>";

                await GetTenantsAsync();
                await GetLocationsAsync();
                await GetSupplierAsync();
                await GetUserAsync();

                SetInitialState();
                await GetReceiptOrderAsync();

                //currentTenantId = tenants.First().AuthPTenantId;
                //warehouseReceiptOrder.TenantId = currentTenantId;
                if (!string.IsNullOrEmpty(warehouseReceiptOrder.Location)) await OnLocationChanged(warehouseReceiptOrder.Location);
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], e.Message);
            }
        }

        private void SetInitialState()
        {
            if (Title.Contains($"{_CLoc["Detail.View"]}"))
            {
                isDisabled = true;
                isDisabledEditLine = true;
                _visibleBtnSubmit = false;
            }

            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];
                ReceiptNo = sub[1];
            }
            else
            {
                currentTenantId = tenants.FirstOrDefault().AuthPTenantId;
                //warehouseReceiptOrder.TenantId = currentTenantId;
            }
        }

        private async Task GetReceiptOrderAsync()
        {
            if (string.IsNullOrEmpty(ReceiptNo)) return;

            var data = await _warehouseReceiptOrderService.GetReceiptOrderAsync(ReceiptNo);

            if (!data.Succeeded)
            {
                ShowNotification(NotificationSeverity.Error, $"{_localizer["FailedToGetReceipt"]}", data.Messages.FirstOrDefault());
                return;
            }

            warehouseReceiptOrder = data.Data;
            selectedReceiptStatus = (EnumReceiptOrderStatus)warehouseReceiptOrder.Status;
            warehouseReceiptOrderLines.AddRange(data.Data.WarehouseReceiptOrderLines);

            currentTenantId = warehouseReceiptOrder.TenantId;

            await RefreshGrid();

            if (warehouseReceiptOrder.Status != EnumReceiptOrderStatus.Draft) isDisabled = true;

            if ((int)warehouseReceiptOrder.Status >= (int)EnumReceiptOrderStatus.Received) isDisabledEditLine = true;
        }

        private async Task GetTenantsAsync()
        {
            var data = await _companiesServices.GetAllAsync();
            if (data.Succeeded) tenants.AddRange(data.Data);
        }

        private async Task GetLocationsAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
        }

        private async Task GetSupplierAsync()
        {
            var data = await _suppliersServices.GetAllAsync();
            if (data.Succeeded) suppliers.AddRange(data.Data);
        }

        private async Task GetUserAsync()
        {
            var data = await _userToTenantServices.GetUsersAsync();
            if (data.Count > 0) users.AddRange(data);
        }

        async Task Submit(WarehouseReceiptOrderDto arg)
        {
            arg.Status = selectedReceiptStatus;
            if (arg.TenantId == default || arg.TenantId == 0)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["TenantIsRequired"]}");
                return;
            }
            if (String.IsNullOrEmpty(ReceiptNo))
            {
                await CreateReceipt(arg);
            }
            else
            {
                Title = _CLoc["Detail.Edit"] + ' ' + _localizer["WarehouseReceipt"];
                await UpdateReceipt(arg);
            }
            StateHasChanged();
        }

        private async Task CreateReceipt(WarehouseReceiptOrderDto arg)
        {
            if (!await ConfirmAction($"{_localizer["DoYouWantToCreateANewReceipt"]}", $"{_localizer["CreateReceipt"]}")) return;

            warehouseReceiptOrder.ReceiptNo = string.Empty;
            warehouseReceiptOrder.Id = Guid.NewGuid();
            var response = await _warehouseReceiptOrderService.InsertWarehouseReceiptOrder(arg);

            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyCreatedReceipt"]}");

                var res = response.Data;
                ReceiptNo = res.ReceiptNo;
                warehouseReceiptOrder.ReceiptNo = res.ReceiptNo;
                arg.WarehouseReceiptOrderLines.ForEach(l => l.ReceiptNo = response.Data.ReceiptNo);
            }
            else
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToCreateReceipt"]}");
            }
        }

        private async Task UpdateReceipt(WarehouseReceiptOrderDto arg)
        {
            if (!await ConfirmAction($"{_CLoc["Confirmation.Update"]} ?", $"{_localizer["UpdateReceipt"]}")) return;
            try
            {
                var response = await _warehouseReceiptOrderService.UpdateWarehouseReceiptOrder(arg);

                if (response.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyEditedReceipt"]}");
                    arg.WarehouseReceiptOrderLines.ForEach(l => l.ReceiptNo = response.Data.ReceiptNo);
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToEditReceipt"]}");
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }

        }

        private async Task<bool> ConfirmAction(string message, string title)
        {
            return await _dialogService.Confirm(message, title, new ConfirmOptions
            {
                OkButtonText = _CLoc["Yes"],
                CancelButtonText = _CLoc["No"],
                AutoFocusFirstElement = true
            }) ?? false;
        }

        private void ShowNotification(NotificationSeverity severity, string summary, string detail, int duration = 5000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }

        private Task RefreshGrid() => _receiptLineProfileGrid?.RefreshDataAsync() ?? Task.CompletedTask;

        private async Task SyncHTData()
        {
            if (!ValidateWarehouseReceiptOrder()) return;

            try
            {
                warehouseReceiptOrder.WarehouseReceiptOrderLines = warehouseReceiptOrderLines;
                var result = await _warehouseReceiptOrderService.SyncHTData(warehouseReceiptOrder);

                if (result.Succeeded)
                {
                    UpdateWarehouseReceiptOrder(result.Data);
                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SyncDataSuccessfully"]}");
                    await RefreshGrid();
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["NoDataFoundForThisReceipt"]}");
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.ToString());
            }
        }


        private bool ValidateWarehouseReceiptOrder()
        {
            if (warehouseReceiptOrder == null)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["Confirmation.RequiredReceipt"]}");
                return false;
            }

            if (warehouseReceiptOrder.WarehouseReceiptOrderLines.Count() > 1 &&
                warehouseReceiptOrder.WarehouseReceiptOrderLines.Any(l => l.Id == Guid.Empty))
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["Confirmation.RequiredReceipt"]}");
                return false;
            }

            return true;
        }

        private void UpdateWarehouseReceiptOrder(WarehouseReceiptOrderDto updatedOrder)
        {
            warehouseReceiptOrder = updatedOrder;
            warehouseReceiptOrderLines = updatedOrder.WarehouseReceiptOrderLines.ToList();
            StateHasChanged();
        }

        private async Task InsertWarehousePutAwayOrder()
        {
            if (!await ConfirmInsertWarehousePutAwayOrder()) return;

            try
            {
                var payload = CreateWarehousePutAwayPayload();
                var res = await _warehousePutAwayServices.InsertWarehousePutAwayOrder(payload);

                warehouseReceiptOrder.Status = EnumReceiptOrderStatus.OnPutaway;
                var response = await _warehouseReceiptOrderService.AdjustActionReceiptOrder(warehouseReceiptOrder);

                if (res.Succeeded && response.Succeeded)
                {
                    UpdateWarehouseReceiptOrderStatus(EnumReceiptOrderStatus.OnPutaway.ToString());
                    selectedReceiptStatus = EnumReceiptOrderStatus.OnPutaway;

                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["CreatedShelvingSuccessfully"]}");
                    isDisabled = true;
                    isDisabledEditLine = true;
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], res.Messages.ToString());
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }
        }

        private async Task<bool> ConfirmInsertWarehousePutAwayOrder()
        {
            if (warehouseReceiptOrder.WarehouseReceiptOrderLines.Count < 1 || warehouseReceiptOrder.WarehouseReceiptOrderLines == null)
            {
                await _dialogService.Confirm($"{_localizer["Validation.CreateShelving"]}", $"{_localizer["CreateShelving"]}",
                    new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true });
                return false;
            }

            return await _dialogService.Confirm($"{_localizer["Confirmation.CreateShelving"]}", $"{_localizer["CreateShelving"]}",
                new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true }) ?? false;
        }

        private IEnumerable<WarehousePutAwayDto> CreateWarehousePutAwayPayload()
        {
            return new List<WarehouseReceiptOrderDto> { warehouseReceiptOrder }
                .Select(_ => new WarehousePutAwayDto
                {
                    Id = _.Id,
                    ReceiptNo = _.ReceiptNo,
                    TenantId = _.TenantId,
                    DocumentNo = _.DocumentNo,
                    Location = _.Location,
                    WarehousePutAwayLines = selectedReceiptOrderLines.Select(r => new WarehousePutAwayLineDto
                    {
                        Id = r.Id,
                        ProductCode = r.ProductCode,
                        UnitId = (int)r.UnitId,
                        JournalQty = r.OrderQty,
                        TransQty = r.TransQty,
                        Bin = r.Bin,
                        LotNo = r.LotNo
                    }).ToList(),
                });
        }

        private async Task AdjustStatusReceipt(string action)
        {
            //validate
            if(warehouseReceiptOrder.TenantId == default || warehouseReceiptOrder.TenantId == 0)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["TenantIsRequired"]}");
                return;
            }
            if (string.IsNullOrEmpty(warehouseReceiptOrder.Location))
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["LocationIsRequired"]}");
                return;
            }
            if (warehouseReceiptOrder.Status != EnumReceiptOrderStatus.Draft)
            {
                bool validateExpriedDate = false;
                WarehouseReceiptOrderLineDto line = new WarehouseReceiptOrderLineDto();
                foreach (var item in warehouseReceiptOrderLines)
                {
                    line = new WarehouseReceiptOrderLineDto();
                    line = item;
                    if (!item.ExpirationDate.HasValue)
                    {
                        validateExpriedDate = true;
                        break;
                    }
                }
            
                if (validateExpriedDate)
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{line?.ProductCode} {_localizer["product expiry date is required"]}");
                    return;
                }
            }
            if (!await ConfirmAdjustStatusReceipt(action)) return;
            
            warehouseReceiptOrder.Status = CommonHelpers.ParseEnum<EnumReceiptOrderStatus>(action);
            var response = await _warehouseReceiptOrderService.AdjustActionReceiptOrder(warehouseReceiptOrder);

            if (response.Succeeded)
            {
                UpdateWarehouseReceiptOrderStatus(action);
                selectedReceiptStatus = (EnumReceiptOrderStatus)warehouseReceiptOrder.Status;
                ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyEditedReceipt"]}");
            }
            else
            {
                warehouseReceiptOrder.Status = warehouseReceiptOrder.Status - 1;
                var msg = response.Messages[0];
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToEditReceipt"] + ". " + _localizer[response.Messages[0]]}");
            }
        }

        private async Task<bool> ConfirmAdjustStatusReceipt(string action)
        {
            return await _dialogService.Confirm($"{_localizer["Confirmation.AdjustReceipt"]}", $"{_localizer["AdjustReceiptStatus"]}",
                new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true }) ?? false;
        }

        private void UpdateWarehouseReceiptOrderStatus(string action)
        {
            if (action.Equals(EnumReceiptOrderStatus.Open.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                isDisabled = true;
            }
            else if (action.Equals(EnumReceiptOrderStatus.Received.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                isDisabled = true;
                isDisabledEditLine = true;
            }
            else if (action.Equals(EnumReceiptOrderStatus.OnPutaway.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                isDisabled = true;
                isDisabledEditLine = true;
            }
        }

        private List<EnumDisplay<EnumReceiptOrderStatus>> GetDisplayReceiptOrderStatus()
        {
            return Enum.GetValues(typeof(EnumReceiptOrderStatus)).Cast<EnumReceiptOrderStatus>().Select(_ => new EnumDisplay<EnumReceiptOrderStatus>
            {
                Value = _,
                DisplayValue = GetValueReceiptOrderStatus(_)
            }).ToList();
        }

        private string GetValueReceiptOrderStatus(EnumReceiptOrderStatus receiptStatus) => receiptStatus switch
        {
            EnumReceiptOrderStatus.Draft => _localizer["Draft"],
            EnumReceiptOrderStatus.Open => _localizer["Open"],
            EnumReceiptOrderStatus.Received => _localizer["Received"],
            EnumReceiptOrderStatus.OnPutaway => _localizer["OnPutaway"],
            EnumReceiptOrderStatus.Completed => _localizer["Completed"],
            _ => throw new ArgumentException("Invalid value for ReceiptOrderStatus", nameof(receiptStatus))
        };

        private string GetStatusColor(EnumReceiptOrderStatus status) => status switch
        {
            EnumReceiptOrderStatus.Draft => "default",
            EnumReceiptOrderStatus.Open => "info",
            EnumReceiptOrderStatus.Received => "primary",
            EnumReceiptOrderStatus.OnPutaway => "error",
            EnumReceiptOrderStatus.Completed => "success",
            _ => "default",
        };

        private bool DisableCheckBoxReceiptLine(WarehouseReceiptOrderLineDto dto)
        {
            if (dto.TransQty == null || dto.TransQty < 0) return true;
            return false;
        }

        private bool VisibleCheckBoxAllReceiptLine()
        {
            if (selectedReceiptOrderLines.Where(line => line.TransQty == null || line.TransQty < 0).Select(line => line).Any()) return false;
            return true;
        }

        private async Task OnLocationChanged(string arg)
        {
            if (Guid.TryParse(warehouseReceiptOrder.Location, out Guid x))
            {
                var data = await _binServices.GetByLocationId(x);
                bins.AddRange(data.Data);
            }
        }

        private async Task CheckArrivalNo(double? data)
        {
            try
            {
                if (warehouseReceiptOrder.ScheduledArrivalNumber != default)
                {
                    warehouseReceiptOrder.ReceiptNo = string.Empty;
                    var result = await _warehouseReceiptOrderService.CreateLineFromArrivalNo(warehouseReceiptOrder);

                    if (result.Succeeded)
                    {
                        if (result.Data.ReceiptNo != warehouseReceiptOrder.ReceiptNo)
                        {
                            ReceiptNo = result.Data.ReceiptNo;
                            Title = _CLoc["Detail.Edit"] + _localizer["WarehouseReceipt"];
                            _navigation.NavigateTo($"/addreceipt/{_CLoc["Detail.Edit"]} {_localizer["WarehouseReceipt"]}|{result.Data.ReceiptNo}");
                        }
                        else
                        {
                            Title = _CLoc["Detail.Create"] + _localizer["WarehouseReceipt"];
                            ReceiptNo = "";
                        }
                        selectedReceiptStatus = (EnumReceiptOrderStatus)result.Data.Status;
                        if (warehouseReceiptOrder.Status != EnumReceiptOrderStatus.Draft) isDisabled = true;
                        if ((int)warehouseReceiptOrder.Status >= (int)EnumReceiptOrderStatus.Received) isDisabledEditLine = true;
                        UpdateWarehouseReceiptOrder(result.Data);
                        await RefreshGrid();
                    }
                    else if(!result.Messages[0].Contains("NoArrivalData"))
                    {
                        ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer[result.Messages[0]]}");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.ToString());
            }

        }
        #region RECEIPT LINE

        void Reset()
        {
            receiptLinesToInsert.Clear();
            receiptLinesToUpdate.Clear();
        }

        void Reset(WarehouseReceiptOrderLineDto detail)
        {
            receiptLinesToInsert.Remove(detail);
            receiptLinesToUpdate.Remove(detail);
        }

        async Task EditRow(WarehouseReceiptOrderLineDto line)
        {
            if (editMode == DataGridEditMode.Single && receiptLinesToInsert.Count() > 0)
            {
                Reset();
            }

            selectedProduct.ProductCode = line.ProductCode;
            selectedProduct.ProductName = line.ProductName;
            selectedProduct.UnitName = line.UnitName;
            selectedProduct.StockAvailableQuantity = line.StockAvailableQuantity == null ? 0 : (int)line.StockAvailableQuantity;
            receiptLinesToUpdate.Add(line);
            await _receiptLineProfileGrid.EditRow(line);
        }

        void OnUpdateRow(WarehouseReceiptOrderLineDto line)
        {
            Reset(line);

        }

        async Task SaveRow(WarehouseReceiptOrderLineDto line)
        {
            line.ProductCode = selectedProduct.ProductCode;
            if (string.IsNullOrEmpty(line.ProductCode))
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _CLoc["Require.ProductCode"],
                    Duration = 4000
                });
                return;
            }
            if (line.OrderQty <= 0 || line.OrderQty == null)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _CLoc["Require.OrderQty"],
                    Duration = 4000
                });
                return;
            }
            if (String.IsNullOrEmpty(line.Bin) && String.IsNullOrEmpty(line.LotNo))
            {
                bool checkProductCodeAndLot = warehouseReceiptOrder.WarehouseReceiptOrderLines.Where(_ => _.Id != line.Id && _.ProductCode == line.ProductCode && _.Bin == line.Bin && _.LotNo == line.LotNo).Any();

                if (checkProductCodeAndLot)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _CLoc["Error"],
                        Detail = string.Empty,
                        Duration = 4000
                    });
                    return;
                }
            }
            if (String.IsNullOrEmpty(selectedProduct.ProductCode))
            {
                var selectedLine = warehouseReceiptOrder.WarehouseReceiptOrderLines.Where(_ => _.Id == line.Id).FirstOrDefault();
                var product = await _productServices.GetByProductCodeAsync(line.ProductCode);
                if (product != null)
                {
                    line.ProductName = product.Data.ProductName;
                    line.UnitId = product.Data.UnitId;
                    line.UnitName = product.Data.UnitName;
                    line.StockAvailableQuantity = product.Data.StockAvailableQuantity;
                }
            }
            else
            {
                line.ProductName = selectedProduct.ProductName;
                line.UnitId = selectedProduct.UnitId;
                line.UnitName = selectedProduct.UnitName;
                line.StockAvailableQuantity = selectedProduct.StockAvailableQuantity;
            }

            var productInfo = await _warehouseTranServices.GetInventoryInfoFlowBinLotFlowModelSearchAsync(new InventoryInfoBinLotSearchRequestDTO
            {
                ProductCodes = new List<ProductCodeSearch> { new ProductCodeSearch { ProductCode = line.ProductCode } }
            });
            if (productInfo.Succeeded && productInfo.Data != null)
            {
                line.StockAvailableQuantity = (int)productInfo.Data.FirstOrDefault().Details.Select(_ => _.InventoryStock).Sum();
            }

            if (!String.IsNullOrEmpty(ReceiptNo))
            {
                if (line.ReceiptNo != "")
                {
                    WarehouseReceiptOrderLine receiptOrderLine = new()
                    {
                        Id = line.Id,
                        ProductCode = line.ProductCode,
                        ReceiptNo = line.ReceiptNo,
                        UnitId = line.UnitId,
                        UnitName = line.UnitName,
                        OrderQty = line.OrderQty,
                        TransQty = line.TransQty,
                        Bin = line.Bin,
                        LotNo = line.LotNo,
                        ExpirationDate = line.ExpirationDate,
                        UpdateAt = DateTime.Now,

                    };
                    var response = _warehouseReceiptOrderLineService.UpdateAsync(receiptOrderLine);

                }
                else
                {
                    line.ReceiptNo = ReceiptNo;
                    line.Id = Guid.NewGuid();
                    WarehouseReceiptOrderLine receiptOrderLine = new()
                    {
                        Id = line.Id,
                        ProductCode = line.ProductCode,
                        ReceiptNo = ReceiptNo,
                        UnitId = line.UnitId,
                        UnitName = line.UnitName,
                        OrderQty = line.OrderQty,
                        TransQty = line.TransQty,
                        Bin = line.Bin,
                        LotNo = line.LotNo,
                        ExpirationDate = line.ExpirationDate,
                        CreateAt = DateTime.Now,

                    };
                    var response = _warehouseReceiptOrderLineService.InsertAsync(receiptOrderLine);
                }

            }
            await _receiptLineProfileGrid.UpdateRow(line);
            selectedProduct = new();
        }

        void CancelEdit(WarehouseReceiptOrderLineDto line)
        {
            Reset(line);
            _receiptLineProfileGrid.CancelEditRow(line);
        }

        async Task DeleteRow(WarehouseReceiptOrderLineDto line)
        {
            string message = $"{_CLoc["Confirmation.Delete"]}?";
            bool isRemoveArrivalNo = false;
            if (line.ArrivalNo != default && warehouseReceiptOrder.WarehouseReceiptOrderLines.Where(x => x.ArrivalNo == line.ArrivalNo).Count() == 1)
            {
                isRemoveArrivalNo = true;
                message = $"{_CLoc["Confirmation.Delete"]}, {_localizer["NotifyRemoveArrivalNo"]}?";
            }
            var confirm = await _dialogService.Confirm(message, $"{_CLoc["Delete"]}",
                new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true }) ?? false;
            if (confirm == true)
            {
                Reset(line);
                if (warehouseReceiptOrder.WarehouseReceiptOrderLines.Contains(line))
                {
                    WarehouseReceiptOrderLine deleteLine = new WarehouseReceiptOrderLine();
                    deleteLine.Id = line.Id;
                    deleteLine.ReceiptNo = line.ReceiptNo;
                    deleteLine.ProductCode = line.ProductCode;
                    await _warehouseReceiptOrderLineService.DeleteAsync(deleteLine);
                    warehouseReceiptOrder.WarehouseReceiptOrderLines.Remove(line);
                    await _receiptLineProfileGrid.Reload();
                    if (isRemoveArrivalNo)
                        warehouseReceiptOrder.ScheduledArrivalNumber = default;
                }
                else
                {
                    _receiptLineProfileGrid.CancelEditRow(line);
                    await _receiptLineProfileGrid.Reload();
                }
            }
        }

        async Task InsertRow()
        {
            if (editMode == DataGridEditMode.Single)
            {
                Reset();
            }

            var line = new WarehouseReceiptOrderLineDto();
            receiptLinesToInsert.Add(line);
            await _receiptLineProfileGrid.InsertRow(line);
        }

        void OnCreateRow(WarehouseReceiptOrderLineDto line)
        {
            warehouseReceiptOrderLines.Add(line);
            warehouseReceiptOrder.WarehouseReceiptOrderLines.Add(line);
            receiptLinesToInsert.Remove(line);
        }
        private async Task OnProductSelected(ProductDto product)
        {
            try
            {
                selectedProduct = product ?? new();
                var productInfo = await _warehouseTranServices.GetInventoryInfoFlowBinLotFlowModelSearchAsync(new InventoryInfoBinLotSearchRequestDTO
                {
                    ProductCodes = new List<ProductCodeSearch> { new ProductCodeSearch { ProductCode = product.ProductCode } }
                });
                if (productInfo.Succeeded)
                {
                   
                    if (productInfo.Data != null)
                    {
                        selectedProduct.StockAvailableQuantity = (int)productInfo.Data.FirstOrDefault().Details.Select(_ => _.AvailableStock).Sum();
                    }
                }
                StateHasChanged();

            }
            catch(Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.ToString());
            }
          
        }
        async Task DeleteReceip()
        {
            var confirm = await _dialogService.Confirm($"{_CLoc["Confirmation.Delete"]}?", $"{_CLoc["Delete"]}",
                new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true }) ?? false;
            if (confirm == true)
            {
                WarehouseReceiptOrder deletedOrder = new WarehouseReceiptOrder();
                deletedOrder.Id = warehouseReceiptOrder.Id;
                await _warehouseReceiptOrderService.DeleteAsync(deletedOrder);
                var deleteLines = new List<WarehouseReceiptOrderLine>();
                warehouseReceiptOrder.WarehouseReceiptOrderLines.ForEach(line =>
                {
                    WarehouseReceiptOrderLine deleteLine = new WarehouseReceiptOrderLine();
                    deleteLine.Id = line.Id;
                    deleteLine.ReceiptNo = line.ReceiptNo;
                    deleteLine.ProductCode = line.ProductCode;
                    deleteLines.Add(deleteLine);


                });
                await _warehouseReceiptOrderLineService.DeleteRangeAsync(deleteLines);
                _navigation.NavigateTo("/");
            }
        }
        private async Task OnChangeTenant(int value)
        {
           
            if (_receiptLineProfileGrid.Count > 0)
            {
                var confirm = await _dialogService.Confirm(_CLoc["Confirm.Message.ChangeTenant"], _CLoc["Confirm.Title.ChangeTenant"], new ConfirmOptions() { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"] });
                if (confirm == true)
                {
                    warehouseReceiptOrderLines = [];
                    receiptLinesToInsert = [];
                    receiptLinesToUpdate = [];
                    warehouseReceiptOrder.WarehouseReceiptOrderLines = [];
                    
                    warehouseReceiptOrder.TenantId = value;
                    currentTenantId = value;
                    _receiptLineProfileGrid.Reload();
                }
                else
                {
                    warehouseReceiptOrder.TenantId = currentTenantId;
                }
                StateHasChanged();
            }
            else
            {
                warehouseReceiptOrder.TenantId = value;
                currentTenantId = value;
            }
        }
        #endregion
    }

}