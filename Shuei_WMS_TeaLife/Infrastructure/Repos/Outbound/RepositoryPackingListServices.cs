using Application.DTOs;
using Application.DTOs.Request.shipment;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Application.Extentions;
using Application.Models;
using Application.Services.Outbound;
using Azure;
using Dapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryPackingListServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IPackingList
    {
        public async Task<Result<string>> CompletePackingAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model)
        {
            try
            {
                using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var item in model)
                        {
                            var s = await dbContext.WarehouseShipmentLines.FirstOrDefaultAsync(x => x.Id == item.Id);

                            if (s != null)
                            {
                                s.PackedQty = item.PackedQty;
                                s.PackedDate = item.PackedDate;
                                s.Status = item.StatusShipment;//   packed = 5,
                            }
                            else
                            {
                                var err = new ErrorResponse();
                                err.Errors.Add("Warning", "Shipping Line id in warehouse shipping line could be not found.");
                                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                            }

                            //var whTran=await dbContext.WarehouseTrans.FirstOrDefaultAsync(x=>x.TransLineId==item.Id);
                            //if(whTran != null)
                            //{
                            //    whTran.DatePhysical=item.PackedDate;
                            //    whTran.StatusIssue=item.StatusIssueWhTran;// Delivered = 4,
                            //}
                            //else throw new Exception($"Shipping Line id in warehouse tran: {item.Id} could be not found.");
                        }

                        var shipment = await dbContext.WarehouseShipments.FirstOrDefaultAsync(x => x.ShipmentNo == model.FirstOrDefault().ShipmentNo);
                        if (shipment != null)
                        {
                            shipment.Status = model.FirstOrDefault().StatusShipment;
                            shipment.DHLPickup = 0;//Cap nhat ve 0 de bao cho ben van chuyen biet la da packing xong.
                            shipment.ShippingBoxesId = model.First().ShippingBoxesId;
                            shipment.ShippingBoxesName = model.First().ShippingBoxesName;
                        }
                        else
                        {
                            var err = new ErrorResponse();
                            err.Errors.Add("Warning", "The shipping number was not found.");
                            return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                        }

                        await dbContext.SaveChangesAsync();

                        // Commit the transaction
                        await transaction.CommitAsync();

                        return await Result<string>.SuccessAsync("Packing is complete.");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if an error occurs
                        await transaction.RollbackAsync();
                        var err = new ErrorResponse();
                        err.Errors.Add("Error", $"Transaction update packed quantity for shipment line failed: {ex.Message}{Environment.NewLine}{ex.InnerException}");
                        return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                    }
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<WarehousePackingListDto>>> GetDataMasterAsync([Body] PackingListSearchRequestDto model)
        {
            try
            {
                List<WarehousePackingListDto> response = new List<WarehousePackingListDto>();

                var c = dbContext.Database.GetConnectionString();
                using (var connection = new SqlConnection(c))
                {
                    var para = new DynamicParameters();

                    para.Add("ShipmentNo", model.InstructionNumber);
                    para.Add("@PlanShipDateFrom", model.ScheduledShipDateFrom);
                    para.Add("@PlanShipDateTo", model.ScheduledShipDateTo);
                    para.Add("DeliveryLocation", model.DeliveryLocation);
                    para.Add("Bin", model.OutgoingBin);
                    para.Add("Status", model.Status);
                    para.Add("TrackingNo", model.TrackingNo);

                    var rowCount = await connection.QueryAsync<PackingListModel>("sp_packingListGetDataMaster", param: para, commandType: System.Data.CommandType.StoredProcedure);

                    var groupByShipmentNo = rowCount.GroupBy(x => x.ShipmentNo);

                    response = groupByShipmentNo.Select(x =>
                    {
                        return new WarehousePackingListDto()
                        {
                            ShipmentNo = x.Key,
                            Id = x.Select(u => u.IdShipment).FirstOrDefault(),
                            LocationName = x.Select(u => u.LocationName).FirstOrDefault(),
                            ShippingCarrierCode = x.Select(u => u.ShippingCarrierCode).FirstOrDefault(),
                            ShippingAddress = x.Select(u => u.ShippingAddress).FirstOrDefault(),
                            PlanShipDate = x.Select(u => u.PlanShipDate).FirstOrDefault(),
                            SalesNo = x.Select(u => u.SalesNo).FirstOrDefault(),
                            StatusOfShipment = x.Select(u => u.StatusOfShipment).FirstOrDefault(),
                            Telephone = x.Select(u => u.Telephone).FirstOrDefault(),
                            Address = x.Select(u => u.Address).FirstOrDefault(),
                            Email = x.Select(u => u.Email).FirstOrDefault(),
                            TrackingNo = x.Select(u => u.TrackingNo).FirstOrDefault(),
                            PickedDate = x.Select(u => u.PickedDate).FirstOrDefault(),
                            PackedDate = x.Select(u => u.PackedDate).FirstOrDefault(),
                            ShipmentLines = x.ToList(),
                        };
                    }).OrderBy(x => x.PlanShipDate).ToList();
                }

                return await Result<List<WarehousePackingListDto>>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<WarehousePackingListDto>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehousePackingListDto>> GetDataMasterByShipmentNoAsync([Path] string shipmentNo)
        {
            var errorsResponse = new ErrorResponse();
            try
            {
                WarehousePackingListDto response = new WarehousePackingListDto();

                var rowCount = await dbContext.Database.SqlQueryRaw<PackingListModel>("sp_packingGetShipmentByShipmentNo @shipmentNo = {0}", shipmentNo).ToListAsync();
                if (rowCount.Count == 0 || rowCount == null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Empty.");
                    return await Result<WarehousePackingListDto >.FailAsync(JsonConvert.SerializeObject(err));                    
                }

                response = rowCount.GroupBy(_ => _.ShipmentNo).Select(x =>
                {
                    return new WarehousePackingListDto()
                    {
                        ShipmentNo = x.Key,
                        Id = x.Select(u => u.IdShipment).FirstOrDefault(),
                        LocationName = x.Select(u => u.LocationName).FirstOrDefault(),
                        ShippingCarrierCode = x.Select(u => u.ShippingCarrierCode).FirstOrDefault(),
                        ShippingBoxesId = x.Select(u => u.ShippingBoxesId).FirstOrDefault(),
                        ShippingBoxesName = x.Select(u => u.ShippingBoxesName).FirstOrDefault(),
                        PrinterName = x.Select(u => u.PrinterName).FirstOrDefault(),
                        ShippingAddress = x.Select(u => u.ShippingAddress).FirstOrDefault(),
                        PlanShipDate = x.Select(u => u.PlanShipDate).FirstOrDefault(),
                        SalesNo = x.Select(u => u.SalesNo).FirstOrDefault(),
                        StatusOfShipment = x.Select(u => u.StatusOfShipment).FirstOrDefault(),
                        Telephone = x.Select(u => u.Telephone).FirstOrDefault(),
                        Address = x.Select(u => u.Address).FirstOrDefault(),
                        Email = x.Select(u => u.Email).FirstOrDefault(),
                        TrackingNo = x.Select(u => u.TrackingNo).FirstOrDefault(),
                        LabelFilePath = x.Select(u => u.LabelFilePath).FirstOrDefault(),
                        LabelFileExtension = x.Select(u => u.LabelFileExtension).FirstOrDefault(),
                        PickedDate = x.Select(u => u.PickedDate).FirstOrDefault(),
                        PackedDate = x.Select(u => u.PackedDate).FirstOrDefault(),
                        InternalRemarks = x.Select(u => u.InternalRemarks).FirstOrDefault(),
                        ShipmentLines = x.ToList(),
                    };
                }).FirstOrDefault();

                int index = 0;
                foreach (var item in response.ShipmentLines)
                {
                    for (int i = 1; i <= item.ShipmentQty; i++)
                    {
                        //var r1 = item.ExpirationDate;
                        //var r = item.ExpirationDate.ToString();
                        response.GenerateDetails.Add(new PackingListGenerateRow()
                        {
                            ProductCode = item.ProductCode,
                            ProductName = item.ProductName,
                            ShipmentLineId = item.Id,
                            ShipmentQty = $"{i}/{item.ShipmentQty}",
                            ExpireDate = item.ExpirationDate.Value.Year > 2024 ? item.ExpirationDate?.ToString("yyyy/MM/dd") : string.Empty,
                            LotNo = item.LotNo,
                            //Qty = 1,
                            Scanned = i <= item.Remaining ? "0" : "1",
                            Index = index
                        });
                        index += 1;
                    }
                }

                return await Result<WarehousePackingListDto>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehousePackingListDto>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehousePackingListDto>> GetDataMasterByTrackingNoAsync([Path] string trackingNo)
        {
            var errorsResponse = new ErrorResponse();
            try
            {
                WarehousePackingListDto response = new WarehousePackingListDto();

                var rowCount = await dbContext.Database.SqlQueryRaw<PackingListModel>("sp_packingGetShipmentByTrackingNo @trackingNo = {0}", trackingNo).ToListAsync();
                if (rowCount.Count == 0 || rowCount == null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Empty.");
                    return await Result<WarehousePackingListDto>.FailAsync(JsonConvert.SerializeObject(err));
                }

                response = rowCount.GroupBy(_ => _.ShipmentNo).Select(x =>
                {
                    return new WarehousePackingListDto()
                    {
                        ShipmentNo = x.Key,
                        Id = x.Select(u => u.IdShipment).FirstOrDefault(),
                        LocationName = x.Select(u => u.LocationName).FirstOrDefault(),
                        ShippingCarrierCode = x.Select(u => u.ShippingCarrierCode).FirstOrDefault(),
                        ShippingBoxesId = x.Select(u => u.ShippingBoxesId).FirstOrDefault(),
                        ShippingBoxesName = x.Select(u => u.ShippingBoxesName).FirstOrDefault(),
                        PrinterName = x.Select(u => u.PrinterName).FirstOrDefault(),
                        ShippingAddress = x.Select(u => u.ShippingAddress).FirstOrDefault(),
                        PlanShipDate = x.Select(u => u.PlanShipDate).FirstOrDefault(),
                        SalesNo = x.Select(u => u.SalesNo).FirstOrDefault(),
                        StatusOfShipment = x.Select(u => u.StatusOfShipment).FirstOrDefault(),
                        Telephone = x.Select(u => u.Telephone).FirstOrDefault(),
                        Address = x.Select(u => u.Address).FirstOrDefault(),
                        Email = x.Select(u => u.Email).FirstOrDefault(),
                        TrackingNo = x.Select(u => u.TrackingNo).FirstOrDefault(),
                        LabelFilePath = x.Select(u => u.LabelFilePath).FirstOrDefault(),
                        LabelFileExtension = x.Select(u => u.LabelFileExtension).FirstOrDefault(),
                        PickedDate = x.Select(u => u.PickedDate).FirstOrDefault(),
                        PackedDate = x.Select(u => u.PackedDate).FirstOrDefault(),
                        ShipmentLines = x.ToList(),
                    };
                }).FirstOrDefault();

                foreach (var item in response.ShipmentLines)
                {
                    for (int i = 1; i <= item.ShipmentQty; i++)
                    {
                        response.GenerateDetails.Add(new PackingListGenerateRow()
                        {
                            ProductCode = item.ProductCode,
                            ProductName = item.ProductName,
                            ShipmentLineId = item.Id,
                            ExpireDate = item.ExpirationDate?.ToString("yyyy/MM/dd"),
                            ShipmentQty = $"{i}/{item.ShipmentQty}",
                            LotNo = item.LotNo,
                            //Qty = 1,
                            Scanned = i <= item.Remaining ? "0" : "1"
                        });
                    }
                }

                return await Result<WarehousePackingListDto>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehousePackingListDto>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<OrderDispatch>> GetPackingLabelFilePathAsync([Path] string trackingNo)
        {
            try
            {
                if (string.IsNullOrEmpty(trackingNo))
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Tracking NO. Empty.");
                    return await Result<OrderDispatch>.FailAsync(JsonConvert.SerializeObject(err));
                }

                var res = await dbContext.OrderDispatches.FirstOrDefaultAsync(_ => _.TrackingNo == trackingNo);

                if (res == null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "The tracking No could not be found.");
                    return await Result<OrderDispatch>.FailAsync(JsonConvert.SerializeObject(err));
                }

                return await Result<OrderDispatch>.SuccessAsync(res, "Success");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<OrderDispatch>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<string>> GetPdfAsBase64Async([Path] string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "File not found or path is invalid.");
                    return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                }

                // Đọc file PDF
                byte[] pdfBytes = System.IO.File.ReadAllBytes(filePath);

                // Chuyển thành Base64
                string base64String = Convert.ToBase64String(pdfBytes);

                // Trả về kết quả
                return await Result<string>.SuccessAsync(base64String, "Successful");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<string>> UpdateOrderDispatchesFilePathAsync([Body] OrderDispatch model)
        {
            try
            {
                var res = await dbContext.OrderDispatches.FindAsync(model.Id);

                if (res == null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Could be not found data.");
                    return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                }

                res.LabelFilePath = model.LabelFilePath;
                res.LabelFileExtension = model.LabelFilePath;
                await dbContext.SaveChangesAsync();

                return await Result<string>.SuccessAsync("Successful");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<string>> UpdatePackedQtyAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model)
        {
            try
            {
                using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var item in model)
                        {
                            var s = await dbContext.WarehouseShipmentLines.FirstOrDefaultAsync(x => x.Id == item.Id);

                            if (s != null)
                            {
                                s.PackedQty = item.PackedQty;
                                s.Status = item.StatusShipment; //EnumShipmentOrderStatus.Packing
                            }
                            else
                            {
                                var err = new ErrorResponse();
                                err.Errors.Add("Warning", "Shipping Line id in warehouse shipping line could be not found.");
                                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                            }
                            //var whTran = await dbContext.WarehouseTrans.FirstOrDefaultAsync(x => x.TransLineId == item.Id);
                            //if (whTran != null)
                            //{
                            //    whTran.StatusIssue = item.StatusIssueWhTran;//  Packing = 3,
                            //}
                            //else throw new Exception($"Shipping Line id in warehouse tran: {item.Id} could be not found.");
                        }

                        var shipment = await dbContext.WarehouseShipments.FirstOrDefaultAsync(x => x.ShipmentNo == model.FirstOrDefault().ShipmentNo);
                        if (shipment != null)
                        {
                            var mData = model.FirstOrDefault();
                            shipment.Status = mData?.StatusShipment;//EnumShipmentOrderStatus.Packing
                            shipment.ShippingBoxesId = mData?.ShippingBoxesId;
                            shipment.ShippingBoxesName = mData?.ShippingBoxesName;
                        }
                        else
                        {
                            var err = new ErrorResponse();
                            err.Errors.Add("Warning", "The shipping number was not found.");
                            return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                        }

                        await dbContext.SaveChangesAsync();

                        // Commit the transaction
                        await transaction.CommitAsync();

                        return await Result<string>.SuccessAsync("Update packed quantity for shipment line successfully.");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if an error occurs
                        await transaction.RollbackAsync();

                        var err = new ErrorResponse();
                        err.Errors.Add("Error", $"Transaction update packed quantity for shipment line failed: {ex.Message}{Environment.NewLine}{ex.InnerException}");
                        return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                    }
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
