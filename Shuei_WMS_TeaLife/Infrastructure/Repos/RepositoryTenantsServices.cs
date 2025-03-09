using Application.Extentions;
using Application.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryTenantsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : ITenants
    {
        public async Task<Result<List<TenantAuth>>> AddRangeAsync([Body] List<TenantAuth> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                //foreach (var item in model)
                //{
                //    item.CreateAt = DateTime.Now;
                //    item.CreateOperatorId = userInfo?.Id;
                //    item.Status = EnumStatus.Activated;

                //}

                await dbContext.TenantAuth.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<TenantAuth>>.SuccessAsync(model, "Add range Tenant successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<TenantAuth>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<TenantAuth>> DeleteRangeAsync([Body] List<TenantAuth> model)
        {
            try
            {
                dbContext.TenantAuth.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<TenantAuth>.SuccessAsync("Delete range Tenant successfull");
            }
            catch (Exception ex)
            {
                return await Result<TenantAuth>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<TenantAuth>>> GetAllAsync()
        {
            try
            {
                return await Result<List<TenantAuth>>.SuccessAsync(await dbContext.TenantAuth.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<TenantAuth>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<TenantAuth>> GetByIdAsync([Path] int id)
        {
            try
            {
                var result = await dbContext.TenantAuth.FindAsync(id);
                return await Result<TenantAuth>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<TenantAuth>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<TenantAuth>> InsertAsync([Body] TenantAuth model)
        {
            try
            {
                await dbContext.TenantAuth.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<TenantAuth>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<TenantAuth>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<TenantAuth>> UpdateAsync([Body] TenantAuth model)
        {
            try
            {
                var dataUpdate = dbContext.TenantAuth.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<TenantAuth>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<TenantAuth>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<TenantAuth>> DeleteAsync([Body] TenantAuth model)
        {
            try
            {
                dbContext.TenantAuth.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<TenantAuth>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<TenantAuth>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
