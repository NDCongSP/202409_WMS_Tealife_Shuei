using Application.DTOs;
using Application.DTOs.Request;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor.Rendering;
using WebUIFinal.Pages.Components;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WebUIFinal.Pages.ReturnOrder
{
    public partial class DialogAddNewReturnOrder
    {
        //params
        [Parameter] public string Action { get; set; }
        [Parameter] public string? ReturnOrderNo { get; set; }

        //variables
        private ReturnOrderDto returnOrderDto = new();
        private bool isDisabled = false;
        private bool isDisabledEditLine = false;
        private bool _showPagerSummary = true;
        private bool allowRowSelectOnRowClick = true;
        private bool _visibleBtnSubmit = true;
        private IEnumerable<WarehouseShipmentDto> _autocompleteShipmentNos;
        private RadzenAutoComplete _autocompleteShipmentNo;
        private ProductAutocomplete _productAutocomplete;
        private WarehouseShipmentDto selectedShipmentNo = new();
        private List<UserDto> users = new();
        private List<string> productCodeFilter = new();
        private string currentReturnOrderNo;

        //line
        DataGridEditMode editMode = DataGridEditMode.Single;
        private RadzenDataGrid<ReturnOrderLineDto>? _returnOrderLineProfileGrid;
        private List<ReturnOrderLineDto> returnLinesToInsert = new();
        private List<ReturnOrderLineDto> returnLinesToUpdate = new();
        private IEnumerable<ProductDto> shippingProducts;
        private RadzenAutoComplete _autocompleteProduct;
        private ProductDto selectedProduct = new();
        private int? selectedStockAvaibility;
        Dictionary<string, ProductDto> productDict = new Dictionary<string, ProductDto>();
        private List<LocationDisplayDto> locations = new();
        //private List<WarehouseShipmentLine> shipmentLines = new();
        double currentEditQty = 0;
        protected override async Task OnInitializedAsync()
        {
            await GetUserAsync();
            if (Action.Equals("create"))
            {
                returnOrderDto.ReturnOrderNo = string.Empty;
                returnOrderDto.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
            }
            if (Action.Equals("edit") && !string.IsNullOrEmpty(ReturnOrderNo))
            {
                var result = await _returnOrderService.GetReturnOrderByReturnNoAsync(ReturnOrderNo);
                returnOrderDto = result.Data;
                var data = await _locationServices.GetAllAsync();
                if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));

                #region TEMPORARY IMPLEMENTATION2
                try
                {
                    if (!string.IsNullOrEmpty(returnOrderDto.ShipmentNo))
                    {
                        var products = await _warehouseShipmentServices.GetProductByShipmentNoAsync(string.Empty, returnOrderDto.ShipmentNo);
                        var model = new QueryModel<WarehouseShipmentSearchModel>
                        {
                            Entity = new WarehouseShipmentSearchModel() { ShipmentNo = returnOrderDto.ShipmentNo, Type = EnumWarehouseTransType.Shipment, Status = EnumShipmentOrderStatus.Completed },
                            PageNumber = 1,
                            PageSize = 10
                        };
                        var result1 = await _warehouseShipmentServices.SearchWhShipments(model);
                        if (result1.Succeeded)
                        {
                            _autocompleteShipmentNos = result1.Data.Items;
                        }
                        else
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Error,
                                Summary = _CLoc["Error"],
                                Detail = _localizer["FailedToSearchShipmentNos", result1.Messages],
                                Duration = 4000
                            });
                        }
                        productCodeFilter = products.Data.Select(x => x.ProductCode).ToList();
                        selectedShipmentNo = _autocompleteShipmentNos.Where(x => x.ShipmentNo == returnOrderDto.ShipmentNo).First();
                        if (products.Succeeded)
                        {
                            shippingProducts = products.Data;
                           // var shipmentLinesData = await _warehouseShipmentLineServices.GetByMasterCodeAsync(returnOrderDto.ShipmentNo);
                          //  if (shipmentLinesData != null)
                            {
                                // shipmentLines = shipmentLinesData.Data;
                                var returnedOrders = await _returnOrderService.GetReturnByShipmentNo(returnOrderDto.ShipmentNo);
                                if (returnedOrders.Succeeded && returnedOrders.Data != null)
                                {
                                    foreach (ReturnOrderLineDto line in returnOrderDto.ReturnOrderLines)
                                    {
                                        var packedQty = shippingProducts.Where(x => x.ProductCode == line.ProductCode).Select(x => x.QuantityShipment).FirstOrDefault();
                                        double? returnedQty = 0;
                                        foreach (ReturnOrderDto order in returnedOrders.Data)
                                        {
                                            returnedQty += order.ReturnOrderLines.Where(x => x.ProductCode == line.ProductCode && x.ReturnOrderNo != returnOrderDto.ReturnOrderNo).Select(x => x.Qty).Sum();

                                        }

                                        //var returnedQty = returnedLines.Data.ReturnOrderLines.Where(x => x.ProductCode == line.ProductCode && x.ReturnOrderNo != returnOrderDto.ReturnOrderNo).Select(x => x.Qty).Sum();
                                        line.AvailableReturnQty = (int) (packedQty - returnedQty);
                                    }
                                }


                               
                               

                            }
                            //returnOrderDto.ReturnOrderLines.ForEach(l => l.AvailableReturnQty = _autocompleteProducts.Where(x => x.ProductCode == l.ProductCode).Select(x => x.QuantityShipment).FirstOrDefault());
                        }
                        else
                        {
                            _notificationService.Notify(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Error,
                                Summary = _CLoc["Error"],
                                Detail = products.Messages.FirstOrDefault() ?? _localizer["FailedToSearchProducts", products.Messages],
                                Duration = 4000
                            });
                        }
                        
                    }
                    else
                    {
                        shippingProducts = null;
                    }
                }
                catch (Exception ex)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _CLoc["Error"],
                        Detail = _localizer["ErrorOccurredWhileSearchingProducts", ex.Message],
                        Duration = 4000
                    });
                }
                #endregion
            }
            
        }

        private async Task OnShipmentNoSelected(object shipmentNo)
        {
            shipmentNo = _autocompleteShipmentNo.Value;
            selectedShipmentNo = _autocompleteShipmentNos.FirstOrDefault(x => x.ShipmentNo == shipmentNo.ToString()) ?? new();
            returnLinesToInsert = new();
            returnLinesToUpdate = new();
            returnOrderDto.ReturnOrderLines = new();
            try
            {
                if (!string.IsNullOrEmpty(selectedShipmentNo.ShipmentNo))
                {
                    returnOrderDto.ShipDate = selectedShipmentNo.PlanShipDate;
                    //var lines = await _warehouseShipmentLineServices.GetByMasterCodeAsync(selectedShipmentNo.ShipmentNo);
                    //shipmentLines = lines.Data;
                    var productDtoList = await _warehouseShipmentServices.GetProductByShipmentNoAsync(string.Empty, selectedShipmentNo.ShipmentNo);
                    
                    if (productDtoList.Succeeded)
                    {
                        productCodeFilter = productDtoList.Data.Select(x => x.ProductCode).ToList();
                        shippingProducts = productDtoList.Data;
                    }
                    else
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = _CLoc["Error"],
                            Detail = _localizer["FailedToSearchProducts", productDtoList.Messages],
                            Duration = 4000
                        });
                    }
                }
                else
                {
                    shippingProducts = null;
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _localizer["ErrorOccurredWhileSearchingProducts", ex.Message],
                    Duration = 4000
                });
            }
        }

        private async Task AutocompleteShipmentNo(LoadDataArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(args.Filter) && args.Filter.Length >= 2)
                {
                    var model = new QueryModel<WarehouseShipmentSearchModel>
                    {
                        Entity = new WarehouseShipmentSearchModel() { ShipmentNo = args.Filter, Type = EnumWarehouseTransType.Shipment, Status = EnumShipmentOrderStatus.Completed },
                        PageNumber = 1,
                        PageSize = 10
                    };
                    var result = await _warehouseShipmentServices.SearchWhShipments(model);
                    if (result.Succeeded)
                    {
                        _autocompleteShipmentNos = result.Data.Items;
                    }
                    else
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = _CLoc["Error"],
                            Detail = _localizer["FailedToSearchShipmentNos", result.Messages],
                            Duration = 4000
                        });
                    }
                }
                else
                {
                    _autocompleteShipmentNos = null;
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _localizer["ErrorOccurredWhileSearchingProducts", ex.Message],
                    Duration = 4000
                });
            }
        }

        private async Task GetUserAsync()
        {
            var data = await _userToTenantServices.GetUsersAsync();
            if (data.Count > 0) users.AddRange(data);
        }

        private async Task Submit(ReturnOrderDto arg)
        {
            // Validate Return Quantity cannot greater than Packed Quantity
            foreach(ReturnOrderLineDto line in returnOrderDto.ReturnOrderLines)
            {
                double totalReturnQty = returnOrderDto.ReturnOrderLines.Where(x => x.ProductCode == line.ProductCode).Select(x => x.Qty).Sum()??0;
                if (totalReturnQty > line.AvailableReturnQty)
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["Returned quantity cannot be greater than packed quantity of shipment."]}");
                    return;
                }
            }
            
            if (Action.Equals("create"))
            {
                await CreateReturnOrder(arg);
            }
            else if (Action.Equals("edit"))
            {
                await UpdateReturnOrder(arg);
            }

            StateHasChanged();
        }

        private async Task CreateReturnOrder(ReturnOrderDto arg)
        {
            try
            {
                if (!await ConfirmAction($"{_localizer["DoYouWantToCreateANewReturnOrder"]}", $"{_localizer["CreateReturnOrder"]}")) return;

                var response = await _returnOrderService.InsertReturnOrderAsync(arg);

                if (response.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyCreatedReturnOrder"]}");

                    var res = response.Data;

                    arg.ReturnOrderLines.ForEach(l => l.ReturnOrderNo = response.Data.ReturnOrderNo);
                    ReturnOrderNo = response.Data.ReturnOrderNo;
                    returnOrderDto.ReturnOrderNo = ReturnOrderNo;
                    //StateHasChanged();
                    //_navigation.NavigateTo($"/return-order/edit/{response.Data.ReturnOrderNo}", true);
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToCreateReturnOrder"]}");
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }
        }

        private async Task UpdateReturnOrder(ReturnOrderDto arg)
        {
            if (!await ConfirmAction($"{_CLoc["Confirmation.Update"]} ?", $"{_localizer["UpdateReturnOrder"]}")) return;
            try
            {
                var response = await _returnOrderService.UpdateReturnOrderAsync(arg);

                if (response.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyEditedReturnOrder"]}");
                    arg.ReturnOrderLines.ForEach(l => l.ReturnOrderNo = response.Data.ReturnOrderNo);
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToEditReturnOrder"]}");
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }
        }

        private async Task DeleteReturnOrder()
        {
            if (!await ConfirmAction($"{_CLoc["Confirmation.Delete"]} ?", $"{_localizer["DeleteReturnOrder"]}")) return;
            try
            {
                var response = await _returnOrderService.DeleteReturnOrderAsync(returnOrderDto.Id);

                if (response.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyDeletedReturnOrder"]}");
                    _navigation.NavigateTo("/return-order-list");
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToDeleteReturnOrder"]}");
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }
        }

        private async Task CreateReceipt()
        {
            // Validate Return Quantity cannot greater than Packed Quantity
            foreach (ReturnOrderLineDto line in returnOrderDto.ReturnOrderLines)
            {
                //double? totalReturnQty = 0;
                double totalReturnQty = returnOrderDto.ReturnOrderLines.Where(x => x.ProductCode == line.ProductCode).Select(x => x.Qty).Sum() ?? 0;
                /*
                var returnedLines = await _returnOrderService.GetReturnByShipmentNo(returnOrderDto.ShipmentNo);
                if (returnedLines.Succeeded && returnedLines.Data != null)
                {
                    totalReturnQty = returnedLines.Data.ReturnOrderLines.Where(x => x.ProductCode == line.ProductCode).Select(x => x.Qty).Sum() - line.Qty;
                }
                */
                if (totalReturnQty > line.AvailableReturnQty)
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["InvalidQty"]}");
                    return;
                }
            }
            if (!await ConfirmAction($"{_localizer["DoYouWantToCreateANewReceipt"]}", $"{_localizer["CreateReceipt"]}")) return;

            var model = new QueryModel<WarehouseShipmentSearchModel>
            {
                Entity = new WarehouseShipmentSearchModel() { ShipmentNo = returnOrderDto.ShipmentNo, Type = EnumWarehouseTransType.Shipment, Status = EnumShipmentOrderStatus.Completed },
                PageNumber = 1,
                PageSize = 10
            };
            var result = await _warehouseShipmentServices.SearchWhShipments(model);
            if (!result.Succeeded) ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToCreateReceipt"]}");

            var shipment = result.Data.Items.FirstOrDefault(x => x.ShipmentNo == returnOrderDto.ShipmentNo.ToString()) ?? new();
            var receiptPayload = new WarehouseReceiptOrderDto()
            {
                ReceiptNo = string.Empty,
                Location = shipment.Location,
                TenantId = shipment.TenantId,
                SupplierId = 0,
                PersonInChargeName = returnOrderDto.PersonInCharge,
                ExpectedDate = new DateOnly(2023, 11, 10),
                PersonInCharge = returnOrderDto.PersonInCharge,
                WarehouseReceiptOrderLines = returnOrderDto.ReturnOrderLines.Select(_ => new WarehouseReceiptOrderLineDto
                {
                    ProductCode = _.ProductCode,
                    UnitId = _.UnitId,
                    OrderQty = _.Qty,
                }).ToList(),
                Status = EnumReceiptOrderStatus.Draft,
                ReferenceType = EnumWarehouseTransType.Return,
                ReferenceNo = returnOrderDto.ReturnOrderNo
            };

            var response = await _warehouseReceiptOrderService.InsertWarehouseReceiptOrder(receiptPayload);

            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyCreatedReceipt"]}");
            }
            else
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToCreateReceipt"]}");
            }
        }

        #region COMMON COMPONENT
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

        private string GetStatusColor(EnumReturnOrderStatus status) => status switch
        {
            EnumReturnOrderStatus.Open => "default",
            EnumReturnOrderStatus.Receiving => "info",
            EnumReturnOrderStatus.Received => "primary",
            EnumReturnOrderStatus.Putaway => "error",
            EnumReturnOrderStatus.Completed => "success",
            _ => "default",
        };

        private string GetValueReturnOrderStatus(EnumReturnOrderStatus status) => status switch
        {
            EnumReturnOrderStatus.Open => _localizer["Open"],
            EnumReturnOrderStatus.Receiving => _localizer["Receiving"],
            EnumReturnOrderStatus.Received => _localizer["Received"],
            EnumReturnOrderStatus.Putaway => _localizer["Putaway"],
            EnumReturnOrderStatus.Completed => _localizer["Completed"],
            _ => status.ToString(),
        };
        #endregion

        #region RETURN ORDER LINE

        void Reset()
        {
            returnLinesToInsert.Clear();
            returnLinesToUpdate.Clear();
        }

        void Reset(ReturnOrderLineDto detail)
        {
            returnLinesToInsert.Remove(detail);
            returnLinesToUpdate.Remove(detail);
        }
        void OnUpdateRow(ReturnOrderLineDto line)
        {
            Reset(line);
        }
        async Task EditRow(ReturnOrderLineDto line)
        {
            selectedStockAvaibility = line.AvailableReturnQty;
            if (editMode == DataGridEditMode.Single && returnLinesToInsert.Count() > 0)
            {
                Reset();
            }
            currentEditQty = line.Qty??0;
            selectedProduct.ProductCode = line.ProductCode;
            selectedProduct.ProductName = line.ProductName;
            selectedProduct.UnitName = line.UnitName;
            selectedProduct.UnitId = line.UnitId;
            selectedProduct.StockAvailableQuantity = line.AvailableReturnQty;
            returnLinesToUpdate.Add(line);
            await _returnOrderLineProfileGrid.EditRow(line);
        }

       

        async Task SaveRow(ReturnOrderLineDto line)
        {
            line.ProductCode = selectedProduct.ProductCode;
            if (string.IsNullOrEmpty(line.ProductCode))
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _localizer["ProductIsRequired"],
                    Duration = 4000
                });
                return;
            }
            if (string.IsNullOrEmpty(selectedProduct.ProductCode))
            {
                var selectedLine = returnOrderDto.ReturnOrderLines.Where(_ => _.Id == line.Id).FirstOrDefault();
                var product = await _productServices.GetByProductCodeAsync(line.ProductCode);
                if (product != null)
                {
                    line.ProductName = product.Data.ProductName;
                    line.UnitId = product.Data.UnitId;
                    line.UnitName = product.Data.UnitName;
                }
            }
            else
            {
                line.ProductName = selectedProduct.ProductName;
                line.UnitName = selectedProduct.UnitName;
                line.UnitId = selectedProduct.UnitId;
                line.AvailableReturnQty = selectedProduct.StockAvailableQuantity;

            }

           // if (line.ReturnOrderNo == "")
          //  {
          //      line.Id = Guid.Empty;
         //   }
            //line.ProductName = selectedProduct.ProductName;
            //line.UnitName = selectedProduct.UnitName;
           // line.UnitId = selectedProduct.UnitId;
           // line.LocationName = locations.Where(_ => _.Id == line.Location).Select(_ => _.LocationName).FirstOrDefault();
            
            
            await _returnOrderLineProfileGrid.UpdateRow(line);
            selectedProduct = new();
        }
      
        void CancelEdit(ReturnOrderLineDto line)
        {
            line.Qty = currentEditQty;
            Reset(line);
            _returnOrderLineProfileGrid.CancelEditRow(line);
        }

        async Task DeleteRow(ReturnOrderLineDto line)
        {
            var confirm = await _dialogService.Confirm($"{_CLoc["Confirmation.Delete"]}?", $"{_CLoc["Delete"]}",
                new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true }) ?? false;
            if (confirm == true)
            {
                Reset(line);
                if (returnOrderDto.ReturnOrderLines.Contains(line))
                {
                    returnOrderDto.ReturnOrderLines.Remove(line);
                    await _returnOrderLineProfileGrid.Reload();
                }
                else
                {
                    _returnOrderLineProfileGrid.CancelEditRow(line);
                    await _returnOrderLineProfileGrid.Reload();
                }
            }
        }

        async Task InsertRow()
        {
            selectedStockAvaibility = 0;
            if (editMode == DataGridEditMode.Single)
            {
                Reset();
            }

            var line = new ReturnOrderLineDto();
            line.Id = Guid.NewGuid();
            returnLinesToInsert.Add(line);
            await _returnOrderLineProfileGrid.InsertRow(line);
        }

        void OnCreateRow(ReturnOrderLineDto line)
        {
            returnOrderDto.ReturnOrderLines.Add(line);
            returnLinesToInsert.Remove(line);
        }

        private async Task AutocompleteProduct(LoadDataArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(returnOrderDto.ShipmentNo))
                {
                    var result = await _warehouseShipmentServices.GetProductByShipmentNoAsync(args.Filter, returnOrderDto.ShipmentNo);
                    if (result.Succeeded)
                    {
                        shippingProducts = result.Data;
                    }
                    else
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = _CLoc["Error"],
                            Detail = _localizer["FailedToSearchProducts", result.Messages],
                            Duration = 4000
                        });
                    }
                }
                else
                {
                    shippingProducts = null;
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _localizer["ErrorOccurredWhileSearchingProducts", ex.Message],
                    Duration = 4000
                });
            }
        }
       
        
        private async Task  OnProductSelected(ProductDto product)
        {
            //check exist productcode & bin
            selectedProduct = product ?? new();
            
            if (CheckExistLine(selectedProduct.ProductCode.ToString()))
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _localizer["ProductIsExisted"],
                    Duration = 4000
                });
                _returnOrderLineProfileGrid.Reset();
                return;
            }
            double? returnedQty = 0;
           // _autocompleteProducts = _productAutocomplete.Values;
          
         //   selectedProduct = (ProductDto)productCode;//_autocompleteProducts.FirstOrDefault(x => x.ProductCode == productCode.ToString()) ?? new();
            
            var packedQty = shippingProducts.Where(x => x.ProductCode == selectedProduct.ProductCode).Select(x => x.QuantityShipment).FirstOrDefault();
           
            var returnedOrders =  await _returnOrderService.GetReturnByShipmentNo(returnOrderDto.ShipmentNo);
            if (returnedOrders.Succeeded && returnedOrders.Data != null)
            {
                foreach(ReturnOrderDto order in returnedOrders.Data)
                {
                    returnedQty += order.ReturnOrderLines.Where(x => x.ProductCode == selectedProduct.ProductCode && x.ReturnOrderNo != returnOrderDto.ReturnOrderNo).Select(x => x.Qty).Sum();

                }
            }
            selectedProduct.StockAvailableQuantity = (int)(packedQty - returnedQty);
            selectedStockAvaibility = selectedProduct.StockAvailableQuantity;
            StateHasChanged();
        }

        private bool CheckExistLine(string productCode)
        {
            var ret = false;
            var list = returnOrderDto.ReturnOrderLines.Where(_ =>
                _.ProductCode == productCode);
            ret =  list.Count() > 0 ? true : false;
            return ret;

        }

        #endregion
    }
}
