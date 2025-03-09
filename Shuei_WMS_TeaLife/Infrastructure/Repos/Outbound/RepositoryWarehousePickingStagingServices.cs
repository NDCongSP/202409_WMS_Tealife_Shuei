using Application.Extentions;
using Application.Services.Outbound;

using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryWarehousePickingStagingServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehousePickingStaging
    {
        public async Task<Result<List<WarehousePickingStaging>>> AddRangeAsync([Body] List<WarehousePickingStaging> model)
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

                await dbContext.WarehousePickingStagings.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<WarehousePickingStaging>>.SuccessAsync(model, "Add range WarehousePickingStaging successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingStaging>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingStaging>> DeleteRangeAsync([Body] List<WarehousePickingStaging> model)
        {
            try
            {
                dbContext.WarehousePickingStagings.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingStaging>.SuccessAsync("Delete range WarehousePickingStaging successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingStaging>> DeleteAsync([Body] WarehousePickingStaging model)
        {
            try
            {
                dbContext.WarehousePickingStagings.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingStaging>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehousePickingStaging>>.SuccessAsync(await dbContext.WarehousePickingStagings.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingStaging>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingStaging>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePickingStaging>.SuccessAsync(await dbContext.WarehousePickingStagings.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingStaging>> InsertAsync([Body] WarehousePickingStaging model)
        {
            try
            {
                await dbContext.WarehousePickingStagings.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePickingStaging>> UpdateAsync([Body] WarehousePickingStaging model)
        {
            try
            {
                dbContext.WarehousePickingStagings.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePickingStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePickingStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePickingStaging>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            try
            {
                return await Result<List<WarehousePickingStaging>>.SuccessAsync(await dbContext.WarehousePickingStagings.Where(x => x.PickNo == putAwayNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePickingStaging>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
