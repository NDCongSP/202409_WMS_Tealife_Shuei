﻿using Application.DTOs.Request.Products;
using Application.DTOs.Response;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using RestEase;
using System.Reflection;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ProductCategoryModel = FBT.ShareModels.Entities.ProductCategory;
using ProductModel = FBT.ShareModels.Entities.Product;
using SupplierModel = FBT.ShareModels.Entities.Supplier;
using UnitModel = FBT.ShareModels.WMS.Unit;

namespace WebUIFinal.Pages.Product
{
    public partial class DialogCardPageAddNewProduct
    {
        [Parameter] public string Title { get; set; }
        public int? ProductId { get; set; }

        private EnumProductStatus selectedStatus;
        private EnumProductType selectedProductType;
        private bool isDisabled = false;
        private bool _disable = true;
        private string? imageBase64String;
        bool _showPagerSummary = true;
        bool allowRowSelectOnRowClick = true;
        bool _visibleBtnSubmit = true;
        bool _isChooseImage = false;

        int shippingQuantity = 100;
        int orderQuantity = 100;

        InventoryInfoDTO _inventory = new InventoryInfoDTO();

        ProductAddUpdateRequestDTO _model = new ProductAddUpdateRequestDTO();

        List<SupplierModel> suppliers = new();
        List<UnitModel> units = new();
        List<CompanyTenant> _tenants = [];
        CompanyTenant _selectTenant;

        IList<ProductJanCode> _selectedJanCodes = [];

        List<ProductCategoryModel> _productCategories = [];
        ProductCategoryModel _selectCatetgory = new ProductCategoryModel();

        List<Currency> _currencyList = [];
        Currency _selectedCurrency = new Currency();

        List<CountryMaster> _countryMasterList = [];
        CountryMaster _selectedCountryMaster = new CountryMaster();

        RadzenDataGrid<ProductJanCode>? _productJanCodeProfileGrid;

        string _productType = EnumProductType.SingleItem.ToString();

        bool _disableBtnDelete = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();

                Constants.PagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

                if (Title.Contains(_localizerCommon["Detail.Create"]))
                {
                    _visibleBtnSubmit = false;
                    _model.ProductStatus = EnumProductStatus.OnSale;
                }

                selectedStatus = EnumProductStatus.All;

                await GetUnitsAsync();
                await GetSupplierAsync();
                await GetProductCategoryAsync();
                await GetCurencyAsync();
                await GetTenantsAsync();
                await GetCountryMasterAsync();

                await GetProductDetail();

                _model.StockAvailableQuanitty = 100;

                Constants.PagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
            }
            finally
            {
                StateHasChanged();
            }
        }

        private async Task GetProductCategoryAsync()
        {
            var data = await _productCategoryServices.GetAllAsync();

            if (!data.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(data.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }
            _productCategories = data.Data;
        }

        private async Task GetSupplierAsync()
        {
            var data = await _suppliersServices.GetAllAsync();
            if (!data.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(data.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }
            suppliers.AddRange(data.Data);
        }

        private async Task GetTenantsAsync()
        {
            var data = await _companiesServices.GetAllAsync();
            if (!data.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(data.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }
            _tenants.AddRange(data.Data);
        }

        private async Task GetCountryMasterAsync()
        {
            var response = await _countryMasterServices.GetAllAsync();

            if (!response.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
                   , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                   , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }

            _countryMasterList = response.Data;
        }

        private async Task GetUnitsAsync()
        {
            var response = await _unitsService.GetAllAsync();

            if (!response.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
                  , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                  , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }

            units.AddRange(response.Data.Where(_ => _.IsDeleted == false));
        }

        private async Task GetProductDetail()
        {
            #region Get product info
            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (Int32.TryParse(sub[1], out int x))
                {
                    ProductId = x;
                }
            }
            selectedProductType = EnumProductType.SingleItem;

            if (!Title.Contains(_localizerCommon["Detail.Create"]))
            {
                var product = await _productServices.GetByIdDtoAsync((int)ProductId);
                if (!product.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(product.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                  , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                  , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                //chuyen model ProductsDTO --> ProductAddUpdateRequestDTO
                SyncModel(product.Data);

                _selectedCurrency = _currencyList.Where(x => x.CurrencyCode == _model.CurrencyCode).FirstOrDefault();
                _selectedCountryMaster = _countryMasterList.FirstOrDefault(_ => _.CountryIso2 == _model.CountryOfOrigin);

                selectedStatus = _model.ProductStatus;
                selectedProductType = _model.ProductType; _productType = _model.ProductType.ToString();
                _selectCatetgory = _productCategories.Where(x => x.Id == _model.CategoryId).FirstOrDefault();
                _selectTenant = _tenants.FirstOrDefault(x => x.AuthPTenantId == _model.CompanyId);

                await GetProductJanCodesAsync();
                await GetInventory(_model.ProductCode);

                fileName = _model.ProductImageName;
                imageBase64String = _model.ImageStorage.ImageBase64;

                #region kiem tra xem product nay da duoc su dung chua
                var checkIsUseReceipt = await _warehouseReceiptOrderLineService.GetByProducCodetAsync(_model.ProductCode);
                var checkIsUseShipment = await _warehouseShipmentLineServices.GetByProducCodetAsync(_model.ProductCode);

                if (!checkIsUseReceipt.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(checkIsUseShipment.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                if (!checkIsUseShipment.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(checkIsUseShipment.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                   , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                   , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                if (checkIsUseReceipt.Data == null && checkIsUseShipment.Data == null) _disableBtnDelete = false;
                else _disableBtnDelete = true;
                #endregion

                ////Get image from Path
                //if (!string.IsNullOrEmpty(_model.ProductImageName) && _model.ProductImageName.Contains("|"))
                //{
                //    var imageInfo = _model?.ProductImageName.Split("|");
                //    fileName = _model.ProductImageName = imageInfo[0];
                //    imageBase64String = imageInfo[1];
                //}

                //imageBase64String = "data:image/png;base64,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";
            }

            #endregion
        }

        private async Task GetProductJanCodesAsync()
        {
            if (ProductId.HasValue)
            {
                var response = await _productJanCodeService.GetByProductId((int)ProductId);

                if (!response.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _model.JanCodeList = response.Data;

                if (_productJanCodeProfileGrid != null)
                    await _productJanCodeProfileGrid.RefreshDataAsync();
            }
        }

        private async Task GetInventory(string productCode)
        {
            var response = await _warehouseTranServices.GetByProductCodeInventoryInfoAsync(productCode);

            if (!response.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }

            _inventory = response.Data == null ? new InventoryInfoDTO() : response.Data;
        }

        private void SyncModel(ProductsDTO model)
        {
            _model.Id = model.Id;
            _model.ProductCode = model.ProductCode;
            _model.VendorId = model.VendorId;
            _model.SupplierId = model.SupplierId;
            _model.ProductType = model.ProductType;
            _model.ProductShortCode = model.ProductShortCode;
            _model.CompanyId = model.CompanyId;
            _model.SaleProductCode = model.SaleProductCode;
            _model.SaleProductName = model.SaleProductName;
            _model.ProductName = model.ProductName;
            _model.ProductEname = model.ProductEname;
            _model.ProductIname = model.ProductIname;
            _model.CategoryId = model.CategoryId;
            _model.IsFdaRegistration = model.IsFdaRegistration;
            _model.ProductModelNumber = model.ProductModelNumber;
            _model.ProductImageName = model.ProductImageName;
            _model.ProductImageUrl = model.ProductImageUrl;
            _model.StockAvailableQuanitty = model.StockAvailableQuanitty;
            _model.UnitId = model.UnitId;
            _model.CurrencyCode = model.CurrencyCode;
            _model.Description = model.Description;
            _model.HsCode = model.HsCode;
            _model.JanCode = model.JanCode;
            _model.Net = model.Net;
            _model.Weight = model.Weight;
            _model.Height = model.Height;
            _model.Length = model.Length;
            _model.Depth = model.Depth;
            _model.BaseCost = model.BaseCost;
            _model.BaseCostOther = model.BaseCostOther;
            _model.RegularPrice = model.RegularPrice;
            _model.Currency = model.Currency;
            _model.CountryOfOrigin = model.CountryOfOrigin;
            _model.ProductStatus = model.ProductStatus;
            _model.RegistrationDate = model.RegistrationDate;
            _model.Remark = model.Remark;
            _model.InventoryMethod = model.InventoryMethod;
            _model.ShippingLimitDays = model.ShippingLimitDays;
            _model.FromApplyPreBundles = model.FromApplyPreBundles;
            _model.ToApplyPreBundles = model.ToApplyPreBundles;
            _model.StockReceiptProcessInstruction = model.StockReceiptProcessInstruction;
            _model.StockThreshold = model.StockThreshold;
            _model.IndividuallyShippedItem = model.IndividuallyShippedItem;
            _model.DataKey = model.DataKey;
            _model.MakerManagementCode = model.MakerManagementCode;
            _model.ProductShortName = model.ProductShortName;
            _model.ProductUrl = model.ProductUrl;
            _model.StandardPrice = model.StandardPrice;
            _model.VendorProductName = model.VendorProductName;
            _model.WarehouseProcessingFlag = model.WarehouseProcessingFlag;
            _model.Width = model.Width;
            _model.ShopifyInventoryItemId = model.ShopifyInventoryItemId;
            _model.ShopifyLocationId = model.ShopifyLocationId;
            _model.ShopifyAdminGraphqlApiId = model.ShopifyAdminGraphqlApiId;
            _model.ImageStorage = model.ImageStorage;
            _model.CreateAt = model.CreateAt;
            _model.CreateOperatorId = model.CreateOperatorId;
            _model.UpdateAt = model.UpdateAt;
            _model.UpdateOperatorId = model.UpdateOperatorId;
        }

        private async Task GetCurencyAsync()
        {
            var response = await _currencyServices.GetAllAsync();

            if (!response.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
                  , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                  , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }

            _currencyList = response.Data.ToList();

            _selectedCurrency = _currencyList.FirstOrDefault(x => x.CurrencyCode.Contains("JPY"));
        }

        async void Submit(ProductAddUpdateRequestDTO arg)
        {
            var currencySelect = _currencyList.FirstOrDefault(x => x.CurrencyCode == _selectedCurrency.CurrencyCode);

            if (currencySelect == null)
            {
                NotificationHelper.ShowNotification(_notificationService
                  , NotificationSeverity.Warning
                  , _localizerNotification["Warning"], _localizerNotification["Currency do not exist."]);

                return;
            }

            _model.CurrencyCode = currencySelect.CurrencyCode;
            _model.Currency = currencySelect.Country;
            _model.CategoryId = _selectCatetgory.Id;
            _model.ProductType = selectedProductType;
            //_model.ProductStatus = selectedStatus;
            _model.CompanyId = _selectTenant.AuthPTenantId;
            _model.DataKey = _selectTenant.DataKey;
            _model.ProductShortCode = _model.SaleProductCode = _model.ProductCode;
            _model.ProductShortName = _model.SaleProductName = _model.ProductName;
            _model.CountryOfOrigin = _selectedCountryMaster?.CountryIso2;

            foreach (var item in _model.JanCodeList)
            {
                item.DataKey = _model.DataKey;
            }

            if (Title.Contains(_localizerCommon["Detail.Create"]))
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Product"]}: {arg.ProductName} {_localizerCommon["Confirmation.Create"]}?", _localizerCommon["Create"], new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                #region Save image
                if (_model.IsUpdateImage)
                {
                    _model.ProductImageName = $"{DateTime.Now.ToString("yyyyMMdd")}_{_model.ProductCode}_{fileName}";
                    _model.ImageStorage.FileName = _model.ProductImageName;
                    _model.ImageStorage.ImageBase64 = imageBase64String;
                    _model.ImageStorage.Type = EnumImageStorageType.Product;
                    _model.ImageStorage.ResourcesId = string.Empty;
                    _model.ImageStorage.Id = Guid.NewGuid();
                }
                #endregion

                var response = await _productServices.InsertDtoAsync(_model);

                if (!response.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService
                    , NotificationSeverity.Success
                    , _localizerNotification["Success"], _localizerNotification["Success"]);
            }
            else if (Title.Contains(_localizerCommon["Detail.Edit"]))
            {
                var confirm = await _dialogService.Confirm(_localizerCommon["Product"] + $": {arg.ProductName} {_localizerCommon["Confirmation.Update"]}?", _localizerCommon["Update"] + _localizerCommon["Product"], new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                #region Save image
                if (_model.IsUpdateImage)
                {
                    _model.ProductImageName = $"{DateTime.Now.ToString("yyyyMMdd")}_{_model.ProductCode}_{fileName}";
                    _model.ImageStorage.FileName = _model.ProductImageName;
                    _model.ImageStorage.ImageBase64 = imageBase64String;
                    _model.ImageStorage.Type = EnumImageStorageType.Product;
                    _model.ImageStorage.ResourcesId = _model.Id.ToString();
                }
                #endregion

                var response = await _productServices.UpdateDtoAsync(_model);

                if (!response.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService
                    , NotificationSeverity.Success
                    , _localizerNotification["Success"], _localizerNotification["Success"]);
            }
        }

        async Task DeleteItemAsync(ProductModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}?", $"{_localizerCommon["Delete"]}: {model.ProductName}", new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var response = await _productServices.DeleteDtoAsync(model.Id);

                if (!response.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService
                     , NotificationSeverity.Success
                     , _localizerNotification["Success"], _localizerNotification["Success"]);

                _navigation.NavigateTo("/productlist");
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService
                    , NotificationSeverity.Error
                    , _localizerNotification["Error"], ex.Message);
            }
        }

        void HandleRemoveFile()
        {
            if (string.IsNullOrEmpty(imageBase64String))
            {
                return;
            }

            imageBase64String = null;
            _model.ProductImageName = null;

            StateHasChanged();
        }

        private List<EnumDisplay<EnumProductType>> GetDisplayProductType()
        {
            return Enum.GetValues(typeof(EnumProductType)).Cast<EnumProductType>().Select(_ => new EnumDisplay<EnumProductType>
            {
                Value = _,
                DisplayValue = GetValueProductType(_)
            }).ToList();
        }

        private string GetValueProductType(EnumProductType productType) => productType switch
        {
            EnumProductType.SingleItem => "Single Item",
            EnumProductType.FixedSet => "Fixed Set",
            EnumProductType.MonthlyChangeSet => "Monthly Change Set",
            _ => throw new ArgumentException("Invalid value for productType", nameof(productType))
        };

        async Task AddProductJanCode()
        {
            if (_selectTenant == null)
            {
                NotificationHelper.ShowNotification(_notificationService
                    , NotificationSeverity.Warning
                    , _localizerNotification["Warning"], _localizerNotification["First, you need to select a tenant."]);

                return;
            }

            ProductJanCodeDto janInfor = new ProductJanCodeDto()
            {
                Id = 0,
                ProductId = _model.Id,
                DataKey = _selectTenant.DataKey
            };

            var res = await _dialogService.OpenAsync<DialogCardPageAddNewProductJanCode>(_localizerCommon["Product.JanCodeCreate"],
                    new Dictionary<string, object>() { { "productJanCode", janInfor }, { "VisibleBtnSubmit", false } },
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
                var selectResult = (ProductJanCode)res;

                var returnModel = _model.JanCodeList.FirstOrDefault(x => x.JanCode == selectResult.JanCode);

                if (returnModel != null)
                {
                    NotificationHelper.ShowNotification(_notificationService
                    , NotificationSeverity.Warning
                    , _localizerNotification["Warning"], _localizerNotification["Jan code has been exist."]);

                    return;
                }

                _model.JanCodeList.Add(selectResult);

                if (_productJanCodeProfileGrid != null)
                    await _productJanCodeProfileGrid.RefreshDataAsync();
            }
        }

        async Task ViewJanCodeItemAsync(ProductJanCode dto)
        {
            var res = await _dialogService.OpenAsync<DialogCardPageAddNewProductJanCode>(_localizerCommon["Product.JanCodeCreate"],
                    new Dictionary<string, object>() { { "productJanCode", dto }, { "VisibleBtnSubmit", false } },
                    new DialogOptions()
                    {
                        Width = "800",
                        Height = "400",
                        Resizable = true,
                        Draggable = true,
                        CloseDialogOnOverlayClick = true
                    });

            if (res == "Success")
            {
                await RefreshDataAsync();
            }
        }
        async Task EditJanCodeItemAsync(ProductJanCode model)
        {
            var modelEdit = new ProductJanCodeDto()
            {
                Id = model.Id,
                ProductId = model.ProductId,
                JanCode = model.JanCode,
                Description = model.Description,
                Status = model.Status,
                DataKey = model.DataKey,
                CreateAt = model.CreateAt,
                CreateOperatorId = model.CreateOperatorId,
                UpdateAt = model.UpdateAt,
                UpdateOperatorId = model.UpdateOperatorId,
            };

            var res = await _dialogService.OpenAsync<DialogCardPageAddNewProductJanCode>(_localizerCommon["Product.JanCodeEdit"],
                    new Dictionary<string, object>() { { "productJanCode", modelEdit }, { "VisibleBtnSubmit", true } },
                    new DialogOptions()
                    {
                        Width = "1000",
                        Height = "400",
                        Resizable = true,
                        Draggable = true,
                        CloseDialogOnOverlayClick = true
                    });

            if (res != null)
            {
                var selectResult = (ProductJanCodeDto)res;

                model.Id = selectResult.Id;
                model.JanCode = selectResult.JanCode;
                model.Description = selectResult.Description;
                model.Status = selectResult.Status;
                model.DataKey = _selectTenant.DataKey;

                var c = _model.JanCodeList;

                if (selectResult.IsDelete == true)
                {
                    DeleteJanCodeItemAsync(model);
                }
            }

            if (_productJanCodeProfileGrid != null)
                await _productJanCodeProfileGrid.RefreshDataAsync();
        }

        async Task DeleteJanCodeItemAsync(ProductJanCode model)
        {
            try
            {
                //var confirm = await _dialogService.Confirm(_localizerCommon["Confirmation.Delete"] + _localizerCommon["Product.JanCode"] + $": {dto.JanCode}?", _localizerCommon["Delete"] + _localizerCommon["Product.JanCode"], new ConfirmOptions()
                //{
                //    OkButtonText = _localizerCommon["Yes"],
                //    CancelButtonText = _localizerCommon["No"],
                //    AutoFocusFirstElement = true,
                //});

                //if (confirm == null || confirm == false) return;

                if (model.Id != 0)
                {
                    var res = await _productJanCodeService.DeleteAsync(model);

                    if (!res.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    NotificationHelper.ShowNotification(_notificationService
                   , NotificationSeverity.Success
                   , _localizerNotification["Success"], _localizerNotification["Success"]);

                    await RefreshDataAsync();
                }
                else
                {
                    _model.JanCodeList.Remove(model);
                }

                if (_productJanCodeProfileGrid != null)
                    await _productJanCodeProfileGrid.RefreshDataAsync();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService
                   , NotificationSeverity.Error
                   , _localizerNotification["Error"], _localizerNotification[$"{ex.Message}{Environment.NewLine}{ex.InnerException}"]);

                return;
            }
        }

        async Task RefreshDataAsync()
        {
            try
            {
                if (ProductId.HasValue)
                {
                    var res = await _productJanCodeService.GetByProductId((int)ProductId);

                    if (!res.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
               , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
               , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    _model.JanCodeList = res.Data.ToList();
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService
                  , NotificationSeverity.Error
                  , _localizerNotification["Error"], _localizerNotification[$"{ex.Message}{Environment.NewLine}{ex.InnerException}"]);
            }
        }
    }
}