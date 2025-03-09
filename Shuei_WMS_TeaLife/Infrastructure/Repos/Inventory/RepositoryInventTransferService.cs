using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Application.Services.Outbound;
using Mapster;
using Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using RestEase;
using FBT.ShareModels.WMS;
using DocumentFormat.OpenXml.EMMA;
using Azure.Core;


namespace Infrastructure.Repos.Inventory
{
    public class RepositoryInventTransferService(ApplicationDbContext dbContext, INumberSequences numberSequences, IWarehouseTran warehouseTran, IHttpContextAccessor contextAccessor) : IInventTransfer
    {
        public async Task<Result<List<InventTransfer>>> AddRangeAsync([Body] List<InventTransfer> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;

                    if (await CheckExist(item)) return await Result<List<InventTransfer>>.FailAsync($"TransferNo {item.TransferNo} Is Existed");
                }

                await dbContext.InventTransfers.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<InventTransfer>>.SuccessAsync(model, "Add range InvenTransfer successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<InventTransfer>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfer>> DeleteRangeAsync([Body] List<InventTransfer> model)
        {
            try
            {
                dbContext.InventTransfers.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventTransfer>.SuccessAsync("Delete range InvenTransfer successfull");
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfer>> DeleteAsync([Body] InventTransfer model)
        {
            try
            {
                dbContext.InventTransfers.Remove(model);
                dbContext.InventTransferLines.Where(x => x.TransferNo == model.TransferNo).ExecuteDelete();
                await dbContext.SaveChangesAsync();
                return await Result<InventTransfer>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<InventTransfer>>> GetAllAsync()
        {
            try
            {
                return await Result<List<InventTransfer>>.SuccessAsync(await dbContext.InventTransfers.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<InventTransfer>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfer>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<InventTransfer>.SuccessAsync(await dbContext.InventTransfers.Where(_ => _.Id == id).FirstOrDefaultAsync());
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfer>> InsertAsync([Body] InventTransfer model)
        {
            try
            {
                // Check if the TransferNo already exists
                if (await CheckExist(model))
                    return await Result<InventTransfer>.FailAsync($"TransferNo {model.TransferNo} Is Existed");
                // Add the new InventTransfer to the database
                await dbContext.InventTransfers.AddAsync(model);

                await dbContext.SaveChangesAsync();

                // Update the number sequences



                return await Result<InventTransfer>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfer>> UpdateAsync([Body] InventTransfer model)
        {
            try
            {
                //var checkCondition = dbContext.InventTransfers.Where(_ => _.Id == model.Id).FirstOrDefault();
                //if (checkCondition == null)
                //    return await Result<InventTransfer>.FailAsync($"ID {model.Id} could not be found");
                dbContext.InventTransfers.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventTransfer>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfer>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        private async Task<bool> CheckExist(InventTransfer model)
        {
            return await dbContext.InventTransfers.AnyAsync(x => x.TransferNo.ToLower() == model.TransferNo.ToLower());
        }

        public async Task<Result<List<InventTransfersDTO>>> GetAllDTO()
        {
            try
            {
                var responseData = new List<InventTransfersDTO>();

                var it = await dbContext.InventTransfers
                    .ToListAsync();
                var locations = await dbContext.Locations.Where(x => it.Select(xx => Guid.Parse(xx.Location)).Contains(x.Id)).ToListAsync();
                var PersonInCharges = await dbContext.Users.Where(x => it.Select(xxx => (xxx.PersonInCharge)).Contains(x.Id)).ToListAsync();
                foreach (var item in it)
                {
                    var locationInfo = locations.FirstOrDefault(x => x.Id.ToString() == item.Location);
                    var PersonInChargeInfo = PersonInCharges.FirstOrDefault(x => x.Id.ToString() == item.PersonInCharge);
                    var tenant = await dbContext.Companies.FirstOrDefaultAsync(x => x.AuthPTenantId == item.TenantId);
                    responseData.Add(new InventTransfersDTO()
                    {

                        Id = item.Id,
                        TransferNo = item.TransferNo,
                        IsDeleted = item.IsDeleted,
                        Location = locationInfo?.LocationName,
                        Status = item.Status,
                        CreateAt = item.CreateAt,
                        UpdateAt = item.UpdateAt,
                        TransferDate = item.TransferDate,
                        UpdateOperatorId = item.UpdateOperatorId,
                        CreateOperatorId = item.CreateOperatorId,
                        TenantId = item.TenantId,
                        Description = item.Description,
                        PersonInCharge = PersonInChargeInfo?.FullName,
                        //       InventTransferLines = await dbContext.InventTransferLines.Where(x => x.InventTransferId == item.Id).ToListAsync(),
                        TenantName=tenant?.FullName
                    });
                }

                return await Result<List<InventTransfersDTO>>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<List<InventTransfersDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfersDTO>> GetByIdDTO(Guid id)
        {
            try
            {
                // var responseData = new InventTransfersDTO();

                var it = await dbContext.InventTransfers.FindAsync(id);

                var responseData = new InventTransfersDTO();

                if (it == null) throw new Exception($"The Id {id} could be not found.");

                responseData.Id = it.Id;
                responseData.TransferNo = it.TransferNo;
                responseData.IsDeleted = it.IsDeleted;
                responseData.Location = it.Location;
                responseData.Status = it.Status;
                responseData.CreateAt = it.CreateAt;
                responseData.UpdateAt = it.UpdateAt;
                responseData.TransferDate = it.TransferDate;
                responseData.UpdateOperatorId = it.UpdateOperatorId;
                responseData.CreateOperatorId = it.CreateOperatorId;
                responseData.TenantId = it.TenantId;
                responseData.Description = it.Description;
                responseData.PersonInCharge = it.PersonInCharge;
                responseData.InventTransferLines = (await dbContext.InventTransferLines
                  .Where(x => x.TransferNo == responseData.TransferNo)
                  .ToListAsync())
                  .Adapt<List<InventTransfersLineDTO>>();

                return await Result<InventTransfersDTO>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfersDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfersDTO>> GetByTransferNoDTO([Path] string transferNo)
        {
            try
            {
                var responseData = new InventTransfersDTO();
                var it = await dbContext.InventTransfers.Where(x => x.TransferNo == transferNo).FirstOrDefaultAsync();
                if (it == null) throw new Exception($"The transferNo {transferNo} could be not found.");

                responseData.Id = it.Id;
                responseData.TransferNo = it.TransferNo;
                responseData.IsDeleted = it.IsDeleted;
                responseData.Location = it.Location;
                responseData.Status = it.Status;
                responseData.CreateAt = it.CreateAt;
                responseData.UpdateAt = it.UpdateAt;
                responseData.TransferDate = it.TransferDate;
                responseData.UpdateOperatorId = it.UpdateOperatorId;
                responseData.CreateOperatorId = it.CreateOperatorId;
                responseData.TenantId = it.TenantId;
                responseData.Description = it.Description;
                responseData.PersonInCharge = it.PersonInCharge;
                var inventTransferLines = dbContext.InventTransferLines.Where(_ => _.TransferNo == responseData.TransferNo);
                if (inventTransferLines.Count() > 0)
                {
                    responseData.InventTransferLines = (await dbContext.InventTransferLines.Where(_ => _.TransferNo == responseData.TransferNo)
                    .Join(dbContext.Products.Where(_ => _.IsDeleted == false), x => x.ProductCode, y => y.ProductCode, (x, y) => new { x, y }).DefaultIfEmpty()
                    .Join(dbContext.Units.Where(_ => _.IsDeleted == false), xy => xy.y.UnitId, z => z.Id, (xy, z) => new { xy, z })
                    .Select(_ => new InventTransfersLineDTO(_.xy.x, _.xy.y, _.z))
                    .ToListAsync());
                    var prodCodes = responseData.InventTransferLines.Select(x => x.ProductCode).ToList();
                    var prodInfo = dbContext.Products.Where(_ => prodCodes.Contains(_.ProductCode))
                        .Join(dbContext.Units, x => x.UnitId, y => y.Id, (x, y) => new { x, y })
                        .Select(_ => new ProductDto
                        {
                            Id = _.x.Id,
                            ProductCode = _.x.ProductCode,
                            ProductName = _.x.ProductName,
                            ProductStatus = _.x.ProductStatus,
                            UnitId = _.x.UnitId,
                            UnitName = _.y.UnitName,
                        }).AsEnumerable();

                    var productCodes = new List<ProductCodeSearch>();
                    prodCodes.ForEach(p => productCodes.Add(new ProductCodeSearch { ProductCode = p }));
                    var modelProduct = new InventoryInfoBinLotSearchRequestDTO { CompanyId = 0, ProductCodes = productCodes };
                    var prodInfoStock = await warehouseTran.GetInventoryInfoFlowBinLotFlowModelSearchAsync(modelProduct);

                    // Mapping data from product
                    if (prodInfoStock.Data != null)
                    {
                        responseData.InventTransferLines.ForEach(item =>
                        {
                            var prod = prodInfo.FirstOrDefault(x => x.ProductCode == item.ProductCode);
                            var prodStock = prodInfoStock.Data.FirstOrDefault(x => x.ProductCode == item.ProductCode);

                            if (prod != null)
                            {
                                item.ProductName = prod.ProductName;
                                item.UnitId = prod.UnitId;
                                item.UnitName = prod.UnitName;
                                if (prodStock != null)
                                {
                                    var stockDetail = prodStock.Details.FirstOrDefault(x => x.BinCode == item.FromBin && x.LotNo == item.FromLotNo);
                                    item.StockAvailable = stockDetail == null ? 0 : (int)stockDetail.InventoryStock;
                                    item.AvailableQuantity = stockDetail == null ? 0 : (int)stockDetail.AvailableStock;

                                }
                            }
                        });
                    }
                }
                return await Result<InventTransfersDTO>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfersDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransfersDTO>> ChangeStatusToCompletedAsync(Guid id)
        {
            try
            {
                var transfer = await dbContext.InventTransfers.FindAsync(id);
                if (transfer == null)
                {
                    return await Result<InventTransfersDTO>.FailAsync($"Transfer with ID {id} not found.");
                }

                transfer.Status = EnumInvenTransferStatus.Completed;
                dbContext.InventTransfers.Update(transfer);
                await dbContext.SaveChangesAsync();

                var responseData = transfer.Adapt<InventTransfersDTO>();
                return await Result<InventTransfersDTO>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<InventTransfersDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> CompletedInventTransfer(Guid id)
        {
            try
            {
                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");

                //update status
                var inventTransfer = await dbContext.InventTransfers.FirstOrDefaultAsync(x => x.Id == id);

                var lines = dbContext.InventTransferLines.Where(x => x.TransferNo == inventTransfer.TransferNo && (String.IsNullOrEmpty(x.FromBin) || String.IsNullOrEmpty(x.ToBin)));
                if (lines.Count() > 0)
                {
                    return await Result<bool>.FailAsync("RequiredBin");
                }
                lines = dbContext.InventTransferLines.Where(x => x.TransferNo == inventTransfer.TransferNo && (String.IsNullOrEmpty(x.FromLotNo)));
                if (lines.Count() > 0)
                {
                    return await Result<bool>.FailAsync("RequiredFromLot");
                }
                var products = await dbContext.Products.Join(dbContext.InventTransferLines.Where(x => x.TransferNo == inventTransfer.TransferNo), x => x.ProductCode, y => y.ProductCode, (x, y) => x).ToListAsync();

                if (inventTransfer == null)
                    return await Result<bool>.FailAsync("TransferNotExist");

                inventTransfer.Status = EnumInvenTransferStatus.Completed;
                inventTransfer.UpdateAt = DateTime.Now;
                inventTransfer.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;
                dbContext.Update(inventTransfer);

                var inventTransferLines = await dbContext.InventTransferLines.Where(x => x.TransferNo == inventTransfer.TransferNo).ToListAsync();
                inventTransferLines.ForEach(x =>
                {
                    x.Status = EnumInvenTransferStatus.Completed;
                    x.UpdateAt = DateTime.Now;
                    x.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;
                });
                dbContext.UpdateRange(inventTransferLines);

                //create warehouse trans
                var warehouseTrans = inventTransferLines.Select(line => new WarehouseTran
                {
                    ProductCode = line.ProductCode,
                    Qty = -(line.Qty ?? 0),
                    Location = inventTransfer.Location,
                    TenantId = inventTransfer.TenantId,
                    Bin = line.FromBin,
                    LotNo = line.FromLotNo,
                    DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                    TransType = EnumWarehouseTransType.Transfer,
                    TransNumber = inventTransfer.TransferNo,
                    TransId = inventTransfer.Id,
                    TransLineId = line.Id,
                    StatusReceipt = null,
                    StatusIssue = EnumStatusIssue.Delivered,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value
                }).Where(x => x.Qty != 0).ToList();
                var warehouseTransReceipt = inventTransferLines.Select(line => new WarehouseTran
                {
                    ProductCode = line.ProductCode,
                    Qty = line.Qty ?? 0,
                    Location = inventTransfer.Location,
                    TenantId = inventTransfer.TenantId,
                    Bin = String.IsNullOrWhiteSpace(line.ToBin) ? "N/A" : line.ToBin,
                    LotNo = String.IsNullOrWhiteSpace(line.ToLotNo) ? line.FromLotNo : line.ToLotNo,
                    DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                    TransType = EnumWarehouseTransType.Transfer,
                    TransNumber = inventTransfer.TransferNo,
                    TransId = inventTransfer.Id,
                    TransLineId = line.Id,
                    StatusReceipt = EnumStatusReceipt.Received,
                    StatusIssue = null,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value
                }).Where(x => x.Qty != 0).ToList();

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
    }
}
