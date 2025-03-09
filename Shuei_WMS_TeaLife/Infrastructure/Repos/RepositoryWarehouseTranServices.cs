using Application.DTOs;
using Application.DTOs.Request.Products;
using Application.Extentions;
using Application.Models;
using Application.Services;
using Dapper;
using FBT.ShareModels.WMS;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryWarehouseTranServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehouseTran
    {
        public async Task<Result<List<WarehouseTran>>> AddRangeAsync([Body] List<WarehouseTran> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;
                }

                await dbContext.WarehouseTrans.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<WarehouseTran>>.SuccessAsync(model, "Add range WarehouseTran successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<WarehouseTran>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseTran>> DeleteRangeAsync([Body] List<WarehouseTran> model)
        {
            try
            {
                dbContext.WarehouseTrans.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseTran>.SuccessAsync("Delete range WarehouseTran successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseTran>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseTran>> DeleteAsync([Body] WarehouseTran model)
        {
            try
            {
                dbContext.WarehouseTrans.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseTran>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseTran>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<WarehouseTran>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseTran>>.SuccessAsync(await dbContext.WarehouseTrans.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<WarehouseTran>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseTran>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseTran>.SuccessAsync(await dbContext.WarehouseTrans.FindAsync(id));
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseTran>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseTran>> InsertAsync([Body] WarehouseTran model)
        {
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateAt = DateTime.Now;
                model.CreateOperatorId = user?.Id;

                await dbContext.WarehouseTrans.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseTran>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseTran>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseTran>> UpdateAsync([Body] WarehouseTran model)
        {
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateAt = DateTime.Now;
                model.UpdateOperatorId = user?.Id;

                dbContext.WarehouseTrans.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseTran>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseTran>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<InventoryInfoDTO>>> GetAllProductInventoryInfoAsync()
        {
            try
            {
                var res = await dbContext.Database.SqlQueryRaw<InventoryInfoDTO>("sp_getInventoryInfo").ToListAsync();

                return await Result<List<InventoryInfoDTO>>.SuccessAsync(res);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<InventoryInfoDTO>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<InventoryInfoDTO>> GetByProductCodeInventoryInfoAsync([Path] string productCode)
        {
            try
            {
                var res = await dbContext.Database.SqlQueryRaw<InventoryInfoDTO>("sp_getInventoryInfo @productCode = {0}", productCode)
                            .ToListAsync();

                return await Result<InventoryInfoDTO>.SuccessAsync(res.FirstOrDefault());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<InventoryInfoDTO>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<InventoryInfoBinLotResponseDTO>>> GetAllProductInventoryInfoFlowBinLotAsync()
        {
            try
            {
                var resInvent = await dbContext.Database.SqlQueryRaw<InventoryInfoBinLot>("sp_getInventoryInfoFlowBinLot").ToListAsync();

                if (resInvent.Count == 0)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Empty.");
                    return await Result<List<InventoryInfoBinLotResponseDTO>>.FailAsync(JsonConvert.SerializeObject(err));
                }


                var inventoryList = new List<InventoryInfoBinLotResponseDTO>();
                var inventDetails = new List<InventoryInfoBinLotDetails>();

                var groups = resInvent.GroupBy(x => x.ProductCode);

                var g = groups.Select(group => new
                {

                    ProductCode = group.Key,
                    ProductName = group.Select(u => u.ProductName).FirstOrDefault(),
                    Details = group.ToList()
                })
                    .ToList();

                foreach (var item in g)
                {
                    inventDetails = new List<InventoryInfoBinLotDetails>();
                    foreach (var item1 in item.Details)
                    {
                        inventDetails.Add(new InventoryInfoBinLotDetails()
                        {
                            CompanyId = item1.TenantId,
                            WarehouseCode = item1.LocationName,
                            BinCode = item1.BinCode,
                            LotNo = item1.LotNo,
                            Expired = item1.Expired,
                            InventoryStock = item1.InventoryStock,
                            OnOrder = item1.OnOrder,
                            AvailableStock = item1.AvailableStock
                        });
                    }

                    inventoryList.Add(new InventoryInfoBinLotResponseDTO()
                    {
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        Details = inventDetails
                    });
                }

                return await Result<List<InventoryInfoBinLotResponseDTO>>.SuccessAsync(inventoryList, "Successful.");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<InventoryInfoBinLotResponseDTO>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<InventoryInfoBinLotResponseDTO>> GetByProductCodeInventoryInfoFlowBinLotAsync([Path] string productCode)
        {
            try
            {
                var resInvent = await dbContext.Database.SqlQueryRaw<InventoryInfoBinLot>("sp_getInventoryInfoFlowBinLot @productCode = {0}", productCode)
                            .ToListAsync();

                if (resInvent.Count == 0)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Empty.");
                    return await Result<InventoryInfoBinLotResponseDTO>.FailAsync(JsonConvert.SerializeObject(err));
                }

                var inventDetails = new List<InventoryInfoBinLotDetails>();
                foreach (var item in resInvent)
                {
                    inventDetails.Add(new InventoryInfoBinLotDetails()
                    {
                        CompanyId = item.TenantId,
                        WarehouseCode = item.LocationName,
                        BinCode = item.BinCode,
                        LotNo = item.LotNo,
                        Expired = item.Expired,
                        InventoryStock = item.InventoryStock,
                        OnOrder = item.OnOrder,
                        AvailableStock = item.AvailableStock
                    });
                }

                var responses = new InventoryInfoBinLotResponseDTO()
                {
                    ProductCode = productCode,
                    ProductName = resInvent.FirstOrDefault().ProductName,
                    Details = inventDetails
                };

                return await Result<InventoryInfoBinLotResponseDTO>.SuccessAsync(responses, "Successful");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<InventoryInfoBinLotResponseDTO>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<InventoryInfoBinLotResponseDTO>>> GetInventoryInfoFlowBinLotFlowModelSearchAsync([Body] InventoryInfoBinLotSearchRequestDTO model)
        {
            try
            {
                string productString = string.Empty;
                int companyId = 0;

                companyId = model.CompanyId;
                foreach (var item in model.ProductCodes)
                {
                    productString = $"{productString},{item.ProductCode}";
                }

                var resInvent = await dbContext.Database.SqlQueryRaw<InventoryInfoBinLot>("sp_getInventoryInfoFlowBinLot @productCode = {0}, @companyId = {1}"
                                                                                            , productString, companyId).ToListAsync();

                if (resInvent.Count == 0)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Empty.");
                    return await Result<List<InventoryInfoBinLotResponseDTO>>.FailAsync(JsonConvert.SerializeObject(err));
                }

                var inventoryList = new List<InventoryInfoBinLotResponseDTO>();
                var inventDetails = new List<InventoryInfoBinLotDetails>();

                var groups = resInvent.GroupBy(x => x.ProductCode);

                var g = groups.Select(group => new
                {

                    ProductCode = group.Key,
                    ProductName = group.Select(u => u.ProductName).FirstOrDefault(),
                    Details = group.ToList()
                })
                    .ToList();

                foreach (var item in g)
                {
                    inventDetails = new List<InventoryInfoBinLotDetails>();
                    foreach (var item1 in item.Details)
                    {
                        inventDetails.Add(new InventoryInfoBinLotDetails()
                        {
                            CompanyId = item1.TenantId,
                            WarehouseCode = item1.LocationName,
                            BinCode = item1.BinCode,
                            LotNo = item1.LotNo,
                            Expired = item1.Expired,
                            InventoryStock = item1.InventoryStock,
                            OnOrder = item1.OnOrder,
                            AvailableStock = item1.AvailableStock
                        });
                    }

                    inventoryList.Add(new InventoryInfoBinLotResponseDTO()
                    {
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        Details = inventDetails
                    });
                }

                return await Result<List<InventoryInfoBinLotResponseDTO>>.SuccessAsync(inventoryList, "Successful.");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<InventoryInfoBinLotResponseDTO>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="bin"></param>
        /// <param name="supplier"></param>
        /// <param name="category"></param>
        /// <param name="productCode"></param>
        /// <param name="productName"></param>
        /// <param name="includedLocation"></param>
        /// <param name="includedBin"></param>
        /// <param name="includedExpiredDate"></param>
        /// <returns></returns>
        public async Task<Result<List<InventoryHistoryDto>>> GetProductInventoryInformationAsync([Body] InventoryInformationSearchModel model)
        {
            try
            {
                var allProducts = new List<InventoryHistoryDto>();
                var baseQuery = dbContext.Products
                .Join(dbContext.WarehouseTrans,
                    p => new { p.ProductCode, TenantId = (int?)p.CompanyId },
                    w => new { w.ProductCode, w.TenantId },
                    (p, w) => new { p, w })
                .Join(dbContext.Locations,
                    pw => pw.w.Location,
                    l => l.Id.ToString(),
                    (pw, l) => new { pw.p, pw.w, l })
                 .Join(dbContext.Companies,
                    pwl => pwl.w.TenantId,
                    t => t.AuthPTenantId,
                    (pw, t) => new { pw.p, pw.w, pw.l, t })
        .GroupJoin(dbContext.Bins,
            pwl => new { BinCode = pwl.w.Bin, LocationId = pwl.l.Id },
            b => new { b.BinCode, b.LocationId },
            (pwl, b) => new { pwl.p, pwl.w, pwl.l, pwl.t, bins = b })
        .SelectMany(
            x => x.bins.DefaultIfEmpty(),
            (x, b) => new { x.p, x.w, x.l, x.t, b })

                .Join(dbContext.Suppliers,
                    pwlb => pwlb.p.SupplierId,
                    s => s.Id,
                    (pwlb, s) => new { pwlb.p, pwlb.w, pwlb.l, pwlb.t, pwlb.b, s })
                .Join(dbContext.ProductCategories,
                    pwlbs => pwlbs.p.CategoryId,
                    c => c.Id,
                    (pwlbs, c) => new { pwlbs.p, pwlbs.w, pwlbs.l, pwlbs.t, pwlbs.b, pwlbs.s, c })
                .Join(dbContext.Units,
                    pwlbsc => pwlbsc.p.UnitId,
                    u => u.Id,
                    (pwlbsc, u) => new { pwlbsc.p, pwlbsc.w, pwlbsc.l, pwlbsc.t, pwlbsc.b, pwlbsc.s, pwlbsc.c, u });
                baseQuery = baseQuery.Where(x => x.w.DatePhysical != null);
                if (!string.IsNullOrWhiteSpace(model.Bin))
                {
                    baseQuery = baseQuery.Where(x => x.b.Id.ToString() == model.Bin);
                }
                if (!string.IsNullOrWhiteSpace(model.Location))
                {
                    baseQuery = baseQuery.Where(x => x.w.Location == model.Location);
                }
                if (!string.IsNullOrWhiteSpace(model.ProductCode))
                {
                    baseQuery = baseQuery.Where(x => x.p.ProductCode.Contains(model.ProductCode));
                }
                if (!string.IsNullOrWhiteSpace(model.ProductName))
                {
                    baseQuery = baseQuery.Where(x => x.p.ProductName.Contains(model.ProductName));
                }
                if (!string.IsNullOrWhiteSpace(model.Supplier))
                {
                    var supplierId = Convert.ToInt32(model.Supplier);
                    baseQuery = baseQuery.Where(x => x.p.SupplierId == supplierId);
                }
                if (!string.IsNullOrWhiteSpace(model.Tenant))
                {
                    var tenantId = Convert.ToInt32(model.Tenant);
                    baseQuery = baseQuery.Where(x => x.p.CompanyId == tenantId && x.w.TenantId == tenantId);
                }
                if (!string.IsNullOrWhiteSpace(model.Category))
                {
                    var categoryId = Convert.ToInt32(model.Category);
                    baseQuery = baseQuery.Where(x => x.p.CategoryId == categoryId);
                }

                if (model.LocationDetail)
                {
                    allProducts = await baseQuery.GroupBy(x => new { x.p.Id, x.p.ProductCode, LocationId = x.l.Id, x.w.Location, x.l.LocationName, x.w.TenantId, x.t.FullName })
                       .Select(g => new InventoryHistoryDto
                       {
                           ProductId = g.Key.Id,
                           ProductCode = g.Key.ProductCode,
                           TenantId = g.Key.TenantId ?? 0,
                           TenantName = g.Key.FullName ?? "N/A",
                           LocationId = g.Key.LocationId,
                           LocationName = g.Key.LocationName,
                           Quantity = g.Sum(x => x.w.Qty),
                       }).AsNoTracking().ToListAsync();
                }

                if (model.BinDetail)
                {
                    allProducts = await baseQuery.GroupBy(x => new { x.p.Id, x.p.ProductCode, BinId = x.b.Id, x.w.Bin, LocationId = x.l.Id, x.w.Location, x.l.LocationName, x.w.TenantId, x.t.FullName })
                       .Select(g => new InventoryHistoryDto
                       {
                           ProductId = g.Key.Id,
                           ProductCode = g.Key.ProductCode,
                           TenantId = g.Key.TenantId ?? 0,
                           TenantName = g.Key.FullName ?? "N/A",
                           BinId = g.Key.BinId,
                           BinCode = g.Key.Bin,
                           LocationId = g.Key.LocationId,
                           LocationName = g.Key.LocationName,
                           Quantity = g.Sum(x => x.w.Qty),
                       }).AsNoTracking().ToListAsync();
                }

                if (model.LotExpirationDetail)
                {
                    allProducts = await baseQuery
                     .GroupJoin(dbContext.Batches,
                         pwlbscu => new { pwlbscu.w.LotNo, pwlbscu.w.ProductCode },
                         b => new { b.LotNo, b.ProductCode },
                         (pwlbscu, batches) => new { pwlbscu.p, pwlbscu.w, pwlbscu.l, pwlbscu.t, pwlbscu.b, pwlbscu.s, pwlbscu.c, pwlbscu.u, Batches = batches })
                     .SelectMany(x => x.Batches.DefaultIfEmpty(),
                         (x, b) => new { x.p, x.w, x.l, x.t, x.b, x.s, x.c, x.u, Batch = b })
                    .GroupBy(x => new { x.p.Id, x.p.ProductCode, BinId = x.b.Id, x.w.Bin, LocationId = x.l.Id, x.w.Location, x.l.LocationName, LotNo = (x.Batch != null ? x.Batch.LotNo : null), ExpirationDate = (x.Batch != null ? x.Batch.ExpirationDate : null), x.w.TenantId, x.t.FullName })
                     .Select(g => new InventoryHistoryDto
                     {
                         ProductId = g.Key.Id,
                         ProductCode = g.Key.ProductCode,
                         TenantId = g.Key.TenantId ?? 0,
                         TenantName = g.Key.FullName ?? "N/A",
                         BinId = g.Key.BinId,
                         BinCode = g.Key.Bin,
                         LocationId = g.Key.LocationId,
                         LocationName = g.Key.LocationName,
                         Lot = g.Key.LotNo,
                         ExpirationDate = g.Key.ExpirationDate,
                         Quantity = g.Sum(x => x.w.Qty),
                     }).AsNoTracking().ToListAsync();
                }
                var allProductTotal = await baseQuery.GroupBy(x => new { x.p.Id, x.p.ProductCode, x.p.ProductName, x.u.UnitName, SupplierId = x.s.Id, x.s.SupplierName, x.c.CategoryName, x.t.FullName, x.w.TenantId })
        .Select(g => new InventoryHistoryDto
        {
            ProductId = g.Key.Id,
            ProductCode = g.Key.ProductCode,
            ProductName = g.Key.ProductName,
            SupplierName = g.Key.SupplierName,
            SupplierId = g.Key.SupplierId,
            TenantName = g.Key.FullName,
            TenantId = g.Key.TenantId ?? 0,
            CategoryName = g.Key.CategoryName,
            UnitName = g.Key.UnitName,
            Quantity = g.Sum(x => x.w.Qty),
            TotalRow = true
        }).AsNoTracking().ToListAsync();
                allProducts.AddRange(allProductTotal);
                List<string> locations = new List<string>();
                var returnAllProducts = allProducts.OrderBy(x => x.ProductId).ThenByDescending(x => x.ProductCode).ThenBy(x => x.LocationName).ToList();
                foreach (var item in returnAllProducts)

                {
                    if (!locations.Contains(item.ProductId + item.LocationName))
                    {
                        locations.Add(item.ProductId + item.LocationName);
                    }
                    else
                    {
                        item.LocationName = "";
                    }

                }

                if (returnAllProducts != null)
                {
                    return await Result<List<InventoryHistoryDto>>.SuccessAsync(returnAllProducts.Where(_ => _.Quantity != 0).ToList());
                }
                else
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", $"Empty.");
                    return await Result<List<InventoryHistoryDto>>.FailAsync(JsonConvert.SerializeObject(err));
                }

            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<List<InventoryHistoryDto>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="bin"></param>
        /// <param name="supplier"></param>
        /// <param name="category"></param>
        /// <param name="productCode"></param>
        /// <param name="productName"></param>
        /// <param name="includedLocation"></param>
        /// <param name="includedBin"></param>
        /// <param name="includedExpiredDate"></param>
        /// <returns></returns>
        public async Task<Result<List<InventoryHistoryDetailsDto>>> GetProductInventoryInformationDetailsAsync([Body] InventoryInformationDetailsSearchModel model)
        {
            try
            {
                var baseQuery = dbContext.Products
         .Join(dbContext.WarehouseTrans,
             p => new { p.ProductCode, p.CompanyId },
             w => new { w.ProductCode, CompanyId = w.TenantId ?? 0 },
             (p, w) => new { p, w })
         .Join(dbContext.Locations,
             pw => pw.w.Location,
             l => l.Id.ToString(),
             (pw, l) => new { pw.p, pw.w, l })
         .GroupJoin(dbContext.Bins,
             pwl => new { BinCode = pwl.w.Bin, LocationId = pwl.l.Id },
             b => new { b.BinCode, b.LocationId },
             (pwl, b) => new { pwl.p, pwl.w, pwl.l, bins = b })
         .SelectMany(
             x => x.bins.DefaultIfEmpty(),
             (x, b) => new { x.p, x.w, x.l, b })
         .Join(dbContext.Suppliers,
             pwlb => pwlb.p.SupplierId,
             s => s.Id,
             (pwlb, s) => new { pwlb.p, pwlb.w, pwlb.l, pwlb.b, s })
         .Join(dbContext.Units,
             pwlbsc => pwlbsc.p.UnitId,
             u => u.Id,
             (pwlbsc, u) => new { pwlbsc.p, pwlbsc.w, pwlbsc.l, pwlbsc.b, pwlbsc.s, u })
         .Join(dbContext.Companies,
             pwlbscu => pwlbscu.w.TenantId,
             t => t.AuthPTenantId,
             (pwlbscu, t) => new { pwlbscu.p, pwlbscu.w, pwlbscu.l, pwlbscu.b, pwlbscu.s, pwlbscu.u, t })
         .GroupJoin(dbContext.Batches,
             pwlbsc => new
             {
                 pwlbsc.p.ProductCode,
                 pwlbsc.w.LotNo,
                 pwlbsc.p.CompanyId
             },
             ial => new { ial.ProductCode, ial.LotNo, CompanyId = ial.TenantId },
             (pwlbsc, ial) => new { pwlbsc.p, pwlbsc.w, pwlbsc.l, pwlbsc.b, pwlbsc.s, pwlbsc.u, pwlbsc.t, adjustments = ial })
         .SelectMany(
             x => x.adjustments.DefaultIfEmpty(),
             (x, adj) => new
             {
                 x.p,
                 x.w,
                 x.l,
                 x.b,
                 x.s,
                 x.u,
                 x.t,
                 ExpirationDate = adj != null ? adj.ExpirationDate : null
             });
                baseQuery = baseQuery.Where(x => x.w.DatePhysical != null);
                baseQuery = baseQuery.Where(x => true && (x.w.TransType != EnumWarehouseTransType.PutAway || x.w.Qty >= 0));  //Customer request to remove - column for Putaway

                if (!string.IsNullOrWhiteSpace(model.Bin))
                {
                    baseQuery = baseQuery.Where(x => x.b.Id.ToString() == model.Bin);
                }
                if (!string.IsNullOrWhiteSpace(model.Location))
                {
                    baseQuery = baseQuery.Where(x => x.w.Location == model.Location);
                }
                if (!string.IsNullOrWhiteSpace(model.ProductCode))
                {
                    baseQuery = baseQuery.Where(x => x.p.ProductCode.Contains(model.ProductCode));
                }
                if (!string.IsNullOrWhiteSpace(model.ProductName))
                {
                    baseQuery = baseQuery.Where(x => x.p.ProductName.Contains(model.ProductName));
                }
                if (!string.IsNullOrWhiteSpace(model.Supplier))
                {
                    var supplierId = Convert.ToInt32(model.Supplier);
                    baseQuery = baseQuery.Where(x => x.p.SupplierId == supplierId);
                }
                if (!string.IsNullOrWhiteSpace(model.Tenant))
                {
                    var tenantId = Convert.ToInt32(model.Tenant);
                    baseQuery = baseQuery.Where(x => x.p.CompanyId == tenantId);
                }
                if (!string.IsNullOrWhiteSpace(model.TransType))
                {
                    var transtypeId = (EnumWarehouseTransType)Convert.ToInt32(model.TransType);
                    baseQuery = baseQuery.Where(x => x.w.TransType == transtypeId);
                }
                if (!string.IsNullOrWhiteSpace(model.Lot))
                {
                    // baseQuery = baseQuery.Where(x => x. == transtypeId);
                }
                if (model.FromDate != null)
                {
                    baseQuery = baseQuery.Where(x => x.w.DatePhysical >= model.FromDate);
                }
                if (model.ToDate != null)
                {
                    baseQuery = baseQuery.Where(x => x.w.DatePhysical <= model.ToDate);
                }

                var allProduct = await baseQuery
         .Select(g => new InventoryHistoryDetailsDto
         {
             ProductId = g.p.Id,
             ProductCode = g.p.ProductCode,
             ProductName = g.p.ProductName,
             SupplierName = g.s.SupplierName,
             SupplierId = g.s.Id,
             TenantName = g.t.FullName,
             TenantId = g.w.TenantId,
             LocationName = g.l.LocationName,
             LocationId = g.l.LocationCD,
             BinCode = g.b.BinCode,
             LotNo = g.w.LotNo,
             UnitName = g.u.UnitName,
             TransNumber = g.w.TransNumber,
             TransType = g.w.TransType,
             DatePhysical = g.w.DatePhysical,
             Quantity = g.w.Qty,
             CreateAt = g.w.CreateAt,
             ExpirationDate = g.ExpirationDate,
         })
         .OrderBy(x => x.ProductCode)
         .ThenByDescending(x => x.DatePhysical)
         .AsNoTracking()
         .ToListAsync();



                if (allProduct != null)
                {
                    return await Result<List<InventoryHistoryDetailsDto>>.SuccessAsync(allProduct);
                }
                else
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", $"Empty.");
                    return await Result<List<InventoryHistoryDetailsDto>>.FailAsync(JsonConvert.SerializeObject(err));
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<List<InventoryHistoryDetailsDto>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<List<InventoryAdjustmentDetailsDto>>> GetInventoryAdjustmentDetailsAsync([Body] InventoryAdjustmentDetailsModel model)
        {
            try
            {
                var allProducts = new List<InventoryAdjustmentDetailsDto>();
                var baseQuery = dbContext.Products
                .Join(dbContext.WarehouseTrans,
                    p => p.ProductCode,
                    w => w.ProductCode,
                    (p, w) => new { p, w })
                .Join(dbContext.Locations,
                    pw => pw.w.Location,
                    l => l.Id.ToString(),
                    (pw, l) => new { pw.p, pw.w, l })
                .Join(dbContext.Bins,
                    pwl => new { BinCode = pwl.w.Bin, LocationId = pwl.l.Id },
                    b => new { b.BinCode, b.LocationId },
                    (pwl, b) => new { pwl.p, pwl.w, pwl.l, b })
                .Join(dbContext.Units,
                    pwlbsc => pwlbsc.p.UnitId,
                    u => u.Id,
                    (pwlbsc, u) => new { pwlbsc.p, pwlbsc.w, pwlbsc.l, pwlbsc.b, u });

                if (!string.IsNullOrWhiteSpace(model.Bin))
                {
                    baseQuery = baseQuery.Where(x => x.b.BinCode == model.Bin);
                }
                if (!string.IsNullOrWhiteSpace(model.Location))
                {
                    baseQuery = baseQuery.Where(x => x.w.Location == model.Location);
                }

                var bundleQuery = baseQuery.Join(dbContext.ProductBundles, pwlbsc => pwlbsc.p.ProductCode, bundle => bundle.ProductCode, (pwlbsc, bundle) => new { pwlbsc.p, pwlbsc.w, pwlbsc.l, pwlbsc.b, pwlbsc.u, bundle });
                bundleQuery = bundleQuery.Where(x => x.bundle.Quantity != 0);
                var allProductTotal = await baseQuery.GroupBy(x => new { x.p.Id, x.p.ProductCode, x.p.ProductName, x.u.UnitName, BinId = x.b.Id, LocationId = x.l.Id })
        .Select(g => new InventoryAdjustmentDetailsDto
        {
            ProductId = g.Key.Id,
            ProductCode = g.Key.ProductCode,
            ProductName = g.Key.ProductName,
            PerSet = false,
            UnitName = g.Key.UnitName,
            StockQuantity = g.Sum(x => x.w.Qty),
            FinalStockQuantity = g.Sum(x => x.w.Qty)
        }).AsNoTracking().ToListAsync();
                var allProductBundle = await bundleQuery.GroupBy(x => new { x.bundle.Id, x.bundle.ProductBundleCode, x.u.UnitName, BinId = x.b.Id, LocationId = x.l.Id })
                .Select(g => new InventoryAdjustmentDetailsDto
                {
                    ProductId = g.Key.Id,
                    ProductCode = g.Key.ProductBundleCode,
                    ProductName = "Bundle",
                    PerSet = true,
                    UnitName = g.Key.UnitName,
                    StockQuantity = g.Min(x => Math.Floor(x.w.Qty / x.bundle.Quantity)),
                    FinalStockQuantity = g.Min(x => Math.Floor(x.w.Qty / x.bundle.Quantity)),
                }).AsNoTracking().ToListAsync();

                allProducts.AddRange(allProductTotal);
                allProducts.AddRange(allProductBundle);
                var returnAllProducts = allProducts.OrderBy(x => x.ProductId).ThenByDescending(x => x.ProductCode).ThenBy(x => x.ProductId).ToList();

                if (returnAllProducts != null)
                {
                    return await Result<List<InventoryAdjustmentDetailsDto>>.SuccessAsync(returnAllProducts);
                }
                else
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", $"Empty.");
                    return await Result<List<InventoryAdjustmentDetailsDto>>.FailAsync(JsonConvert.SerializeObject(err));
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<List<InventoryAdjustmentDetailsDto>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }


        public async Task<Result<List<ProductDto>>> GetProductAdjustmentAsync([Body] InventoryAdjustmentDetailsModel model)
        {
            try
            {
                var baseQuery = dbContext.Products
                .Join(dbContext.WarehouseTrans,
                    p => p.ProductCode,
                    w => w.ProductCode,
                    (p, w) => new { p, w })
                .Join(dbContext.Locations,
                    pw => pw.w.Location,
                    l => l.Id.ToString(),
                    (pw, l) => new { pw.p, pw.w, l })
                .Join(dbContext.Bins,
                    pwl => new { BinCode = pwl.w.Bin, LocationId = pwl.l.Id },
                    b => new { b.BinCode, b.LocationId },
                    (pwl, b) => new { pwl.p, pwl.w, pwl.l, b })
                .Join(dbContext.Units,
                    pwlbsc => pwlbsc.p.UnitId,
                    u => u.Id,
                    (pwlbsc, u) => new { pwlbsc.p, pwlbsc.w, pwlbsc.l, pwlbsc.b, u });

                if (!string.IsNullOrWhiteSpace(model.Bin))
                {
                    baseQuery = baseQuery.Where(x => x.b.Id.ToString() == model.Bin);
                }
                if (model.TenantId != null)
                {
                    baseQuery = baseQuery.Where(x => x.w.TenantId == model.TenantId);
                }
                if (!string.IsNullOrWhiteSpace(model.Location))
                {
                    baseQuery = baseQuery.Where(x => x.w.Location == model.Location);
                }
                var allProductTotal = await baseQuery.GroupBy(x => new { x.p.Id, x.p.ProductCode, x.p.ProductType, x.p.ProductName, x.u.UnitName, BinId = x.b.Id, LocationId = x.l.Id })
        .Select(g => new ProductDto
        {
            ProductCode = g.Key.ProductCode,
            ProductName = g.Key.ProductName,
            UnitName = g.Key.UnitName,
            StockAvailableQuantityTrans = (int)g.Sum(x => x.w.Qty),
            ProductType = g.Key.ProductType
        }).AsNoTracking().ToListAsync();

                if (allProductTotal != null)
                {
                    return await Result<List<ProductDto>>.SuccessAsync(allProductTotal);
                }
                else
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", $"Empty.");
                    return await Result<List<ProductDto>>.FailAsync(JsonConvert.SerializeObject(err));
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<List<ProductDto>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<InventoryAdjustmentDetailsDto>>> GetInventoryBundleAdjustmentDetailsAsync([Body] InventoryAdjustmentDetailsModel model)
        {
            try
            {
                var allProducts = await (from bp in dbContext.ProductBundles
                                         join p in dbContext.Products on bp.ProductCode equals p.ProductCode
                                         from u in dbContext.Units.Where(u => u.Id == p.UnitId).DefaultIfEmpty() // LEFT JOIN with Units
                                         from wa in dbContext.WarehouseTrans.Where(wa =>
                                         wa.ProductCode == bp.ProductBundleCode
                                         && wa.Location == model.Location
                                         && wa.LotNo == model.LotNo
                                         ).DefaultIfEmpty()
                                         where bp.ProductBundleCode == model.BundleCode
                                         select new InventoryAdjustmentDetailsDto
                                         {
                                             ProductCode = bp.ProductCode,
                                             ProductName = p.ProductCode,
                                             PerSet = false,
                                             StockQuantity = bp.Quantity * wa.Qty,
                                         }).Distinct().ToListAsync();
                if (allProducts != null)
                {
                    return await Result<List<InventoryAdjustmentDetailsDto>>.SuccessAsync(allProducts);
                }
                else
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", $"Empty.");
                    return await Result<List<InventoryAdjustmentDetailsDto>>.FailAsync(JsonConvert.SerializeObject(err));
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<List<InventoryAdjustmentDetailsDto>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }


        public async Task<Result<List<InventoryAdjustmentDetailsDto>>> UpdateInventoryAdjustmentAsync([Body] List<InventoryAdjustmentDetailsDto> model)
        {
            try
            {
                List<WarehouseTran> changedItems = new List<WarehouseTran>();
                foreach (var item in model)
                {
                    changedItems.Add(new WarehouseTran()
                    {
                        ProductCode = item.ProductCode,

                    });
                }
                await dbContext.WarehouseTrans.AddRangeAsync(changedItems);
                await dbContext.SaveChangesAsync();
                return await Result<List<InventoryAdjustmentDetailsDto>>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<List<InventoryAdjustmentDetailsDto>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<int>> GetQtyFromWarehouseTrans(InventoryAdjustmentDetailsModel model)
        {
            try
            {
                var totalQuantity = await dbContext.WarehouseTrans
                    .Where(x => x.ProductCode == model.ProductCode && x.Location == model.Location && x.Bin == model.Bin && x.StatusReceipt != null)
                    .SumAsync(x => x.Qty);
                return Result<int>.Success((int)totalQuantity);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<int>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<InventAdjustmentsLineDTO>>> GetInventoryAdjustmentLinesDefaultProduct(InventoryAdjustmentDetailsModel model)
        {
            try
            {
                var productLines = await (from wt in dbContext.WarehouseTrans
                                          join p in dbContext.Products on wt.ProductCode equals p.ProductCode
                                          join bin in dbContext.Bins on new { BinCode = wt.Bin, LocationId = (wt.Location) } equals new { bin.BinCode, LocationId = bin.LocationId.ToString() }
                                          join unit in dbContext.Units on p.UnitId equals unit.Id into productUnits
                                          from unit in productUnits
                                          where wt.Location == model.Location && bin.Id.ToString() == model.Bin
                                          && (model.LotNo == null || wt.LotNo == model.LotNo)
                                          && (model.ProductCode == null || model.ProductCode == wt.ProductCode)
                                          && model.TenantId == wt.TenantId && p.CompanyId == model.TenantId
                                          group wt by new { wt.ProductCode, wt.LotNo, p.ProductName, unit.UnitName, p.ProductType } into g
                                          select new InventAdjustmentsLineDTO()
                                          {
                                              ProductCode = g.Key.ProductCode,
                                              LotNo = g.Key.LotNo,
                                              UnitName = g.Key.UnitName,
                                              StockAvailable = g.Sum(y => y.Qty),
                                              ProductName = g.Key.ProductName,
                                          }).Where(x => x.StockAvailable != 0)
                                          .ToListAsync();
                productLines.ForEach(item =>
                {
                    var batchInfo = dbContext.Batches.FirstOrDefault(x => x.TenantId == model.TenantId && x.ProductCode == item.ProductCode && x.LotNo == item.LotNo);
                    if (batchInfo != null)
                    {
                        item.ExpirationDate = batchInfo.ExpirationDate;
                    }
                });
                return Result<List<InventAdjustmentsLineDTO>>.Success(productLines);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<List<InventAdjustmentsLineDTO>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}