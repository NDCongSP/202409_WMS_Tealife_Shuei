using Application.DTOs.Request.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using AdjustmentModel = FBT.ShareModels.WMS.InventAdjustment;


namespace WebUIFinal.Pages.InventoryManagement
{
    public partial class InventoryAdjustmentDetails
    {
        List<LocationDisplayDto> locations = new();
        List<BinDisplayDto> bins = new();

        [Parameter] public string Title { get; set; }        
        public string? AdjustmentNo { get; set; }

        private bool isDisabled = false;
        private InventAdjustmentDTO warehouseAdjustment = new InventAdjustmentDTO();
        
        private List<InventAdjustmentsLineDTO> warehouseAdjustmentLine = new List<InventAdjustmentsLineDTO>();
        private RadzenDataGrid<InventAdjustmentsLineDTO>? _adjustmentLineProfileGrid;
        private List<UserDto> users = new List<UserDto>();
        private List<InventAdjustmentDTO> adjustment = new List<InventAdjustmentDTO>();

        private List<TenantDto> tenants = new();
        private string tenantIdModel ;
        private int tenantIdModelValue;


        DataGridEditMode editMode = DataGridEditMode.Single;
        List<InventAdjustmentsLineDTO> AdjustmentLineToInsert = new ();
        List<InventAdjustmentsLineDTO> AdjustmentLineToUpdate = new ();
        private IEnumerable<ProductDto> _autocompleteProducts;
        private RadzenAutoComplete _autocompleteProduct;
        private ProductDto selectedProduct = new();
        private List<ProductDto> productList = new (); // List to hold products
        DateOnly? expirationDate;
        FBT.ShareModels.WMS.NumberSequences sequenceAdjustmentData;
        protected override async Task OnParametersSetAsync()
        {
            await GetLocationFilterListAsync();

            await GetBinFilterListAsync();
            await GetUserAsync();
            await GetTenantsAsync();
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            await GetAdjustmentAsync();
            if (warehouseAdjustment.AdjustmentDate == default)
            {
                warehouseAdjustment.AdjustmentDate = DateTime.Today;
            }
            if (string.IsNullOrEmpty(warehouseAdjustment.PersonInCharge))
            {
                warehouseAdjustment.PersonInCharge = GlobalVariable.UserAuthorizationInfo.UserId;
            }
            if (string.IsNullOrEmpty(warehouseAdjustment.Location))
            {
                var firstLocation = locations.FirstOrDefault();
                warehouseAdjustment.Location = firstLocation != null ? firstLocation.Id : "";
            }

            if (Title.Contains($"{_CLoc["Detail.Edit"]}"))
            {
                var adjustmentResult = await _inventAdjustmentServices.GetByAdjustmentNoDTOAsync(AdjustmentNo);

                if (!adjustmentResult.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = "Adjustment No null or not found",
                        Duration = 1000
                    });

                    return;
                }

                AdjustmentNo = adjustmentResult.Data.AdjustmentNo;

                isDisabled = adjustmentResult.Data.Status == EnumInventoryAdjustmentStatus.Completed ? true : false;
            }
        }
        private async Task GetTenantsAsync()
        {
            var data = await _companiesServices.GetAllAsync();

            if (data.Succeeded)
            {
                tenants.Clear();
                tenants.AddRange(data.Data.Select(_ => new TenantDto { Id = _.AuthPTenantId.ToString(), TenantName = _.FullName }));
            }
        }

        private bool CheckDisableLoadProduct(InventAdjustmentDTO data)
        {
            return data.Status == EnumInventoryAdjustmentStatus.Completed || data.Bin == Guid.Empty.ToString() || string.IsNullOrEmpty(data.Bin);
        }
        private async Task GetUserAsync()
        {
            var data = await _userToTenantServices.GetUsersAsync();
            if (data.Count > 0) users.AddRange(data);
        }
        private async Task GetLocationFilterListAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
        }

        private async Task GetBinFilterListAsync()
        {
            var data = await _binServices.GetAllAsync();
            if (data.Succeeded) bins.AddRange(data.Data.Select(_ => new BinDisplayDto { Id = _.Id.ToString(), BinCode = _.BinCode }));
        }
        private async Task UpdateListBin()
        {
            var data = await _binServices.GetAllAsync();
            bins.Clear();
            if (data.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(warehouseAdjustment.Location))
                {
                    data.Data = data.Data.Where(x => x.LocationId.ToString() == warehouseAdjustment.Location).ToList();
                }
                bins.AddRange(data.Data.Select(_ => new BinDisplayDto { Id = _.Id.ToString(), BinCode = _.BinCode }));
            }
        }
        private async Task UpdateTenantValue()
        {
            if (warehouseAdjustment.Id != Guid.Empty)
            {

                if (!await ConfirmAction($"{_CLoc["Confirm.Message.ChangeTenant"]}", $"{_CLoc["Confirm.Title.ChangeTenant"]}"))
                {
                    // If the user selects "No", reset the selected tenant to the previous value
                    tenantIdModel = tenantIdModelValue.ToString();
                    return;
                }
                tenantIdModelValue = Convert.ToInt32(tenantIdModel);
                if (warehouseAdjustment.Id != Guid.Empty)
                {
                    await _inventAdjustmentServices.DeleteAllAdjustmentLineAsync(warehouseAdjustment.Id);
                }
                warehouseAdjustment.TenantId = tenantIdModelValue;

                await LoadAdjustmentLine();

            }
            else
            {
                tenantIdModelValue = Convert.ToInt32(tenantIdModel);
            }
        }

        async Task Submit(InventAdjustmentDTO arg)
        {
            if (Title.Contains($"{_CLoc["Detail.Create"]}"))
            {
                // Update the sequence number in the database
                var sequenceAdjustment = await _numberSequenceServices.GetNumberSequenceByType("Adjustment");
                if (sequenceAdjustment.Succeeded)
                {
                    warehouseAdjustment.AdjustmentNo = $"{sequenceAdjustment.Data.Prefix}{sequenceAdjustment.Data.CurrentSequenceNo.ToString().PadLeft((int)sequenceAdjustment.Data.SequenceLength, '0')}";
                    // Update the sequence number in the database
                    sequenceAdjustment.Data.CurrentSequenceNo += 1;
                    sequenceAdjustmentData = sequenceAdjustment.Data;
                }
                await CreateAdjustment(arg);
                await _numberSequenceServices.UpdateAsync(sequenceAdjustmentData);
            }
            else if (Title.Contains($"{_CLoc["Detail.Edit"]}"))
            {
                await UpdateAdjustment(arg);
            }
            StateHasChanged();
        }

        private async Task GetAdjustmentAsync()
        {

            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (sub.Length > 1)
                {
                    AdjustmentNo = sub[1].Trim();
                }
            }

            if (!string.IsNullOrEmpty(AdjustmentNo))
            {
                //get adjustment + get adjustment line
                //get daault products
                var adjustmentInfo = await _inventAdjustmentServices.GetByAdjustmentNoDTOAsync(AdjustmentNo);
                if (!adjustmentInfo.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = "Result warehouse null or not found",
                        Duration = 1000
                    });

                    return;
                }

                warehouseAdjustment = adjustmentInfo.Data;
                tenantIdModel = adjustmentInfo.Data.TenantId.ToString();
                tenantIdModelValue = Convert.ToInt32(tenantIdModel);
                await LoadAdjustmentLine();
                if (!string.IsNullOrEmpty(warehouseAdjustment.Location))
                    await UpdateListBin();
            }
        }

        private async Task LoadAdjustmentLine()
        {
            var adjustmentLineDataDefaultProduct = await _warehouseTranServices.GetInventoryAdjustmentLinesDefaultProduct(new InventoryAdjustmentDetailsModel()
            {
                Location = warehouseAdjustment.Location,
                Bin = warehouseAdjustment.Bin,
                TenantId = Convert.ToInt32(tenantIdModel),

            });
            if (warehouseAdjustment.Status != EnumInventoryAdjustmentStatus.Completed && (adjustmentLineDataDefaultProduct != null && adjustmentLineDataDefaultProduct.Succeeded))
            {
                warehouseAdjustmentLine.Clear();
                foreach (var item in adjustmentLineDataDefaultProduct.Data)
                {
                    if (warehouseAdjustment.InventAdjustmentLines != null && warehouseAdjustment.InventAdjustmentLines.Count > 0)
                    {
                        var lines = warehouseAdjustment.InventAdjustmentLines.FirstOrDefault(
                            x => x.ProductCode == item.ProductCode && x.LotNo == item.LotNo
                            );
                        if (lines != null)
                        {
                            item.Qty = lines.Qty;
                            item.FinalQty = item.Qty + item.FinalQty;
                            item.Id = lines.Id;
                            item.ExpirationDate = lines.ExpirationDate;
                        }
                    }
                }
                foreach (var item in warehouseAdjustment.InventAdjustmentLines)
                {
                    if (!adjustmentLineDataDefaultProduct.Data.Any(x => x.ProductCode == item.ProductCode && x.LotNo == item.LotNo))
                    {
                        warehouseAdjustmentLine.Add(item);
                    }
                }
                warehouseAdjustmentLine.AddRange(adjustmentLineDataDefaultProduct.Data);
                warehouseAdjustment.InventAdjustmentLines = warehouseAdjustmentLine;
               // _adjustmentLineProfileGrid.Data = warehouseAdjustment.InventAdjustmentLines;
              await _adjustmentLineProfileGrid.Reload();
            }
        }
      
        private async Task CreateAdjustment(InventAdjustmentDTO arg)
        {
            if (!await ConfirmAction($"{_localizer["DoYouWantToCreateANewAdjustment"]}", $"{_localizer["CreateAdjustment"]}")) return;
            // Get the next sequence number for AdjustmentNo

            var adjustment = new AdjustmentModel
            {
                Id = Guid.NewGuid(),
                AdjustmentNo = arg.AdjustmentNo,
                Location = arg.Location,
                Bin = arg.Bin,
                AdjustmentDate = arg.AdjustmentDate,
                Status = arg.Status,
                PersonInCharge = arg.PersonInCharge,
                TenantId = Convert.ToInt32( tenantIdModel)

            };

            var response = await _inventAdjustmentServices.InsertAsync(adjustment);

            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyCreatedAdjustment"]}");
           
                _navigation.NavigateTo($"/adjustment-details/{_CLoc["Detail.Edit"]}|{adjustment.AdjustmentNo}");

            }
            else
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["SuccessfullyCreatedAdjustment"]}");
            }
        }

        private async Task UpdateAdjustment(InventAdjustmentDTO arg)
        {
            if (!await ConfirmAction($"{_CLoc["Confirmation.Update"]} ?", $"{_localizer["UpdateAdjustment"]}")) return;
            try
            {
                var checkPayload = await _inventAdjustmentServices.GetByIdAsync(arg.Id);

                if (checkPayload != null)
                {
                    checkPayload.Data.AdjustmentNo = arg.AdjustmentNo;
                    checkPayload.Data.Location = arg.Location;
                    checkPayload.Data.AdjustmentDate = arg.AdjustmentDate;
                    checkPayload.Data.Status = arg.Status;
                    checkPayload.Data.Bin = arg.Bin;
                    checkPayload.Data.PersonInCharge = arg.PersonInCharge;
                    checkPayload.Data.TenantId = Convert.ToInt32(tenantIdModel);

                    var response = await _inventAdjustmentServices.UpdateAsync(checkPayload.Data);
                    if (response.Succeeded)
                    {
                        ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyEditedAdjustment"]}");

                    }
                    else
                    {
                        ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToEditAdjustment"]}");
                    }
                    await GetAdjustmentAsync();
                }

                //After save, reload default lines

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

        private async Task DeleteItemAsync(InventAdjustmentDTO model)
        {


            if (!await ConfirmAction(_CLoc["Confirmation.Delete"], _CLoc["Delete"])) return;

            try
            {
                var res = await _inventAdjustmentServices.DeleteAsync(new FBT.ShareModels.WMS.InventAdjustment
                {
                    Id = model.Id,
                    AdjustmentNo = model.AdjustmentNo,
                    Location = model.Location,
                    AdjustmentDate = model.AdjustmentDate,
                    PersonInCharge = model.PersonInCharge
                });

                if (res.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], "");
                    adjustment.Remove(model);
                    _navigation.NavigateTo($"/inventory-adjustment-list");

                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], res.Messages?.FirstOrDefault()?.ToString());
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }

        }


        private async Task ReloadProductList()
        {
            //get adjustment + get adjustment line
            var adjustmentLineDataDefaultProduct = await _warehouseTranServices.GetInventoryAdjustmentLinesDefaultProduct(new InventoryAdjustmentDetailsModel()
            {
                Location = warehouseAdjustment.Location,
                Bin = warehouseAdjustment.Bin,
                TenantId = Convert.ToInt32(tenantIdModel),
            });
            if (warehouseAdjustment.Status != EnumInventoryAdjustmentStatus.Completed && (adjustmentLineDataDefaultProduct != null && adjustmentLineDataDefaultProduct.Succeeded))
            {
                warehouseAdjustmentLine.Clear();
                if(warehouseAdjustment.InventAdjustmentLines != null)
                    warehouseAdjustment.InventAdjustmentLines.Clear();
                
                warehouseAdjustmentLine.AddRange(adjustmentLineDataDefaultProduct.Data);
                warehouseAdjustment.InventAdjustmentLines = warehouseAdjustmentLine;
                await _adjustmentLineProfileGrid.Reload();
            }
        }

        private async Task<List<ProductDto>> LoadTenantProductsAsync()
        {
            var result = await _productServices.AutocompleteProductAsync(null, Convert.ToInt32(tenantIdModel));

            if (result.Succeeded)
            {
                return result.Data.ToList();
            }
            else
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _localizer["FailedToLoadProducts", result.Messages],
                    Duration = 4000
                });
                return new List<ProductDto>();
            }
        }
        
        private async Task AutocompleteProduct(LoadDataArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(args.Filter) && args.Filter.Length >= 2)
                {
                    var result = await _productServices.SearchByProductCodeAsync(args.Filter);
                    if (result.Succeeded)
                    {
                        _autocompleteProducts = result.Data;
                    }
                    else
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = _CLoc["Error"],
                            Detail = _CLoc["FailedToSearchProducts", result.Messages],
                            Duration = 4000
                        });
                    }
                }
                else
                {
                    _autocompleteProducts = null;
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _CLoc["ErrorOccurredWhileSearchingProducts", ex.Message],
                    Duration = 4000
                });
            }
        }


        public async Task ChangeLotNo(string lotNo)
        {
            //check exist lotno
            expirationDate = null;
            var getExpirationByLotNo = await _batchServices.GetBatchByLotNo(new GetBatchByLotNoDto { TenantId = tenantIdModelValue, ProductCode = selectedProduct.ProductCode, LotNo = lotNo });
            if (getExpirationByLotNo.Succeeded)
            {
                expirationDate = getExpirationByLotNo.Data.ExpirationDate;
            }
        }

        private void OnProductSelected(object productCode)
        {
            productCode = _autocompleteProduct.Value;
            selectedProduct = _autocompleteProducts.FirstOrDefault(x => x.ProductCode == productCode.ToString()) ?? new();
        }

        async Task InsertRow()
        {
            if (warehouseAdjustment.InventAdjustmentLines == null)
            {
                warehouseAdjustment.InventAdjustmentLines = new List<InventAdjustmentsLineDTO>();
            }

            if (_adjustmentLineProfileGrid == null)
            {
                // Handle the case where _adjustmentLineProfileGrid is null
                // For example, you might want to log an error or throw an exception
                throw new InvalidOperationException(_localizer["_adjustmentLineProfileGrid is not initialized."]);
            }
            expirationDate = null;
            var newLine = new InventAdjustmentsLineDTO(); // Create a new line item
            warehouseAdjustment.InventAdjustmentLines.Insert(0, newLine); // Add it to the list
            await _adjustmentLineProfileGrid.InsertRow(newLine); // Insert the new row into the grid
        }

        async Task EditRow(InventAdjustmentsLineDTO item)
        {
            if (editMode == DataGridEditMode.Single && AdjustmentLineToInsert.Count > 0)
            {
                Reset();
            }
            AdjustmentLineToUpdate.Add(item);
            expirationDate = item.ExpirationDate;
            try
            {
                var products = await LoadTenantProductsAsync();
                if (products.Any())
                {
                    selectedProduct = products.FirstOrDefault(p => p.ProductCode == item.ProductCode);

                    if (selectedProduct != null)
                    {
                        item.ProductName = selectedProduct.ProductName;
                        item.UnitId = selectedProduct.UnitId;
                        item.UnitName = selectedProduct.UnitName;
                        item.ProductCode = selectedProduct.ProductCode;
                    }
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _localizer["ErrorLoadingProductData"],
                    Duration = 4000
                });
            }
            selectedLotno = item.LotNo;
            await OnProductChange(selectedProduct);
            await _adjustmentLineProfileGrid.EditRow(item);
        }
        string selectedLotno { get; set; }
        private void OnUpdateRow(InventAdjustmentsLineDTO item)
        {
            // Update the warehouseAdjustmentLine with the updated item
            Reset(item);
        }
       
        private void OnCancelEdit(InventAdjustmentsLineDTO item)
        {
           
            warehouseAdjustmentLine.Remove(item);
            _adjustmentLineProfileGrid.CancelEditRow(item);
        }

        private void OnCreateRow(InventAdjustmentsLineDTO item)
        {
            warehouseAdjustmentLine.Insert(0, item);
            warehouseAdjustment.InventAdjustmentLines.Insert(0, item);
            AdjustmentLineToInsert.Remove(item);
        }
        void Reset()
        {
            AdjustmentLineToInsert.Clear();
            AdjustmentLineToUpdate.Clear();
        }
        void Reset(InventAdjustmentsLineDTO item)
        {
            AdjustmentLineToInsert.Remove(item);
            AdjustmentLineToUpdate.Remove(item);
        }

        private async Task DeleteRow(InventAdjustmentsLineDTO item)
        {
            var confirm = await _dialogService.Confirm(_localizer["DeleteWarehouseAdjustmentConfirmation"], _CLoc["Confirmation.Delete"], new ConfirmOptions() { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"] });
            if (confirm == true)
            {
                Reset(item);
                if (warehouseAdjustment.InventAdjustmentLines.Contains(item))
                {
                    InventAdjustmentLine  deleteLine = new InventAdjustmentLine();
                    deleteLine.Id = item.Id;
                        deleteLine.ProductCode = item.ProductCode;
                        _inventAdjustmentLineServices.DeleteAsync(deleteLine);
                    if(item.StockAvailable != null)
                    {
                        var itemIndex = warehouseAdjustment.InventAdjustmentLines.IndexOf(item);
                        warehouseAdjustment.InventAdjustmentLines[itemIndex].Qty = null;
                        warehouseAdjustment.InventAdjustmentLines[itemIndex].FinalQty = null;
                    }
                    else
                    {
                        warehouseAdjustment.InventAdjustmentLines.Remove(item);
                    }
                    await _adjustmentLineProfileGrid.Reload();        
                }
                else
                {
                    _adjustmentLineProfileGrid.CancelEditRow(item);
                    await _adjustmentLineProfileGrid.Reload();
                } 
            }
        }
        
        private async Task SaveRow(InventAdjustmentsLineDTO item)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedProduct.ProductCode))
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
                if (expirationDate == null)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _CLoc["Error"],
                        Detail = _localizer["ExpirationDateIsRequired"],
                        Duration = 4000
                    });
                    return;
                }
                if (item.Id != Guid.Empty)
                {
                    // Update existing line
                    var inventAdjustmentsLine = new InventAdjustmentLine
                    {
                        Id = item.Id,
                        ProductCode = selectedProduct.ProductCode,
                        UnitId = selectedProduct.UnitId,
                        AdjustmentNo = AdjustmentNo,
                        LotNo = item.LotNo,
                        ExpirationDate = expirationDate,
                        Qty = item.Qty,
                        UpdateAt = DateTime.Now,
                        Reasons = item.Reasons
                    };
                    var response = await _inventAdjustmentLineServices.UpdateAsync(inventAdjustmentsLine);
                    if (response != null && response.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = _CLoc["Success"],
                            Detail = _localizer["AdjustmentLineUpdateSuccessfully"],
                            Duration = 4000
                        });
                        // Update the matching line item in warehouseAdjustment.InventAdjustmentLines
                        foreach (var line in warehouseAdjustment.InventAdjustmentLines)
                        {
                            if (line.Id == item.Id)
                            {
                                line.ProductCode = selectedProduct.ProductCode;
                                line.UnitId = selectedProduct.UnitId;
                                line.ExpirationDate = expirationDate;
                                line.LotNo = item.LotNo;
                                line.Qty = item.Qty;
                                line.UpdateAt = DateTime.Now;
                                line.Reasons = item.Reasons;
                                break;
                            }
                        }
                        await _adjustmentLineProfileGrid.Reload(); // Refresh grid data
                    }
                    else
                    {
                        throw new Exception(_localizer["Failed to update adjustment line"]);
                    }
                }
                else
                {                   
                    item.AdjustmentNo = warehouseAdjustment.AdjustmentNo;

                    item.Id = Guid.NewGuid();
                    var inventAdjustmentsLine = new InventAdjustmentLine
                    {
                        Id = item.Id,
                        ProductCode = selectedProduct.ProductCode,
                        AdjustmentNo = warehouseAdjustment.AdjustmentNo,
                        ExpirationDate = expirationDate ,
                        UnitId = selectedProduct.UnitId,
                        LotNo = item.LotNo ?? string.Empty,
                        Qty = item.Qty,
                        UpdateAt = DateTime.Now,
                        Reasons = item.Reasons

                    };
                    var response = await _inventAdjustmentLineServices.InsertAsync(inventAdjustmentsLine);
                    if (response != null && response.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = _CLoc["Success"],
                            Detail = _localizer["AdjustmentLineCreateSuccessfully"],
                            Duration = 4000
                        });
                        // Update the matching line item in warehouseAdjustment.InventAdjustmentLines
                        foreach (var line in warehouseAdjustment.InventAdjustmentLines)
                        {
                            if (line.Id == item.Id)
                            {
                                line.ProductCode = selectedProduct.ProductCode;
                                line.UnitId = selectedProduct.UnitId;
                                line.ExpirationDate = expirationDate;
                                line.LotNo = item.LotNo;
                                line.Qty = item.Qty;
                                line.UpdateAt = DateTime.Now;
                                line.Reasons = item.Reasons;
                                break;
                            }
                        }
                        await _adjustmentLineProfileGrid.Reload(); // Refresh grid data
                    }
                    else
                    {
                        throw new Exception(_localizer["Failed to create adjustment line"]);
                    }
                }

                await _adjustmentLineProfileGrid.UpdateRow(item);
                selectedProduct = new();
            }
            catch (Exception ex)
            {
                // Handle exception appropriately
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = ex.Message,
                    Duration = 4000
                });
            }
        }


        private void CancelEdit(InventAdjustmentsLineDTO item)
        {
            
            warehouseAdjustmentLine.Remove(item);
            _adjustmentLineProfileGrid.CancelEditRow(item);
        }

        private async Task OnProductChange(ProductDto prod)
        {

            selectedProduct = prod;
            var currentProduct = await _productServices.GetByProductCodeAsync(selectedProduct.ProductCode);

            foreach (var item in warehouseAdjustmentLine)
            {
                if(selectedProduct.ProductCode == item.ProductCode)
                {
                    item.UnitName = currentProduct.Data.UnitName;
                    item.ProductName = currentProduct.Data.ProductName;

                }
            }

            StateHasChanged();
        }
        private string GetLocalizedStatus(EnumInventoryAdjustmentStatus status)
        {
            return status switch
            {
                EnumInventoryAdjustmentStatus.InProcess => _localizer["InProcess"],
                EnumInventoryAdjustmentStatus.Completed => _localizer["Completed"],
                _ => status.ToString(),
            };
        }

        private List<EnumDisplay<EnumInventoryAdjustmentStatus>> GetDisplayAdjustmentStatus()
        {
            return Enum.GetValues(typeof(EnumInventoryAdjustmentStatus)).Cast<EnumInventoryAdjustmentStatus>().Select(_ => new EnumDisplay<EnumInventoryAdjustmentStatus>
            {
                Value = _,
                DisplayValue = GetValueAdjustmentStatus(_)
            }).ToList();
        }

        private string GetValueAdjustmentStatus(EnumInventoryAdjustmentStatus AdjustmentStatus) => AdjustmentStatus switch
        {

            EnumInventoryAdjustmentStatus.InProcess => _localizer["InProcess"],
            EnumInventoryAdjustmentStatus.Completed => _localizer["Completed"],
            _ => throw new ArgumentException(_localizer["Invalid value for AdjustmentStatus"], nameof(AdjustmentStatus))
        };
        private async Task CompleteAdjustmentAsync(InventAdjustmentDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.AdjustmentNo))
            {
                ShowNotification(NotificationSeverity.Error, "Error", "Invalid adjustment ID.");
                return;
            }

            var response = await _inventAdjustmentServices.CompletedInventAdjustmentAsync(model.Id);
            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, _CLoc["Success"], _localizer["Adjustment completed successfully"]);
                await GetAdjustmentAsync(); // Reload the page to refresh data
            }
            else
            {
                ShowNotification(NotificationSeverity.Error, "Error", "Failed to complete adjustment.");
            }
        }

        private async Task CalculateFinalStockQuantity(InventAdjustmentsLineDTO data, double availableStock)
        {
            data.FinalQty = data.Qty + availableStock;

            //with each inventory, multiple it into K
            if(true) //product is bunlde
            {
                foreach(var item in _inventoryBundleItems)
                {
                    double? qtyPerItem = selectedProduct.StockAvailableQuantityTrans;
                    if(qtyPerItem != 0)
                    {
                        item.AdjustmentQuantity = (double)(data.Qty * item.StockQuantity / qtyPerItem);
                        item.FinalStockQuantity = item.AdjustmentQuantity  + item.StockQuantity ?? 0;
                    }
                }
                await JSRuntime.InvokeVoidAsync("eval", "");
            }
        }

        InventAdjustmentsLineDTO selectedInventoryAdjustmentItem = null;
        private bool showBundleGrid = false;
        private List<InventoryAdjustmentDetailsDto> _inventoryBundleItems = new List<InventoryAdjustmentDetailsDto>();

        //private async Task HandleInventoryAdjustmentGridSelect(InventAdjustmentsLineDTO selectedItem)
        //{
        //    selectedInventoryAdjustmentItem = selectedItem;

        //    showBundleGrid = selectedItem.PerSet;

        //    if (selectedItem.PerSet)
        //    {
        //        await FetchBundleItems(selectedInventoryAdjustmentItem);
        //    }
        //    else
        //    {
        //        _inventoryBundleItems = null; // Clear the bundle grid data
        //    }

        //    // Optional: Force the bundle grid to re-render
        //    await JSRuntime.InvokeVoidAsync("eval", "");
        //}

        private InventoryAdjustmentDetailsModel _searchModel = new InventoryAdjustmentDetailsModel();
        List<InventoryAdjustmentDetailsDto> _dataGridBundle = new();
        RadzenDataGrid<InventoryAdjustmentDetailsDto>? _inventoryBundleGrid;
        IList<InventoryAdjustmentDetailsDto> selectedInventoryBundleList = [];

        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        private async Task FetchBundleItems(InventAdjustmentsLineDTO selectedItem)
        {
            try
            {
                _searchModel.BundleCode = selectedItem.ProductCode;
                _searchModel.Location = warehouseAdjustment.Location;
                _searchModel.Bin = warehouseAdjustment.Bin;
                _searchModel.LotNo = selectedItem.LotNo;
                var res = await _warehouseTranServices.GetInventoryBundleAdjustmentDetailsAsync(_searchModel);

                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = res.Messages.ToString(),
                    });
                    return;
                }

                _dataGridBundle = res.Data.ToList();
                _inventoryBundleItems = _dataGridBundle;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }
        }
    }
}
