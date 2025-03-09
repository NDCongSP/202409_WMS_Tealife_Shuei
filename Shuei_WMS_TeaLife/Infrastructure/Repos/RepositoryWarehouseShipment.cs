using Application.DTOs;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Models;
using Application.Services;
using Application.Services.Authen;
using Application.Services.Outbound;
using AutoMapper;
using Azure.Core;
using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Presentation;
using FBT.ShareModels.Entities;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Infrastructure.Validators;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using RestEase;
using System.Linq.Dynamic.Core;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehouseShipment(ApplicationDbContext dbContext, INumberSequences numberSequences,
        IWarehouseTran warehouseTran, IHttpContextAccessor contextAccessor, IAccount account, IConfiguration configuration) : IWarehouseShipment
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<Result<List<WarehouseShipment>>> AddRangeAsync([Body] List<WarehouseShipment> model)
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

                await dbContext.WarehouseShipments.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<WarehouseShipment>>.SuccessAsync(model, "Add range WarehouseShipment successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseShipment>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> DeleteRangeAsync([Body] List<WarehouseShipment> model)
        {
            try
            {
                dbContext.WarehouseShipments.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipment>.SuccessAsync("Delete range WarehouseShipment successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> DeleteAsync([Body] WarehouseShipment model)
        {
            try
            {
                dbContext.WarehouseShipments.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipment>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseShipment>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseShipment>>.SuccessAsync(await dbContext.WarehouseShipments.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseShipment>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseShipment>.SuccessAsync(await dbContext.WarehouseShipments.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> InsertAsync([Body] WarehouseShipment model)
        {
            try
            {
                await dbContext.WarehouseShipments.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipment>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> UpdateAsync([Body] WarehouseShipment model)
        {
            try
            {
                dbContext.WarehouseShipments.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipment>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipment>> GetByMasterCodeAsync([Path] string shipmentNo)
        {
            try
            {
                return await Result<WarehouseShipment>.SuccessAsync(await dbContext.WarehouseShipments.Where(x => x.ShipmentNo == shipmentNo).FirstAsync());
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipment>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentDto>> CreateWarehouseShipmentAsync([Body] WarehouseShipmentDto model)
        {
            try
            {
                //validator
                var validator = new WareHouseShipmentValidator();
                var result = validator.Validate(model);
                if (!result.IsValid)
                {
                    return await Result<WarehouseShipmentDto>.FailAsync(String.Join(',', result.Errors));
                }
                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");
                var warehouseShipment = model.Adapt<WarehouseShipment>();
                var newShipmentNo = await numberSequences.GetNoByType(model.ShipmentType.ToString());

                warehouseShipment.ShipmentNo = newShipmentNo;

                warehouseShipment.CreateAt = DateTime.Now;
                warehouseShipment.CreateOperatorId = userInfo == null ? "" : userInfo.Value;

                await dbContext.WarehouseShipments.AddAsync(warehouseShipment);
                await dbContext.SaveChangesAsync();
                var updateSequence = await numberSequences.IncreaseNumberSequenceByType(model.ShipmentType.ToString());

                var warehouseShipmentLines = new List<WarehouseShipmentLine>();
                if (model.WareHouseShipmentLineDtos != null && model.WareHouseShipmentLineDtos.Any())
                {
                    model.WareHouseShipmentLineDtos.ForEach(line =>
                    {
                        var item = line.Adapt<WarehouseShipmentLine>();
                        item.ShipmentNo = warehouseShipment.ShipmentNo;
                        item.CreateAt = DateTime.Now;
                        item.CreateOperatorId = userInfo == null ? "" : userInfo.Value;
                        warehouseShipmentLines.Add(item);
                    });
                    await dbContext.WarehouseShipmentLines.AddRangeAsync(warehouseShipmentLines);

                    var warehouseTrans = warehouseShipmentLines.Select(line => new WarehouseTran
                    {
                        ProductCode = line.ProductCode,
                        Qty = -(line.ShipmentQty ?? 0),
                        Location = warehouseShipment.Location,
                        TenantId = warehouseShipment.TenantId,
                        Bin = line.Bin,
                        LotNo = line.LotNo,
                        TransType = EnumWarehouseTransType.Shipment,
                        TransNumber = warehouseShipment.ShipmentNo,
                        TransId = warehouseShipment.Id,
                        TransLineId = line.Id,
                        StatusReceipt = null,
                        StatusIssue = EnumStatusIssue.OnOrder,
                        CreateAt = DateTime.Now,
                        CreateOperatorId = userInfo?.Value
                    }).ToList();

                    await dbContext.AddRangeAsync(warehouseTrans);
                }
                if (!string.IsNullOrEmpty(warehouseShipment.SalesNo))
                {
                    //update order status 
                    await dbContext.Orders.Where(x => x.OrderId == warehouseShipment.SalesNo)
                        .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.OrderStatus, "30")
                                                  .SetProperty(xx => xx.UpdateOperatorId, userInfo == null ? "WMS-UpdateStatus" : $"WMS-UpdateStatus-" + userInfo.Value)
                                                  .SetProperty(xx => xx.UpdateAt, DateTime.Now));
                }
                await dbContext.SaveChangesAsync();
                model.Id = warehouseShipment.Id;
                model.ShipmentNo = warehouseShipment.ShipmentNo;
                return await Result<WarehouseShipmentDto>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentDto>> UpdateWarehouseShipmentAsync([Body] WarehouseShipmentDto model)
        {
            try
            {
                var existingShipment = await dbContext.WarehouseShipments.FindAsync(model.Id);
                if (existingShipment == null)
                {
                    return await Result<WarehouseShipmentDto>.FailAsync("ShipmentNotFound");
                }
                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");
                existingShipment.UpdateAt = DateTime.Now;
                existingShipment.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;

                model.Adapt(existingShipment);
                dbContext.WarehouseShipments.Update(existingShipment);

                if (model.WareHouseShipmentLineDtos != null && model.WareHouseShipmentLineDtos.Any())
                {
                    var existingLines = await dbContext.WarehouseShipmentLines
                        .Where(l => l.ShipmentNo == model.ShipmentNo)
                        .ToListAsync();

                    dbContext.WarehouseShipmentLines.RemoveRange(existingLines);

                    var newLines = model.WareHouseShipmentLineDtos.Select(line =>
                    {
                        var item = line.Adapt<FBT.ShareModels.WMS.WarehouseShipmentLine>();
                        item.CreateAt = DateTime.Now;
                        item.CreateOperatorId = userInfo == null ? "" : userInfo.Value;
                        item.ShipmentNo = model.ShipmentNo;
                        return item;
                    }).ToList();

                    await dbContext.WarehouseShipmentLines.AddRangeAsync(newLines);
                }

                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipmentDto>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentDto>.FailAsync($"{ex.Message}");
            }
        }

        public async Task<Result<PageList<WarehouseShipmentDto>>> SearchWhShipments([Body] QueryModel<WarehouseShipmentSearchModel> model)
        {
            try
            {
                var data = model.Entity ?? new WarehouseShipmentSearchModel();
                var query = dbContext.WarehouseShipments.AsEnumerable().Where(x => x.IsDeleted == false && x.ShipmentType == data.Type
                        && (x.ShipmentNo.Contains(data.ShipmentNo ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                        && (string.IsNullOrEmpty(data.SalesNo) || x.SalesNo == data.SalesNo)
                        && (data.EstimateShipDateFrom == default || x.PlanShipDate >= data.EstimateShipDateFrom)
                        && (data.EstimateShipDateTo == default || x.PlanShipDate <= data.EstimateShipDateTo)
                        && (data.Status == default || data.Status == EnumShipmentOrderStatus.All || x.Status == data.Status)
                        && (data.BinId == default || x.BinId == data.BinId.ToString())
                        && (data.TenantId == default || x.TenantId == data.TenantId)
                        && (data.OutboundLocationId == default || x.Location == data.OutboundLocationId.ToString())
                    ).OrderBy(x => x.ShipmentNo);
                var pagedList = PageList<WarehouseShipmentDto>.PagedResult(query.AsQueryable(), model.PageNumber, model.PageSize);
                var userInfos = dbContext.Users.Where(x => pagedList.Items.Select(x => x.CreateOperatorId).Contains(x.Id));

                foreach (var item in pagedList.Items)
                {
                    var userInfo = userInfos.FirstOrDefault(x => x.Id == item.CreateOperatorId);
                    if (userInfo != null)
                        item.CreateOperatorId = userInfo.FullName;
                }
                return await Result<PageList<WarehouseShipmentDto>>.SuccessAsync(pagedList);
            }
            catch (Exception ex)
            {
                return await Result<PageList<WarehouseShipmentDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentDto>> GetShipmentByIdAsync([Path] Guid id)
        {
            try
            {
                var info = await dbContext.WarehouseShipments.FirstOrDefaultAsync(x => x.Id == id);
                if (info == null)
                    return await Result<WarehouseShipmentDto>.FailAsync($"ShipmentIsNotExisted");
                var item = await dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == info.ShipmentNo).ToListAsync();

                var result = new WarehouseShipmentDto(info, item);
                if (result == null)
                {
                    return await Result<WarehouseShipmentDto>.FailAsync($"ShipmentIsNotExisted");
                }

                var prodCodes = item.Select(x => x.ProductCode);
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
                //mapping data from product
                result.WareHouseShipmentLineDtos.ForEach(item =>
                {
                    var prod = prodInfo.FirstOrDefault(x => x.ProductCode == item.ProductCode);
                    var prodStock = prodInfoStock.Data.FirstOrDefault(x => x.ProductCode == item.ProductCode);

                    if (prod != null)
                    {
                        item.ProductName = prod.ProductName;
                        item.UnitId = prod.UnitId;
                        item.Unit = prod.UnitName;
                        if (prodStock != null)
                        {
                            var stockDetail = prodStock.Details.FirstOrDefault(x => x.BinCode == item.Bin && x.LotNo == item.LotNo);
                            item.StockAvailable = stockDetail == null ? 0 : (int)stockDetail.InventoryStock;
                            item.AvailableQuantity = stockDetail == null ? 0 : (int)stockDetail.AvailableStock;
                            item.ExpirationDate = stockDetail == null ? default : stockDetail.Expired;
                        }
                    }
                });
                return await Result<WarehouseShipmentDto>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> ConfirmShipmentAsync([Path] Guid id)
        {
            try
            {
                var existingShipment = await dbContext.WarehouseShipments
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (existingShipment == null)
                    return await Result<bool>.FailAsync($"ShipmentIsNotExisted");
                var userInfo = contextAccessor.HttpContext?.User.FindFirst("UserId");

                existingShipment.Status = EnumShipmentOrderStatus.Open;
                existingShipment.UpdateAt = DateTime.Now;
                existingShipment.UpdateOperatorId = userInfo?.Value;
                dbContext.Update(existingShipment);
                await dbContext.SaveChangesAsync();

                var shipmentLines = dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == existingShipment.ShipmentNo).AsEnumerable();
                if (shipmentLines.Count() > 0)
                {
                    shipmentLines.ForEach(item =>
                    {
                        item.Status = EnumShipmentOrderStatus.Open;
                        item.UpdateAt = DateTime.Now;
                        item.UpdateOperatorId = userInfo?.Value;
                    });
                    dbContext.UpdateRange(shipmentLines);

                    // Add new WarehouseTrans lines
                    var warehouseTrans = shipmentLines.Select(line => new WarehouseTran
                    {
                        ProductCode = line.ProductCode,
                        Qty = -(line.ShipmentQty ?? 0),
                        Location = existingShipment.Location,
                        TenantId = existingShipment.TenantId,
                        Bin = line.Bin,
                        TransType = EnumWarehouseTransType.Shipment,
                        TransNumber = existingShipment.ShipmentNo,
                        TransId = existingShipment.Id,
                        TransLineId = line.Id,
                        StatusReceipt = null,
                        StatusIssue = EnumStatusIssue.OnOrder,
                        CreateAt = DateTime.Now,
                        CreateOperatorId = userInfo?.Value
                    }).ToList();

                    await dbContext.WarehouseTrans.AddRangeAsync(warehouseTrans);
                    await dbContext.SaveChangesAsync();
                }

                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<string>> CreatePickingManualAsync(SubmitCompletedShipmentDto data)
        {
            var result = await CreatePickingAsync(data);
            if (result.Item1 == true)
                return await Result<string>.SuccessAsync(result.Item4, "");
            return await Result<string>.FailAsync("Failed");
        }

        public async Task<CreatePickingModel> CreatePickingAsync(SubmitCompletedShipmentDto data)
        {
            try
            {
                string listShipmentId = string.Empty;

                if (data.Id.Count > 0)
                {
                    foreach (var item in data?.Id)
                    {
                        if (!string.IsNullOrEmpty(listShipmentId))
                            listShipmentId = $"{listShipmentId},{item.ToString().ToUpper()}";
                        else listShipmentId = item.ToString().ToUpper();
                    }
                }

                using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        //Return shipmentLine join some fiels from shipment  bin, tenant,...
                        var pickingData = await dbContext.Database.SqlQueryRaw<SortShipmentForPickingModel>("sp_GetDataForCreatePicking @shipmentId = {0}, @companyId= {1}", listShipmentId, data.TenantId).ToListAsync();

                        //Group base on ShipmentNo and sort flow Shipmetno and SortedNum
                        /*
                         WarehousShipments 
                        PlanShipDate { get; set; }
                        LocationName { get; set; }
                        BinJontNumber
                        SortedNum
                        List<WarehouseLines>
                         */
                        var groupsShipmentNo = pickingData?.GroupBy(x => x.ShipmentNo).Select(_ => new ShipmentForPickingGroupShipmentCodeModel()
                        {
                            ShipmentNo = _.Key,
                            ShipmentId = _.Select(u => u.ShipmentId ?? Guid.Empty).FirstOrDefault(),
                            ShippingCarrierCode = _.Select(u => u.ShippingCarrierCode).FirstOrDefault(),
                            TenantId = _.Select(u => u.TenantId).FirstOrDefault(),
                            PlanShipDate = _.Select(u => u.PlanShipDate).FirstOrDefault(),
                            LocationName = _.Select(u => u.LocationName).FirstOrDefault(),
                            BinJontNumber = _.Select(u => u.BinJontNumber).FirstOrDefault(),
                            SortedNum = _.Select(u => u.SortedNum).FirstOrDefault(),
                            ShipmentLines = _.AsList()
                        }).OrderBy(_ => _.ShipmentNo).ThenBy(_ => _.SortedNum).ToList();

                        //group flow CarrierCode and TenantId
                        //thi is the final data to go to creating picking
                        /*
                        ShippingCarrierCode
                        TenantId
                        List<ShipmentForPickingGroupShipmentCodeModel>*/
                        var masterDataToPick = groupsShipmentNo
                                               .GroupBy(x => new { x.TenantId, x.ShippingCarrierCode }) // Group by multiple columns
                                               .Select(group => new PickingDataFinal()
                                               {
                                                   TenantId = group.Key.TenantId,
                                                   ShippingCarrierCode = group.Key.ShippingCarrierCode,
                                                   Shipments = group.ToList()
                                               })
                                               .ToList();

                        if (!masterDataToPick.Any())
                            return new CreatePickingModel(true, 0, 0);

                        //Declare parametters to generate dataa to update DB
                        var userInfo = contextAccessor.HttpContext?.User.FindFirst("UserId");
                        var numberSequence = await dbContext.SequencesNumber.FirstOrDefaultAsync(x => x.JournalType == "Picking");
                        int currentPickingNo = numberSequence.CurrentSequenceNo.Value;
                        int successCount = 0;//the total of new picking has been updateted.
                        int totalCount = 0;//the total of new picking
                        var pickingNos = new List<string>();

                        var dateCreate = DateTime.Now;//get datetime for syn                                              
                        var dataUpdateShipments = new List<WarehouseShipment>();
                        var dataUpdateShipmentLines = new List<WarehouseShipmentLine>();
                        var dataInsertPickings = new List<WarehousePickingList>();
                        var dataInsertPickingLines = new List<WarehousePickingLine>();

                        var configMap = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<ShipmentForPickingGroupShipmentCodeModel, WarehouseShipmentLine>();
                        });
                        var mapper = configMap.CreateMapper();

                        //GROUP TenantId and CarrierCode
                        foreach (var itemMaster in masterDataToPick)
                        {
                            // Split into groups of max 5 shipments
                            var shipmentBatches = itemMaster.Shipments.Batch(5);
                            totalCount += shipmentBatches.Count();

                            foreach (var itemBatch in shipmentBatches)
                            {
                                string pickingNo = $"{numberSequence.Prefix}{currentPickingNo++.ToString().PadLeft(numberSequence.SequenceLength.Value, '0')}";
                                var batchList = itemBatch.ToList();
                                var shipmentNos = batchList.Select(s => s.ShipmentNo);

                                #region GENERATE models to update db: warehouseShpment and warehousePicking
                                //WarehousePickingLists
                                var pickingId = Guid.NewGuid();
                                dataInsertPickings.Add(new WarehousePickingList()
                                {
                                    Id = pickingId,
                                    PickNo = pickingNo,
                                    TenantId = batchList.FirstOrDefault().TenantId,
                                    PersonInCharge = data.Manager,
                                    CreateAt = dateCreate,
                                    CreateOperatorId = userInfo?.Value,
                                    Location = batchList.FirstOrDefault().LocationName,
                                    EstimatedShipDate = batchList.FirstOrDefault().PlanShipDate,
                                    Status = EnumShipmentOrderStatus.Picking,
                                    Remarks = data.Remarks
                                });

                                //WarehouseShipments, WarehouseShipmentLines and WarehousePickingLines
                                foreach (var itemShipment in batchList)
                                {
                                    //WarehouseShipments
                                    var shipment = await dbContext.WarehouseShipments.FirstOrDefaultAsync(x => x.Id == itemShipment.ShipmentId);
                                    shipment.Status = EnumShipmentOrderStatus.Picking;
                                    shipment.UpdateAt = dateCreate;
                                    shipment.UpdateOperatorId = userInfo?.Value;
                                    shipment.PickingNo = pickingNo;
                                    //Add dataa to model dataUpdateShipments to update BD
                                    dataUpdateShipments.Add(shipment);

                                    //WarehouseShipmentLines
                                    itemShipment.ShipmentLines.ForEach(l =>
                                    {
                                        l.Status = EnumShipmentOrderStatus.Picking;
                                        l.UpdateAt = dateCreate;
                                        l.UpdateOperatorId = userInfo?.Value;
                                    });

                                    var shipmentLine = mapper.Map<List<WarehouseShipmentLine>>(itemShipment.ShipmentLines);
                                    dataUpdateShipmentLines.AddRange(shipmentLine);

                                    //WarehousePickingLines
                                    foreach (var itemShipmentLine in itemShipment.ShipmentLines)
                                    {
                                        var pl = itemShipmentLine.Adapt<WarehousePickingLine>();
                                        pl.PickNo = pickingNo;
                                        pl.PickQty = itemShipmentLine.ShipmentQty;
                                        pl.CreateAt = dateCreate;
                                        pl.CreateOperatorId = userInfo?.Value;
                                        pl.IsDeleted = false;
                                        pl.ShipmentLineId = itemShipmentLine.Id;
                                        pl.UpdateAt = null;
                                        pl.UpdateOperatorId = null;

                                        dataInsertPickingLines.Add(pl);
                                    }

                                    dataInsertPickingLines?.ForEach(pl =>
                                    {
                                        pickingNo = pickingNo;

                                    });
                                }
                                #endregion
                            }
                        }

                        //update the sequencesNum for picking
                        await dbContext.SequencesNumber.Where(x => x.JournalType == "Picking")
                              .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.CurrentSequenceNo, currentPickingNo));
                        //------------------------------------------------------------------
                        //update DB
                        await dbContext.WarehousePickingLists.AddRangeAsync(dataInsertPickings);
                        await dbContext.WarehousePickingLines.AddRangeAsync(dataInsertPickingLines);
                        dbContext.WarehouseShipments.UpdateRange(dataUpdateShipments);
                        dbContext.WarehouseShipmentLines.UpdateRange(dataUpdateShipmentLines);
                        await dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();

                        successCount = dataInsertPickings.Count;

                        return new CreatePickingModel(true, successCount, totalCount, String.Join(',', pickingNos));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new CreatePickingModel(false, 0, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                return new CreatePickingModel(false, 0, 0);
            }
        }

        public async Task<CreatePickingModel> CreateAutoPickingAsync(string remarks, string tenantIds)
        {
            List<string> tenantList = new List<string>();
            if (!string.IsNullOrEmpty(tenantIds))
                tenantList = tenantIds.Split(',').ToList();
            if (tenantIds.Contains("*") || tenantIds.Contains("All") || string.IsNullOrEmpty(tenantIds))
                tenantList = new List<string>();
            var shipments = await dbContext.WarehouseShipments
            .Where(x => x.Status == EnumShipmentOrderStatus.Open && (tenantList.Count == 0 || tenantList.Contains(x.TenantId.ToString())))
            .Select(x => x.Id)
            .ToListAsync();
            //check manager 
            var pickingManager = await account.UserGetByEmailAsync("Pickingbatch@shuei.vn");
            if (pickingManager == null)
            {
                //create manager 
                var newManager = await account.CreateAccountAsync(new Application.DTOs.Request.Account.CreateAccountRequestDTO
                {
                    FullName = "ピックバッチ",//"Picking batch"
                    Email = "Pickingbatch@shuei.vn",
                    UserName = "Picking_Batch",
                    Password = _configuration["PickingManagerPassword"] ?? "@Tealife123$$",
                    Status = EnumStatus.Inactivated,
                });
                if (newManager.Flag)
                {
                    pickingManager = await account.UserGetByEmailAsync("Pickingbatch@shuei.vn");
                }
            }
            return await CreatePickingAsync(new SubmitCompletedShipmentDto
            {
                Id = shipments,
                Manager = pickingManager.Id,
                Remarks = remarks
            });
        }

        public async Task<Result<bool>> DeleteShipmentAsync([Path] Guid id)
        {
            try
            {
                var shipment = await dbContext.WarehouseShipments.FirstOrDefaultAsync(x => x.Id == id);
                if (shipment == null)
                    return await Result<bool>.FailAsync($"ShipmentIsNotExisted");
                dbContext.Remove(shipment);
                await dbContext.SaveChangesAsync();
                await dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == shipment.ShipmentNo)
                .ExecuteDeleteAsync();
                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> CheckMultipleShipmentCreatePicking([Path] List<Guid> ids)
        {
            if (ids.Count == 0)
                return await Result<bool>.SuccessAsync(true);
            try
            {
                var shipment = dbContext.WarehouseShipments.Where(x => ids.Contains(x.Id))
                .Select(x => new
                {
                    Tenant = x.TenantId,
                    ShippingCarrier = x.ShippingCarrierCode
                }).AsEnumerable();
                if (shipment.Select(x => x.Tenant).Distinct().Count() > 1 || shipment.Select(x => x.ShippingCarrier).Distinct().Count() > 1)
                    return await Result<bool>.SuccessAsync(true);
                return await Result<bool>.SuccessAsync(false);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<DeliveryNoteModel> GetDeliveryNote([Path] Guid id)
        {
            try
            {
                var deliveryNote = await dbContext.WarehouseShipments.Where(x => x.Id == id)
                .Select(x => new DeliveryNoteModel
                {
                    ShipmentNo = x.ShipmentNo,
                    Barcode = GlobalVariable.GenerateBarcodeBase64(x.ShipmentNo, 200, 50),
                }).FirstOrDefaultAsync();
                if (deliveryNote == null)
                    return new DeliveryNoteModel();
                deliveryNote.Items = await dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == deliveryNote.ShipmentNo)
                .Join(dbContext.Products,
                x => x.ProductCode,
                y => y.ProductCode,
                (x, y) => new ShipmentLineSummaryModel
                {
                    ProductName = y.ProductEname,
                    Quantity = (int)x.ShipmentQty,
                    PackedQuantity = (int)x.PackedQty,
                    ProductImage = "https://upload.wikimedia.org/wikipedia/commons/6/65/No-Image-Placeholder.svg",
                }).ToListAsync();
                var param = await dbContext.WarehouseParameters.FirstOrDefaultAsync(x => x.Status == 1);
                if (param != null)
                {
                    deliveryNote.QRCode = GlobalVariable.GenerateQRCode(param.CompanyWebsite);
                    deliveryNote.CompanyName = param.CompanyName;
                    deliveryNote.CompanyEmail = param.CompanyEmail;
                }
                return deliveryNote;
            }
            catch (Exception ex)
            {
                return new DeliveryNoteModel();
            }
        }

        public async Task<List<DeliveryNoteModel>> GetDeliveryNotes([Body] List<Guid> ids)
        {
            try
            {
                var deliveryNotes = await (from ws in dbContext.WarehouseShipments
                                           where ids.Contains(ws.Id)
                                           join o in dbContext.Orders
                                           on ws.SalesNo equals o.OrderId into orders
                                           from o in orders.DefaultIfEmpty()
                                           group o by new { ws.ShipmentNo, ws.TenantId, ws.SalesNo } into g
                                           select new DeliveryNoteModel
                                           {
                                               ShipmentNo = g.Key.ShipmentNo,
                                               TenantId = g.Key.TenantId,
                                               Barcode = GlobalVariable.GenerateBarcodeBase64(g.Key.ShipmentNo, 200, 50),
                                               IsSplitOrder = (from od in dbContext.OrderDispatches
                                                               where od.OrderId == g.Key.SalesNo
                                                               select od).Count() > 1,
                                               OrderData = g.FirstOrDefault()
                                           }).ToListAsync();
                if (deliveryNotes == null)
                    return new List<DeliveryNoteModel>();
                var shipmentLines = await dbContext.WarehouseShipmentLines.Where(x => deliveryNotes.Select(s => s.ShipmentNo).Contains(x.ShipmentNo))
                .Join(dbContext.Products,
                x => x.ProductCode,
                y => y.ProductCode,
                (x, y) => new ShipmentLineSummaryModel
                {
                    ShipmentNo = x.ShipmentNo,
                    ProductName = y.ProductEname,
                    Quantity = x.ShipmentQty == default ? 0 : (int)x.ShipmentQty,
                    PackedQuantity = x.PackedQty == default ? 0 : (int)x.PackedQty,
                    ProductImage = dbContext.ImageStorages
                   .Where(img => img.Type == EnumImageStorageType.Product && img.ResourcesId == y.Id.ToString() && img.IsDeleted == false)
                   .Select(img => img.ImageBase64) // Assuming ImageUrl is the property that holds the image path
                   .FirstOrDefault() ?? "",
                }).ToListAsync();
                var tenantInfos = await dbContext.Companies.Where(x => deliveryNotes.Select(s => s.TenantId).Contains(x.AuthPTenantId)).ToListAsync();

                deliveryNotes.ForEach(shipment =>
                {
                    var tenantInfo = tenantInfos.FirstOrDefault(x => x.AuthPTenantId == shipment.TenantId);
                    if (tenantInfo != null)
                    {
                        shipment.QRCode = GlobalVariable.GenerateQRCode(tenantInfo.SiteAddress);
                        shipment.Logo = tenantInfo.LogoImage;
                        shipment.ShopName = tenantInfo.ShopName;
                        shipment.DeliveryNoteMessage = tenantInfo.DeliveryNoteMessage;
                        shipment.SiteAddress = tenantInfo.SiteAddress;
                        shipment.CompanyName = tenantInfo.CompanyName;
                        shipment.CompanyEmail = tenantInfo.ContactEmailAddress;
                        shipment.SiteAddressName = tenantInfo.SiteAddressName;
                    }
                    shipment.Items = shipmentLines.Where(x => x.ShipmentNo == shipment.ShipmentNo).ToList();
                });

                return deliveryNotes;
            }
            catch (Exception ex)
            {
                return new List<DeliveryNoteModel>();
            }
        }

        public async Task<Result<bool>> CompletedShipmentAsync([Path] Guid id)
        {
            try
            {
                var existingShipment = await dbContext.WarehouseShipments
                .FirstOrDefaultAsync(x => x.Id == id && x.Status == EnumShipmentOrderStatus.Packed);
                if (existingShipment == null)
                    return await Result<bool>.FailAsync($"ShipmentIsNotExisted");
                var userInfo = contextAccessor.HttpContext?.User.FindFirst("UserId");

                existingShipment.Status = EnumShipmentOrderStatus.Completed;
                existingShipment.UpdateAt = DateTime.Now;
                existingShipment.UpdateOperatorId = userInfo?.Value;
                dbContext.Update(existingShipment);
                await dbContext.SaveChangesAsync();
                //update order to complete 
                if (!string.IsNullOrEmpty(existingShipment.SalesNo))
                {
                    await dbContext.OrderDispatches.Where(x => x.Id == existingShipment.OrderDispatchId)
                    .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.OrderDispatchStatus, "40")
                                             .SetProperty(xx => xx.UpdateOperatorId, userInfo == null ? "WMS-UpdateStatus" : $"WMS-UpdateStatus-" + userInfo.Value)
                                             .SetProperty(xx => xx.UpdateAt, DateTime.Now));
                    var allItemsCompleted = await dbContext.OrderDispatches
                    .Where(x => x.OrderId == existingShipment.SalesNo)
                    .AllAsync(x => x.OrderDispatchStatus == "40");

                    if (allItemsCompleted)
                    {
                        await dbContext.Orders.Where(x => x.OrderId == existingShipment.SalesNo)
                           .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.OrderStatus, "40")
                                                     .SetProperty(xx => xx.UpdateOperatorId, userInfo == null ? "WMS-UpdateStatus" : $"WMS-UpdateStatus-" + userInfo.Value)
                                                     .SetProperty(xx => xx.UpdateAt, DateTime.Now));
                    }
                }
                var shipmentLines = dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == existingShipment.ShipmentNo).AsEnumerable();
                if (shipmentLines.Count() > 0)
                {
                    shipmentLines.ForEach(item =>
                    {
                        item.Status = EnumShipmentOrderStatus.Completed;
                        item.UpdateAt = DateTime.Now;
                        item.UpdateOperatorId = userInfo?.Value;
                    });
                    dbContext.UpdateRange(shipmentLines);

                    // update WarehouseTrans lines
                    var warehouseTrans = await dbContext.WarehouseTrans
                    .Where(x => x.TransId == existingShipment.Id)
                    .ToListAsync();

                    foreach (var trans in warehouseTrans)
                    {
                        trans.Qty = -1 * (double)shipmentLines.First(s => s.Id == trans.TransLineId).PackedQty;
                        trans.DatePhysical = DateOnly.FromDateTime(DateTime.Now);
                        trans.StatusIssue = EnumStatusIssue.Delivered;
                        trans.UpdateAt = DateTime.Now;
                        trans.UpdateOperatorId = userInfo?.Value ?? "";
                    }

                    dbContext.UpdateRange(warehouseTrans);
                    await dbContext.SaveChangesAsync();
                }

                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result> CompleteShipmentsAsync([Body] List<string> shipmentNos)
        {
            try
            {
                List<string> errorMessages = new List<string>();
                var userInfo = contextAccessor.HttpContext?.User.FindFirst("UserId");

                var shipments = await dbContext.WarehouseShipments
                .Where(p => shipmentNos.Contains(p.ShipmentNo) && p.Status == EnumShipmentOrderStatus.Packed)
                .ToListAsync();

                foreach (var shipment in shipments)
                {
                    shipment.Status = EnumShipmentOrderStatus.Completed;
                    shipment.UpdateAt = DateTime.Now;
                    shipment.UpdateOperatorId = userInfo?.Value;
                    dbContext.Update(shipment);

                    if (!string.IsNullOrEmpty(shipment.SalesNo))
                    {
                        await dbContext.OrderDispatches.Where(x => x.Id == shipment.OrderDispatchId)
                           .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.OrderDispatchStatus, "40")
                                                     .SetProperty(xx => xx.UpdateOperatorId, userInfo == null ? "WMS-UpdateStatus" : $"WMS-UpdateStatus-" + userInfo.Value)
                                                     .SetProperty(xx => xx.UpdateAt, DateTime.Now));

                        var allItemsCompleted = await dbContext.OrderDispatches
                           .Where(x => x.OrderId == shipment.SalesNo)
                           .AllAsync(x => x.OrderDispatchStatus == "40");

                        if (allItemsCompleted)
                        {
                            await dbContext.Orders.Where(x => x.OrderId == shipment.SalesNo)
                                .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.OrderStatus, "40")
                                                          .SetProperty(xx => xx.UpdateOperatorId, userInfo == null ? "WMS-UpdateStatus" : $"WMS-UpdateStatus-" + userInfo.Value)
                                                          .SetProperty(xx => xx.UpdateAt, DateTime.Now));
                        }
                    }

                    var shipmentLines = dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == shipment.ShipmentNo).AsEnumerable();
                    if (shipmentLines.Any())
                    {
                        shipmentLines.ForEach(item =>
                        {
                            item.Status = EnumShipmentOrderStatus.Completed;
                            item.UpdateAt = DateTime.Now;
                            item.UpdateOperatorId = userInfo?.Value;
                        });
                        dbContext.UpdateRange(shipmentLines);

                        var warehouseTrans = await dbContext.WarehouseTrans
                           .Where(x => x.TransId == shipment.Id)
                           .ToListAsync();

                        foreach (var trans in warehouseTrans)
                        {
                            trans.Qty = -1 * (double)shipmentLines.First(s => s.Id == trans.TransLineId).PackedQty;
                            trans.DatePhysical = DateOnly.FromDateTime(DateTime.Now);
                            trans.StatusIssue = EnumStatusIssue.Delivered;
                            trans.UpdateAt = DateTime.Now;
                            trans.UpdateOperatorId = userInfo?.Value ?? "";
                        }

                        dbContext.UpdateRange(warehouseTrans);
                    }
                }

                await dbContext.SaveChangesAsync();
                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ?
                $"{ex.Message}{Environment.NewLine}{ex.InnerException}" : ex.Message;
                return await Result.FailAsync(errorMessage);
            }
        }

        public async Task<Result<IEnumerable<ProductDto>>> GetProductByShipmentNoAsync(string? code, string shipmentNo)
        {
            try
            {
                var checkLines = await dbContext.WarehouseShipmentLines.Where(_ => _.ShipmentNo == shipmentNo).AnyAsync();

                if (!checkLines)
                {
                    return await Result<IEnumerable<ProductDto>>.FailAsync("NoProductsIncludedInThisShipment");
                }

                var baseQuery = (from shipment in dbContext.WarehouseShipmentLines.Where(_ => _.ShipmentNo == shipmentNo)
                                 join product in dbContext.Products.Where(product => product.IsDeleted == false) on shipment.ProductCode equals product.ProductCode into products
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
                                     (int)dbContext.WarehouseShipmentLines.Where(s => s.ProductCode == product.ProductCode && s.ShipmentNo == shipmentNo).Sum(s => s.PackedQty)
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

        public async Task<Result<bool>> DHLPickup([Body] List<Guid> ids)
        {
            try
            {
                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");
                await dbContext.WarehouseShipments.Where(x => ids.Contains(x.Id) && x.ShippingCarrierCode == "DHL")
                .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.DHLPickup, 0)
                                     .SetProperty(xx => xx.UpdateAt, DateTime.Now)
                                     .SetProperty(xx => xx.UpdateOperatorId, userInfo.Value));
                return await Result<bool>.SuccessAsync(true);

            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");

            }
        }
        #region MOVEMENTS
        public async Task<Result<WarehouseShipmentDto>> CreateWarehouseMovementAsync([Body] WarehouseShipmentDto model)
        {
            try
            {
                //validator
                var validator = new WareHouseShipmentValidator();
                var result = validator.Validate(model);
                if (!result.IsValid)
                {
                    return await Result<WarehouseShipmentDto>.FailAsync(String.Join(',', result.Errors));
                }
                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");
                var warehouseMovement = model.Adapt<WarehouseShipment>();
                var newMovementNo = await numberSequences.GetNoByType("Movement");

                warehouseMovement.ShipmentNo = newMovementNo;
                warehouseMovement.Status = EnumShipmentOrderStatus.Open;

                warehouseMovement.CreateAt = DateTime.Now;
                warehouseMovement.CreateOperatorId = userInfo == null ? "" : userInfo.Value;

                await dbContext.WarehouseShipments.AddAsync(warehouseMovement);
                await dbContext.SaveChangesAsync();
                var updateSequence = await numberSequences.IncreaseNumberSequenceByType("Movement");

                var warehouseMovementLines = new List<WarehouseShipmentLine>();
                if (model.WareHouseShipmentLineDtos != null && model.WareHouseShipmentLineDtos.Any())
                {
                    model.WareHouseShipmentLineDtos.ForEach(line =>
                    {
                        var item = line.Adapt<WarehouseShipmentLine>();
                        item.Status = EnumShipmentOrderStatus.Open;
                        item.ShipmentNo = warehouseMovement.ShipmentNo;
                        item.CreateAt = DateTime.Now;
                        item.CreateOperatorId = userInfo == null ? "" : userInfo.Value;
                        warehouseMovementLines.Add(item);
                    });
                    await dbContext.WarehouseShipmentLines.AddRangeAsync(warehouseMovementLines);
                }
                await dbContext.SaveChangesAsync();

                // Add new WarehouseTrans lines
                var warehouseTrans = warehouseMovementLines.Select(line => new WarehouseTran
                {
                    ProductCode = line.ProductCode,
                    Qty = -(line.ShipmentQty ?? 0),
                    Location = line.Location,
                    TenantId = warehouseMovement.TenantId,
                    Bin = line.Bin,
                    LotNo = line.LotNo,
                    TransType = EnumWarehouseTransType.Movement,
                    TransNumber = warehouseMovement.ShipmentNo,
                    TransId = warehouseMovement.Id,
                    TransLineId = line.Id,
                    StatusReceipt = null,
                    StatusIssue = EnumStatusIssue.OnOrder,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value
                }).ToList();

                await dbContext.WarehouseTrans.AddRangeAsync(warehouseTrans);
                await dbContext.SaveChangesAsync();

                model.Id = warehouseMovement.Id;
                model.ShipmentNo = warehouseMovement.ShipmentNo;
                return await Result<WarehouseShipmentDto>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<PageList<WarehouseShipmentDto>>> SearchWhMovements([Body] QueryModel<WarehouseShipmentSearchModel> model)
        {
            try
            {
                var data = model.Entity ?? new WarehouseShipmentSearchModel();
                var query = dbContext.WarehouseShipments.AsEnumerable().Where(x => x.IsDeleted == false && x.ShipmentType == data.Type
                && (x.ShipmentNo.Contains(data.ShipmentNo ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                && (string.IsNullOrEmpty(data.SalesNo) || x.SalesNo == data.SalesNo)
                && (data.EstimateShipDateFrom == default || x.PlanShipDate >= data.EstimateShipDateFrom)
                && (data.EstimateShipDateTo == default || x.PlanShipDate <= data.EstimateShipDateTo)
                && (data.Status == default || x.Status == data.Status)
                && (data.BinId == default || x.BinId == data.BinId.ToString())
                && (data.TenantId == default || x.TenantId == data.TenantId)
                && (data.OutboundLocationId == default || x.Location == data.OutboundLocationId.ToString())
                ).OrderBy(x => x.ShipmentNo);
                var pagedList = PageList<WarehouseShipmentDto>.PagedResult(query.AsQueryable(), model.PageNumber, model.PageSize);
                return await Result<PageList<WarehouseShipmentDto>>.SuccessAsync(pagedList);
            }
            catch (Exception ex)
            {
                return await Result<PageList<WarehouseShipmentDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> ShipMovementAsync([Path] Guid id)
        {
            try
            {
                var existingMovement = await dbContext.WarehouseShipments
                .FirstOrDefaultAsync(x => x.Id == id);
                if (existingMovement == null)
                    return await Result<bool>.FailAsync($"MovementNotExisted");
                var userInfo = contextAccessor.HttpContext?.User.FindFirst("UserId");

                existingMovement.Status = EnumShipmentOrderStatus.Delivered;
                existingMovement.UpdateAt = DateTime.Now;
                existingMovement.UpdateOperatorId = userInfo?.Value;
                dbContext.Update(existingMovement);
                await dbContext.SaveChangesAsync();

                var movementLines = dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == existingMovement.ShipmentNo).AsEnumerable();
                if (movementLines.Count() > 0)
                {
                    movementLines.ForEach(item =>
                    {
                        item.Status = EnumShipmentOrderStatus.Delivered;
                        item.UpdateAt = DateTime.Now;
                        item.UpdateOperatorId = userInfo?.Value;
                    });
                    dbContext.UpdateRange(movementLines);

                    // update WarehouseTrans lines
                    var warehouseTransLines = await dbContext.WarehouseTrans
                    .Where(x => x.TransId == existingMovement.Id)
                    .ToListAsync();

                    foreach (var line in warehouseTransLines)
                    {
                        line.Qty = -1 * ((double)movementLines.First(s => s.Id == line.TransLineId).PackedQty);
                        line.DatePhysical = DateOnly.FromDateTime(DateTime.Now);
                        line.StatusIssue = EnumStatusIssue.Delivered;
                        line.UpdateAt = DateTime.Now;
                        line.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;
                    }

                    dbContext.UpdateRange(warehouseTransLines);
                    await dbContext.SaveChangesAsync();
                }
                #region GENERATE RECEIPT
                string receiptNo = await numberSequences.GetNoByType("Receipt");
                var newReceipt = new WarehouseReceiptOrder
                {
                    ReceiptNo = receiptNo,
                    Location = string.Empty,
                    ExpectedDate = existingMovement.PlanShipDate,
                    TenantId = existingMovement.TenantId,
                    PersonInCharge = existingMovement.PersonInCharge,
                    Status = EnumReceiptOrderStatus.Draft,
                    CreateAt = DateTime.Now,
                    ReferenceType = EnumWarehouseTransType.Movement,
                    ReferenceNo = existingMovement.ShipmentNo
                };

                await dbContext.WarehouseReceiptOrders.AddAsync(newReceipt);

                var receiptOrderLines = movementLines.Select(_ => new WarehouseReceiptOrderLine
                {
                    Id = _.Id,
                    ReceiptNo = receiptNo,
                    ProductCode = _.ProductCode,
                    OrderQty = _.PackedQty,
                    Bin = "N/A",
                    LotNo = "N/A",
                    ExpirationDate = existingMovement.PlanShipDate,
                    UnitId = _.UnitId,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo == null ? "" : userInfo.Value
                });
                await dbContext.AddRangeAsync(receiptOrderLines);
                await dbContext.SaveChangesAsync();
                await numberSequences.IncreaseNumberSequenceByType("Receipt");

                // Add new WarehouseTrans lines
                var warehouseTrans = receiptOrderLines.Select(line => new WarehouseTran
                {
                    ProductCode = line.ProductCode,
                    Qty = (double)line.OrderQty,
                    Bin = line.Bin,
                    LotNo = line.LotNo,
                    TransType = EnumWarehouseTransType.Receipt,
                    TransNumber = newReceipt.ReceiptNo,
                    TransId = newReceipt.Id,
                    TransLineId = line.Id,
                    StatusReceipt = EnumStatusReceipt.Ordered,
                    StatusIssue = null,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value
                }).ToList();
                await dbContext.AddRangeAsync(warehouseTrans);
                await dbContext.SaveChangesAsync();
                #endregion
                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentDto>> UpdateMovementAsync([Body] WarehouseShipmentDto model)
        {
            try
            {
                var existingShipment = await dbContext.WarehouseShipments.FindAsync(model.Id);
                if (existingShipment == null)
                {
                    return await Result<WarehouseShipmentDto>.FailAsync("ShipmentIsNotExisted");
                }
                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");
                existingShipment.UpdateAt = DateTime.Now;
                existingShipment.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;

                model.Adapt(existingShipment);
                dbContext.WarehouseShipments.Update(existingShipment);

                if (model.WareHouseShipmentLineDtos != null && model.WareHouseShipmentLineDtos.Any())
                {
                    var existingLines = await dbContext.WarehouseShipmentLines
                    .Where(l => l.ShipmentNo == model.ShipmentNo)
                    .ToListAsync();

                    dbContext.WarehouseShipmentLines.RemoveRange(existingLines);

                    var newLines = model.WareHouseShipmentLineDtos.Select(line =>
                    {
                        var item = line.Adapt<FBT.ShareModels.WMS.WarehouseShipmentLine>();
                        item.CreateAt = DateTime.Now;
                        item.CreateOperatorId = userInfo == null ? "" : userInfo.Value;
                        item.ShipmentNo = model.ShipmentNo;
                        return item;
                    }).ToList();

                    await dbContext.WarehouseShipmentLines.AddRangeAsync(newLines);
                }

                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipmentDto>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        #endregion

        //report 
        public async Task<Result<List<ShipmentReportDTO>>> GetShippingReportAsync()
        {
            var shipments = await dbContext.WarehouseShipments
            .Select(ws => new
            {
                ws.TenantId,
                ws.TenantName,
                ws.ShippingCarrierCode,
                ws.ShipmentNo
            })
            .ToListAsync();

            var report = shipments
            .GroupBy(ws => ws.TenantId)
            .Select(g =>
            {
                var firstShipment = g.First();
                var shipmentNo = firstShipment.ShipmentNo;

                return new ShipmentReportDTO
                {
                    TenantName = firstShipment.TenantName,
                    ShipmentType = firstShipment.ShippingCarrierCode.ToString(),
                    PackedQty = GetPackedAndPickQty(shipmentNo),
                    RemainingQty = CalculateRemainingQty(shipmentNo),
                    ProgressRateString = CalculateProgressRate(shipmentNo),
                    ProgressRate = ProgressRate(shipmentNo),
                    Productivity = CalculateProductivity(shipmentNo),
                    totalPackedQty = totalPackedQty(shipmentNo),
                    totalShipmentQty = totalShipmentQty(shipmentNo),
                    RemainingIdentificationNumber = GetRemainingIdentificationNumber(shipmentNo)
                };
            })
            .ToList();

            return Result<List<ShipmentReportDTO>>.Success(report);
        }


        private int totalPackedQty(string shipmentNo)
        {
            var shipment = dbContext.WarehouseShipments
            .FirstOrDefault(ws => ws.ShipmentNo == shipmentNo);

            if (shipment == null)
            {

                return 0; // Default value if shipment not found
            }
            var shipmentLines = dbContext.WarehouseShipmentLines
            .Where(wsl => wsl.ShipmentNo == shipmentNo)
            .ToList();
            int totalPackedQty = shipmentLines.Sum(wsl => (int?)wsl.PackedQty) ?? 0;
            return totalPackedQty;
        }

        private int totalShipmentQty(string shipmentNo)
        {
            var shipment = dbContext.WarehouseShipments
            .FirstOrDefault(ws => ws.ShipmentNo == shipmentNo);

            if (shipment == null)
            {

                return 0; // Default value if shipment not found
            }
            var shipmentLines = dbContext.WarehouseShipmentLines
            .Where(wsl => wsl.ShipmentNo == shipmentNo)
            .ToList();
            int totalShipmentQty = shipmentLines.Sum(wsl => (int?)wsl.ShipmentQty) ?? 0;
            return totalShipmentQty;
        }

        private int CalculateRemainingQty(string shipmentNo)
        {
            var shipment = dbContext.WarehouseShipments
            .Where(ws => ws.Status == EnumShipmentOrderStatus.Open &&
                ws.PlanShipDate.HasValue && ws.PlanShipDate.Value <= DateOnly.FromDateTime(DateTime.Now))
            .FirstOrDefault(ws => ws.ShipmentNo == shipmentNo);

            if (shipment == null)
            {
                return 0;
            }

            var shipmentLines = dbContext.WarehouseShipmentLines
            .Where(wsl => wsl.ShipmentNo == shipmentNo)
            .ToList();

            int? totalShipmentQty = shipmentLines.Sum(wsl => (int?)wsl.ShipmentQty) ?? 0;
            int? totalPackedQty = shipmentLines.Sum(wsl => (int?)wsl.PackedQty) ?? 0;

            var pickingLines = dbContext.WarehousePickingLines
            .Where(wpl => wpl.PickNo == shipment.PickingNo)
            .ToList();

            int? totalPickQty = pickingLines.Sum(wpl => (int?)wpl.PickQty) ?? 0;
            int? totalActualQty = pickingLines.Sum(wpl => (int?)wpl.ActualQty) ?? 0;

            int remainingQty = totalShipmentQty.GetValueOrDefault() - (totalPackedQty.GetValueOrDefault());

            return remainingQty;
        }

        private string? GetPackedAndPickQty(string shipmentNo)
        {
            var shipment = dbContext.WarehouseShipments
            .FirstOrDefault(ws => ws.ShipmentNo == shipmentNo);

            if (shipment == null)
            {

                return "0/0"; // Default value if shipment not found
            }

            var shipmentLines = dbContext.WarehouseShipmentLines
            .Where(wsl => wsl.ShipmentNo == shipmentNo)
            .ToList();

            int totalPackedQty = shipmentLines.Sum(wsl => (int?)wsl.PackedQty) ?? 0;
            int totalShipmentQty = shipmentLines.Sum(wsl => (int?)wsl.ShipmentQty) ?? 0;

            var pickingLines = dbContext.WarehousePickingLines
            .Where(wpl => wpl.PickNo == shipment.PickingNo)
            .ToList();

            int totalPickQty = pickingLines.Sum(wpl => (int?)wpl.PickQty) ?? 0;

            return $"{totalPackedQty}/{totalShipmentQty}";
        }

        private string? CalculateProgressRate(string shipmentNo)
        {
            var shipmentLines = dbContext.WarehouseShipmentLines
            .Where(wsl => wsl.ShipmentNo == shipmentNo)
            .ToList();

            if (!shipmentLines.Any())
            {
                return "0";
            }

            int? totalShipmentQty = shipmentLines.Sum(wsl => (int?)wsl.ShipmentQty);
            int? totalPackedQty = shipmentLines.Sum(wsl => (int?)wsl.PackedQty);

            if (totalShipmentQty == 0)
            {
                return "0";
            }

            double progressRate = totalShipmentQty.HasValue && totalShipmentQty.Value != 0
                       ? Math.Round((double)totalPackedQty / totalShipmentQty.Value * 100, 2)
                        : 0;

            return $"{progressRate}%";
        }

        private double ProgressRate(string shipmentNo)
        {
            var shipmentLines = dbContext.WarehouseShipmentLines
            .Where(wsl => wsl.ShipmentNo == shipmentNo)
            .ToList();

            if (!shipmentLines.Any())
            {
                return 0;
            }

            int? totalShipmentQty = shipmentLines.Sum(wsl => (int?)wsl.ShipmentQty);
            int? totalPackedQty = shipmentLines.Sum(wsl => (int?)wsl.PackedQty);

            if (totalShipmentQty == 0)
            {
                return 0;
            }

            double progressRate = totalShipmentQty.HasValue && totalShipmentQty.Value != 0
                       ? Math.Round((double)totalPackedQty / totalShipmentQty.Value * 100, 2)
                        : 0;
            return progressRate;
        }

        private string? CalculateProductivity(string shipmentNo)
        {
            var shipmentLines = dbContext.WarehouseShipmentLines
            .Where(wsl => wsl.ShipmentNo == shipmentNo)
            .ToList();

            if (!shipmentLines.Any())
            {
                return "0/h";
            }

            int? totalPackedQty = shipmentLines.Sum(wsl => (int?)wsl.PackedQty) ?? 0;

            decimal productivity = (decimal)totalPackedQty / shipmentLines.Count;

            return $"{productivity}/h";
        }

        private int PackedQty(string shipmentNo)
        {
            var shipment = dbContext.WarehouseShipments
            .FirstOrDefault(ws => ws.ShipmentNo == shipmentNo);

            if (shipment == null)
            {
                return 0;
            }

            var shipmentLines = dbContext.WarehouseShipmentLines
            .Where(wsl => wsl.ShipmentNo == shipmentNo)
            .ToList();

            int? totalShipmentQty = shipmentLines.Sum(wsl => (int?)wsl.ShipmentQty) ?? 0;
            int? totalPackedQty = shipmentLines.Sum(wsl => (int?)wsl.PackedQty) ?? 0;

            var pickingLines = dbContext.WarehousePickingLines
            .Where(wpl => wpl.PickNo == shipment.PickingNo)
            .ToList();

            int? totalPickQty = pickingLines.Sum(wpl => (int?)wpl.PickQty) ?? 0;
            int? totalActualQty = pickingLines.Sum(wpl => (int?)wpl.ActualQty) ?? 0;

            int PackedQty = (totalPackedQty ?? 0) | (totalPickQty ?? 0);




            return PackedQty;
        }

        private int GetRemainingIdentificationNumber(string shipmentNo)
        {
            var shipment = dbContext.WarehouseShipments
            .FirstOrDefault(ws => ws.ShipmentNo == shipmentNo);

            if (shipment == null)
            {
                return 0;
            }

            var shipmentLines = dbContext.WarehouseShipmentLines
            .Where(wsl => wsl.ShipmentNo == shipmentNo)
            .ToList();

            var pickingLines = dbContext.WarehousePickingLines
            .Where(wpl => wpl.PickNo == shipment.PickingNo)
            .ToList();
            int? totalShipmentQty = shipmentLines.Sum(wsl => (int?)wsl.ShipmentQty);
            int? totalPackedQty = shipmentLines.Sum(wsl => (int?)wsl.PackedQty) ?? 0;
            int? totalPickQty = pickingLines.Sum(wpl => (int?)wpl.PickQty) ?? 0;
            int? totalActualQty = pickingLines.Sum(wpl => (int?)wpl.ActualQty);

            int remainingIdentificationNumber = totalShipmentQty.GetValueOrDefault() - totalPackedQty.GetValueOrDefault();

            return remainingIdentificationNumber;
        }

        public async Task<Tuple<int, int, List<Tuple<string, string>>>> GenerateShipmentFromOrder(string tenantIds)
        {
            try
            {
                List<string> tenantList = new List<string>();
                if (!string.IsNullOrEmpty(tenantIds))
                    tenantList = tenantIds.Split(',').ToList();
                if (tenantIds.Contains("*") || tenantIds.Contains("All") || string.IsNullOrEmpty(tenantIds))
                    tenantList = new List<string>();
                var existOrderShipment = await dbContext.WarehouseShipments.Select(x => x.SalesNo).Distinct().ToListAsync();
                var orders = await (from dis in dbContext.OrderDispatches.AsNoTracking()
                                    where
                                    //!existOrderShipment.Contains(dis.OrderId) && 
                                    dis.IsDeleted == false
                                        && (dis.CallingApiDeliveryStatus == -1 || dis.CallingApiDeliveryStatus == 1)
                                        && (dis.FdaRegistrationStatus == 0 || dis.FdaRegistrationStatus == 3)
                                        && dis.IsCourierAssigned == true
                                        && (tenantList.Count == 0 || tenantList.Contains(dis.CompanyId.ToString()))
                                    join ord in dbContext.Orders.AsNoTracking() on dis.OrderId equals ord.OrderId into ordGroup
                                    from ord in ordGroup.DefaultIfEmpty()
                                    where ord.OrderStatus == "20" && ord.SubscriptionStatus != -1
                                        && ord.HadCheckAttachItem != -1
                                        && (ord.OnHoldStatus == 0 || ord.OnHoldStatus == 2)
                                        && ord.StockUpStatus == 1
                                        && (ord.AutoSplitDeliveryStatus == 1 || ord.AutoSplitDeliveryStatus == 2)
                                    join prod in dbContext.OrderDispatchProducts.AsNoTracking() on dis.Id equals prod.DispatchHeaderId into prodGroup
                                    select new
                                    {
                                        orderDispatche = new
                                        {
                                            dis.Id,
                                            dis.OrderId,
                                            dis.CompanyId,
                                            dis.ShipmentDate,
                                            dis.DeliveryCompanyCode,
                                            dis.TrackingNo,
                                            OrderDispatchProducts = prodGroup.ToList()
                                        },
                                        order = ord
                                    }).ToListAsync();
                Console.WriteLine($"Total order: {orders.Count}");
                int success = 0, outOfStock = 0;
                List<Tuple<string, string>> logOutStock = new List<Tuple<string, string>>();
                var numberSequence = await dbContext.SequencesNumber.FirstOrDefaultAsync(x => x.JournalType == "Shipment");
                int currentShipmentNo = numberSequence.CurrentSequenceNo.Value;

                #region MASTERDATA
                var tenant = new List<CompanyTenant>();
                if (orders.Any())
                {
                    tenant = await dbContext.Companies.Where(ten => orders.Select(x => x.orderDispatche.CompanyId).Contains(ten.AuthPTenantId)).ToListAsync();
                }
                var param = await dbContext.WarehouseParameters.FirstOrDefaultAsync(x => x.ShipmentLocation != default);
                var locationInfo = await dbContext.Locations.FirstOrDefaultAsync(x => x.Id.ToString() == param.ShipmentLocation);
                var prodIds = orders.SelectMany(x => x.orderDispatche.OrderDispatchProducts.Select(p => p.ProductCode)).Distinct().ToList();
                var prodInfos = await dbContext.Products.Where(x => prodIds.Contains(x.ProductCode))
                .Select(x => new
                {
                    ProductCode = x.ProductCode,
                    ProductSaleCode = x.SaleProductCode,
                    UnitId = x.UnitId,
                }).ToListAsync();

                #endregion
                List<int> orderIdsSuccess = new();
                foreach (var order in orders)
                {
                    if (order.orderDispatche.OrderDispatchProducts.Any())
                    {
                        var prodCodeSearch = order.orderDispatche.OrderDispatchProducts.Select(x => x.ProductCode)
                        .Select(x => new Application.DTOs.ProductCodeSearch { ProductCode = x }).ToList();
                        var prodStocks = await warehouseTran.GetInventoryInfoFlowBinLotFlowModelSearchAsync(new Application.DTOs.InventoryInfoBinLotSearchRequestDTO
                        {
                            CompanyId = order.orderDispatche.CompanyId,
                            ProductCodes = prodCodeSearch
                        });
                        if (!prodStocks.Succeeded || prodStocks.Data == null)
                        {
                            outOfStock++;
                            logOutStock.Add(new Tuple<string, string>(order.orderDispatche.Id.ToString(), "Out of stock: ")); // Log OrderDispatchId and missing product codes
                            continue;
                        }

                        var missingProducts = order.orderDispatche.OrderDispatchProducts
                           .Where(x =>
                               !prodStocks.Data.Any(prod => prod.ProductCode == x.ProductCode) ||
                               x.ShippedQty > prodStocks.Data.Where(prod => prod.ProductCode == x.ProductCode)
                               .Select(d => d.Details).FirstOrDefault()?.Where(dd => dd.BinCode != default && dd.LotNo != default && dd.AvailableStock > 0).Sum(x => x.AvailableStock))
                           .Select(x => x.ProductCode)
                           .ToList();

                        if (missingProducts.Any())
                        {
                            outOfStock++;
                            logOutStock.Add(new Tuple<string, string>(order.orderDispatche.Id.ToString(), "Out of stock: " + string.Join(", ", missingProducts))); // Log OrderDispatchId and missing product codes
                            continue;
                        }

                        //var planShipDate = DateOnly.TryParse(order.orderDispatche.ShipmentDate, out var shipmentDate);
                        string shipmentNo = $"{numberSequence.Prefix}{currentShipmentNo++.ToString().PadLeft(numberSequence.SequenceLength.Value, '0')}";
                        var newShipment = new WarehouseShipment
                        {
                            Id = Guid.NewGuid(),
                            ShipmentNo = shipmentNo,
                            CreateAt = DateTime.Now,
                            CreateOperatorId = "ShipmentBatch",

                            Email = order.order.DeliveryMail,
                            Address = order.order.DeliveryAddress1,
                            Telephone = order.order.DeliveryPhone,
                            ShippingAddress = order.order.DeliveryAddress1,

                            PersonInCharge = order.order.DeliveryName,
                            PersonInChargeName = order.order.DeliveryName,
                            ShippingCarrierCode = order.orderDispatche.DeliveryCompanyCode,

                            TenantId = order.orderDispatche.CompanyId,
                            TenantName = tenant.FirstOrDefault(x => x.AuthPTenantId == order.orderDispatche.CompanyId)?.FullName,

                            Location = param != null ? param.ShipmentLocation : "",
                            LocationName = locationInfo == null ? "" : locationInfo.LocationName,

                            PlanShipDate = DateOnly.FromDateTime(DateTime.Now),
                            OrderDispatchId = order.orderDispatche.Id,
                            SalesNo = order.orderDispatche.OrderId,
                            TrackingNo = order.orderDispatche.TrackingNo,
                            ShipmentType = EnumWarehouseTransType.Shipment,
                            Status = EnumShipmentOrderStatus.Open,
                        };
                        await dbContext.AddAsync(newShipment);

                        foreach (var item in order.orderDispatche.OrderDispatchProducts.Where(x => x.ShippedQty > 0))
                        {
                            var prodQuantity = prodStocks.Data.FirstOrDefault(x => x.ProductCode == item.ProductCode)?.Details
                                    .Where(x => x.BinCode != default && x.LotNo != default && x.AvailableStock > 0).OrderBy(x => x.Expired).ToList();
                            if (prodQuantity != null)
                            {
                                var prodUnit = prodInfos.FirstOrDefault(x => x.ProductCode == item.ProductCode);
                                int i = 0;
                                double? remainQuantity = item.ShippedQty;
                                while (remainQuantity != 0)
                                {
                                    var productQty = prodQuantity[i++];
                                    double? quantity = remainQuantity >= productQty.AvailableStock ? productQty.AvailableStock : remainQuantity;
                                    var newShipmentLine = new WarehouseShipmentLine
                                    {
                                        Id = Guid.NewGuid(),
                                        ProductCode = item.ProductCode,
                                        Bin = productQty.BinCode,
                                        LotNo = productQty.LotNo,
                                        CreateAt = DateTime.Now,
                                        CreateOperatorId = "ShipmentBatch",
                                        Location = param != null ? param.ShipmentLocation : "",
                                        ShipmentNo = shipmentNo,
                                        ShipmentQty = quantity,
                                        ExpirationDate = productQty.Expired == null ? default : productQty.Expired.Value,
                                        Status = EnumShipmentOrderStatus.Open,
                                        UnitId = prodUnit == null ? 0 : prodUnit.UnitId,
                                        PackedQty = 0,
                                    };
                                    remainQuantity = remainQuantity - quantity;

                                    await dbContext.AddAsync(newShipmentLine);
                                    var warehouseTrans = new WarehouseTran
                                    {
                                        ProductCode = newShipmentLine.ProductCode,
                                        Qty = -(newShipmentLine.ShipmentQty ?? 0),

                                        Bin = string.IsNullOrEmpty(newShipmentLine.Bin) ? "N/A" : newShipmentLine.Bin,
                                        LotNo = string.IsNullOrEmpty(newShipmentLine.LotNo) ? "N/A" : newShipmentLine.LotNo,

                                        Location = newShipmentLine.Location,
                                        TenantId = newShipment.TenantId,
                                        TransType = EnumWarehouseTransType.Shipment,
                                        TransNumber = newShipmentLine.ShipmentNo,
                                        TransId = newShipment.Id,
                                        TransLineId = newShipmentLine.Id,
                                        StatusReceipt = null,
                                        StatusIssue = EnumStatusIssue.OnOrder,
                                        CreateAt = DateTime.Now,
                                        CreateOperatorId = "ShipmentBatch"
                                    };
                                    await dbContext.AddAsync(warehouseTrans);
                                }
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        success++;
                        logOutStock.Add(new Tuple<string, string>(order.orderDispatche.Id.ToString(), "Shipment created successfully."));
                        orderIdsSuccess.Add(order.order.Id);
                    }
                }
                if (orderIdsSuccess.Count > 0)
                {
                    await dbContext.Orders.Where(x => orderIdsSuccess.Contains(x.Id))
                       .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.OrderStatus, "30")
                                                 .SetProperty(xx => xx.UpdateAt, DateTime.Now)
                                                 .SetProperty(xx => xx.UpdateOperatorId, "WMS-UpdateStatus")
                       );

                    await dbContext.SequencesNumber.Where(x => x.JournalType == "Shipment")
                    .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.CurrentSequenceNo, currentShipmentNo));
                }
                return new Tuple<int, int, List<Tuple<string, string>>>(success, orders.Count, logOutStock);
            }
            catch (Exception ex)
            {
                LogHelpers.LogFile("GENERATE_SHIPMENT", $"ERROR: {ex.Message}");
                return new Tuple<int, int, List<Tuple<string, string>>>(0, 0, new List<Tuple<string, string>>());
            }
        }
    }
}
