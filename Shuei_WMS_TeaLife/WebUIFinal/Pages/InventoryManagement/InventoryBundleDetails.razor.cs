using Application.DTOs;
using Application.DTOs.Request.Products;
using Application.Models;
using FBT.ShareModels.WMS;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Application.Extentions.ApiRoutes;
using static QRCoder.Core.QRCodeGenerator;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Identity;
using FBT.ShareModels.Entities;

namespace WebUIFinal.Pages.InventoryManagement
{
    public partial class InventoryBundleDetails
    {
        [Parameter] public string Title { get; set; }

        private string TransNo { get; set; } = "";

        private InventBundleDTO _warehouseBundle = new();
        private List<InventBundlesLineDTO> _warehouseBundleLine = new();
        private List<UserDto> _users = new();
        private List<LocationDisplayDto> _locations = new();
        private List<BinDisplayDto> _bins = new();

        private List<BundleDisplayDto> _productBundleList = new();
        private RadzenDataGrid<InventBundlesLineDTO>? _bundleLineProfileGrid;

        private ProductDto selectedProduct = new();
        private List<TenantDto> tenants = new();
        private string tenantIdModel;



        protected override async Task OnInitializedAsync()
        {
            await GetLocationsAsync();
            await GetBinsAsync();
            await GetUsersAsync();
            await GetTenantsAsync();
           await GetBundlesAsync();

            if (Title.Contains($"{_CLoc["Detail.Create"]}"))
            {
                await GenerateTransNo();
            }
            else if (Title.Contains($"{_CLoc["Detail.Edit"]}"))
            {
                await GetBundleAsync();
            }
        }

        private async Task GenerateTransNo()
        {
            var sequenceBundle = await _numberSequenceServices.GetNumberSequenceByType("Bundle");
            if (sequenceBundle.Succeeded)
            {
                _warehouseBundle.TransNo = $"{sequenceBundle.Data.Prefix}{sequenceBundle.Data.CurrentSequenceNo.ToString().PadLeft((int)sequenceBundle.Data.SequenceLength, '0')}";
                _warehouseBundle.ExpirationDate = DateTime.Now;
                _warehouseBundle.TransDate = DateTime.Now;
                sequenceBundle.Data.CurrentSequenceNo += 1;
                _warehouseBundle.TenantId = Convert.ToInt32( tenants[0].Id);
                tenantIdModel = tenants[0].Id;
                await UpdateTenantValue();
                _warehouseBundle.Location =  _locations[0].Id;
                _warehouseBundle.Bin = "";
                _warehouseBundle.BinCode = "NA";

                _warehouseBundle.LocationName = _locations[0].LocationName;
                _warehouseBundle.PersonInCharge = GlobalVariable.UserAuthorizationInfo.UserId;

                await _numberSequenceServices.UpdateAsync(sequenceBundle.Data);
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
        private async Task UpdateTenantValue()
        {
            if (_warehouseBundle.Id != Guid.Empty)
            {
                if (!await ConfirmAction($"{_CLoc["Confirm.Message.ChangeTenant"]}", $"{_CLoc["Confirm.Title.ChangeTenant"]}"))
                {
                    // If the user selects "No", reset the selected tenant to the previous value
                    tenantIdModel = _warehouseBundle.TenantId.ToString();
                    return;
                }
                var data = await _productBundleServices.GetAllDistinctAsync();
                _warehouseBundle.ProductBundleCode = "";
                if (!string.IsNullOrWhiteSpace(tenantIdModel))
                {
                    data.Data = data.Data.Where(x => x.CompanyId == Convert.ToInt32(tenantIdModel)).ToList();
                    _warehouseBundle.TenantId = Convert.ToInt32(tenantIdModel);

                }
                if (_warehouseBundle.Id != Guid.Empty)
                {
                    await _inventBundleServices.DeleteAllBundleLineAsync(_warehouseBundle.Id);
                }
                _productBundleList.Clear();
                _productBundleList.AddRange(data.Data.Select(_ => new BundleDisplayDto { ProductBundleCode = _.ProductBundleCode.ToString(), ProductBundleName = _.ProductBundleCode }));
            }
            else
            {
                var data = await _productBundleServices.GetAllDistinctAsync();
                _warehouseBundle.ProductBundleCode = "";
                if (!string.IsNullOrWhiteSpace(tenantIdModel))
                {
                    data.Data = data.Data.Where(x => x.CompanyId == Convert.ToInt32(tenantIdModel)).ToList();
                    _warehouseBundle.TenantId = Convert.ToInt32(tenantIdModel);

                }
                if (string.IsNullOrWhiteSpace(tenantIdModel))
                {
                    _masterProductProducts = _masterProductProducts.Where(x => x.TenantId == Convert.ToInt32(tenantIdModel)).ToList();
                }
                _productBundleList.Clear();
                _productBundleList.AddRange(data.Data.Select(_ => new BundleDisplayDto { ProductBundleCode = _.ProductBundleCode.ToString(), ProductBundleName = _.ProductBundleCode }));

            }

        }

        public async Task UpdateExpiredDate(string productCode)
        {
            if (!string.IsNullOrWhiteSpace(productCode))
            {
                var productBundle = await _inventBundleLineServices.GetProductsByBundleCode(productCode);
                var lastDate = productBundle.Data.OrderBy(x => x.ExpirationDate).FirstOrDefault();
                if (lastDate != null)
                {
                    _warehouseBundle.ExpirationDate = lastDate.ExpirationDate;
                }

                if (_warehouseBundle.Id != Guid.Empty)
                {
                    //await LoadBundleLinesAsync();
                    //disabled all button for line
                }
            }
        }



        private async Task GetBundleAsync()
        {
            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];
                TransNo = sub.Length > 1 ? sub[1].Trim() : "";
            }

            if (!string.IsNullOrEmpty(TransNo))
            {
                var bundleInfo = await _inventBundleServices.GetByTransNoDTOAsync(TransNo);


                if (!bundleInfo.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Error, _localizer["Error"], "Result warehouse null or not found");
                    return;
                }

                _warehouseBundle = bundleInfo.Data;
                tenantIdModel = _warehouseBundle.TenantId.ToString();

                await GetProductCodesAsync();
                await LoadBundleLinesAsync();


                if (!string.IsNullOrEmpty(_warehouseBundle.Location))
                {
                    await UpdateListBin();
                }
            }
        }

        private async Task LoadBundleLinesAsync()
        {
            // Depened on Bundle Code, We will get the list product
            // EAch product, if exists line -> don't show this line
            // not exists, show this product

            var productBundle = await _inventBundleLineServices.GetProductsByTransNo(_warehouseBundle.TransNo);
            if (productBundle.Succeeded && productBundle.Data != null)
            {
                _warehouseBundleLine.Clear();
                foreach (var product in productBundle.Data)
                {
                    var existingLine = _warehouseBundleLine.FirstOrDefault(x => x.ProductCode == product.ProductCode && x.LotNo == product.LotNo);
                    if (existingLine != null) //update from line already updated
                    {
                        product.StockAvailable = existingLine.StockAvailable;
                        product.DemandQty = product.ProductQuantity * _warehouseBundle.Qty;
                        _warehouseBundleLine.Add(product);
                    }
                    else
                    {
                        product.DemandQty = product.ProductQuantity * _warehouseBundle.Qty;
                    }
                }
                _warehouseBundleLine.AddRange(productBundle.Data);
                _warehouseBundle.InventBundleLines = _warehouseBundleLine;
            }
            await _bundleLineProfileGrid.Reload();

        }

        private async Task GetUsersAsync()
        {
            var data = await _userToTenantServices.GetUsersAsync();
            if (data.Count > 0)
            {
                _users.AddRange(data);
            }
        }

        private async Task GetLocationsAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded)
            {
                _locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
            }
        }

        private async Task GetBinsAsync()
        {
            var data = await _binServices.GetAllAsync();
            if (data.Succeeded)
            {
                _bins.AddRange(data.Data.Select(_ => new BinDisplayDto { Id = _.Id.ToString(), BinCode = _.BinCode }));
            }
        }

        List<InventBundlesLineDTO> _masterProductProducts = new List<InventBundlesLineDTO>();
        private async Task GetProductCodesAsync()
        {
            _productCodes = new List<SelectListItem>();
            _lotNos = new List<SelectListItem>();

            var masterProductProducts = await _inventBundleServices.GetAllMasterByBundleCode(_warehouseBundle.ProductBundleCode);
           
           if (masterProductProducts.Succeeded && masterProductProducts.Data != null)
            {
                _masterProductProducts = masterProductProducts.Data;
                if (!string.IsNullOrWhiteSpace(tenantIdModel))
                {
                    _masterProductProducts = _masterProductProducts.Where(x => x.TenantId == Convert.ToInt32(tenantIdModel)).ToList();
                }
                var data = _masterProductProducts.Select(x => new { x.ProductCode, x.ProductName }).Distinct();
                var dataLotNo = _masterProductProducts.Select(x => x.LotNo).Distinct();

                if (data != null)
                {
                    _productCodes.AddRange(data.Select(_ => new SelectListItem
                    {
                        Text = _.ProductName,
                        Value = _.ProductCode
                    }));
                }
                if (dataLotNo != null)
                {
                    _lotNos.AddRange(dataLotNo.Select(_ => new SelectListItem
                    {
                        Text = _,
                        Value = _
                    }));
                }
            }
        }

        private void GetProductChanged(InventBundlesLineDTO data)
        {
            var product = _masterProductProducts.FirstOrDefault(x => x.ProductCode == data.ProductCode && x.Location == data.Location
             && x.LotNo == data.LotNo && x.Bin == data.Bin);
            if (product != null)
            {
                data.ProductQuantity = product.ProductQuantity;
                data.ProductName = product.ProductName;
                selectedProduct.ProductName = product.ProductName;
                data.StockAvailable = product.StockAvailable;
                data.DemandQty = product.ProductQuantity * _warehouseBundle.Qty;
                data.ExpirationDate = product.ExpirationDate;

            }
            else
            {
                var data1 = _masterProductProducts.FirstOrDefault(x => x.ProductCode == data.ProductCode);
                data.ProductName = data1.ProductName;
                selectedProduct.ProductName = data1.ProductName;
                data.ProductQuantity = data1.ProductQuantity;
                data.ExpirationDate = null;
                data.StockAvailable = null;
            }

            var dataLocations = _masterProductProducts.Where(x => x.ProductCode == data.ProductCode).Select(x => new { x.Location, x.LocationName }).Distinct();
            if (dataLocations != null)
            {
                _lineLocations =
                [
                    .. dataLocations.Select(_ => new SelectListItem
                    {
                        Text = _.LocationName,
                        Value = _.Location
                    }),
                ];
            }

            var dataBins = _masterProductProducts.Where(x => x.ProductCode == data.ProductCode && x.Location == data.Location).Select(x => x.Bin).Distinct();
            if (dataBins != null)
            {
                _lineBins =
                [
                    .. dataBins.Select(_ => new SelectListItem
                    {
                        Text = _,
                        Value = _
                    }),
                ];
            }
            var dataLotNo = _masterProductProducts.Where(x => x.ProductCode == data.ProductCode && x.Location == data.Location && x.Bin == data.Bin).Select(x => x.LotNo).Distinct();
            if (dataLotNo != null)
            {
                _lotNos =
                [
                    .. dataLotNo.Select(_ => new SelectListItem
                    {
                        Text = _,
                        Value = _
                    }),
                ];
            }
        }

        List<SelectListItem> _lotNos;
        List<SelectListItem> _productCodes;
        List<SelectListItem> _lineLocations;
        List<SelectListItem> _lineBins;


        private async Task GetBundlesAsync()
        {
            var data = await _productBundleServices.GetAllDistinctAsync();
            if (data.Succeeded)
            {
                _productBundleList.AddRange(data.Data.Select(_ => new BundleDisplayDto { ProductBundleCode = _.ProductBundleCode.ToString(), ProductBundleName = _.ProductBundleCode }));
            }
        }

        private async Task UpdateListBin()
        {
            var data = await _binServices.GetAllAsync();
            _bins.Clear();
            if (data.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(_warehouseBundle.Location))
                {
                    data.Data = data.Data.Where(x => x.LocationId.ToString() == _warehouseBundle.Location).ToList();
                }
                _bins.AddRange(data.Data.Select(_ => new BinDisplayDto { Id = _.Id.ToString(), BinCode = _.BinCode }));
            }
        }

        private async Task Submit(InventBundleDTO arg)
        {
            if (Title.Contains($"{_CLoc["Detail.Create"]}"))
            {
                await CreateBundle(arg);
            }
            else if (Title.Contains($"{_CLoc["Detail.Edit"]}"))
            {
                await UpdateBundle(arg);
            }

            StateHasChanged();
        }

        private async Task CreateBundle(InventBundleDTO arg)
        {
            if (!await ConfirmAction($"{_localizer["DoYouWantToCreateANewInventoryPreset"]}", $"{_localizer["CreateInventoryPreset"]}"))
            {
                return;
            }
            
            var bundle = new FBT.ShareModels.WMS.InventBundle
            {
                Id = Guid.NewGuid(),
                TransNo = arg.TransNo,
                Location = arg.Location,
                Bin = arg.Bin,
                ProductBundleCode = arg.ProductBundleCode,
                TransDate = arg.TransDate,
                PersonInCharge = arg.PersonInCharge,
                LotNo = arg.LotNo,
                ExpirationDate = arg.ExpirationDate,
                Qty = arg.Qty,
                TenantId = Convert.ToInt32(tenantIdModel)
            };
            if (string.IsNullOrWhiteSpace(bundle.Bin))
            {
                bundle.Bin = "N/A";
            }
            if (string.IsNullOrWhiteSpace(bundle.LotNo))
            {
                bundle.LotNo = "N/A";
            }
            var response = await _inventBundleServices.InsertAsync(bundle);

            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyCreatedInventoryPreset"]}");
                _navigation.NavigateTo($"/bundle-details/{_CLoc["Detail.Edit"]}|{bundle.TransNo}");
                await GetBundleAsync();

            }
            else
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["SuccessfullyCreatedInventoryPreset"]}");
            }
        }

        private async Task UpdateBundle(InventBundleDTO arg)
        {
            //Load Button
            if (!await ConfirmAction($"{_CLoc["Confirmation.Update"]} ?", $"{_localizer["UpdateInventoryPreset"]}"))
            {
                return;
            }

            try
            {
                var checkPayload = await _inventBundleServices.GetByIdAsync(arg.Id);

                if (checkPayload != null)
                {
                    checkPayload.Data.TransNo = arg.TransNo;
                    checkPayload.Data.Location = arg.Location;
                    checkPayload.Data.TransDate = arg.TransDate;
                    checkPayload.Data.ProductBundleCode = arg.ProductBundleCode;
                    //checkPayload.Data.Status = arg.Status;
                    checkPayload.Data.Bin = arg.Bin;
                    checkPayload.Data.Qty = arg.Qty;
                    checkPayload.Data.LotNo = arg.LotNo;
                    checkPayload.Data.ExpirationDate = arg.ExpirationDate;
                    checkPayload.Data.PersonInCharge = arg.PersonInCharge;

                    checkPayload.Data.TenantId = Convert.ToInt32(tenantIdModel);


                    if (string.IsNullOrWhiteSpace(checkPayload.Data.Bin))
                    {
                        checkPayload.Data.Bin = "N/A";
                    }
                    if (string.IsNullOrWhiteSpace(checkPayload.Data.LotNo))
                    {
                        checkPayload.Data.LotNo = "N/A";
                    }
                    var response = await _inventBundleServices.UpdateAsync(checkPayload.Data);
                    if (response.Succeeded)
                    {
                        ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyEditedInventoryPreset"]}");
                    }
                    else
                    {
                        ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToEditBundle"]}");
                    }

                    await GetBundleAsync();
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

        private async Task DeleteItemAsync(InventBundleDTO model)
        {
            if (!await ConfirmAction(_CLoc["Confirmation.Delete"], _CLoc["Delete"]))
            {
                return;
            }

            try
            {
                var res = await _inventBundleServices.DeleteAsync(new FBT.ShareModels.WMS.InventBundle
                {
                    Id = model.Id,
                    TransNo = model.TransNo,
                    Location = model.Location,
                    TransDate = model.TransDate,
                    PersonInCharge = model.PersonInCharge
                });

                if (res.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], "");
                    _navigation.NavigateTo($"/inventory-bundle-list");
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

        private string GetLocalizedStatus(EnumStatusBundle status)
        {
            return _localizer[status.ToString()].ToString();
 
        }
        

        private async Task CompleteBundleAsync(InventBundleDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.TransNo))
            {
                ShowNotification(NotificationSeverity.Error, "Error", "Invalid bundle ID.");
                return;
            }

            var response = await _inventBundleServices.CompletedInventBundleAsync(model.Id);
            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, _CLoc["Success"], _localizer["Bundle completed successfully"]);
                await GetBundleAsync();
            }
            else
            {
                ShowNotification(NotificationSeverity.Error, "Error", "Failed to complete bundle.");
            }
        }

        private async Task UpdateBundleList()
        {
            //change qty when change bundle
        }

        //private void OnUpdateRow(InventAdjustmentsLineDTO item)
        //{
        //    // Update the warehouseAdjustmentLine with the updated item
        //    Reset(item);
        //}
        //private void OnUpdateRow()
        //{
        //    // Update the warehouseAdjustmentLine with the updated item
        //}
        //void Reset()
        //{
        //}

        //void Reset(InventAdjustmentsLineDTO item)
        //{
        //}

        async Task InsertRow()
        {
            if (_warehouseBundle.InventBundleLines == null)
            {
                _warehouseBundle.InventBundleLines = new List<InventBundlesLineDTO>();
            }

            if (_bundleLineProfileGrid == null)
            {
                // Handle the case where _adjustmentLineProfileGrid is null
                // For example, you might want to log an error or throw an exception
                throw new InvalidOperationException("_bundleLineProfileGrid is not initialized.");
            }

            var newLine = new InventBundlesLineDTO(); // Create a new line item
            _warehouseBundle.InventBundleLines.Insert(0, newLine); // Add it to the list
            await _bundleLineProfileGrid.InsertRow(newLine); // Insert the new row into the grid
        }

        async Task EditRow(InventBundlesLineDTO item)
        {
            if (editMode == DataGridEditMode.Single && BundleLinesToInsert.Count > 0)
            {
                Reset();
            }
            GetProductChanged(item);
            BundleLinesToUpdate.Add(item);
            await _bundleLineProfileGrid.EditRow(item);
        }

        async Task DeleteRow(InventBundlesLineDTO item)
        {
            var confirm = await _dialogService.Confirm(_localizer["DeleteWarehouseAdjustmentConfirmation"], _CLoc["Confirmation.Delete"], new ConfirmOptions() { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"] });
            if (confirm == true)
            {
                Reset(item);

                InventBundleLine deleteLine = new InventBundleLine()
                {
                    Id = item.Id,
                    ProductCode = item.ProductCode,
                };
               await  _inventBundleLineServices.DeleteAsync(deleteLine);
                _warehouseBundle.InventBundleLines.Remove(item);
                await _bundleLineProfileGrid.Reload();
            }
        }

        async Task PutawayAsync(InventBundleDTO item)
        {
            try
            {
                var confirm = await _dialogService.Confirm(_localizer["ProcessPutaway.Confirmation"], _localizer["Confirmation.Putaway"], new ConfirmOptions() { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"] });
                if (confirm == true)
                {
                    //If not match number, not allow to push
                    List<InventBundlesLineDTO> validLine = new List<InventBundlesLineDTO>();
                    foreach (var itemLine in item.InventBundleLines)
                    {
                        if (!validLine.Any(x => x.ProductCode == itemLine.ProductCode))
                        {
                            validLine.Add(itemLine);
                        }
                        else
                        {
                            foreach (var indexValidLine in validLine)
                            {
                                if (indexValidLine.ProductCode == itemLine.ProductCode)
                                {
                                    indexValidLine.ActualQty += itemLine.ActualQty;

                                }
                            }
                        }
                    }
                    foreach (var indexValidLine in validLine)
                    {
                        if (indexValidLine.ActualQty != indexValidLine.DemandQty)
                        {
                            throw new Exception(_localizer["Product {Product} not meet the demand quality"].Value.Replace("{Product}", indexValidLine.ProductCode));
                        }
                    }
                    var response =  await _inventBundleServices.CreatePutawayAsync(item.Id);

                    await _bundleLineProfileGrid.Reload();
                    await GetBundleAsync();

                    if (response.Succeeded)
                    {
                        ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyCreatedPutaway"]}");
                    }
                    else
                    {
                        ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToEditBundle"]}");
                    }

                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = ex.Message,
                    Duration = 4000
                });
            }
        }

        void Reset()
        {
            BundleLinesToInsert.Clear();
            BundleLinesToUpdate.Clear();
        }

        void Reset(InventBundlesLineDTO item)
        {
            BundleLinesToInsert.Remove(item);
            BundleLinesToUpdate.Remove(item);
        }

        private async Task SaveRow(InventBundlesLineDTO item)
        {
            try
            {
                if (item.ActualQty > item.DemandQty)
                {
                    throw new Exception($"{_localizer["Actual Qty Cannot Greater Than Demand Qty"]}");

                }
                if (item.ActualQty > item.StockAvailable)
                {
                    throw new Exception($"{_localizer["Actual Qty Cannot Greater Than Stock Available"]}");
                }

                if (item.Id != Guid.Empty)
                {
                    // Update existing line
                    var inventBundleLine = new InventBundleLine
                    {
                        Id = item.Id,
                        ProductCode = item.ProductCode,
                        UnitId = item.UnitId,
                        Location = item.Location,
                        TransNo = _warehouseBundle.TransNo,
                        LotNo = item.LotNo,
                        ActualQty = item.ActualQty,
                        DemandQty = item.DemandQty,
                        UpdateAt = DateTime.Now,
                        Bin = item.Bin,
                        ExpirationDate = item.ExpirationDate,
                        UpdateOperatorId = _warehouseBundle.CreateOperatorId
                    };
                    var response = await _inventBundleLineServices.UpdateAsync(inventBundleLine);
                    if (response != null && response.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = _CLoc["Success"],
                            Detail = _localizer["BundleLineUpdateSuccessfully"],
                            Duration = 4000
                        });
                        await _bundleLineProfileGrid.Reload(); // Refresh grid data
                        await GetBundleAsync();
                    }
                    else
                    {
                        throw new Exception("Failed to update Bundle line");
                    }
                }
                else
                {
                    var inventBundleLine = new InventBundleLine
                    {
                        Id = Guid.NewGuid(),
                        ProductCode = item.ProductCode,
                        UnitId = item.UnitId,
                        TransNo = _warehouseBundle.TransNo,
                        Location = item.Location,
                        LotNo = item.LotNo,
                        ActualQty = item.ActualQty,
                        DemandQty = item.DemandQty,
                        Bin = item.Bin,
                        ExpirationDate = item.ExpirationDate,
                        CreateAt = DateTime.Now,
                        CreateOperatorId = _warehouseBundle.CreateOperatorId
                    };
                    var response = await _inventBundleLineServices.InsertAsync(inventBundleLine);
                    if (response != null && response.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = _CLoc["Success"],
                            Detail = _localizer["PresetLineCreateSuccessfully"],
                            Duration = 4000
                        });
                        await _bundleLineProfileGrid.Reload(); // Refresh grid data
                        await GetBundleAsync();
                    }
                    else
                    {
                        throw new Exception("Failed to create Bundle line");
                    }
                    foreach(var item1 in _warehouseBundleLine)
                    {
                        if(item1.ProductCode == inventBundleLine.ProductCode && item1.Bin == inventBundleLine.Bin && inventBundleLine.LotNo == item1.LotNo)
                        {
                            item1.Id = inventBundleLine.Id;
                        }
                    }
                    _warehouseBundle.InventBundleLines = _warehouseBundleLine;
                }
                await _bundleLineProfileGrid.UpdateRow(item);
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


        private void CancelEdit(InventBundlesLineDTO item)
        {

            _warehouseBundleLine.Remove(item);
            _bundleLineProfileGrid.CancelEditRow(item);
        }

        DataGridEditMode editMode = DataGridEditMode.Single;
        List<InventBundlesLineDTO> BundleLinesToInsert = new List<InventBundlesLineDTO>();
        List<InventBundlesLineDTO> BundleLinesToUpdate = new List<InventBundlesLineDTO>();

        private async Task PrintReportPicking()
        {
            await _localStorage.SetItemAsync("WarehouseBundle", _warehouseBundle);
            await JSRuntime.InvokeVoidAsync("openTab", "/print-report-picking");
        }
    }
}