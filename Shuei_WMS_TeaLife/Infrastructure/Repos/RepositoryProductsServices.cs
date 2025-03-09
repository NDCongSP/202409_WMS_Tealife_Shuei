using Application.DTOs;
using Application.DTOs.Request.Products;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Services;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using RestEase;
using System.Runtime.InteropServices;
using Product = FBT.ShareModels.Entities.Product;

namespace Infrastructure.Repos
{
    public class RepositoryProductsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IProducts
    {
        public async Task<Result<List<Product>>> AddRangeAsync([Body] List<Product> model)
        {
            var err = new ErrorResponse();
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;
                }

                await dbContext.Products.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<Product>>.SuccessAsync(model, "Add range Products successfull");
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Product>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<List<Product>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Product>> DeleteRangeAsync([Body] List<Product> model)
        {
            var err = new ErrorResponse();
            try
            {
                dbContext.Products.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Product>.SuccessAsync("Delete range Products successfull");
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Product>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Product>> DeleteAsync([Body] Product model)
        {
            var err = new ErrorResponse();
            try
            {
                dbContext.Products.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Product>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Product>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<Product>>> GetAllAsync()
        {
            var err = new ErrorResponse();
            try
            {
                return await Result<List<Product>>.SuccessAsync(await dbContext.Products.ToListAsync());
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Product>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Product>> GetByIdAsync([Path] int id)
        {
            var err = new ErrorResponse();
            try
            {
                Product result = await dbContext.Products.FindAsync(id);
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.ProductImageName))
                    {
                        //var fileName = @"C:\Images\Products\" + result.ProductImageName;
                        //if (File.Exists(fileName))
                        //{
                        //    var imageArray = File.ReadAllBytes(fileName);
                        //    var base64Image = Convert.ToBase64String(imageArray);

                        //    //dồn chung ImageName và string base64 của ảnh trả về cho client cắt ra xử
                        //    var typeImage = result.ProductImageName.Split('.')[1];
                        //    if (typeImage == "png")
                        //    {
                        //        result.ProductImageName = $"{result.ProductImageName}|data:image/png;base64,{base64Image}";
                        //    }
                        //    else if (typeImage == "jpeg" || typeImage == "jpg")
                        //    {
                        //        result.ProductImageName = $"{result.ProductImageName}|data:image/jpeg;base64,{base64Image}";
                        //    }
                        //    else if (typeImage == "svg")
                        //    {
                        //        result.ProductImageName = $"{result.ProductImageName}|data:image/svg+xml;base64,{base64Image}";
                        //    }
                        //}
                        //else result.ProductImageName = string.Empty;
                        var res = LoadImage(result.ProductImageName);

                        if (!res.Flag) throw new Exception(res.Message);

                        result.ProductImageName = res.Message;
                    }
                    return await Result<Product>.SuccessAsync(result);
                }
                else
                {
                    err.Errors.Add("Warning", "Could not found the product.");
                    return await Result<Product>.FailAsync(JsonConvert.SerializeObject(err));
                }

            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Product>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Product>> InsertAsync([Body] Product model)
        {
            var err = new ErrorResponse();
            try
            {
                var existCD = await dbContext.Products.Where(x => x.ProductCode == model.ProductCode).FirstOrDefaultAsync();
                if (existCD != null)
                {
                    return await Result<Product>.FailAsync($"Product code: {model.ProductCode} is already created");
                }

                // lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateOperatorId = userInfo.Id;
                model.CreateAt = DateTime.Now;

                await dbContext.Products.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Product>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Product>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Product>> UpdateAsync([Body] Product model)
        {
            var err = new ErrorResponse();
            try
            {
                // lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateOperatorId = userInfo.Id;
                model.UpdateAt = DateTime.Now;

                var dataUpdate = dbContext.Products.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Product>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Product>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<string>> UploadProductImage([Path] ImageInfoDTO model)
        {
            //var folderPath = @"C:\Images\Products";
            //if (!Directory.Exists(folderPath))
            //{
            //    Directory.CreateDirectory(folderPath);
            //}
            ////var fileName = @"C:\Images\Products\" + model.ProductCode + ".jpeg";
            //var fileName = @"C:\Images\Products\" + model.FileNameImage;
            //if (File.Exists(fileName))
            //{
            //    File.Delete(fileName);
            //}
            //if (model?.ProductImage?.Length > 0)
            //{
            //    var s = model.ProductImage.Split(',')[1];
            //    //File.WriteAllBytes(Path.Combine(folderPath, model.ProductCode + ".jpeg"), Convert.FromBase64String(s));
            //    File.WriteAllBytes(Path.Combine(folderPath, model.FileNameImage), Convert.FromBase64String(s));
            //}

            var res = UploadImage(model);
            return await Result<string>.SuccessAsync(res.Message, "");
        }

        public async Task<Result<IEnumerable<ProductDto>>> GetProductListAsync()
        {
            var err = new ErrorResponse();
            try
            {
                //var result = await dbContext.Products
                //.Where(product => product.IsDeleted != true)
                //.Join(dbContext.ProductCategories, x => x.CategoryId, y => y.Id, (x, y) => new { x, CategoryName = y.CategoryName })
                //.Join(dbContext.Units, xy => xy.x.UnitId, z => z.Id, (xy, z) => new { xy, UnitName = z.UnitName, UnitId = z.Id })
                //.Join(dbContext.Suppliers, xyz => xyz.xy.x.SupplierId, s => s.Id, (xyz, s) => new { xyz, SupplierName = s.SupplierName })
                //.Select(product => new ProductDto
                //{
                //    Id = product.xyz.xy.x.Id,
                //    ProductCode = product.xyz.xy.x.ProductCode,
                //    ProductName = product.xyz.xy.x.ProductName,
                //    ProductStatus = product.xyz.xy.x.ProductStatus,
                //    CategoryName = product.xyz.xy.CategoryName,
                //    UnitName = product.xyz.UnitName,
                //    UnitId = product.xyz.UnitId,
                //    SupplierName = product.SupplierName,
                //    ProductStatusString = ((EnumProductStatus)product.xyz.xy.x.ProductStatus).ToString(),
                //})
                //.ToListAsync();

                var result = (from product in dbContext.Products.Where(product => product.IsDeleted == false)
                              join cate in dbContext.ProductCategories on product.CategoryId equals cate.Id into productCategories
                              from cate in productCategories
                              join unit in dbContext.Units on product.UnitId equals unit.Id into productUnits
                              from unit in productUnits
                              join supplier in dbContext.Suppliers on product.SupplierId equals supplier.Id into productSuppliers
                              from supplier in productSuppliers
                              join janCode in dbContext.ProductJanCodes on product.Id equals janCode.ProductId into productJanCodes
                              select new ProductDto
                              (
                                  product,
                                  supplier.SupplierName,
                                  unit.UnitName,
                                  cate.CategoryName,
                                  productJanCodes.ToList(),
                                  (int)dbContext.WarehouseShipmentLines.Where(s => s.ProductCode == product.ProductCode && s.Status == EnumShipmentOrderStatus.Open).Sum(s => s.ShipmentQty)
                              )).AsEnumerable();

                return await Result<IEnumerable<ProductDto>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<IEnumerable<ProductDto>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<IEnumerable<ProductDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductDto>> GetByProductCodeAsync(string code)
        {
            var err = new ErrorResponse();
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    err.Errors.Add("Warning", $"Product code is required.");
                    return await Result<ProductDto>.FailAsync(JsonConvert.SerializeObject(err));
                }

                // 1. Fetch the product 
                var product = await dbContext.Products
                    .FirstOrDefaultAsync(p => p.IsDeleted == false && p.ProductCode == code);

                if (product == null)
                    return null;

                // 2. Fetch related data (including ProductJanCodes)
                var productJanCodes = await dbContext.ProductJanCodes
                    .Where(j => j.ProductId == product.Id)
                    .ToListAsync();

                var supplier = await dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == product.SupplierId);
                var unit = await dbContext.Units.FirstOrDefaultAsync(u => u.Id == product.UnitId);
                var category = await dbContext.ProductCategories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);

                // 3. Calculate StockAvailableQuantityTrans (from wh trans)
                var stockAvailableQuantityTrans = await dbContext.WarehouseTrans
                    .Where(wt => wt.ProductCode == code)
                    .SumAsync(wt => wt.Qty);

                // 4. Create the ProductDto
                var result = new ProductDto
                (
                    product,
                    supplier != null ? supplier.SupplierName : null,
                    unit != null ? unit.UnitName : null,
                    category != null ? category.CategoryName : null,
                    productJanCodes,
                    (int)dbContext.WarehouseShipmentLines
                        .Where(s => s.ProductCode == product.ProductCode && s.Status == EnumShipmentOrderStatus.Open)
                        .Sum(s => s.ShipmentQty)
                );

                result.StockAvailableQuantityTrans = (int)stockAvailableQuantityTrans;


                if (result != null)
                {
                    return await Result<ProductDto>.SuccessAsync(result);
                }
                else
                {
                    err.Errors.Add("Warning", $"Could be not found data.");
                    return await Result<ProductDto>.FailAsync(JsonConvert.SerializeObject(err));
                }
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductDto>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<ProductDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<IEnumerable<ProductDto>>> SearchByProductCodeAsync(string? code)
        {
            var err = new ErrorResponse();
            try
            {
                var result = (from product in dbContext.Products.Where(product => product.IsDeleted == false && (string.IsNullOrEmpty(code) || product.ProductCode.ToLower().Contains(code.ToLower())))
                              join cate in dbContext.ProductCategories on product.CategoryId equals cate.Id into productCategories
                              from cate in productCategories
                              join unit in dbContext.Units on product.UnitId equals unit.Id into productUnits
                              from unit in productUnits
                              join supplier in dbContext.Suppliers on product.SupplierId equals supplier.Id into productSuppliers
                              from supplier in productSuppliers
                              join janCode in dbContext.ProductJanCodes on product.Id equals janCode.ProductId into productJanCodes
                              select new ProductDto
                              (
                                  product,
                                  supplier.SupplierName,
                                  unit.UnitName,
                                  cate.CategoryName,
                                  productJanCodes.ToList(),
                                  (int)dbContext.WarehouseShipmentLines.Where(s => s.ProductCode == product.ProductCode && s.Status == EnumShipmentOrderStatus.Open).Sum(s => s.ShipmentQty)
                              )).AsEnumerable();

                if (result != null)
                {
                    return await Result<IEnumerable<ProductDto>>.SuccessAsync(result);
                }
                else
                {
                    err.Errors.Add("Warning", $"Could be not found data.");
                    return await Result<IEnumerable<ProductDto>>.FailAsync(JsonConvert.SerializeObject(err));
                }
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<IEnumerable<ProductDto>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }


        public async Task<Result<List<ProductsDTO>>> GetAllDtoAsync([Body] ProductSearchRequestDTO model)
        {
            var err = new ErrorResponse();
            try
            {
                var res = await dbContext.Database.SqlQueryRaw<ProductsDTO>("sp_productGetDataMaster @ProductCode = {0}, @CategoryId = {1}, @ProductStatus = {2}, @TenantId = {3}"
                                                                                , model.ProductCode, model.CategoryId, ((int?)model.ProductStatus), model.TenantId).ToListAsync();

                foreach (var item in res)
                {
                    item.ProductJanCode = await dbContext.ProductJanCodes.Where(x => x.ProductId == item.Id).ToListAsync();
                }

                if (res == null || res.Count == 0)
                {
                    err.Errors.Add("Warning", "Could be not found data.");
                    return await Result<List<ProductsDTO>>.FailAsync(JsonConvert.SerializeObject(err));
                }

                return await Result<List<ProductsDTO>>.SuccessAsync(res);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ProductsDTO>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductsDTO>> GetByIdDtoAsync([Path] int id)
        {
            var err = new ErrorResponse();
            try
            {
                var res = await dbContext.Database.SqlQueryRaw<ProductsDTO>("sp_productGetDataMaster @id = {0}", id)
                            .ToListAsync();

                if (res.FirstOrDefault() == null) throw new Exception($"Product ID: {id} could not found.");

                var response = res.FirstOrDefault();

                //response.ProductImageName = LoadImage(response.ProductImageName).Message;
                var image = await dbContext.ImageStorages.Where(x => x.ResourcesId == id.ToString() && x.Type == EnumImageStorageType.Product).FirstOrDefaultAsync();

                if (image != null)
                {
                    response.ImageStorage = image;
                }

                return await Result<ProductsDTO>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductsDTO>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductAddUpdateRequestDTO>> InsertDtoAsync([Body] ProductAddUpdateRequestDTO model)
        {
            var err = new ErrorResponse();
            try
            {
                var existCD = await dbContext.Products.Where(x => x.ProductCode == model.ProductCode).FirstOrDefaultAsync();
                if (existCD != null)
                {
                    err.Errors.Add("Warning", $"Sorry, product code is already created.");

                    return await Result<ProductAddUpdateRequestDTO>.FailAsync(JsonConvert.SerializeObject(err));

                    //return await Result<ProductAddUpdateRequestDTO>.FailAsync($"Product code: {model.ProductCode} is already created");
                }

                using (var tran = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        //var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");
                        var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                        var modelInsert = SyncModel(model);
                        modelInsert.CreateOperatorId = userInfo.Id;
                        modelInsert.CreateAt = DateTime.Now;

                        //save product
                        await dbContext.Products.AddAsync(modelInsert);
                        await dbContext.SaveChangesAsync();

                        //dave product jan code
                        foreach (var item in model.JanCodeList)
                        {
                            item.ProductId = modelInsert.Id;
                        }
                        await dbContext.ProductJanCodes.AddRangeAsync(model.JanCodeList);

                        //save image
                        if (model.IsUpdateImage)
                        {
                            //save folder
                            //var res =await ImageHelpers.UploadImageAsync(model.ImageInfo);
                            //if (!res.Succeeded) throw new Exception($"Upload image fail: {res.Messages.FirstOrDefault()}");

                            //var checkImage = await dbContext.ImageStorages.Where(x => x.Id == model.ImageStorage.Id).FirstOrDefaultAsync();
                            //if (checkImage == null)
                            //{
                            //    model.ImageStorage.ResourcesId = modelInsert.Id.ToString();
                            //    model.ImageStorage.CreateAt = DateTime.Now;
                            //    model.ImageStorage.CreateOperatorId = userInfo.Id;

                            //    await dbContext.ImageStorages.AddAsync(model.ImageStorage);
                            //}
                            //else
                            //{
                            //    checkImage.UpdateAt = DateTime.Now;
                            //    checkImage.UpdateOperatorId = userInfo.Id;
                            //    checkImage.FileName = model.ProductImageName;
                            //    checkImage.ImageBase64 = model.ImageStorage.ImageBase64;
                            //}

                            if (!string.IsNullOrEmpty(model.ImageStorage.ImageBase64))
                            {
                                model.ImageStorage.ResourcesId = modelInsert.Id.ToString();
                                model.ImageStorage.CreateAt = DateTime.Now;
                                model.ImageStorage.CreateOperatorId = userInfo.Id;

                                model.ImageStorage.Id = Guid.NewGuid();
                                await dbContext.ImageStorages.AddAsync(model.ImageStorage);
                            }
                        }

                        await dbContext.SaveChangesAsync();

                        await tran.CommitAsync();

                        model.Id = modelInsert.Id;
                        return await Result<ProductAddUpdateRequestDTO>.SuccessAsync(model);
                    }
                    catch (Exception ex)
                    {
                        await tran.RollbackAsync();

                        throw new Exception($"{ex.Message} | {ex.InnerException}");
                    }
                }
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductAddUpdateRequestDTO>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<ProductAddUpdateRequestDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ProductAddUpdateRequestDTO>> UpdateDtoAsync([Body] ProductAddUpdateRequestDTO model)
        {
            var err = new ErrorResponse();
            try
            {
                using (var tran = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);
                        var modelInsert = SyncModel(model);

                        modelInsert.UpdateOperatorId = userInfo.Id;
                        modelInsert.UpdateAt = DateTime.Now;

                        //save product
                        dbContext.Products.Update(modelInsert);
                        //save product jan code
                        foreach (var item in model.JanCodeList)
                        {

                            if (item.Id != 0)
                            {
                                item.UpdateAt = DateTime.Now;
                                item.UpdateOperatorId = userInfo.Id;

                                dbContext.ProductJanCodes.Update(item);
                            }
                            else
                            {
                                item.CreateAt = DateTime.Now;
                                item.CreateOperatorId = userInfo?.Id;

                                await dbContext.ProductJanCodes.AddAsync(item);
                            }
                        }

                        //save image
                        await dbContext.ImageStorages.Where(_ => _.ResourcesId == model.Id.ToString()).ExecuteDeleteAsync();
                        if (model.IsUpdateImage)
                        {
                            //var checkImage = await dbContext.ImageStorages.Where(x => x.Id == model.ImageStorage.Id).FirstOrDefaultAsync();

                            //if (checkImage == null)
                            //{
                            //    model.ImageStorage.ResourcesId = modelInsert.Id.ToString();
                            //    model.ImageStorage.CreateAt = DateTime.Now;
                            //    model.ImageStorage.CreateOperatorId = userInfo.Id;

                            //    await dbContext.ImageStorages.AddAsync(model.ImageStorage);
                            //}
                            //else
                            //{
                            //    checkImage.UpdateAt = DateTime.Now;
                            //    checkImage.UpdateOperatorId = userInfo.Id;
                            //    checkImage.FileName = model.ProductImageName;
                            //    checkImage.ImageBase64 = model.ImageStorage.ImageBase64;
                            //}

                            if (!string.IsNullOrEmpty(model.ImageStorage.ImageBase64))
                            {
                                model.ImageStorage.ResourcesId = modelInsert.Id.ToString();
                                model.ImageStorage.CreateAt = DateTime.Now;
                                model.ImageStorage.CreateOperatorId = userInfo.Id;

                                model.ImageStorage.Id = Guid.NewGuid();
                                await dbContext.ImageStorages.AddAsync(model.ImageStorage);
                            }
                        }

                        await dbContext.SaveChangesAsync();

                        await tran.CommitAsync();

                        return await Result<ProductAddUpdateRequestDTO>.SuccessAsync(model);
                    }
                    catch (Exception ex)
                    {
                        await tran.RollbackAsync();

                        throw new Exception($"{ex.Message} | {ex.InnerException}");
                    }
                }
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductAddUpdateRequestDTO>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        private GeneralResponse UploadImage(ImageInfoDTO model)
        {
            GeneralResponse response = new GeneralResponse();
            try
            {
                var folderPath = @"C:\Images\Products";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                //var fileName = @"C:\Images\Products\" + model.ProductCode + ".jpeg";
                var fileName = @"C:\Images\Products\" + model.FileName;
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                if (model.ImageBase64.Length > 0)
                {
                    var s = model.ImageBase64.Split(',')[1];
                    File.WriteAllBytes(Path.Combine(folderPath, model.FileName), Convert.FromBase64String(s));
                }

                response.Flag = true;
                response.Message = "Save image successfully.";
                //return await Result<String>.SuccessAsync(model.ProductCode + ".jpeg", "");
                return response;
            }
            catch (Exception ex)
            {
                response.Flag = false;
                response.Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}";
                return response;
            }
        }

        private GeneralResponse LoadImage(string ProductImageName)
        {
            GeneralResponse response = new GeneralResponse();
            try
            {
                var fileName = @"C:\Images\Products\" + ProductImageName;
                if (File.Exists(fileName))
                {
                    var imageArray = File.ReadAllBytes(fileName);
                    var base64Image = Convert.ToBase64String(imageArray);

                    //dồn chung ImageName và string base64 của ảnh trả về cho client cắt ra xử
                    var typeImage = ProductImageName.Split('.')[1];
                    if (typeImage == "png")
                    {
                        ProductImageName = $"{ProductImageName}|data:image/png;base64,{base64Image}";
                    }
                    else if (typeImage == "jpeg" || typeImage == "jpg")
                    {
                        ProductImageName = $"{ProductImageName}|data:image/jpeg;base64,{base64Image}";
                    }
                    else if (typeImage == "svg")
                    {
                        ProductImageName = $"{ProductImageName}|data:image/svg+xml;base64,{base64Image}";
                    }
                }
                else ProductImageName = string.Empty;

                response.Flag = true;
                response.Message = ProductImageName;

                return response;
            }
            catch (Exception ex)
            {
                response.Flag = false;
                response.Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}";
                return response;
            }
        }

        private Product SyncModel(ProductAddUpdateRequestDTO model)
        {
            Product _model = new Product();

            _model.Id = model.Id;
            _model.ProductCode = model.ProductCode;
            _model.VendorId = model.VendorId;
            _model.SupplierId = model.SupplierId;
            _model.ProductType = model.ProductType;
            _model.ProductShortCode = model.ProductShortCode;
            _model.ProductShortName = model.ProductShortName;
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
            _model.CreateAt = model.CreateAt;
            _model.CreateOperatorId = model.CreateOperatorId;
            _model.UpdateAt = model.UpdateAt;
            _model.UpdateOperatorId = model.UpdateOperatorId;

            return _model;
        }

        public async Task<Result<string>> DeleteDtoAsync([Path] int id)
        {
            var err = new ErrorResponse();
            try
            {
                using (var tran = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var d = await dbContext.Products.FindAsync(id);
                        d.IsDeleted = true;
                        dbContext.Products.Update(d);

                        var dj = await dbContext.ProductJanCodes.Where(x => x.ProductId == id).ToListAsync();
                        if (dj != null && dj.Count > 0)
                        {
                            dj.ForEach(x => x.IsDeleted = true);
                            dbContext.ProductJanCodes.UpdateRange(dj);
                        }

                        var imageData = await dbContext.ImageStorages.Where(x => x.ResourcesId == id.ToString() && x.Type == EnumImageStorageType.Product).ToListAsync();
                        if (imageData != null && imageData.Count > 0)
                            dbContext.RemoveRange(imageData);

                        await dbContext.SaveChangesAsync();

                        tran.Commit();
                        return await Result<string>.SuccessAsync($"Delete product success.");
                    }
                    catch (Exception ex)
                    {
                        await tran.RollbackAsync();
                        throw new Exception($"{ex.Message} | {ex.InnerException}");
                    }
                }
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<IEnumerable<ProductDto>>> GetProductByShipmentNoAsync(string code, string shipmentNo)
        {
            try
            {
                var baseQuery = (from shipment in dbContext.WarehouseShipmentLines.Where(_ => _.ShipmentNo == shipmentNo)
                                 join product in dbContext.Products.Where(product => product.IsDeleted == false && string.IsNullOrEmpty(code) || product.ProductCode.ToLower().Contains(code.ToLower())) on shipment.ProductCode equals product.ProductCode into products
                                 from product in products
                                 join cate in dbContext.ProductCategories on product.CategoryId equals cate.Id into productCategories
                                 from cate in productCategories
                                 join unit in dbContext.Units on product.UnitId equals unit.Id into productUnits
                                 from unit in productUnits
                                 join supplier in dbContext.Suppliers on product.SupplierId equals supplier.Id into productSuppliers
                                 from supplier in productSuppliers
                                 join janCode in dbContext.ProductJanCodes on product.Id equals janCode.ProductId into productJanCodes
                                 select new ProductDto
                                 (
                                     product,
                                     supplier.SupplierName,
                                     unit.UnitName,
                                     cate.CategoryName,
                                     productJanCodes.ToList(),
                                     (int)dbContext.WarehouseShipmentLines.Where(s => s.ProductCode == product.ProductCode && s.Status == EnumShipmentOrderStatus.Open).Sum(s => s.ShipmentQty)
                                 )).AsEnumerable();

                var duplicateNames = baseQuery
                  .GroupBy(r => r.ProductCode)
                  .Where(x => x.Count() > 0)
                  .Select(g => g.Key);

                var records = baseQuery
                  .Join(duplicateNames, f => f.ProductCode, n => n, (f, n) => f);

                if (records != null)
                {
                    return await Result<IEnumerable<ProductDto>>.SuccessAsync(records);
                }
                else
                {
                    return await Result<IEnumerable<ProductDto>>.FailAsync("");
                }
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<ProductDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }



        public async Task<Result<IEnumerable<ProductDto>>> AutocompleteProductAsync(string? keyword, int tenantId)
        {
            var err = new ErrorResponse();
            try
            {
                var result = (from product in dbContext.Products.Where(product => product.IsDeleted == false
                                && (tenantId == 0 || tenantId == product.CompanyId)
                                && (string.IsNullOrEmpty(keyword) || product.ProductCode.ToLower().Contains(keyword.ToLower())))
                              join cate in dbContext.ProductCategories on product.CategoryId equals cate.Id into productCategories
                              from cate in productCategories.DefaultIfEmpty() // Left join
                              join unit in dbContext.Units on product.UnitId equals unit.Id into productUnits
                              from unit in productUnits.DefaultIfEmpty() // Left join
                              join supplier in dbContext.Suppliers on product.SupplierId equals supplier.Id into productSuppliers
                              from supplier in productSuppliers.DefaultIfEmpty() // Left join
                              join janCode in dbContext.ProductJanCodes on product.Id equals janCode.ProductId into productJanCodes
                              select new ProductDto
                              {
                                  ProductCode = product.ProductCode,
                                  ProductName = product.ProductName,
                                  UnitName = unit != null ? unit.UnitName : null, // Handle potential null
                                  StockAvailableQuantity = product.StockAvailableQuanitty,
                                  UnitId = product.UnitId,
                                  ProductType = product.ProductType,

                              }).AsEnumerable();

                if (result != null)
                {
                    return await Result<IEnumerable<ProductDto>>.SuccessAsync(result);
                }
                else
                {
                    err.Errors.Add("Warning", "Could be not found data.");
                    return await Result<IEnumerable<ProductDto>>.FailAsync(JsonConvert.SerializeObject(err));
                }
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<IEnumerable<ProductDto>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<ProductCategory>>> GetProductCategoriesAsync()
        {
            var err = new ErrorResponse();
            try
            {
                var result = await dbContext.ProductCategories.ToListAsync();

                if (result != null)
                {
                    return await Result<List<ProductCategory>>.SuccessAsync(result);
                }
                else
                {
                    err.Errors.Add("Warning", "Could be not found data.");
                    return await Result<List<ProductCategory>>.FailAsync(JsonConvert.SerializeObject(err));
                    return await Result<List<ProductCategory>>.FailAsync("");
                }
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ProductCategory>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Product>> GetByUnitAsync([Path] int unitId)
        {
            var err = new ErrorResponse();
            try
            {
                var result = await dbContext.Products.Where(x => x.UnitId == unitId).FirstOrDefaultAsync();
                return await Result<Product>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Product>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Product>> GetByCatetgoryAsync([Path] int categoryId)
        {
            var err = new ErrorResponse();
            try
            {
                var result = await dbContext.Products.Where(x => x.CategoryId == categoryId).FirstOrDefaultAsync();
                return await Result<Product>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Product>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Product>> GetBySupplierAsync([Path] int supplierId)
        {
            var err = new ErrorResponse();
            try
            {
                var result = await dbContext.Products.Where(x => x.SupplierId == supplierId).FirstOrDefaultAsync();
                return await Result<Product>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Product>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
