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
using WebUIFinal.Core;

namespace Infrastructure.Repos
{
    public class RepositoryInventAdjustmentService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IInventAdjustment
    {
        public async Task<Result<List<InventAdjustment>>> AddRangeAsync([Body] List<InventAdjustment> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;

                    if (await CheckExist(item)) return await Result<List<InventAdjustment>>.FailAsync($"AdjustmentNo {item.AdjustmentNo} Is Existed");
                }

                await dbContext.InventAdjustments.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<InventAdjustment>>.SuccessAsync(model, "Add range InvenAdjustment successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<InventAdjustment>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustment>> DeleteRangeAsync([Body] List<InventAdjustment> model)
        {
            try
            {
                dbContext.InventAdjustments.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventAdjustment>.SuccessAsync("Delete range InvenAdjustment successfull");
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustment>> DeleteAsync([Body] InventAdjustment model)
        {
            try
            {
                dbContext.InventAdjustments.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventAdjustment>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<InventAdjustment>>> GetAllAsync()
        {
            try
            {
                return await Result<List<InventAdjustment>>.SuccessAsync(await dbContext.InventAdjustments.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<InventAdjustment>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustment>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<InventAdjustment>.SuccessAsync(await dbContext.InventAdjustments.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustment>> InsertAsync([Body] InventAdjustment model)
        {
            try
            {
                //check required
                if (await CheckExist(model))
                    return await Result<InventAdjustment>.FailAsync($"AdjustmentNo {model.AdjustmentNo} Is Existed");
                await dbContext.InventAdjustments.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventAdjustment>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustment>> UpdateAsync([Body] InventAdjustment model)
        {
            try
            {
                //check required
                if (await dbContext.InventAdjustments.CountAsync(x => x.Id == model.Id) == 0)
                    return await Result<InventAdjustment>.FailAsync($"ID {model.Id} could not be found");
                dbContext.InventAdjustments.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventAdjustment>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        private async Task<bool> CheckExist(InventAdjustment model)
        {
            return await dbContext.InventAdjustments.AnyAsync(x => x.AdjustmentNo.ToLower() == model.AdjustmentNo.ToLower());
        }

        public async Task<Result<List<InventAdjustmentDTO>>> GetAllDTOAsync()
        {
            try
            {
                var responseData = await (from item in dbContext.InventAdjustments
                                          where item.IsDeleted  != true
                                          join location in dbContext.Locations on item.Location equals location.Id.ToString()
                                          join bin in dbContext.Bins on item.Bin equals bin.Id.ToString()
                                          join user in dbContext.Users on item.PersonInCharge equals user.Id.ToString() into users
                                          from user in users.DefaultIfEmpty() 
                                          select new InventAdjustmentDTO()
                                          {
                                              Id = item.Id,
                                              AdjustmentNo = item.AdjustmentNo,
                                              IsDeleted = item.IsDeleted,
                                              Location = item.Location,
                                              Bin = item.Bin,
                                              LocationName = location.LocationName,
                                              BinCode = bin.BinCode,
                                              Status = item.Status,
                                              CreateAt = item.CreateAt,
                                              UpdateAt = item.UpdateAt,
                                              AdjustmentDate = item.AdjustmentDate,
                                              UpdateOperatorId = item.UpdateOperatorId,
                                              CreateOperatorId = item.CreateOperatorId,
                                              TenantId = item.TenantId,
                                              Description = item.Description,
                                              PersonInCharge = user != null ? user.FullName : "",
                                              InventAdjustmentLines = (from adjustmentLine in dbContext.InventAdjustmentLines
                                                                       where adjustmentLine.AdjustmentNo == item.AdjustmentNo
                                                                       join product in dbContext.Products on adjustmentLine.ProductCode equals product.ProductCode
                                                                       join unit in dbContext.Units on product.UnitId equals unit.Id
                                                                       select new InventAdjustmentsLineDTO
                                                                       {
                                                                           ProductCode = adjustmentLine.ProductCode,
                                                                           Qty = adjustmentLine.Qty,
                                                                           AdjustmentNo = adjustmentLine.AdjustmentNo,
                                                                           Reasons = adjustmentLine.Reasons,
                                                                           LotNo = adjustmentLine.LotNo,
                                                                           ProductName = product.ProductName,
                                                                           UnitName = unit.UnitName
                                                                       }).ToList()
                                          }).OrderByDescending(x => x.CreateAt).AsNoTracking().ToListAsync();

                return await Result<List<InventAdjustmentDTO>>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<List<InventAdjustmentDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustmentDTO>> GetByIdDTOAsync(Guid id)
        {
            try
            {
                var responseData = new InventAdjustmentDTO();

                var it = await dbContext.InventAdjustments.FindAsync(id);

                if (it == null) throw new Exception($"The Id {id} could be not found.");

                responseData.Id = it.Id;
                responseData.AdjustmentNo = it.AdjustmentNo;
                responseData.IsDeleted = it.IsDeleted;
                responseData.Location = it.Location;
                responseData.Status = it.Status;
                responseData.CreateAt = it.CreateAt;
                responseData.UpdateAt = it.UpdateAt;
                responseData.AdjustmentDate = it.AdjustmentDate;
                responseData.UpdateOperatorId = it.UpdateOperatorId;
                responseData.CreateOperatorId = it.CreateOperatorId;
                responseData.TenantId = it.TenantId;
                responseData.Description = it.Description;
                responseData.PersonInCharge = it.PersonInCharge;
                responseData.InventAdjustmentLines = await dbContext.InventAdjustmentLines.Where(x => x.AdjustmentNo == it.AdjustmentNo)
                    .Select(x => new InventAdjustmentsLineDTO()
                    {
                        ProductCode = x.ProductCode,
                        Qty = x.Qty,
                        AdjustmentNo = x.AdjustmentNo,
                        Reasons = x.Reasons,
                        LotNo = x.LotNo,
                    })
                    .ToListAsync();

                return await Result<InventAdjustmentDTO>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustmentDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustmentDTO>> GetByAdjustmentNoDTOAsync([Path] string adjustmentNo)
        {
            try
            {
                var responseData = new InventAdjustmentDTO();
                var it = await dbContext.InventAdjustments.Where(x=>x.AdjustmentNo==adjustmentNo).AsNoTracking().FirstOrDefaultAsync();
                if (it == null) throw new Exception($"The transferNo {adjustmentNo} could be not found.");
                responseData.Id = it.Id;
                responseData.AdjustmentNo = it.AdjustmentNo;
                responseData.IsDeleted = it.IsDeleted;
                responseData.Location = it.Location;
                responseData.Bin = it.Bin;
                responseData.Status = it.Status;
                responseData.CreateAt = it.CreateAt;
                responseData.UpdateAt = it.UpdateAt;
                responseData.AdjustmentDate = it.AdjustmentDate;
                responseData.UpdateOperatorId = it.UpdateOperatorId;
                responseData.CreateOperatorId = it.CreateOperatorId;
                responseData.TenantId = it.TenantId;
                responseData.Description = it.Description;
                responseData.PersonInCharge = it.PersonInCharge;
                responseData.TenantId = it.TenantId;

                responseData.InventAdjustmentLines = await (
                from adjustmentLine in dbContext.InventAdjustmentLines
                where adjustmentLine.AdjustmentNo == adjustmentNo
                join product in dbContext.Products on new { adjustmentLine.ProductCode, it.TenantId }  equals new { product.ProductCode, TenantId = product.CompanyId }
                join unit in dbContext.Units on product.UnitId equals unit.Id
    select new InventAdjustmentsLineDTO
    {
        Id = adjustmentLine.Id,
        ProductCode = adjustmentLine.ProductCode,
        Qty = adjustmentLine.Qty,
        AdjustmentNo = adjustmentLine.AdjustmentNo,
        Reasons = adjustmentLine.Reasons,
        LotNo = adjustmentLine.LotNo,
        ProductName = product.ProductName,
        UnitName = unit.UnitName,
        ExpirationDate = adjustmentLine.ExpirationDate
    }
).AsNoTracking().ToListAsync();


                return await Result<InventAdjustmentDTO>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustmentDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> CompletedInventAdjustmentAsync(Guid id)
        {
            try
            {
                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");

                //update status
                var inventAdjustment = await dbContext.InventAdjustments.FirstOrDefaultAsync(x => x.Id == id);
                if (inventAdjustment == null)
                    return await Result<bool>.FailAsync("AdjustmentNotExist");

                inventAdjustment.Status = EnumInventoryAdjustmentStatus.Completed;
                inventAdjustment.UpdateAt = DateTime.Now;
                inventAdjustment.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;
                dbContext.Update(inventAdjustment);

                var inventAdjustmentLines = await dbContext.InventAdjustmentLines.Where(x => x.AdjustmentNo == inventAdjustment.AdjustmentNo).ToListAsync();
                inventAdjustmentLines.ForEach(x =>
                {
                    x.Status = EnumInventoryAdjustmentStatus.Completed;
                    x.UpdateAt = DateTime.Now;
                    x.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;
                });
                dbContext.UpdateRange(inventAdjustmentLines);
                var binName = dbContext.Bins.First(x => x.Id.ToString() == inventAdjustment.Bin).BinCode;
                //create warehouse trans
                var warehouseTransIssue = inventAdjustmentLines.Where(x => x.Qty < 0).Select(line => new WarehouseTran
                {
                    Id = line.Id,
                    ProductCode = line.ProductCode,
                    Qty = line.Qty ?? 0,
                    Location = inventAdjustment.Location,
                    TenantId = inventAdjustment.TenantId,
                    Bin = String.IsNullOrWhiteSpace(binName) ? "N/A" : binName,
                    LotNo = String.IsNullOrWhiteSpace(line.LotNo) ? CommonHelpers.ParseLotno(line.ExpirationDate, inventAdjustment.AdjustmentNo) : line.LotNo,
                    DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                    TransType = EnumWarehouseTransType.Adjustment,
                    TransNumber = inventAdjustment.AdjustmentNo,
                    TransId = inventAdjustment.Id,
                    TransLineId = line.Id,
                    StatusReceipt = null,
                    StatusIssue = EnumStatusIssue.Delivered,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value
                }).ToList();
                var warehouseTransReceipt = inventAdjustmentLines.Where(x => x.Qty > 0).Select(line => new WarehouseTran
                {
                    ProductCode = line.ProductCode,
                    Qty = line.Qty ?? 0,
                    Location = inventAdjustment.Location,
                    TenantId = inventAdjustment.TenantId,
                    Bin = String.IsNullOrWhiteSpace(binName) ? "N/A" : binName,
                    LotNo = String.IsNullOrWhiteSpace(line.LotNo) ? CommonHelpers.ParseLotno(line.ExpirationDate, inventAdjustment.AdjustmentNo) : line.LotNo,
                    DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                    TransType = EnumWarehouseTransType.Adjustment,
                    TransNumber = inventAdjustment.AdjustmentNo,
                    TransId = inventAdjustment.Id,
                    TransLineId = line.Id,
                    StatusReceipt = EnumStatusReceipt.Received,
                    StatusIssue = null,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value
                }).ToList();
                var updatedBatches = new List<Batches>();
                foreach (var line in inventAdjustmentLines)
                {
                    var lotNo = String.IsNullOrWhiteSpace(line.LotNo) ? CommonHelpers.ParseLotno(line.ExpirationDate, inventAdjustment.AdjustmentNo) : line.LotNo;
                    //await dbContext.Batches.Where(x => x.TenantId == inventAdjustment.TenantId && x.ProductCode == line.ProductCode && x.LotNo == lotNo).ExecuteDeleteAsync();
                    int count  = updatedBatches.Where(x => x.TenantId == inventAdjustment.TenantId && x.ProductCode == line.ProductCode && x.LotNo == lotNo).Count();
                    if (count == 0 && line.ExpirationDate != null)
                    {
                        updatedBatches.Add(new Batches
                        {
                            Id = Guid.NewGuid(),
                            TenantId = inventAdjustment.TenantId,
                            LotNo = lotNo,
                            ProductCode = line.ProductCode,
                            ExpirationDate = line.ExpirationDate,
                            CreateAt = DateTime.Now,
                            CreateOperatorId = userInfo == null ? "" : userInfo.Value,
                            IsDeleted = false
                        });
                    }
                  
                }
                foreach (var line in updatedBatches)
                {
                    dbContext.Batches.Where(x => x.TenantId == line.TenantId && x.ProductCode == line.ProductCode && x.LotNo == line.LotNo).ExecuteDelete();
                }
                await dbContext.AddRangeAsync(warehouseTransIssue);
                await dbContext.AddRangeAsync(warehouseTransReceipt);
                await dbContext.AddRangeAsync(updatedBatches);
                await dbContext.SaveChangesAsync();
                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> DeleteAllAdjustmentLineAsync([Path] Guid id)
        {
            try
            {
                var adjustmentInfo = await dbContext.InventAdjustments.FirstAsync(x => x.Id == id);
                var adjustmentLines = await dbContext.InventAdjustmentLines.Where(x => x.AdjustmentNo == adjustmentInfo.AdjustmentNo).ToListAsync();

                if (adjustmentLines != null && adjustmentLines.Count > 0)
                {
                    dbContext.InventAdjustmentLines.RemoveRange(adjustmentLines);
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
