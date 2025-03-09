
using Application.Extentions;
using Application.Services.Authen;

using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryPermissionTenantServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IPermissionTenant
    {
        public async Task<Result<List<PermissionsTenant>>> AddRangeAsync([Body] List<PermissionsTenant> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;
                    item.Status = EnumStatus.Activated;

                }

                await dbContext.PermissionsTenants.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<PermissionsTenant>>.SuccessAsync(model, "Add range Permissions Tenants successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<PermissionsTenant>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<PermissionsTenant>> DeleteRangeAsync([Body] List<PermissionsTenant> model)
        {
            try
            {
                dbContext.PermissionsTenants.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<PermissionsTenant>.SuccessAsync("Delete range Permissions Tenants successfull");
            }
            catch (Exception ex)
            {
                return await Result<PermissionsTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<PermissionsTenant>> DeleteAsync([Body] PermissionsTenant model)
        {
            try
            {
                dbContext.PermissionsTenants.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<PermissionsTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<PermissionsTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<PermissionsTenant>>> GetAllAsync()
        {
            try
            {
                return await Result<List<PermissionsTenant>>.SuccessAsync(await dbContext.PermissionsTenants.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<PermissionsTenant>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<PermissionsTenant>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<PermissionsTenant>.SuccessAsync(await dbContext.PermissionsTenants.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<PermissionsTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<PermissionsTenant>> InsertAsync([Body] PermissionsTenant model)
        {
            try
            {
                await dbContext.PermissionsTenants.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<PermissionsTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<PermissionsTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<PermissionsTenant>> UpdateAsync([Body] PermissionsTenant model)
        {
            try
            {
                dbContext.PermissionsTenants.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<PermissionsTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<PermissionsTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
