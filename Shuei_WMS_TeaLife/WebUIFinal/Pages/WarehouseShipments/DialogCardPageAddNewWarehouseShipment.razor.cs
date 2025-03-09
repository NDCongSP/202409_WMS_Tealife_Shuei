using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebUIFinal.Pages.WarehouseShipments;
public partial class DialogCardPageAddNewWarehouseShipment
{
    public string Title { get; set; }
    [Parameter]
    public string Mode { get; set; }
    private WarehouseShipmentDto model = new WarehouseShipmentDto();
    private bool isDisabled = false;
    private List<Location> locations;
    private List<FBT.ShareModels.Entities.Product> products;
    private List<FBT.ShareModels.WMS.ShippingCarrier> shippingCarriers;
    int currentTenantId;

    #region MASTER DATA
    private List<SelectListItem> _locations;
    private List<CompanyTenant> _tenants;
    private List<Bin> _bins, _allBins;
    private List<OrderSelectList> _orders;
    private List<FBT.ShareModels.Entities.Product> _products;
    private List<FBT.ShareModels.WMS.ShippingCarrier> _shippingCarriers;
    private List<SelectListItem> _personInChargeList;
    private List<SelectListItem> _lotNo;
    private IEnumerable<ProductDto> _autocompleteProducts;
    private RadzenAutoComplete _autocompleteProduct;
    private string productName;
    private ProductDto selectedProduct = new();
    private List<InventoryInfoBinLotDetails> InventoryInfoBinLotDetails = new();
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await GetMasterDataAsync();
        await InitializeModel();
    }

    private async Task GetMasterDataAsync()
    {
        try
        {
            var locationResult = await _locationServices.GetAllAsync();
            var tenantResult = await _companiesServices.GetAllAsync();
            var binResult = await _binServices.GetAllAsync();
            var productResult = await _productServices.GetAllAsync();
            var shippingCarrierResult = await _shippingCarrierServices.GetAllAsync();
            var personInChargeResult = await _categoriesService.GetUserDropdown();

            _locations = locationResult.Data.Select(x => new SelectListItem
            {
                Text = x.LocationName,
                Value = x.Id.ToString()
            }).ToList();
            _tenants = tenantResult.Data;
            _allBins = _bins = binResult.Data;
            _products = productResult.Data;
            _shippingCarriers = shippingCarrierResult.Data;
            _personInChargeList = personInChargeResult.Data;
        }
        catch (Exception ex)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _commonLocalizer["FailedToLoadMasterData", ex.Message],
                Duration = 5000
            });
        }
    }

    private void OnChangeLocation(object locationId)
    {
        //_bins = _allBins.Where(x => x.LocationId == Guid.Parse(locationId.ToString())).ToList();
    }
    #endregion

    private async Task InitializeModel()
    {
        if (Mode.StartsWith("Edit"))
        {
            var sub = Mode.Split('|');
            var shipmentId = sub[1];
            var result = await _warehouseShipmentServices.GetShipmentByIdAsync(Guid.Parse(shipmentId));
            if (result.Succeeded)
            {
                model = result.Data;
                currentTenantId = model.TenantId;
                _bins = _allBins.Where(x => x.LocationId == Guid.Parse(model.Location)).ToList();
                _orders = await _categoriesService.GetOrders(model.SalesNo);
            }
            else
            {
                NotifyError(result.Messages);
            }
            Title = _shipmentLocalizer["EditWarehouseShipment"];
            isDisabled = model.Status >= FBT.ShareModels.EnumShipmentOrderStatus.Open;
        }
        else
        {
            Title = _shipmentLocalizer["CreateWarehouseShipment"];
            _orders = await _categoriesService.GetOrders();
        }
    }

    private async Task OnProductChange(ProductDto prod)
    {
        selectedProduct = prod;
        InventoryInfoBinLotDetails = new List<InventoryInfoBinLotDetails>();
        var productInfo = await _warehouseTranServices.GetInventoryInfoFlowBinLotFlowModelSearchAsync(new InventoryInfoBinLotSearchRequestDTO
        {
            //CompanyId = model.TenantId,
            ProductCodes = new List<ProductCodeSearch> { new ProductCodeSearch { ProductCode = prod.ProductCode } }
        });
        if (productInfo.Succeeded && productInfo.Data != null)
        {
            InventoryInfoBinLotDetails = productInfo.Data.First().Details.Where(x => x.AvailableStock > 0 && !string.IsNullOrEmpty(x.BinCode) && !string.IsNullOrEmpty(x.LotNo)).ToList();
            _bins = InventoryInfoBinLotDetails.Select(x => new Bin
            {
                BinCode = x.BinCode,
            }).DistinctBy(x => x.BinCode).ToList();
            _lotNo = InventoryInfoBinLotDetails.Select(x => new SelectListItem
            {
                Value = x.LotNo,
                Text = x.BinCode,
            }).DistinctBy(x => x.Value).ToList();
        }
        else
        {
            NotifyError(productInfo.Messages);
        }
        StateHasChanged();
    }

    private void BinItemChange(object binCode)
    {
        if (binCode == null)
        {
            _lotNo = new List<SelectListItem>();
        }
        else
        {
            _lotNo = InventoryInfoBinLotDetails.Where(x => x.BinCode == binCode.ToString()).Select(x => new SelectListItem
            {
                Value = x.LotNo,
                Text = x.BinCode,
                Selected = x.LotNo == "N/A"
            }).DistinctBy(x => x.Value).ToList();
        }
    }

    private void LotNoItemChange(object lotNo)
    {
        if (lotNo == null)
        {
            selectedProduct.StockAvailableQuantityTrans = 0;
            selectedProduct.StockAvailableQuantity = 0;
            selectedProduct.ExpirationDate = null;
        }
        else
        {
            var lotInfo = _lotNo.FirstOrDefault(x => x.Value == lotNo.ToString());
            if (lotInfo != null)
            {
                var binInfo = InventoryInfoBinLotDetails.FirstOrDefault(x => x.BinCode == lotInfo.Text && x.LotNo == lotNo.ToString());
                selectedProduct.StockAvailableQuantityTrans = binInfo == null ? 0 : (int)binInfo.InventoryStock;
                selectedProduct.StockAvailableQuantity = binInfo == null ? 0 : (int)binInfo.AvailableStock;
                selectedProduct.ExpirationDate = binInfo.Expired;
            }
        }
    }

    private async Task Submit(WarehouseShipmentDto model)
    {
        if (model.WareHouseShipmentLineDtos.Count == 0)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _commonLocalizer["PleaseEnterDetail"],
                Duration = 4000
            });
        }
        else
        {
            #region MAPPING NAME
            var location = _locations.FirstOrDefault(x => x.Value == model.Location);
            if (location != null)
            {
                model.LocationName = location.Text;
            }

            var tenant = _tenants.FirstOrDefault(x => x.AuthPTenantId == model.TenantId);
            if (tenant != null)
            {
                model.TenantName = tenant.FullName;
            }
            var personInCharge = _personInChargeList.FirstOrDefault(x => x.Value == model.PersonInCharge);
            if (personInCharge != null)
            {
                model.PersonInChargeName = personInCharge.Text;
            }
            #endregion

            if (Mode == "Create")
            {
                var result = await _warehouseShipmentServices.CreateWarehouseShipmentAsync(model);
                if (result.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _commonLocalizer["Success"],
                        Detail = _shipmentLocalizer["WarehouseShipmentCreatedSuccessfully"],
                        Duration = 4000
                    });
                    model.ShipmentNo = result.Data.ShipmentNo;
                    model.Id = result.Data.Id;
                    Mode = "Edit";
                    StateHasChanged();
                }
                else
                {
                }
            }
            else if (Mode.StartsWith("Edit"))
            {
                var result = await _warehouseShipmentServices.UpdateWarehouseShipmentAsync(model);
                if (result.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _commonLocalizer["Success"],
                        Detail = _shipmentLocalizer["WarehouseShipmentUpdatedSuccessfully"],
                        Duration = 4000
                    });
                }
                else
                {
                    NotifyError(result.Messages);
                }
            }
        }
    }

    async Task DeleteItemAsync(Guid id)
    {
        try
        {
            var confirm = await _dialogService.Confirm(_commonLocalizer["Confirmation.Delete"] + _shipmentLocalizer["WarehouseShipment"] + "?", _commonLocalizer["Delete"] + " " + model.ShipmentNo, new ConfirmOptions()
            {
                OkButtonText = _commonLocalizer["Yes"],
                CancelButtonText = _commonLocalizer["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;
            var res = await _warehouseShipmentServices.DeleteShipmentAsync(id);

            if (res.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = _commonLocalizer["Success"],
                    Detail = _shipmentLocalizer["WarehouseShipmentDeletedSuccessfully"],
                    Duration = 4000
                });
                _navigation.NavigateTo("/ShippingInstructions");
            }
            else
            {
                NotifyError(res.Messages);
            }
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

    private async Task ConfirmShipment()
    {
        var confirm = await _dialogService.Confirm(_shipmentLocalizer["ConfirmWarehouseShipmentConfirmation"], _shipmentLocalizer["UpdateConfirmation"], new ConfirmOptions() { OkButtonText = _commonLocalizer["Yes"], CancelButtonText = _commonLocalizer["No"] });
        if (confirm == true)
        {
            var result = await _warehouseShipmentServices.ConfirmShipmentAsync(model.Id);
            if (result.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = _commonLocalizer["Success"],
                    Detail = _shipmentLocalizer["WarehouseShipmentConfirmedSuccessfully"],
                    Duration = 4000
                });
                model.Status = EnumShipmentOrderStatus.Open;
                isDisabled = true;
            }
            else
            {
                NotifyError(result.Messages);
            }
        }
    }

    private async Task CompletedShipment()
    {
        var confirm = await _dialogService.Confirm(_shipmentLocalizer["CompletedWarehouseShipmentConfirmation"], _shipmentLocalizer["UpdateConfirmation"], new ConfirmOptions() { OkButtonText = _commonLocalizer["Yes"], CancelButtonText = _commonLocalizer["No"] });
        if (confirm == true)
        {
            var result = await _warehouseShipmentServices.CompletedShipmentAsync(model.Id);
            if (result.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = _commonLocalizer["Success"],
                    Detail = _shipmentLocalizer["WarehouseShipmentCompletedSuccessfully"],
                    Duration = 4000
                });
                model.Status = EnumShipmentOrderStatus.Completed;
                isDisabled = true;
            }
            else
            {
                NotifyError(result.Messages);
            }
        }
    }
    public async void ChangeTenant(int tenantId)
    {
        if (shipmentDetailsGrid.Count > 0)
        {
            var confirm = await _dialogService.Confirm(_commonLocalizer["Confirm.Message.ChangeTenant"], _commonLocalizer["Confirm.Title.ChangeTenant"], new ConfirmOptions() { OkButtonText = _commonLocalizer["Yes"], CancelButtonText = _commonLocalizer["No"] });
            if (confirm == true)
            {
                shipmentDetailsGrid.Reload();
                shipmentDetailsToInsert = new List<WarehouseShipmentLineDto>();
                model.WareHouseShipmentLineDtos = new List<WarehouseShipmentLineDto>();
                currentTenantId = tenantId;
            }
            else
            {
                model.TenantId = currentTenantId;
            }
            StateHasChanged();
        }
        else
        {
            currentTenantId = tenantId;
        }
    }

    void SelectedSalesOrder(object data)
    {
        var orderInfo = _orders.First(x => x.OrderId == data.ToString());
        model.TrackingNo = orderInfo.TrackingNo;
        model.OrderDispatchId = orderInfo.OrderDispatchId;
    }

    //detail
    RadzenDataGrid<WarehouseShipmentLineDto> shipmentDetailsGrid;
    DataGridEditMode editMode = DataGridEditMode.Single;

    List<WarehouseShipmentLineDto> shipmentDetailsToInsert = new List<WarehouseShipmentLineDto>();
    List<WarehouseShipmentLineDto> shipmentDetailsToUpdate = new List<WarehouseShipmentLineDto>();

    void Reset()
    {
        shipmentDetailsToInsert.Clear();
        shipmentDetailsToUpdate.Clear();
    }

    void Reset(WarehouseShipmentLineDto detail)
    {
        shipmentDetailsToInsert.Remove(detail);
        shipmentDetailsToUpdate.Remove(detail);
    }

    async Task EditRow(WarehouseShipmentLineDto detail)
    {
        if (editMode == DataGridEditMode.Single && shipmentDetailsToInsert.Count() > 0)
        {
            Reset();
        }

        selectedProduct = new ProductDto
        {
            ProductCode = detail.ProductCode,
            ProductName = detail.ProductName,
            UnitId = (int)detail.UnitId,
            UnitName = detail.Unit,
            StockAvailableQuantityTrans = detail.StockAvailable,
            QuantityShipment = detail.StockAvailable - detail.AvailableQuantity,
            ExpirationDate = detail.ExpirationDate
        };
        await OnProductChange(selectedProduct);
        BinItemChange(detail.Bin);
        LotNoItemChange(detail.LotNo);
        shipmentDetailsToUpdate.Add(detail);
        await shipmentDetailsGrid.EditRow(detail);
    }

    void OnUpdateRow(WarehouseShipmentLineDto detail)
    {
        Reset(detail);
        // Update logic here
    }

    async Task SaveRow(WarehouseShipmentLineDto detail)
    {
        if (string.IsNullOrEmpty(selectedProduct.ProductCode))
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["ProductIsRequired"],
                Duration = 4000
            });
            return;
        }
        if (string.IsNullOrEmpty(detail.Bin))
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["BinIsRequired"],
                Duration = 4000
            });
            return;
        }
        if (_lotNo.FirstOrDefault(x => x.Value == detail.LotNo) == null)
            detail.LotNo = null;
        if (string.IsNullOrEmpty(detail.LotNo))
        {
            var lotNoInfo = InventoryInfoBinLotDetails.FirstOrDefault(x => x.BinCode == detail.Bin && x.LotNo == "N/A" && x.AvailableStock > detail.ShipmentQty);
            //check LotNo N/A
            if (lotNoInfo != null)
            {
                detail.LotNo = "N/A";
                selectedProduct.StockAvailableQuantityTrans = lotNoInfo.InventoryStock == null ? 0 : (int)lotNoInfo.InventoryStock;
                selectedProduct.StockAvailableQuantity = lotNoInfo.AvailableStock == null ? 0 : (int)lotNoInfo.AvailableStock;
                selectedProduct.ExpirationDate = lotNoInfo.Expired;
            }
            else
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = _shipmentLocalizer["LotNoIsRequired"],
                    Duration = 4000
                });
                return;
            }
        }
        //check exist productcode & bin
        if (CheckExistLine(detail))
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["ProductIsExisted"],
                Duration = 4000
            });
            return;
        }
        if (detail.ShipmentQty == 0 || detail.ShipmentQty == default)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["ShipmentQtyIsRequired"],
                Duration = 4000
            });
            return;
        }
        detail.Id = Guid.NewGuid();
        detail.ProductCode = selectedProduct.ProductCode;
        detail.ProductName = selectedProduct.ProductName;
        detail.Unit = selectedProduct.UnitName;
        detail.StockAvailable = (int)selectedProduct.StockAvailableQuantityTrans;
        detail.AvailableQuantity = (int)(selectedProduct.StockAvailableQuantityTrans - selectedProduct.QuantityShipment);
        detail.PackedQty = 0;
        detail.UnitId = selectedProduct.UnitId;
        detail.Location = model.Location;
        detail.ShipmentNo = model.ShipmentNo;
        detail.ExpirationDate = selectedProduct.ExpirationDate;
        await shipmentDetailsGrid.UpdateRow(detail);
        selectedProduct = new();
    }

    private bool CheckExistLine(WarehouseShipmentLineDto detail)
    {
        return model.WareHouseShipmentLineDtos.Any(line =>
            line.Id != detail.Id &&
            line.ProductCode == detail.ProductCode &&
            line.Bin == detail.Bin);
    }

    void CancelEdit(WarehouseShipmentLineDto detail)
    {
        Reset(detail);
        shipmentDetailsGrid.CancelEditRow(detail);
    }

    async Task DeleteRow(WarehouseShipmentLineDto detail)
    {
        var confirm = await _dialogService.Confirm(_shipmentLocalizer["DeleteWarehouseShipmentProductConfirmation"], _commonLocalizer["DeleteConfirmation"], new ConfirmOptions() { OkButtonText = _commonLocalizer["Yes"], CancelButtonText = _commonLocalizer["No"] });
        if (confirm == true)
        {
            Reset(detail);
            if (model.WareHouseShipmentLineDtos.Contains(detail))
            {
                model.WareHouseShipmentLineDtos.Remove(detail);
                await shipmentDetailsGrid.Reload();
            }
            else
            {
                shipmentDetailsGrid.CancelEditRow(detail);
                await shipmentDetailsGrid.Reload();
            }
        }

    }

    async Task InsertRow()
    {
        if (editMode == DataGridEditMode.Single)
        {
            Reset();
        }

        var detail = new WarehouseShipmentLineDto()
        {
            Bin = model.BinId
        };
        shipmentDetailsToInsert.Add(detail);
        await shipmentDetailsGrid.InsertRow(detail);
    }

    void OnCreateRow(WarehouseShipmentLineDto detail)
    {
        model.WareHouseShipmentLineDtos.Add(detail);
        shipmentDetailsToInsert.Remove(detail);
    }

    async Task CreatePickingAsync()
    {
        var d = new SubmitCompletedShipmentDto
        {
            Id = new List<Guid> { model.Id }
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
            model.Status = EnumShipmentOrderStatus.Picking;
        }
    }

    async void PrintDeliveryNote()
    {
        var _dataTransfer = new List<Guid> { model.Id };
        await _localStorage.SetItemAsync("DeliveryNoteTransfer", _dataTransfer);
        await JSRuntime.InvokeVoidAsync("openTab", "/deliverynote");
    }

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
}