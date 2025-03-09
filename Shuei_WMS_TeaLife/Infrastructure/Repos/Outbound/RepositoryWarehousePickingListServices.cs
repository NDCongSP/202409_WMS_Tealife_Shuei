using Application.DTOs;
using Application.DTOs.Request;
using Application.DTOs.Request.Picking;
using Application.DTOs.Response;
using Application.DTOs.Transfer;
using Application.Extentions;
using Application.Services.Outbound;
using DocumentFormat.OpenXml;
using Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehousePickingListServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor, IWarehouseShipment warehouseShipment) : IWarehousePickingList
    {
        public async Task<Result<List<WarehousePickingList>>> AddRangeAsync([Body] List<WarehousePickingList> model)
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

                await dbContext.WarehousePickingLists.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<WarehousePickingList>>.SuccessAsync(model, "Add range WarehousePickingList successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingList>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingList>> DeleteRangeAsync([Body] List<WarehousePickingList> model)
        {
            try
            {
                dbContext.WarehousePickingLists.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingList>.SuccessAsync("Delete range WarehousePickingList successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingList>> DeleteAsync([Body] WarehousePickingList model)
        {
            try
            {
                dbContext.WarehousePickingLists.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingList>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingList>>> GetAllAsync()
        {
            try
            {
                // Truy vấn danh sách từ cơ sở dữ liệu
                var pickingLists = await dbContext.WarehousePickingLists.ToListAsync();

                // Kiểm tra xem danh sách có null hoặc rỗng không
                if (pickingLists == null || !pickingLists.Any())
                {
                    return await Result<List<WarehousePickingList>>.FailAsync("No data found.");
                }

                // Trả về kết quả thành công nếu có dữ liệu
                return await Result<List<WarehousePickingList>>.SuccessAsync(pickingLists);
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null
                    ? $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}"
                    : ex.Message;

                return await Result<List<WarehousePickingList>>.FailAsync(errorMessage);
            }
        }

        public async Task<Result<WarehousePickingList>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePickingList>.SuccessAsync(await dbContext.WarehousePickingLists.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingList>> InsertAsync([Body] WarehousePickingList model)
        {
            try
            {
                await dbContext.WarehousePickingLists.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingList>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingList>> UpdateAsync([Body] WarehousePickingList model)
        {
            try
            {
                dbContext.WarehousePickingLists.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingList>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingList>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingList>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            try
            {
                return await Result<List<WarehousePickingList>>.SuccessAsync(await dbContext.WarehousePickingLists.Where(x => x.PickNo == putAwayNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingList>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingDTO>>> GetWarehousePickingDTOAsync([Body] QueryModel<PickingListSearchRequestDto> data)
        {
            try
            {
                var model = data.Entity ?? new PickingListSearchRequestDto();

                var query = (from picking in dbContext.WarehousePickingLists
                             join shipment in dbContext.WarehouseShipments
                             on picking.PickNo equals shipment.PickingNo into shipmentGroup
                             from sg in shipmentGroup.DefaultIfEmpty()
                             join pickingLine in dbContext.WarehousePickingLines
                             on picking.PickNo equals pickingLine.PickNo into pickingLineGroup
                             from pl in pickingLineGroup.DefaultIfEmpty()
                             where
                                 (string.IsNullOrEmpty(model.PickingNo) || picking.PickNo.Contains(model.PickingNo)) &&
                                 (string.IsNullOrEmpty(model.ShipmentNumber) || sg.ShipmentNo.Contains(model.ShipmentNumber)) &&
                                 (string.IsNullOrEmpty(model.Location) || picking.Location.Contains(model.Location)) &&
                                 (string.IsNullOrEmpty(model.Bin) || pl.Bin == model.Bin) &&
                                 (!model.PlanShipDateFrom.HasValue || sg.PlanShipDate >= model.PlanShipDateFrom) &&
                                 (!model.PlanShipDateTo.HasValue || sg.PlanShipDate <= model.PlanShipDateTo) &&
                                 (!model.Status.HasValue || picking.Status == model.Status.Value)
                             select new { picking, sg, pl });

                var groupedQuery = query.AsEnumerable();

                var result = groupedQuery.GroupBy(x => new
                {
                    x.picking.PickNo,
                    x.picking.Location,
                    x.picking.PersonInCharge,
                    x.picking.CreateAt,
                    x.picking.Status
                })
                .Select(g => new WarehousePickingDTO
                {
                    PickNo = g.Key.PickNo,
                    Location = g.Key.Location,
                    PersonInCharge = g.Key.PersonInCharge,
                    PickedDate = g.Key.CreateAt.HasValue ? DateOnly.FromDateTime(g.Key.CreateAt.Value) : null,
                    PlanShipDate = g.Where(x => x.sg != null && x.sg.PlanShipDate.HasValue)
                                   .OrderBy(x => Math.Abs((x.sg.PlanShipDate.Value.DayNumber - DateOnly.FromDateTime(DateTime.Now).DayNumber)))
                                   .Select(x => x.sg.PlanShipDate.Value)
                                   .FirstOrDefault(),
                    Status = g.Key.Status,
                    ShipmentNo = string.Join("/", g.Where(x => x.sg != null && !string.IsNullOrEmpty(x.sg.ShipmentNo))
                                             .Select(x => x.sg.ShipmentNo)
                                             .Distinct()),
                    CreateAt = g.Key.CreateAt.HasValue ? DateOnly.FromDateTime(g.Key.CreateAt.Value) : null
                }).OrderByDescending(x => x.PickNo).Skip((data.PageNumber - 1) * data.PageSize)
                                      .Take(data.PageSize).ToList();

                return await Result<List<WarehousePickingDTO>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result> DeletePickingAsync([FromRoute] string pickNo)
        {
            try
            {
                List<string> errorMessages = new List<string>();
                // remove picking
                var picking = await dbContext.WarehousePickingLists
                    .FirstOrDefaultAsync(x => x.PickNo == pickNo);
                if (picking == null)
                {
                    return await Result.FailAsync("No picking found with the specified PickNo.");
                }
                dbContext.WarehousePickingLists.Remove(picking);

                //remove picking line
                var pickinglines = dbContext.WarehousePickingLines
                    .Where(p => p.PickNo == pickNo).ToList();
                if (!pickinglines.Any())
                {
                    errorMessages.Add("No picking line found with the specified PickNo.");
                }
                dbContext.WarehousePickingLines.RemoveRange(pickinglines);

                // clean picking code in shipment
                var shipments = dbContext.WarehouseShipments
                                .Where(p => p.PickingNo == pickNo).ToList();
                if (!shipments.Any())
                {
                    errorMessages.Add("No shipment found with the specified PickNo.");
                }
                else
                {
                    foreach (var shipment in shipments)
                    {
                        shipment.PickingNo = null;
                        shipment.Status = EnumShipmentOrderStatus.Open;
                        shipment.UpdateAt = DateTime.Now;
                    }
                }

                if (errorMessages.Any())
                {
                    foreach (var error in errorMessages)
                    {
                        Console.WriteLine(error);
                    }
                }

                // save changes
                await dbContext.SaveChangesAsync();

                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ?
                    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}" : ex.Message;
                return await Result.FailAsync(errorMessage);
            }
        }

        public async Task<Result> CompletePickingAsync([FromRoute] string pickNo)
        {
            try
            {
                List<string> errorMessages = new List<string>();
                var picking = await dbContext.WarehousePickingLists
                    .FirstOrDefaultAsync(x => x.PickNo == pickNo);
                if (picking == null)
                {
                    return await Result.FailAsync("No picking found with the specified PickNo.");
                }
                picking.Status = EnumShipmentOrderStatus.Picked;
                picking.PickedDate = DateOnly.FromDateTime(DateTime.Now);
                var shipments = dbContext.WarehouseShipments
                                .Where(p => p.PickingNo == pickNo).ToList();
                if (!shipments.Any())
                {
                    errorMessages.Add("No shipment found with the specified PickNo.");
                }
                else
                {
                    foreach (var shipment in shipments)
                    {
                        shipment.Status = EnumShipmentOrderStatus.Picked;
                        shipment.UpdateAt = DateTime.Now;
                    }
                }
                // save changes
                await dbContext.SaveChangesAsync();

                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ?
                    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}" : ex.Message;
                return await Result.FailAsync(errorMessage);
            }
        }

        public async Task<Result> CompletePickingsAsync([FromBody] List<string> pickNos)
        {
            try
            {
                List<string> errorMessages = new List<string>();
                var userInfo = contextAccessor.HttpContext.User.FindFirst("UserId");

                //var picking = await dbContext.WarehousePickingLists
                //    .Where(x => x.Status == EnumShipmentOrderStatus.Picking && pickNos.Contains(x.PickNo))
                //    .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.Status, EnumShipmentOrderStatus.Picked)
                //                              .SetProperty(xx => xx.UpdateAt, DateTime.Now)
                //                              .SetProperty(xx => xx.UpdateOperatorId, userInfo == null ? "" : userInfo.Value)
                //    );

                var pickings = await dbContext.WarehousePickingLists
                            .Where(x => x.Status == EnumShipmentOrderStatus.Picking && pickNos.Contains(x.PickNo)).ToListAsync();

                var pickingNos = pickings.Select(x => x.PickNo).ToList();
                var stagingPicking = await SyncFromStagingAsync(pickingNos);

                //get picking lines
                var pickingLines = await dbContext.WarehousePickingLines.Where(x => pickingNos.Contains(x.PickNo)).ToListAsync();

                foreach (var x in pickings)
                {
                    var currentPickingLines = pickingLines.Where(pl => pl.PickNo == x.PickNo).ToList();
                    var hasPartialPicking = currentPickingLines.Any(pl => pl.PickQty != pl.ActualQty);
                    x.Status = EnumShipmentOrderStatus.Picked;
                    x.UpdateAt = DateTime.Now;
                    x.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;
                    // Update picking lines status
                    currentPickingLines.ForEach(pl =>
                    {
                        pl.Status = EnumShipmentOrderStatus.Picked;
                        pl.UpdateAt = DateTime.Now;
                        pl.UpdateOperatorId = x.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;
                    });
                    dbContext.UpdateRange(currentPickingLines);
                }
                dbContext.UpdateRange(pickings);
                await dbContext.SaveChangesAsync();

                var shipments = await dbContext.WarehouseShipments
                                .Where(p => p.Status == EnumShipmentOrderStatus.Picking && pickNos.Contains(p.PickingNo)).ToListAsync();
                shipments.ForEach(x =>
                {
                    x.Status = EnumShipmentOrderStatus.Picked;
                    x.UpdateAt = DateTime.Now;
                    x.UpdateOperatorId = userInfo == null ? "" : userInfo.Value;
                });
                dbContext.UpdateRange(shipments);
                await dbContext.SaveChangesAsync();

                await dbContext.WarehouseShipmentLines
                               .Where(p => shipments.Select(x => x.ShipmentNo).Contains(p.ShipmentNo))
                               .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.Status, EnumShipmentOrderStatus.Picked)
                                             .SetProperty(xx => xx.UpdateAt, DateTime.Now)
                                             .SetProperty(xx => xx.UpdateOperatorId, userInfo == null ? "" : userInfo.Value)
                   );
                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ?
                    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}" : ex.Message;
                return await Result.FailAsync(errorMessage);
            }
        }

        public async Task<Result<ResponseCompleteModel>> AutoCompletePickingsAsync()
        {
            try
            {
                var pickings = await dbContext.WarehousePickingLists
                            .Where(x => x.HHTStatus == EnumHHTStatus.Done && x.Status == EnumShipmentOrderStatus.Picking).ToListAsync();
                if (pickings.Count == 0)
                    return Result<ResponseCompleteModel>.Success(new ResponseCompleteModel(0, 0));

                var pickingNos = pickings.Select(x => x.PickNo).ToList();
                var stagingPicking = await SyncFromStagingAsync(pickingNos);

                //get picking lines
                var pickingLines = await dbContext.WarehousePickingLines.Where(x => pickingNos.Contains(x.PickNo)).ToListAsync();
                int success = 0;
                foreach (var x in pickings)
                {
                    var currentPickingLines = pickingLines.Where(pl => pl.PickNo == x.PickNo).ToList();
                    var hasPartialPicking = currentPickingLines.Any(pl => pl.PickQty != pl.ActualQty);
                    x.Status = EnumShipmentOrderStatus.Picked;
                    x.UpdateAt = DateTime.Now;
                    x.UpdateOperatorId = "system";
                    // Update picking lines status
                    currentPickingLines.ForEach(pl =>
                    {
                        pl.Status = EnumShipmentOrderStatus.Picked;
                        pl.UpdateAt = DateTime.Now;
                        pl.UpdateOperatorId = "system";
                    });
                    success++;
                    dbContext.UpdateRange(currentPickingLines);
                }
                dbContext.UpdateRange(pickings);
                await dbContext.SaveChangesAsync();

                var shipments = await dbContext.WarehouseShipments
                                .Where(p => p.Status == EnumShipmentOrderStatus.Picking && pickings.Select(xx => xx.PickNo).Contains(p.PickingNo)).ToListAsync();
                shipments.ForEach(x =>
                {
                    x.Status = EnumShipmentOrderStatus.Picked;
                    x.UpdateAt = DateTime.Now;
                    x.UpdateOperatorId = "system";
                });
                dbContext.UpdateRange(shipments);
                await dbContext.SaveChangesAsync();

                await dbContext.WarehouseShipmentLines
                               .Where(p => shipments.Select(x => x.ShipmentNo).Contains(p.ShipmentNo))
                               .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.Status, EnumShipmentOrderStatus.Picked)
                                             .SetProperty(xx => xx.UpdateAt, DateTime.Now)
                                             .SetProperty(xx => xx.UpdateOperatorId, "system")
                   );
                return Result<ResponseCompleteModel>.Success(new ResponseCompleteModel(pickings.Count, success));
            }
            catch (Exception ex)
            {
                return Result<ResponseCompleteModel>.Fail(ex);
            }
        }

        public async Task<Result<List<WarehousePickingLineDTO>>> SyncToHTTmpAsync([Body] List<WarehousePickingLineDTO> model)
        {
            try
            {
                var pickingDTOs = model.ToList();
                var pickingStagings = dbContext.WarehousePickingStagings
                                          .Where(x => x.PickNo == model.First().PickNo)
                                          .ToList();

                foreach (var item in pickingStagings.GroupBy(x => new { x.ProductCode, x.Bin, x.LotNo }))
                {
                    var pickingLineIndex = model.FindIndex(x => x.ProductCode == item.Key.ProductCode && x.Bin == item.Key.Bin && x.Lot == item.Key.LotNo);
                    if (pickingLineIndex >= 0)
                    {
                        model[pickingLineIndex].ActualQty = item.Sum(ps => ps.ActualQty);
                        model[pickingLineIndex].Lot = item.First().LotNo ?? string.Empty;
                        model[pickingLineIndex].Bin = item.First().Bin ?? string.Empty;
                    }
                    //else
                    //{
                    //model.Add(new WarehousePickingLineDTO
                    //{
                    //    Id = Guid.NewGuid(),
                    //    ProductCode = item.Key.ProductCode,
                    //    Bin = item.Key.Bin ?? string.Empty,
                    //    Lot = item.Key.LotNo ?? string.Empty,
                    //    PickQty = item.Sum(ps => ps.ActualQty),
                    //    ActualQty = item.Sum(ps => ps.ActualQty)
                    //});
                    //}
                }
                //remove item not have data
                //model = model.Where(x => !(x.ActualQty == 0 || x.ActualQty == default)).ToList();

                return await Result<List<WarehousePickingLineDTO>>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingLineDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result> SyncFromStagingAsync([Body] List<string> pickNos)
        {
            try
            {
                var pickingLines = await dbContext.WarehousePickingLines
                    .Where(x => pickNos.Contains(x.PickNo))
                    .ToListAsync();

                var stagingData = await dbContext.WarehousePickingStagings
                    .Where(x => pickNos.Contains(x.PickNo))
                    .ToListAsync();

                foreach (var pickNo in pickNos)
                {
                    var pickingLinesForPickNo = pickingLines.Where(x => x.PickNo == pickNo).ToList();
                    var stagingForPickNo = stagingData.Where(x => x.PickNo == pickNo).ToList();

                    foreach (var stagingGroup in stagingForPickNo.GroupBy(x => new { x.ProductCode, x.Bin, x.LotNo }))
                    {
                        var matchingPickingLine = pickingLinesForPickNo.FirstOrDefault(x =>
                            x.ProductCode == stagingGroup.Key.ProductCode &&
                            x.Bin == stagingGroup.Key.Bin &&
                            x.LotNo == stagingGroup.Key.LotNo);

                        if (matchingPickingLine != null)
                        {
                            matchingPickingLine.ActualQty = stagingGroup.Sum(ps => ps.ActualQty);
                            matchingPickingLine.LotNo = stagingGroup.First().LotNo ?? string.Empty;
                            matchingPickingLine.Bin = stagingGroup.First().Bin ?? string.Empty;
                            dbContext.Update(matchingPickingLine);
                        }
                    }
                }

                await dbContext.SaveChangesAsync();
                return await Result.SuccessAsync("Sync from staging completed successfully");
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ?
                    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}" : ex.Message;
                return await Result.FailAsync(errorMessage);
            }
        }

        public async Task<string> GetBarcodeDataAsync(string No)
        {
            try
            {
                if (string.IsNullOrEmpty(No))
                {
                    return ("Input value cannot be null or empty.");
                }

                var barcodeBase64 = GlobalVariable.GenerateBarcodeBase64(No, 200, 50);
                if (string.IsNullOrEmpty(barcodeBase64))
                {
                    return ("");
                }

                // Chắc chắn giá trị barcodeBase64 được gán vào Data
                return barcodeBase64;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<List<CoverSheetNDeliveryNoteDto>> GetDataCoverSheetNDeliveryNote(List<string> ids)
        {
            var result = new List<CoverSheetNDeliveryNoteDto>();
            try
            {
                var pickingLists = await dbContext.WarehousePickingLists
                    .Where(p => ids.Contains(p.PickNo))
                    .ToListAsync();

                var pickingLines = await dbContext.WarehousePickingLines
                    .Where(pl => pickingLists.Select(p => p.PickNo).Contains(pl.PickNo))
                    .ToListAsync();

                var shipments = await dbContext.WarehouseShipments
                    .Where(s => pickingLists.Select(p => p.PickNo).Contains(s.PickingNo))
                    .ToListAsync();

                var shipmentLines = await dbContext.WarehouseShipmentLines
                    .Where(sl => shipments.Select(s => s.ShipmentNo).Contains(sl.ShipmentNo))
                    .ToListAsync();

                var orders = await dbContext.Orders.Where(x => shipments.Select(xx => xx.SalesNo).Contains(x.OrderId)).ToListAsync();

                var query = pickingLists.Select(p => new CoverSheetNDeliveryNoteDto
                {
                    DataTransfer = new List<PickingSlipInfos>
                    {
                        new PickingSlipInfos
                        {
                            Barcode = GlobalVariable.GenerateBarcodeBase64(p.PickNo, 200, 50),
                            PickInfo = new WarehousePickingDTO
                            {
                                Location = p.Location,
                                PersonInCharge = p.PersonInCharge,
                                PickedDate = p.PickedDate,
                                PickNo = p.PickNo,
                                PlanShipDate = p.EstimatedShipDate,
                                ShipmentNo = string.Join("/", shipments
                                    .Where(s => s.PickingNo == p.PickNo)
                                    .Select(s => s.ShipmentNo)),
                                CreateAt = p.CreateAt.HasValue?DateOnly.FromDateTime(p.CreateAt.Value):null
                            },
                            PickLineInfos = pickingLines
                                .Where(pl => pl.PickNo == p.PickNo)
                                .Adapt<List<WarehousePickingLineDTO>>(),
                            ShipmentInfos = shipments
                                .Where(s => s.PickingNo == p.PickNo)
                                .Select(s => new WarehousePickingShipmentDTO
                                {
                                    Id = s.Id,
                                    OrderNo = s.SalesNo,
                                    ShipmentNo = s.ShipmentNo,
                                    TotalQuantity = (double)shipmentLines
                                        .Where(sl => sl.ShipmentNo == s.ShipmentNo)
                                        .Sum(sl => sl.ShipmentQty),
                                    OrderDate = orders != null && orders.Count > 0 && !string.IsNullOrEmpty(s.SalesNo) ?
                                        DateOnly.FromDateTime((DateTime)orders.FirstOrDefault(x => x.OrderId == s.SalesNo)?.OrderDate) : null,
                                    TenantFullName = s.TenantName,
                                    PlanShipDate = s.PlanShipDate,
                                    OrderDeliveryCompany = s.ShippingCarrierCode
                                }).ToList()
                        }
                    },
                    DeliveryNotes = new List<DeliveryNoteModel>()
                }).ToList();
                //handle create delivery note 
                foreach (var item in query.Where(x => x.DataTransfer.Count > 0))
                {
                    var shipmentIds = item.DataTransfer.First().ShipmentInfos.Select(x => x.Id).ToList();
                    item.DeliveryNotes = await warehouseShipment.GetDeliveryNotes(shipmentIds);

                    foreach (var item1 in item.DataTransfer)
                    {
                        foreach (var item2 in item1.PickLineInfos)
                        {
                            var p = await dbContext.Products.FirstOrDefaultAsync(_ => _.ProductCode == item2.ProductCode);

                            item2.ProductName = p.ProductIname ?? null;
                        }
                    }
                }
                result.AddRange(query);
                return result;
            }
            catch (Exception ex)
            {
                return new List<CoverSheetNDeliveryNoteDto>();
            }
        }
    }
}
