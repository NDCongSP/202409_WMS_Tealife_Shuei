using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Application.Services.Outbound;

using Infrastructure.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using RestEase;
using System.Linq;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryInventBundleLineService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IInventBundleLines
    {
        public async Task<Result<List<InventBundleLine>>> AddRangeAsync([Body] List<InventBundleLine> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;

                    if (await CheckExist(item)) return await Result<List<InventBundleLine>>.FailAsync($"{item.TransNo}|{item.ProductCode}|{item.LotNo}|{item.ActualQty} Is Existed");
                }

                await dbContext.InventBundleLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<InventBundleLine>>.SuccessAsync(model, "Add range InventBundleLine line successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<InventBundleLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundleLine>> DeleteRangeAsync([Body] List<InventBundleLine> model)
        {
            try
            {
                dbContext.InventBundleLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventBundleLine>.SuccessAsync("Delete range InventBundleLine line successfull");
            }
            catch (Exception ex)
            {
                return await Result<InventBundleLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundleLine>> DeleteAsync([Body] InventBundleLine model)
        {
            try
            {
                dbContext.InventBundleLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventBundleLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventBundleLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<InventBundleLine>>> GetAllAsync()
        {
            try
            {
                return await Result<List<InventBundleLine>>.SuccessAsync(await dbContext.InventBundleLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<InventBundleLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<InventBundlesLineDTO>>> GetProductsByBundleCode(string BundleCode)
        {
            try
            {
                var listUnitProduts = await 
                    ( from pb in dbContext.ProductBundles
                      where pb.ProductBundleCode == BundleCode
                      select new { pb }
                    )
                    .GroupBy(x => x.pb.ProductCode) // Group by ProductCode
                    .Select(g => g.First()) // Select the first item from each ProductCode group
                    .ToListAsync();

                List<InventBundlesLineDTO> warehouseInfos = new List<InventBundlesLineDTO>();
                foreach (var item in listUnitProduts)
                {
                    var warehouseInfo = await GetLeastExpiredBatch(item.pb.ProductCode, item.pb.CompanyId);
                    if(warehouseInfo != null)
                        warehouseInfos.Add(warehouseInfo);
                }

                return await Result<List<InventBundlesLineDTO>>.SuccessAsync(warehouseInfos);
            }
            catch (Exception ex)
            {
                return await Result<List<InventBundlesLineDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<InventBundlesLineDTO>>> GetProductsByTransNo(string TransNo)
        {
            try
            {
                var bundle = dbContext.InventBundles.First(x => x.TransNo == TransNo);
                var listUnitProduts = await
                    (from pb in dbContext.ProductBundles
                     where pb.ProductBundleCode == bundle.ProductBundleCode && pb.CompanyId == bundle.TenantId
                     select new { pb }
                    )
                    .GroupBy(x => x.pb.ProductCode) // Group by ProductCode
                    .Select(g => g.First()) // Select the first item from each ProductCode group
                    .ToListAsync();
                var listInventBundlesLines = await (from line in dbContext.InventBundleLines
                                                    join location in dbContext.Locations
                                                        on line.Location equals location.Id.ToString()
                                                    where line.TransNo == TransNo
                                                    select new InventBundlesLineDTO()
                                                    {
                                                        ProductCode = line.ProductCode,
                                                        Location = line.Location,
                                                        LocationName = location.LocationName, // Add LocationName
                                                        Bin = line.Bin,
                                                        DemandQty = line.DemandQty,
                                                        TransNo = TransNo,
                                                        ActualQty = line.ActualQty,
                                                        ExpirationDate = line.ExpirationDate,
                                                        LotNo = line.LotNo,
                                                        UnitId = line.UnitId,
                                                        Id = line.Id,
                                                    })
                                                    .ToListAsync();
                foreach (var item in listUnitProduts)
                {
                    if (!listInventBundlesLines.Exists(x => x.ProductCode == item.pb.ProductCode))
                    {
                        var newLine = new InventBundlesLineDTO() //product not in line
                        {
                            ProductCode = item.pb.ProductCode,
                            DemandQty = item.pb.Quantity * bundle.Qty,
                            TransNo = bundle.TransNo,
                            ProductQuantity = item.pb.Quantity
                        };
                        listInventBundlesLines.Add(newLine);
                    }
                }
                foreach (var item in listInventBundlesLines)
                {
                    var productData = listUnitProduts.First(x => x.pb.ProductCode == item.ProductCode);
                    var singleProduct = dbContext.Products.First(x => x.ProductCode == item.ProductCode && x.CompanyId == bundle.TenantId);
                    item.ProductQuantity = productData.pb.Quantity;
                    item.UnitId = singleProduct.UnitId;
                    item.ProductName = singleProduct.ProductName;
                    var warehouseInfo = await GetLeastExpiredBatch(item.ProductCode, bundle.TenantId);

                    if (item.Id == Guid.Empty && warehouseInfo != null)
                    {
                        item.Bin = warehouseInfo.Bin;
                        item.Location = warehouseInfo.Location;
                        item.LocationName = warehouseInfo.LocationName;
                        item.LotNo = warehouseInfo.LotNo;
                        item.ExpirationDate = warehouseInfo.ExpirationDate;
                    }

                    //Based on code, lotno & bin, get the qty
                    var result = dbContext.WarehouseTrans
                         .Where(wt => wt.ProductCode == item.ProductCode && wt.Bin == item.Bin && wt.LotNo == item.LotNo && wt.TenantId == bundle.TenantId)
                         .GroupBy(wt => new { wt.ProductCode, wt.Bin, wt.LotNo })
                         .Select(g => new
                         {
                             g.Key.ProductCode,
                             g.Key.Bin,
                             g.Key.LotNo,
                             Total = g.Sum(wt => wt.Qty)
                         })
                       .FirstOrDefault();
                    if (result != null)
                        item.StockAvailable = result.Total;
                }

                return await Result<List<InventBundlesLineDTO>>.SuccessAsync(listInventBundlesLines);
            }
            catch (Exception ex)
            {
                return await Result<List<InventBundlesLineDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        private async Task<InventBundlesLineDTO> GetLeastExpiredBatch(string productCode, int tenantId)
        {
            // 1. Fetch from WarehouseTrans and join with Locations
            var query1 = await (from wt in dbContext.WarehouseTrans
                                join l in dbContext.Locations on wt.Location equals l.Id.ToString()
                                where wt.ProductCode == productCode && wt.TenantId == tenantId
                                select new { wt, l })
                              .ToListAsync();

            // 2. Fetch all batches for the given product code and tenant ID
            var batches = await dbContext.Batches
              .Where(b => b.ProductCode == productCode && b.TenantId == tenantId)
              .ToListAsync();

            // 3. Create a list to hold the results
            var result = new List<InventBundlesLineDTO>();

            // 4. Loop through the results from the first query and assign ExpirationDate from batches
            foreach (var item in query1)
            {
                var batch = batches.FirstOrDefault(b => b.ProductCode == item.wt.ProductCode && b.LotNo == item.wt.LotNo);

                var dto = new InventBundlesLineDTO
                {
                    ProductCode = item.wt.ProductCode,
                    Bin = item.wt.Bin,
                    Location = item.wt.Location,
                    LocationName = item.l.LocationName,
                    LotNo = item.wt.LotNo,
                    ExpirationDate = batch != null && batch.ExpirationDate.HasValue ? batch.ExpirationDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    StockAvailable = dbContext.WarehouseTrans
                      .Where(t => t.ProductCode == item.wt.ProductCode && t.Bin == item.wt.Bin && t.LotNo == item.wt.LotNo)
                      .Sum(t => t.Qty)
                };

                if (dto.StockAvailable > 0)
                {
                    result.Add(dto);
                }
            }

            // 5. Order the results by ExpirationDate
            result = result.OrderBy(x => x.ExpirationDate).ToList();

            // 6. Return the first item (least expired)
            return result.FirstOrDefault();
        }


        public async Task<Result<InventBundleLine>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<InventBundleLine>.SuccessAsync(await dbContext.InventBundleLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<InventBundleLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundleLine>> InsertAsync([Body] InventBundleLine model)
        {
            try
            {
                //check required
                if (await CheckExist(model))
                    return await Result<InventBundleLine>.FailAsync($"{model.TransNo}|{model.ProductCode}|{model.LotNo}|{model.ActualQty} Is Existed");
                await dbContext.InventBundleLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventBundleLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventBundleLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundleLine>> UpdateAsync([Body] InventBundleLine model)
        {
            try
            {
                //check required
                if (await dbContext.InventBundleLines.AsNoTracking().FirstAsync(x => x.Id == model.Id) == null)
                    return await Result<InventBundleLine>.FailAsync($"{model.TransNo}|{model.ProductCode}|{model.LotNo}|{model.ActualQty} could be not found.");
                dbContext.InventBundleLines.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventBundleLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventBundleLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        private async Task<bool> CheckExist(InventBundleLine model)
        {
            return await dbContext.InventBundleLines.AnyAsync(x => x.TransNo == model.TransNo && x.ProductCode == model.ProductCode && x.LotNo == model.LotNo
                                                                    && x.ActualQty == model.ActualQty);
        }
    }
}
