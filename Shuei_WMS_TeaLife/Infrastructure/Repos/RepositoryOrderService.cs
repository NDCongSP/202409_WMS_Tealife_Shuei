using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class RepositoryOrderService : IOrder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessore;

        public RepositoryOrderService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessore)
        {
            _dbContext = dbContext;
            _contextAccessore = contextAccessore;
        }

        public async Task<Result<List<OrderReportDto>>> GetOrderReport()
        {
            // Query for Pending status
            var pendingQuery = 
                from order in _dbContext.Orders
                join tenant in _dbContext.TenantAuth 
                    on order.CompanyId equals tenant.TenantId
                where !_dbContext.OrderDispatches.Any(d => d.OrderId == order.OrderId)
                group order by new { order.CompanyId, tenant.TenantFullName } into g
                select new OrderReportDto
                {
                    TenantId = g.Key.CompanyId,
                    TenantName = g.Key.TenantFullName,
                    Status = "Pending",
                    QtyOrderedIsPending = g.Count()
                };

            // Query for Out of Stock status
            var outOfStockQuery = 
                from order in _dbContext.Orders
                join dispatch in _dbContext.OrderDispatches 
                    on order.OrderId equals dispatch.OrderId
                join tenant in _dbContext.TenantAuth 
                    on order.CompanyId equals tenant.TenantId
                where dispatch.StockUpStatus == -1
                group order by new { order.CompanyId, tenant.TenantFullName } into g
                select new OrderReportDto
                {
                    TenantId = g.Key.CompanyId,
                    TenantName = g.Key.TenantFullName,
                    Status = "OutOfStock",
                    QtyOrderedIsPending = g.Count()
                };

            // Query for FDA status
            var fdaQuery = 
                from order in _dbContext.Orders
                join dispatch in _dbContext.OrderDispatches 
                    on order.OrderId equals dispatch.OrderId
                join tenant in _dbContext.TenantAuth 
                    on order.CompanyId equals tenant.TenantId
                where dispatch.FdaRegistrationStatus == 1
                group order by new { order.CompanyId, tenant.TenantFullName } into g
                select new OrderReportDto
                {
                    TenantId = g.Key.CompanyId,
                    TenantName = g.Key.TenantFullName,
                    Status = "FDA",
                    QtyOrderedIsPending = g.Count()
                };

            // Query for Order Confirmation status
            var confirmationQuery = 
                from order in _dbContext.Orders
                join tenant in _dbContext.TenantAuth 
                    on order.CompanyId equals tenant.TenantId
                where order.OnHoldStatus == 1
                group order by new { order.CompanyId, tenant.TenantFullName } into g
                select new OrderReportDto
                {
                    TenantId = g.Key.CompanyId,
                    TenantName = g.Key.TenantFullName,
                    Status = "OrderConfirmation",
                    QtyOrderedIsPending = g.Count()
                };

            // Combine all queries and execute
            var result = await pendingQuery
                .Union(outOfStockQuery)
                .Union(fdaQuery)
                .Union(confirmationQuery)
                .ToListAsync();

            return Result<List<OrderReportDto>>.Success(result);
        }
    }
}
