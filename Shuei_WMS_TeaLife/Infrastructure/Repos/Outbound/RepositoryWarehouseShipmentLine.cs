using Application.Extentions;
using Application.Services.Outbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehouseShipmentLine(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehouseShipmentLine
    {
        public async Task<Result<List<WarehouseShipmentLine>>> AddRangeAsync([Body] List<WarehouseShipmentLine> model)
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

                await dbContext.WarehouseShipmentLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<WarehouseShipmentLine>>.SuccessAsync(model, "Add range WarehouseShipmentLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseShipmentLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentLine>> DeleteRangeAsync([Body] List<WarehouseShipmentLine> model)
        {
            try
            {
                dbContext.WarehouseShipmentLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipmentLine>.SuccessAsync("Delete range WarehouseShipmentLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentLine>> DeleteAsync([Body] WarehouseShipmentLine model)
        {
            try
            {
                dbContext.WarehouseShipmentLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipmentLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseShipmentLine>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseShipmentLine>>.SuccessAsync(await dbContext.WarehouseShipmentLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseShipmentLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentLine>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseShipmentLine>.SuccessAsync(await dbContext.WarehouseShipmentLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentLine>> InsertAsync([Body] WarehouseShipmentLine model)
        {
            try
            {
                await dbContext.WarehouseShipmentLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipmentLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentLine>> UpdateAsync([Body] WarehouseShipmentLine model)
        {
            try
            {
                dbContext.WarehouseShipmentLines.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseShipmentLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseShipmentLine>>> GetByMasterCodeAsync([Path] string shipmentNo)
        {
            try
            {
                return await Result<List<WarehouseShipmentLine>>.SuccessAsync(await dbContext.WarehouseShipmentLines.Where(x => x.ShipmentNo == shipmentNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseShipmentLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseShipmentLine>> GetByProducCodetAsync([Path] string productCode)
        {
            try
            {
                return await Result<WarehouseShipmentLine>.SuccessAsync(await dbContext.WarehouseShipmentLines.Where(x => x.ProductCode == productCode).FirstOrDefaultAsync());
            }
            catch (Exception ex)
            {
                return await Result<WarehouseShipmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
