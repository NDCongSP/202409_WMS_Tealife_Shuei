using Application.DTOs.Request.Picking;
using Application.DTOs;
using Application.Extentions;
using Application.Services.Outbound;

using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehousePickingLineServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehousePickingLine
    {
        private readonly ILogger<RepositoryWarehousePickingLineServices> logger;
        public async Task<Result<List<WarehousePickingLine>>> AddRangeAsync([Body] List<WarehousePickingLine> model)
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

                await dbContext.WarehousePickingLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<WarehousePickingLine>>.SuccessAsync(model, "Add range WarehousePickingLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingLine>> DeleteRangeAsync([Body] List<WarehousePickingLine> model)
        {
            try
            {
                dbContext.WarehousePickingLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingLine>.SuccessAsync("Delete range WarehousePickingLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingLine>> DeleteAsync([Body] WarehousePickingLine model)
        {
            try
            {
                dbContext.WarehousePickingLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingLine>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehousePickingLine>>.SuccessAsync(await dbContext.WarehousePickingLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingLine>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePickingLine>.SuccessAsync(await dbContext.WarehousePickingLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingLine>> InsertAsync([Body] WarehousePickingLine model)
        {
            try
            {
                await dbContext.WarehousePickingLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingLine>> UpdateAsync([Body] WarehousePickingLine model)
        {
            try
            {
                dbContext.WarehousePickingLines.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingLine>>> GetByMasterCodeAsync([Path] string pickingNo)
        {
            try
            {
                return await Result<List<WarehousePickingLine>>.SuccessAsync(await dbContext.WarehousePickingLines.Where(x => x.PickNo == pickingNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        
        public async Task<Result<List<WarehousePickingLineDTO>>> GetPickingLineDTOAsync([Path] string pickNo)
        {
            try
            {
                var pickingLines = await dbContext.WarehousePickingLines
                                  .Where(x => x.PickNo == pickNo)
                                  .ToListAsync();
                var pickingLineDTOs = new List<WarehousePickingLineDTO>();

                foreach (var pickingLine in pickingLines)
                {
                    var product = dbContext.Products.FirstOrDefault(p => p.ProductCode == pickingLine.ProductCode);
                    var unit = dbContext.Units.FirstOrDefault(p => p.Id == pickingLine.UnitId);
                    var dto = new WarehousePickingLineDTO()
                    {
                        Id = pickingLine.Id,
                        PickNo = pickingLine.PickNo != null ? pickingLine.PickNo : "",
                        ShipmentNo = pickingLine.ShipmentNo != null ? pickingLine.ShipmentNo : "",
                        ProductCode = pickingLine.ProductCode != null ? pickingLine.ProductCode : "",
                        ProductName = product != null ? product.ProductIname : "",
                        Unit = unit != null ? unit.UnitName : "",
                        Lot = pickingLine.LotNo != null ? pickingLine.LotNo : "",
                        Bin = pickingLine.Bin != null ? pickingLine.Bin : "",
                        InstructionsNumber = 10,
                        PickQty = pickingLine.PickQty,
                        ActualQty = pickingLine.ActualQty,
                        ExpirationDate = pickingLine.ExpirationDate
                    };
                    pickingLineDTOs.Add(dto);
                }
                return await Result<List<WarehousePickingLineDTO>>.SuccessAsync(pickingLineDTOs.ToList());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingLineDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingShipmentDTO>>> GetShipmentsByPickAsync([Path] string pickingNo)
        {
            // Validate input data
            if (string.IsNullOrWhiteSpace(pickingNo))
            {
                return await Result<List<WarehousePickingShipmentDTO>>.FailAsync("Picking number cannot be null or empty.");
            }

            try
            {
                // Retrieve the list of shipments
                var shipments = await dbContext.WarehouseShipments
                    .Where(x => x.PickingNo == pickingNo)
                    .ToListAsync();

                var pickingShipmentDTOs = new List<WarehousePickingShipmentDTO>();

                foreach (var shipment in shipments)
                {
                    var shipmentLines = await dbContext.WarehouseShipmentLines
                            .Where(x => x.ShipmentNo == shipment.ShipmentNo)
                            .ToListAsync();

                    double totalQuantity = 0;

                    foreach (var shipmentLine in shipmentLines)
                    {
                        if (shipmentLine.ShipmentQty != null && shipmentLine.ShipmentQty > 0)
                        {
                            totalQuantity += shipmentLine.ShipmentQty.Value;
                        }
                    }

                    var tenant = dbContext.Companies.FirstOrDefault(x => x.AuthPTenantId == shipment.TenantId);
                    var order = dbContext.Orders.FirstOrDefault(x => x.OrderId == shipment.SalesNo);
                    var pickingShipmentDTO = new WarehousePickingShipmentDTO()
                    {
                        OrderNo = shipment.SalesNo ?? "",
                        ShipmentNo = shipment.ShipmentNo,
                        TotalQuantity = totalQuantity,
                        TenantFullName = tenant?.FullName ?? "",
                        OrderDeliveryCompany = shipment.ShippingCarrierCode ?? "",
                        OrderDate = order?.OrderDate.HasValue == true ? DateOnly.FromDateTime(order.OrderDate.Value) : (DateOnly?)null,
                        PlanShipDate = shipment.PlanShipDate,
                    };

                    pickingShipmentDTOs.Add(pickingShipmentDTO);
                }

                // Return success result after processing all shipments
                return await Result<List<WarehousePickingShipmentDTO>>.SuccessAsync(pickingShipmentDTOs);
            }
            catch (Exception ex)
            {
                // Log the error (assuming you have a logger)
                logger.LogError(ex, "An error occurred while fetching shipments for picking number: {PickingNo}", pickingNo);
                return await Result<List<WarehousePickingShipmentDTO>>.FailAsync($"Error fetching shipments: {ex.Message}");
            }
        }
        public async Task<Result> UpdateWarehousePickingLinesAsync(List<WarehousePickingLineDTO> models)
        {
            try
            {
                foreach (var model in models)
                {
                    var result = await dbContext.WarehousePickingLines
                        .FirstOrDefaultAsync(x => x.Id == model.Id);

                    if (result != null)
                    {
                        result.ActualQty = model.ActualQty;
                        result.LotNo = model.Lot;
                        result.Bin = model.Bin;
                        result.ExpirationDate = model.ExpirationDate;
                        dbContext.WarehousePickingLines.Update(result);
                    }
                }

                await dbContext.SaveChangesAsync();

                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                return await Result.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
