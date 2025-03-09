using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryInventBundleService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IInventBundle
    {
        public async Task<Result<List<InventBundle>>> AddRangeAsync([Body] List<InventBundle> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;

                    if (await CheckExist(item)) return await Result<List<InventBundle>>.FailAsync($"TransNo {item.TransNo} Is Existed");
                }

                await dbContext.InventBundles.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<InventBundle>>.SuccessAsync(model, "Add range InvenAdjustment successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<InventBundle>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundle>> DeleteRangeAsync([Body] List<InventBundle> model)
        {
            try
            {
                dbContext.InventBundles.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventBundle>.SuccessAsync("Delete range InvenAdjustment successfull");
            }
            catch (Exception ex)
            {
                return await Result<InventBundle>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundle>> DeleteAsync([Body] InventBundle model)
        {
            try
            {
                dbContext.InventBundles.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventBundle>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventBundle>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<InventBundle>>> GetAllAsync()
        {
            try
            {
                return await Result<List<InventBundle>>.SuccessAsync(await dbContext.InventBundles.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<InventBundle>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundle>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<InventBundle>.SuccessAsync(await dbContext.InventBundles.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<InventBundle>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundle>> InsertAsync([Body] InventBundle model)
        {
            try
            {
                //check required
                if (await CheckExist(model))
                    return await Result<InventBundle>.FailAsync($"TransNo {model.TransNo} Is Existed");
                await dbContext.InventBundles.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventBundle>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventBundle>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundle>> UpdateAsync([Body] InventBundle model)
        {
            try
            {
                //check required
                if (await dbContext.InventBundles.CountAsync(x => x.Id == model.Id) == 0)
                    return await Result<InventBundle>.FailAsync($"ID {model.Id} could not be found");
                dbContext.InventBundles.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventBundle>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventBundle>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        private async Task<bool> CheckExist(InventBundle model)
        {
            return await dbContext.InventBundles.AnyAsync(x => x.TransNo.ToLower() == model.TransNo.ToLower());
        }

        public async Task<Result<List<InventBundleDTO>>> GetAllDTOAsync()
        {
            try
            {
                var responseData = await (from item in dbContext.InventBundles
                                          where item.IsDeleted != true
                                          join location in dbContext.Locations on item.Location equals location.Id.ToString()
                                          join user in dbContext.Users on item.PersonInCharge equals user.Id.ToString() into users
                                          from user in users.DefaultIfEmpty()
                                          select new InventBundleDTO()
                                          {
                                              Id = item.Id,
                                              TransNo = item.TransNo,
                                              IsDeleted = item.IsDeleted,
                                              ProductBundleCode = item.ProductBundleCode,
                                              Qty = item.Qty,
                                              Location = item.Location,
                                              Bin = item.Bin,
                                              LocationName = location.LocationName,
                                              BinCode = item.Bin,
                                              Status = item.Status,
                                              CreateAt = item.CreateAt,
                                              UpdateAt = item.UpdateAt,
                                              TransDate = item.TransDate,
                                              UpdateOperatorId = item.UpdateOperatorId,
                                              CreateOperatorId = item.CreateOperatorId,
                                              Description = item.Description,
                                              PersonInCharge = user != null ? user.FullName : "",
                                              InventBundleLines = (from adjustmentLine in dbContext.InventBundleLines
                                                                   where adjustmentLine.TransNo == item.TransNo
                                                                   join product in dbContext.Products on adjustmentLine.ProductCode equals product.ProductCode
                                                                   join unit in dbContext.Units on product.UnitId equals unit.Id
                                                                   select new InventBundlesLineDTO
                                                                   {
                                                                       ProductCode = adjustmentLine.ProductCode,
                                                                       ActualQty = adjustmentLine.ActualQty,
                                                                       TransNo = adjustmentLine.TransNo,
                                                                       LotNo = adjustmentLine.LotNo,
                                                                       ProductName = product.ProductName,
                                                                       UnitName = unit.UnitName
                                                                   }).ToList()
                                          }).OrderByDescending(x => x.CreateAt).AsNoTracking().ToListAsync();


                var productBundles = await (from item in dbContext.ProductBundles
                                            join tenant in dbContext.TenantAuth
                                            on item.CompanyId equals tenant.TenantId
                                            select new {
                                                item.ProductBundleCode,
                                                item.CompanyId,
                                                tenant.TenantFullName,
                                            }).ToListAsync();

                var retunData = productBundles.DistinctBy(x => x.ProductBundleCode).ToList();

                foreach ( var item in responseData)
                {
                  var dataTenant =  retunData.FirstOrDefault(x => x.ProductBundleCode == item.ProductBundleCode);
                    if(dataTenant != null)
                    {
                        item.TenantName = dataTenant.TenantFullName;
                    }
                }
                return await Result<List<InventBundleDTO>>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<List<InventBundleDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundleDTO>> GetByIdDTOAsync(Guid id)
        {
            try
            {
                var responseData = new InventBundleDTO();

                var it = await dbContext.InventBundles.FindAsync(id);

                if (it == null) throw new Exception($"The Id {id} could be not found.");

                responseData.Id = it.Id;
                responseData.TransNo = it.TransNo;
                responseData.IsDeleted = it.IsDeleted;
                responseData.Location = it.Location;
                responseData.Status = it.Status;
                responseData.CreateAt = it.CreateAt;
                responseData.UpdateAt = it.UpdateAt;
                responseData.TransDate = it.TransDate;
                responseData.UpdateOperatorId = it.UpdateOperatorId;
                responseData.CreateOperatorId = it.CreateOperatorId;
                responseData.Description = it.Description;
                responseData.PersonInCharge = it.PersonInCharge;
                responseData.InventBundleLines = await dbContext.InventBundleLines.Where(x => x.TransNo == it.TransNo)
                    .Select(x => new InventBundlesLineDTO()
                    {
                        ProductCode = x.ProductCode,
                        ActualQty = x.ActualQty,
                        TransNo = x.TransNo,
                        LotNo = x.LotNo,
                    })
                    .ToListAsync();

                return await Result<InventBundleDTO>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<InventBundleDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventBundleDTO>> GetByTransNoDTOAsync([Path] string TransNo)
        {
            try
            {
                // 1. Fetch the InventBundle
                var inventBundle = await dbContext.InventBundles
                    .Where(x => x.TransNo == TransNo)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (inventBundle == null)
                {
                    throw new Exception($"The transferNo {TransNo} could be not found.");
                }

                // 2. Fetch the related InventBundleLines
                var inventBundleLines = await dbContext.InventBundleLines
                    .Where(l => l.TransNo == TransNo)
                    .Select(l => new InventBundlesLineDTO
                    {
                        Id = l.Id,
                        ProductCode = l.ProductCode,
                        ActualQty = l.ActualQty,
                        TransNo = l.TransNo,
                        LotNo = l.LotNo,
                        
                        // Fetch ProductName and UnitName using separate queries
                        ProductName = dbContext.Products.Where(p => p.ProductCode == l.ProductCode).Select(p => p.ProductName).FirstOrDefault(),
                        UnitName = dbContext.Units.Where(u => u.Id == dbContext.Products.Where(p => p.ProductCode == l.ProductCode).Select(p => p.UnitId).FirstOrDefault()).Select(u => u.UnitName).FirstOrDefault()
                    })
                    .AsNoTracking()
                    .ToListAsync();

                // 3. Create and populate the InventBundleDTO
                var responseData = new InventBundleDTO
                {
                    Id = inventBundle.Id,
                    ProductBundleCode = inventBundle.ProductBundleCode,
                    TransNo = inventBundle.TransNo,
                    IsDeleted = inventBundle.IsDeleted,
                    Location = inventBundle.Location,
                    Bin = inventBundle.Bin,
                    Status = inventBundle.Status,
                    CreateAt = inventBundle.CreateAt,
                    UpdateAt = inventBundle.UpdateAt,
                    TransDate = inventBundle.TransDate,
                    UpdateOperatorId = inventBundle.UpdateOperatorId,
                    CreateOperatorId = inventBundle.CreateOperatorId,
                    Description = inventBundle.Description,
                    PersonInCharge = inventBundle.PersonInCharge,
                    LotNo = inventBundle.LotNo,
                    Qty = inventBundle.Qty,
                    ExpirationDate = inventBundle.ExpirationDate,
                    InventBundleLines = inventBundleLines,
                    TenantId = inventBundle.TenantId, 
                    ProductName = await dbContext.Products.Where(_ => inventBundle.ProductBundleCode == _.ProductCode && inventBundle.TenantId == _.CompanyId).Select(_ => _.ProductName).FirstOrDefaultAsync(),
                };

                return await Result<InventBundleDTO>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<InventBundleDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }



        public async Task<Result<List<InventBundlesLineDTO>>> GetAllMasterByBundleCode([Path] string ProductBundleCode)
        {
            try
            {
                // 1. Fetch the necessary data from ProductBundles, Products, and WarehouseTrans
                var initialQuery = await (from pb in dbContext.ProductBundles
                                          join p in dbContext.Products on new { pb.ProductCode, pb.CompanyId } equals new { p.ProductCode, p.CompanyId }
                                          join wt in dbContext.WarehouseTrans on new { pb.ProductCode, TenantId = (int?)pb.CompanyId } equals new { wt.ProductCode, wt.TenantId }
                                          join lo in dbContext.Locations on wt.Location equals lo.Id.ToString()
                                          where pb.ProductBundleCode == ProductBundleCode && pb.Quantity > 0
                                          select new { wt, pb, p, lo })
                                         .ToListAsync();

                // 2. Group and aggregate the data
                var groupedData = initialQuery
                    .GroupBy(x => new { x.wt.TenantId, x.wt.ProductCode, x.wt.Location, x.lo.LocationName, x.wt.Bin, x.wt.LotNo, x.p.ProductName, x.pb.Quantity })
                    .Select(g => new
                    {
                        g.Key.TenantId,
                        g.Key.ProductCode,
                        g.Key.Location,
                        g.Key.LocationName,
                        g.Key.Bin,
                        g.Key.LotNo,
                        g.Key.ProductName,
                        g.Key.Quantity,
                        StockAvailable = g.Sum(x => x.wt.Qty) // Sum Qty instead of Count
                    }).Where(x => x.StockAvailable > 0)
                    .ToList();

                // 3. Fetch the ExpirationDate from Batches for each group
                var result = new List<InventBundlesLineDTO>();
                foreach (var group in groupedData)
                {
                    var expirationDate = await dbContext.Batches
                        .Where(b => b.LotNo == group.LotNo && b.TenantId == group.TenantId && b.ProductCode == group.ProductCode)
                        .Select(b => b.ExpirationDate)
                        .FirstOrDefaultAsync();

                    result.Add(new InventBundlesLineDTO
                    {
                        TenantId = group.TenantId ?? 0,
                        ProductCode = group.ProductCode,
                        Location = group.Location,
                        LocationName = group.LocationName,
                        Bin = group.Bin,
                        LotNo = group.LotNo,
                        ExpirationDate = expirationDate.HasValue ? expirationDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                        ProductQuantity = group.Quantity,
                        ProductName = group.ProductName,
                        StockAvailable = group.StockAvailable
                    });
                }

                // 4. Order the results
                result = result.OrderBy(x => x.ProductCode)
                               .ThenBy(x => x.Bin)
                               .ThenBy(x => x.LotNo)
                               .ThenBy(x => x.ExpirationDate)
                               .ToList();

                return await Result<List<InventBundlesLineDTO>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<List<InventBundlesLineDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> CompletedInventBundleAsync([Body] Guid id)
        {
            try
            {
                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");

                //update status
                var InventBundle = await dbContext.InventBundles.FirstOrDefaultAsync(x => x.Id == id);
                if (InventBundle == null)
                    return await Result<bool>.FailAsync("TransNotExist");

                InventBundle.Status = EnumStatusBundle.Completed;
                InventBundle.UpdateAt = DateTime.Now;
                InventBundle.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;
                dbContext.Update(InventBundle);

                var InventBundleLines = await dbContext.InventBundleLines.Where(x => x.TransNo == InventBundle.TransNo).ToListAsync();
                InventBundleLines.ForEach(x =>
                {
                    x.Status = EnumStatus.Activated;
                });
                dbContext.UpdateRange(InventBundleLines);
                //create warehouse trans
                var warehouseTrans = InventBundleLines.Where(x => x.ActualQty < 0).Select(line => new WarehouseTran
                {
                    Id = line.Id,
                    ProductCode = line.ProductCode,
                    Qty = line.ActualQty ?? 0,
                    Location = InventBundle.Location,
                    // TenantId = InventBundle.TenantId,
                    Bin = String.IsNullOrWhiteSpace(InventBundle.Bin) ? "N/A" : InventBundle.Bin,
                    LotNo =  String.IsNullOrWhiteSpace(line.LotNo) ? "N/A" : line.LotNo,
                    DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                    TransType = EnumWarehouseTransType.Adjustment,
                    TransNumber = InventBundle.TransNo,
                    TransId = InventBundle.Id,
                    TransLineId = line.Id,
                    StatusReceipt = null,
                    StatusIssue = EnumStatusIssue.Delivered,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value
                }).ToList();
                var warehouseTransReceipt = InventBundleLines.Where(x => x.ActualQty > 0).Select(line => new WarehouseTran
                {
                    ProductCode = line.ProductCode,
                    Qty = line.ActualQty ?? 0,
                    Location = InventBundle.Location,
                    //TenantId = InventBundle.TenantId,
                    Bin = String.IsNullOrWhiteSpace(InventBundle.Bin) ? "N/A" : InventBundle.Bin,
                    LotNo = String.IsNullOrWhiteSpace(line.LotNo) ? "N/A" : line.LotNo,
                    DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                    TransType = EnumWarehouseTransType.Adjustment,
                    TransNumber = InventBundle.TransNo,
                    TransId = InventBundle.Id,
                    TransLineId = line.Id,
                    StatusReceipt = EnumStatusReceipt.Received,
                    StatusIssue = null,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value
                }).ToList();

                await dbContext.AddRangeAsync(warehouseTrans);
                await dbContext.AddRangeAsync(warehouseTransReceipt);
                await dbContext.SaveChangesAsync();
                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> CreatePutawayAsync([Path] Guid id)
        {
            var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");


            //Update status of bundle item
            var bundleIem = await dbContext.InventBundles.FirstAsync(a => a.Id == id);


            //delete old putaway if exists
            var oldputaway = await dbContext.WarehousePutAways.FirstOrDefaultAsync(x => x.DocumentNo == bundleIem.TransNo);
            if (oldputaway != null)
            {
                var oldputawayLine = await dbContext.WarehousePutAwayLines.Where(x => x.PutAwayNo == oldputaway.PutAwayNo).ToListAsync();
                if(oldputawayLine != null && oldputawayLine.Count > 0)
                {
                    dbContext.WarehousePutAwayLines.RemoveRange(oldputawayLine);
                    dbContext.WarehousePutAways.RemoveRange(oldputaway);
                    await dbContext.SaveChangesAsync();
                }
            }

            var productDetails = await dbContext.ProductBundles.FirstAsync(x => x.ProductBundleCode == bundleIem.ProductBundleCode && x.CompanyId == bundleIem.TenantId);
            bundleIem.Status = EnumStatusBundle.Putaway;

            var dataProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.ProductCode == bundleIem.ProductBundleCode && x.CompanyId == bundleIem.TenantId);
            dbContext.InventBundles.Update(bundleIem);

            try
            {
                await dbContext.WarehouseTrans.AddAsync(new WarehouseTran()
                {
                    ProductCode = bundleIem.ProductBundleCode,
                    Qty = bundleIem.Qty ?? 0,
                    Bin = String.IsNullOrWhiteSpace(bundleIem.LotNo) ? "N/A" : bundleIem.Bin,
                    Location = bundleIem.Location,
                    LotNo = String.IsNullOrWhiteSpace(bundleIem.LotNo) ? "N/A" : bundleIem.LotNo,
                    DatePhysical = bundleIem.TransDate.HasValue ? DateOnly.FromDateTime(bundleIem.TransDate.Value) : null,
                    TransNumber = bundleIem.TransNo,
                    StatusReceipt = EnumStatusReceipt.Received,
                    TenantId = bundleIem.TenantId,
                    CreateOperatorId = userInfo?.Value,
                    TransType = EnumWarehouseTransType.Bundle
                });

                var sequenceAdjustment = await dbContext.SequencesNumber.FirstAsync(x => x.JournalType == "Putaway");

                var putawayNo = $"{sequenceAdjustment.Prefix}{sequenceAdjustment.CurrentSequenceNo.ToString().PadLeft((int)sequenceAdjustment.SequenceLength, '0')}";

                sequenceAdjustment.CurrentSequenceNo++;
                await dbContext.WarehousePutAways.AddAsync(new WarehousePutAway()
                {
                    PutAwayNo = putawayNo,
                    CreateAt = DateTime.UtcNow,
                    Location = bundleIem.Location,
                    ReceiptNo = bundleIem.TransNo,
                    DocumentDate = bundleIem.TransDate.HasValue ? DateOnly.FromDateTime(bundleIem.TransDate.Value) : null,
                    DocumentNo = bundleIem.TransNo,
                    TenantId = bundleIem.TenantId,
                    TransDate = bundleIem.TransDate.HasValue ? DateOnly.FromDateTime(bundleIem.TransDate.Value) : null,
                    CreateOperatorId = userInfo?.Value,
                    ReferenceType = EnumWarehouseTransType.Bundle,
                    ReferenceNo = bundleIem.TransNo,
                });

                await dbContext.WarehousePutAwayLines.AddAsync(new WarehousePutAwayLine()
                {
                    PutAwayNo = putawayNo,
                    Bin = bundleIem.Bin,
                    ProductCode = bundleIem.ProductBundleCode,
                    LotNo = bundleIem.LotNo,
                    ExpirationDate = bundleIem.ExpirationDate.HasValue ? DateOnly.FromDateTime(bundleIem.ExpirationDate.Value) : null,
                    JournalQty = bundleIem.Qty,
                    Status = EnumStatus.Inactivated,
                    UnitId = dataProduct?.UnitId,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value,
                    TransQty = bundleIem.Qty,
                    TenantId = bundleIem.TenantId,
                    
                });

                await dbContext.SaveChangesAsync();

                var putawayInfor = dbContext.WarehousePutAways.First(x => x.PutAwayNo == putawayNo);

                var allBundleLine = await dbContext.InventBundleLines.Where(x => x.TransNo == bundleIem.TransNo)
                .ToListAsync();
                foreach (var line in allBundleLine)
                {

                    await dbContext.WarehouseTrans.AddAsync(new WarehouseTran()
                    {
                        ProductCode = line.ProductCode,
                        Qty = -1 * (line.ActualQty ?? 0),
                        Bin = String.IsNullOrWhiteSpace(line.Bin) ? "N/A" : line.Bin,
                        Location = line.Location,
                        LotNo = String.IsNullOrWhiteSpace(line.LotNo) ? "N/A" : line.LotNo,
                        DatePhysical = bundleIem.TransDate.HasValue ? DateOnly.FromDateTime(bundleIem.TransDate.Value) : null,
                        TransNumber = bundleIem.TransNo,
                        StatusIssue = EnumStatusIssue.Delivered,
                        TenantId = productDetails.CompanyId,
                        CreateOperatorId = userInfo?.Value,
                        TransType = EnumWarehouseTransType.Bundle

                    });


                }

                var count = await dbContext.SaveChangesAsync();
                return await Result<bool>.SuccessAsync(count > 0);
            }
            catch (Exception c)
            {
                throw c;
            }
        }

        private bool IsBundleValid(InventBundleDTO item)
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
                    throw new Exception("Product {Product} not meet the demand quality".Replace("{Product}", indexValidLine.ProductCode));
                }
            }
            return true;
        }

        public async Task<Result<bool>> UploadFromHandheldAsync([Body] InventBundleDTO InventBundle)
        {
            try
            {
                IsBundleValid(InventBundle);
                // find list inventory bundle line by inventoty bundle id
                if (InventBundle.Id == Guid.Empty && string.IsNullOrWhiteSpace(InventBundle.TransNo))
                    throw new Exception("Invalid Id or TransNo");
                InventBundle bundleData = null;
                if (InventBundle.Id != Guid.Empty)
                {
                    bundleData = await dbContext.InventBundles.FirstAsync(x => x.Id == InventBundle.Id);
                }
                else
                {
                    bundleData = await dbContext.InventBundles.FirstAsync(x => x.TransNo == InventBundle.TransNo);
                }
                var linesToDelete = await dbContext.InventBundleLines
                    .Where(x => x.TransNo == bundleData.TransNo)
                    .ToListAsync();
                dbContext.InventBundleLines.RemoveRange(linesToDelete);
                await dbContext.SaveChangesAsync();
                var productDetails = await dbContext.ProductBundles.FirstAsync(x => x.ProductBundleCode == bundleData.ProductBundleCode);

                var productData = await dbContext.Products.FirstAsync(x => x.ProductCode == bundleData.ProductBundleCode);


                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");
                var allBundleLines = InventBundle.InventBundleLines.Select(x => new InventBundleLine()
                {
                    ActualQty = x.ActualQty,
                    IsDeleted = false,
                    Bin = x.Bin,
                    CreateAt = x.CreateAt,
                    DemandQty = x.DemandQty,
                    ExpirationDate = x.ExpirationDate,
                    LotNo = x.LotNo,
                    Location = x.Location,
                    ProductCode = x.ProductCode,
                    Status = x.Status,
                    TransNo = x.TransNo,
                    UnitId = productData.UnitId,
                });
                await dbContext.InventBundleLines.AddRangeAsync(allBundleLines);
                await dbContext.SaveChangesAsync();
                return await CreatePutawayAsync(bundleData.Id);
            }catch( Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> DeleteAllBundleLineAsync([Path] Guid id)
        {
            try
            {
                var bundleInfo = await dbContext.InventBundles.FirstAsync(x => x.Id == id);
                var bundleLines = await dbContext.InventBundleLines.Where(x => x.TransNo == bundleInfo.TransNo).ToListAsync();

                if (bundleLines != null && bundleLines.Count > 0)
                {
                    dbContext.InventBundleLines.RemoveRange(bundleLines);
                }
                var count = await dbContext.SaveChangesAsync();
                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");

            }
        }
    }
}