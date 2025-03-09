using Application.DTOs;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;


namespace Infrastructure.Repos
{
    public class RepositoryCategoryServices(ApplicationDbContext dbContext) : ICategories
    {
        public async Task<Result<List<SelectListItem>>> GetUserDropdown()
        {
            var err = new ErrorResponse();
            try
            {
                var result = await dbContext.ApplicationUsers.AsNoTracking()
                    .Select(x => new SelectListItem
                    {
                        Text = x.FullName,
                        Value = x.Id
                    }).ToListAsync();
                return await Result<List<SelectListItem>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<SelectListItem>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<SelectListItem>>> GetBinByLocation(string? locationId = default)
        {
            var err = new ErrorResponse();
            try
            {
                var result = await dbContext.Bins.AsNoTracking()
                    .Where(x => x.LocationId.ToString() == locationId)
                    .Select(x => new SelectListItem
                    {
                        Text = x.BinCode,
                        Value = x.BinCode
                    }).ToListAsync();
                return await Result<List<SelectListItem>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<SelectListItem>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<List<OrderSelectList>> GetOrders(string? orderId = default)
        {
            try
            {
                var shipmentOrders = await dbContext.WarehouseShipments
                    .Where(x => x.IsDeleted == false && !string.IsNullOrEmpty(x.SalesNo)).Select(x => x.SalesNo).Distinct().ToListAsync();
                shipmentOrders = shipmentOrders.Except(new List<string?> { orderId }).ToList();
                //var result = await dbContext.OrderDispatches.AsNoTracking()
                //    .Where(x => !shipmentOrders.Contains(x.OrderId)).Select(x => new OrderSelectList
                //    {
                //        OrderId = x.OrderId,
                //        TrackingNo = x.TrackingNo,
                //    }).ToListAsync();
                var result = await dbContext.Orders.AsNoTracking()
                    .Where(x => !shipmentOrders.Contains(x.OrderId)
                        && (orderId == x.OrderId || x.OrderStatus == "20"))
                    .Join(dbContext.OrderDispatches,
                        x => x.OrderId,
                        y => y.OrderId,
                        (x, y) => new OrderSelectList
                        {
                            OrderDispatchId = y.Id,
                            OrderId = x.OrderId,
                            TrackingNo = y.TrackingNo,
                        }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return new List<OrderSelectList>();
            }
        }

    }
}
