using Application.DTOs;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using FBT.ShareModels.WMS;
using TransferModel = FBT.ShareModels.WMS.InventTransfer;
using CompanyTenantModel = FBT.ShareModels.Entities.CompanyTenant;
using TransfersLineModel = FBT.ShareModels.WMS.InventTransferLine;
using static Application.Extentions.ApiRoutes;
using Radzen.Blazor.Rendering;
using Polly;
using Application.Services.Inbound;
using WebUIFinal.Pages.Product;
using System.Reflection;
using FBT.ShareModels.Entities;
using Application.Models;
using Application.Services;
using RestEase;


namespace WebUIFinal.Pages.WarehouseTransfer
{
    public partial class WarehouseTransferDetails
    {
        [Parameter] public string Title { get; set; }
        public string? TransferNo { get; set; }

        public Guid Id { get; set; }
        private bool isDisabled = false;
        private InventTransfersDTO warehouseTransfer = new InventTransfersDTO();
        private InventTransfersLineDTO TransferLine = new InventTransfersLineDTO();
        private List<InventTransfersLineDTO> warehouseTransferLine = new List<InventTransfersLineDTO>();
        private List<LocationDisplayDto> locations = new List<LocationDisplayDto>();
        private RadzenDataGrid<InventTransfersLineDTO>? _transferLineProfileGrid;
        private List<UserDto> users = new List<UserDto>();
        private List<InventTransfersDTO> transfer = new List<InventTransfersDTO>();
        private InventTransfersDTO warehouseTransferBin = new();

        private List<SelectListItem> _lotNo;

        private List<InventoryInfoBinLotDetails> InventoryInfoBinLotDetails = new();
        private LocationDisplayDto selectLocationDisplayDto;
        DataGridEditMode editMode = DataGridEditMode.Single;
        List<InventTransfersLineDTO> TransferLineToInsert = new List<InventTransfersLineDTO>();
        List<InventTransfersLineDTO> TransferLineToUpdate = new List<InventTransfersLineDTO>();
        private IEnumerable<ProductDto> _autocompleteProducts;

        private RadzenAutoComplete _autocompleteProduct;
        private ProductDto selectedProduct = new();
        List<Bin> bins = new List<Bin>();
        List<Bin> binsto = new List<Bin>();
        Dictionary<string, ProductDto> productDict = new Dictionary<string, ProductDto>();
        private List<ProductDto> productList = new List<ProductDto>(); // List to hold products

        List<CompanyTenantModel> _tenants = [];
        CompanyTenantModel _selectTenant;

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                await GetLocationssAsync();
                await GetUserAsync();
                await GetTenantsAsync();

                await GetTransferAsync();
                if (warehouseTransfer.TransferDate == default)
                {
                    warehouseTransfer.TransferDate = DateOnly.FromDateTime(DateTime.Today);
                }
                /*
                if (Title.Contains($"{_localizerCommon["Detail.Create"]}"))
                {
                    var sequenceTransfer = await _numberSequenceServices.GetNumberSequenceByType("Transfer");
                    if (sequenceTransfer.Succeeded)
                    {
                        warehouseTransfer.TransferNo = $"{sequenceTransfer.Data.Prefix}{sequenceTransfer.Data.CurrentSequenceNo.ToString().PadLeft((int)sequenceTransfer.Data.SequenceLength, '0')}";
                        // Update the sequence number in the database


                        await _numberSequenceServices.UpdateAsync(sequenceTransfer.Data);
                    }
                }
                */
                if (Title.Contains($"{_localizerCommon["Detail.Edit"]}"))
                {
                    var transferResult = await _warehouseTransferService.GetByTransferNoDTO(TransferNo);
                    TransferNo = transferResult.Data.TransferNo;
                }
                if (!string.IsNullOrEmpty(warehouseTransfer.Location)) await OnLocationChanged(warehouseTransfer.Location);
                isDisabled = warehouseTransfer.Status == EnumInvenTransferStatus.Completed ? true : false;
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], ex.Message);
            }
        }

        private async Task GetTenantsAsync()
        {
            var data = await _companiesServices.GetAllAsync();

            if (!data.Succeeded)
            {
                ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], data.Messages.FirstOrDefault());
                return;
            }
            _tenants.AddRange(data.Data);
        }
        private async Task GetUserAsync()
        {
            var data = await _userToTenantServices.GetUsersAsync();
            if (data.Count > 0) users.AddRange(data);
        }
        private async Task GetLocationssAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
        }

        async Task Submit(InventTransfersDTO arg)
        {
            //update tenantId
            arg.TenantId = _selectTenant.AuthPTenantId;

            if (Title.Contains($"{_localizerCommon["Detail.Create"]}"))
            {
                await CreateTransfer(arg);
            }
            else// if (Title.Contains($"{_localizerCommon["Detail.Edit"]}"))
            {
                await UpdateTransfer(arg);
            }

            StateHasChanged();
        }

        private async Task GetTransferAsync()
        {

            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (sub.Length > 1)
                {
                    TransferNo = sub[1].Trim();
                }
            }

            if (!string.IsNullOrEmpty(TransferNo))
            {
                var data = await _warehouseTransferService.GetByTransferNoDTO(TransferNo);
                if (!data.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = "Result warehouse null or not found",
                        Duration = 1000
                    });

                    return;
                }

                warehouseTransfer = data.Data;

                _selectTenant = _tenants.Where(x => x.AuthPTenantId == warehouseTransfer.TenantId).FirstOrDefault();

                if (data.Data.InventTransferLines != null)
                {
                    warehouseTransferLine.AddRange(data.Data.InventTransferLines);
                }



            }
        }

        private async Task CreateTransfer(InventTransfersDTO arg)
        {
            // Set the TransferNo for each line in warehouseTransferLine

            if (!await ConfirmAction($"{_localizer["DoYouWantToCreateANewTransfer"]}", $"{_localizer["CreateTransfer"]}")) return;
            // Get the next sequence number for TransferNo
            var sequenceTransfer = await _numberSequenceServices.GetNumberSequenceByType("Transfer");
            if (sequenceTransfer == null)
                throw new Exception("Sequence for Transfer not found");

            // Generate a new TransferNo using the current sequence number
            warehouseTransfer.TransferNo = $"{sequenceTransfer.Data.Prefix}{sequenceTransfer.Data.CurrentSequenceNo.ToString().PadLeft((int)sequenceTransfer.Data.SequenceLength, '0')}";
            foreach (var line in warehouseTransferLine)
            {
                line.TransferNo = warehouseTransfer.TransferNo;
            }

            var transfer = new TransferModel
            {
                Id = Guid.NewGuid(),
                TransferNo = arg.TransferNo,
                Location = arg.Location,
                TransferDate = arg.TransferDate,
                Status = arg.Status,
                PersonInCharge = arg.PersonInCharge,
                TenantId = warehouseTransfer.TenantId,
            };

            var response = await _warehouseTransferService.InsertAsync(transfer);

            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, _localizerCommon["Success"], $"{_localizer["SuccessfullyCreatedTransfer"]}");
                sequenceTransfer.Data.CurrentSequenceNo += 1;
                await _numberSequenceServices.UpdateAsync(sequenceTransfer.Data);
                _navigation.NavigateTo($"/addtransfer/{_localizerCommon["Detail.Edit"]}|{transfer.TransferNo}");
                await GetTransferAsync();

            }
            else
            {
                ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], $"{_localizer["SuccessfullyCreatedTransfer"]}");
            }
        }

        private async Task UpdateTransfer(InventTransfersDTO arg)
        {
            if (!await ConfirmAction($"{_localizerCommon["Confirmation.Update"]} ?", $"{_localizer["UpdateTransfer"]}")) return;
            try
            {
                var checkPayload = await _warehouseTransferService.GetByIdAsync(arg.Id);

                if (checkPayload != null)
                {
                    checkPayload.Data.TransferNo = arg.TransferNo;
                    checkPayload.Data.Location = arg.Location;
                    checkPayload.Data.TransferDate = arg.TransferDate;
                    checkPayload.Data.Status = arg.Status;
                    checkPayload.Data.PersonInCharge = arg.PersonInCharge;
                    checkPayload.Data.TenantId = arg.TenantId;

                    var response = await _warehouseTransferService.UpdateAsync(checkPayload.Data);
                    if (response.Succeeded)
                    {
                        ShowNotification(NotificationSeverity.Success, _localizerCommon["Success"], $"{_localizer["SuccessfullyEditedTransfer"]}");

                    }
                    else
                    {
                        ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], $"{_localizer["FailedToEditTransfer"]}");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], ex.Message);
            }

        }


        private async Task<bool> ConfirmAction(string message, string title)
        {
            return await _dialogService.Confirm(message, title, new ConfirmOptions
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
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

        private async Task DeleteItemAsync(InventTransfersDTO model)
        {


            if (!await ConfirmAction(_localizerCommon["Confirmation.Delete"], _localizerCommon["Delete"])) return;

            try
            {
                var res = await _warehouseTransferService.DeleteAsync(new FBT.ShareModels.WMS.InventTransfer
                {
                    Id = model.Id,
                    TransferNo = model.TransferNo,
                    Location = model.Location,
                    TransferDate = model.TransferDate,
                    PersonInCharge = model.PersonInCharge
                });

                if (res.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Success, _localizerCommon["Success"], "");
                    transfer.Remove(model);
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], res.Messages?.FirstOrDefault()?.ToString());
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], ex.Message);
            }

        }

        private async Task OnLocationChanged(string arg)
        {
            if (Guid.TryParse(arg, out Guid locationId)) // Ensure the argument is parsed correctly
            {
                var data = await _binServices.GetByLocationId(locationId);
                selectLocationDisplayDto = locations.FirstOrDefault(x => x.Id == arg);
                if (data.Succeeded) // Check if the data retrieval was successful
                {
                    binsto.Clear(); // Clear existing bins before adding new ones
                    binsto.AddRange(data.Data); // Add the new bins to the list
                    if (selectedProduct != null)
                    {
                        OnProductChange(selectedProduct);
                    }
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizer["FailedToLoadBins", data.Messages],
                        Duration = 4000
                    });
                }
            }
            else
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _localizerCommon["Warning"],
                    Detail = _localizer["InvalidLocationId"],
                    Duration = 4000
                });
            }
        }

        private async Task<List<ProductDto>> LoadProductsAsync()
        {
            var result = await _productServices.GetProductListAsync();
            if (result.Succeeded)
            {
                return result.Data.Take(5).ToList();
            }
            else
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
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
                            Summary = _localizerCommon["Error"],
                            Detail = _localizerCommon["FailedToSearchProducts", result.Messages],
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
                    Summary = _localizerCommon["Error"],
                    Detail = _localizerCommon["ErrorOccurredWhileSearchingProducts", ex.Message],
                    Duration = 4000
                });
            }
        }

        // ... existing code ...

        private void OnProductSelected(object productCode)
        {
            productCode = _autocompleteProduct.Value;
            selectedProduct = _autocompleteProducts.FirstOrDefault(x => x.ProductCode == productCode.ToString()) ?? new();
        }

        async Task InsertRow()
        {
            if (warehouseTransfer.InventTransferLines == null)
            {
                warehouseTransfer.InventTransferLines = new List<InventTransfersLineDTO>();
            }

            if (_transferLineProfileGrid == null)
            {
                // Handle the case where _transferLineProfileGrid is null
                // For example, you might want to log an error or throw an exception
                throw new InvalidOperationException("_transferLineProfileGrid is not initialized.");
            }

            var newLine = new InventTransfersLineDTO(); // Create a new line item
            warehouseTransfer.InventTransferLines.Add(newLine); // Add it to the list
            await _transferLineProfileGrid.InsertRow(newLine); // Insert the new row into the grid
        }

        async Task EditRow(InventTransfersLineDTO item)
        {
            try
            {
                if (editMode == DataGridEditMode.Single && TransferLineToInsert.Count > 0)
                {
                    Reset();
                }


                selectedProduct = new ProductDto
                {
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    UnitId = item.UnitId,
                    UnitName = item.UnitName,
                    StockAvailableQuantityTrans = (int)item.StockAvailable,
                    QuantityShipment = (int)item.StockAvailable - (int)item.AvailableQuantity,

                };
                BinItemChange(item.ToBin);
                LotNoItemChange(item.ToLotNo, item);
                await OnProductChange(selectedProduct);
                TransferLineToUpdate.Add(item);
                await _transferLineProfileGrid.EditRow(item);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 4000
                });
            }
        }
        private void OnUpdateRow(InventTransfersLineDTO item)
        {
            // Update the warehouseTransferLine with the updated item
            Reset(item);
        }

        private void OnCancelEdit(InventTransfersLineDTO item)
        {

            warehouseTransferLine.Remove(item);
            _transferLineProfileGrid.CancelEditRow(item);
        }

        private void OnCreateRow(InventTransfersLineDTO item)
        {
            warehouseTransferLine.Add(item);
            warehouseTransfer.InventTransferLines.Add(item);
            TransferLineToInsert.Remove(item);
        }
        void Reset()
        {
            TransferLineToInsert.Clear();
            TransferLineToInsert.Clear();
        }
        void Reset(InventTransfersLineDTO item)
        {
            TransferLineToInsert.Remove(item);
            TransferLineToUpdate.Remove(item);
        }

        private async Task DeleteRow(InventTransfersLineDTO item)
        {
            var confirm = await _dialogService.Confirm(_localizer["DeleteWarehouseTransferConfirmation"], _localizerCommon["DeleteConfirmation"], new ConfirmOptions() { OkButtonText = _localizerCommon["Yes"], CancelButtonText = _localizerCommon["No"] });
            if (confirm == true)
            {
                Reset(item);
                if (warehouseTransfer.InventTransferLines.Contains(item))
                {
                    InventTransferLine deleteLine = new InventTransferLine();
                    deleteLine.Id = item.Id;
                    deleteLine.ProductCode = item.ProductCode;
                    _warehouseTransferServiceLine.DeleteAsync(deleteLine);
                    warehouseTransfer.InventTransferLines.Remove(item);
                    await _transferLineProfileGrid.Reload();
                }
                else
                {
                    _transferLineProfileGrid.CancelEditRow(item);
                    await _transferLineProfileGrid.Reload();
                }
            }
        }
        private async Task SaveRow(InventTransfersLineDTO item)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedProduct.ProductCode))
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizerCommon["Require.ProductCode"],
                        Duration = 4000
                    });
                    return;
                }
                //if (String.IsNullOrEmpty(selectedProduct.ProductCode))
                //{
                //    var selectedLine = warehouseTransfer.InventTransferLines.Where(_ => _.Id == item.Id).FirstOrDefault();
                //    var product = await _productServices.GetByProductCodeAsync(item.ProductCode);
                //    if (product != null)
                //    {
                //        item.ProductCode = product.Data.ProductCode;
                //        item.ProductName = product.Data.ProductName;
                //        item.UnitId = product.Data.UnitId;
                //        item.UnitName = product.Data.UnitName;
                //        item.StockAvailable = product.Data.StockAvailableQuantityTrans;
                //        item.AvailableQuantity = product.Data.StockAvailableQuantity;
                //    }
                //}




                if (item.Id != Guid.Empty)
                {
                    // Update existing line
                    var inventTransfersLine = new InventTransferLine
                    {
                        Id = item.Id,
                        ProductCode = selectedProduct.ProductCode,
                        InventTransferId = warehouseTransfer.Id,
                        UnitId = selectedProduct.UnitId,
                        TransferNo = TransferNo,
                        ToBin = item.ToBin,
                        FromBin = item.FromBin,
                        FromLotNo = item.FromLotNo,
                        ToLotNo = item.ToLotNo,
                        Qty = item.Qty, // Ensure this is the correct quantity
                        UpdateAt = DateTime.Now
                    };
                    var response = await _warehouseTransferServiceLine.UpdateAsync(inventTransfersLine);
                    if (response != null && response.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = _localizerCommon["Success"],
                            Detail = _localizer["TransferLineUpdateSuccessfully"],
                            Duration = 4000
                        });
                        await _transferLineProfileGrid.Reload(); // Refresh grid data
                    }
                    else
                    {
                        throw new Exception("Failed to update transfer line");
                    }
                }
                else
                {
                    // Create new line
                    if (warehouseTransfer.Id == Guid.Empty)
                    {
                        var sequenceTransfer = await _numberSequenceServices.GetNumberSequenceByType("Transfer");
                        if (sequenceTransfer == null)
                            throw new Exception("Sequence for Transfer not found");

                        if (sequenceTransfer.Succeeded)
                        {
                            sequenceTransfer.Data.CurrentSequenceNo++;
                            TransferNo = $"{sequenceTransfer.Data.Prefix}{sequenceTransfer.Data.CurrentSequenceNo.ToString().PadLeft((int)sequenceTransfer.Data.SequenceLength, '0')}";
                            warehouseTransfer.TransferNo = TransferNo;
                            // Update the sequence number in the database


                            await _numberSequenceServices.UpdateAsync(sequenceTransfer.Data);
                            warehouseTransfer.Id = Guid.NewGuid();
                            var transfer = new TransferModel
                            {
                                Id = warehouseTransfer.Id,
                                TransferNo = warehouseTransfer.TransferNo,
                                Location = warehouseTransfer.Location,
                                TransferDate = warehouseTransfer.TransferDate,
                                Status = warehouseTransfer.Status,
                                PersonInCharge = warehouseTransfer.PersonInCharge,
                                TenantId = warehouseTransfer.TenantId,
                            };

                            var responseTransfer = await _warehouseTransferService.InsertAsync(transfer);
                        }
                    }
                    item.TransferNo = warehouseTransfer.TransferNo;

                    item.Id = Guid.NewGuid();
                    var inventTransfersLine = new InventTransferLine
                    {
                        Id = item.Id,
                        ProductCode = selectedProduct.ProductCode,
                        TransferNo = warehouseTransfer.TransferNo,
                        InventTransferId = warehouseTransfer.Id,
                        UnitId = selectedProduct.UnitId,
                        ToBin = item.ToBin ?? string.Empty,
                        FromBin = item.FromBin ?? string.Empty,
                        FromLotNo = item.FromLotNo ?? string.Empty,
                        ToLotNo = item.ToLotNo ?? string.Empty,
                        Qty = item.Qty,
                        UpdateAt = DateTime.Now
                    };
                    var response = await _warehouseTransferServiceLine.InsertAsync(inventTransfersLine);
                    if (response != null && response.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = _localizerCommon["Success"],
                            Detail = _localizer["TransferLineCreateSuccessfully"],
                            Duration = 4000
                        });
                        await _transferLineProfileGrid.Reload(); // Refresh grid data
                    }
                    else
                    {
                        throw new Exception("Failed to create transfer line");
                    }
                }
                item.ProductCode = selectedProduct.ProductCode;
                item.ProductName = selectedProduct.ProductName;
                item.UnitId = selectedProduct.UnitId;
                item.UnitName = selectedProduct.UnitName;
                item.StockAvailable = selectedProduct.StockAvailableQuantityTrans;
                item.AvailableQuantity = (int)(selectedProduct.StockAvailableQuantityTrans - selectedProduct.QuantityShipment);
                await _transferLineProfileGrid.UpdateRow(item);
                selectedProduct = new();
            }
            catch (Exception ex)
            {
                // Handle exception appropriately
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 4000
                });
            }
        }


        private void CancelEdit(InventTransfersLineDTO item)
        {

            warehouseTransferLine.Remove(item);
            _transferLineProfileGrid.CancelEditRow(item);
        }


        // The error message indicates that a null value is being passed to the `Enumerable.First` method.
        // To fix this issue, you need to add a null check before calling the `First` method.

        private async Task OnProductChange(ProductDto prod)
        {
            if (selectLocationDisplayDto == null)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = _localizerWarehouseReceiptResource["LocationIsRequired"],
                    Duration = 4000
                });
                return;
            }
            selectedProduct = prod;
            var productInfo = await _warehouseTranServices.GetInventoryInfoFlowBinLotFlowModelSearchAsync(new InventoryInfoBinLotSearchRequestDTO
            {
                CompanyId = warehouseTransfer.TenantId,
                ProductCodes = new List<ProductCodeSearch> { new ProductCodeSearch { ProductCode = prod.ProductCode } }
            });
            if (productInfo.Succeeded)
            {
                if (productInfo.Data != null && productInfo.Data.Any())
                {
                    InventoryInfoBinLotDetails = productInfo.Data.First().Details.Where(x => x.AvailableStock > 0 && !string.IsNullOrEmpty(x.BinCode) && !string.IsNullOrEmpty(x.LotNo) && x.WarehouseCode == selectLocationDisplayDto.LocationName).ToList();
                    bins = InventoryInfoBinLotDetails.Select(x => new Bin
                    {
                        BinCode = x.BinCode,
                    }).DistinctBy(x => x.BinCode).ToList();
                    _lotNo = InventoryInfoBinLotDetails.Select(x => new SelectListItem
                    {
                        Value = x.LotNo,
                        Text = x.BinCode,
                    }).DistinctBy(x => x.Value).ToList();
                }
            }
            StateHasChanged();
        }
        private void BinItemChange(object binCode)
        {
            _lotNo = InventoryInfoBinLotDetails.Where(x => x.BinCode == binCode.ToString()).Select(x => new SelectListItem
            {
                Value = x.LotNo,
                Text = x.BinCode,
            }).DistinctBy(x => x.Value).ToList();
        }

        private void LotNoItemChange(object lotNo, InventTransfersLineDTO item)
        {
            var lotInfo = _lotNo.FirstOrDefault(x => x.Value == lotNo.ToString());
            if (lotInfo != null)
            {
                item.ToLotNo = lotNo.ToString();
                var binInfo = InventoryInfoBinLotDetails.FirstOrDefault(x => x.BinCode == lotInfo.Text && x.LotNo == lotNo.ToString());
                selectedProduct.StockAvailableQuantityTrans = binInfo == null ? 0 : (int)binInfo.InventoryStock;
                selectedProduct.StockAvailableQuantity = binInfo == null ? 0 : (int)binInfo.AvailableStock;
                //   _transferLineProfileGrid.Reload(); // Refresh grid data

            }
        }

        private List<EnumDisplay<EnumInvenTransferStatus>> GetDisplayTransferStatus()
        {
            return Enum.GetValues(typeof(EnumInvenTransferStatus)).Cast<EnumInvenTransferStatus>().Select(_ => new EnumDisplay<EnumInvenTransferStatus>
            {
                Value = _,
                DisplayValue = GetValueTransferStatus(_)
            }).ToList();
        }

        private string GetValueTransferStatus(EnumInvenTransferStatus TransferStatus) => TransferStatus switch
        {

            EnumInvenTransferStatus.InProcess => _localizer["InProcess"],
            EnumInvenTransferStatus.Completed => _localizer["Completed"],
            _ => throw new ArgumentException("Invalid value for TransferStatus", nameof(TransferStatus))
        };
        private async Task CompleteTransferAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], "Invalid transfer ID.");
                return;
            }
            var data = await _warehouseTransferService.GetByTransferNoDTO(TransferNo);
            if (!data.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = "Result warehouse null or not found",
                    Duration = 1000
                });

                return;
            }

            warehouseTransfer = data.Data;

            if (data.Data.InventTransferLines != null)
            {
                warehouseTransferLine.Clear();
                warehouseTransferLine.AddRange(data.Data.InventTransferLines);
            }
            if (warehouseTransferLine.Count == 0)
            {
                ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], _localizerCommon["No record to execute."]);
                return;
            }
            if (warehouseTransferLine.Where(x => x.Qty == 0).ToList().Count > 0)
            {
                ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], _localizerCommon["Quantity must be greater than 0."]);
                return;
            }
            var response = await _warehouseTransferService.CompletedInventTransfer(id);
            if (response.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = _localizerCommon["Success"],
                    Detail = _localizer["Transfercompletedsuccessfully"],
                    Duration = 4000
                });
                warehouseTransfer.Status = EnumInvenTransferStatus.Completed;
                //await GetTransferAsync(); // Reload the page to refresh data

                isDisabled = true;

                StateHasChanged();
            }
            else
            {
                if (response.Messages != null && response.Messages.Count > 0)
                {
                    ShowNotification(NotificationSeverity.Error, _localizerCommon["Error"], _localizer[response.Messages[0]]);
                }
            }
        }

        async void TenantChanged(object item)
        {
            var tenant = (CompanyTenantModel)item;

            //get currnent tenant for revert when cancel
            var currentTenant = _tenants.Where(x => x.AuthPTenantId == warehouseTransfer.TenantId).FirstOrDefault();

            if (warehouseTransfer.InventTransferLines == null || warehouseTransfer.InventTransferLines.Count == 0)
            {
                warehouseTransfer.TenantId = tenant.AuthPTenantId;
                return;
            }

            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirm.Message.ChangeTenant"]}"
                , _localizerCommon["Confirm.Title.ChangeTenant"]
                , new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                });

            if (confirm == null || confirm == false)
            {
                _selectTenant = currentTenant;
                StateHasChanged();

                return;
            }

            warehouseTransfer.TenantId = tenant.AuthPTenantId;

            //delete inventTransferLine in D
            // Cấu hình AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InventTransfersLineDTO, InventTransferLine>();
            });
            var mapper = config.CreateMapper();

            var deleteModel = mapper.Map<List<InventTransferLine>>(warehouseTransfer.InventTransferLines);

            await _inventTransferLineServices.DeleteRangeAsync(deleteModel);

            warehouseTransfer.InventTransferLines = new List<InventTransfersLineDTO>();

            StateHasChanged();
        }
    }
}
