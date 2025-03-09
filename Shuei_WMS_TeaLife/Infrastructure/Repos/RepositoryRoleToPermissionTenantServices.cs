
using Application.Extentions;
using Application.Services.Authen;

using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryRoleToPermissionTenantTenantServices(ApplicationDbContext dbContext ,IHttpContextAccessor contextAccessor):IRoleToPermissionTenant
    {
        public async Task<Result<List<RoleToPermissionTenant>>> AddRangeAsync([Body] List<RoleToPermissionTenant> model)
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

                await dbContext.RoleToPermissionTenants.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<RoleToPermissionTenant>>.SuccessAsync(model, "Add range RoleToPermissionTenant successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<RoleToPermissionTenant>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<RoleToPermissionTenant>> DeleteRangeAsync([Body] List<RoleToPermissionTenant> model)
        {
            try
            {
                dbContext.RoleToPermissionTenants.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<RoleToPermissionTenant>.SuccessAsync("Delete range RoleToPermissionTenant successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<RoleToPermissionTenant>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<RoleToPermissionTenant>> DeleteAsync([Body] RoleToPermissionTenant model)
        {
            try
            {
                dbContext.RoleToPermissionTenants.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<RoleToPermissionTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<RoleToPermissionTenant>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<RoleToPermissionTenant>>> GetAllAsync()
        {
            try
            {
                return await Result<List<RoleToPermissionTenant>>.SuccessAsync(await dbContext.RoleToPermissionTenants.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<RoleToPermissionTenant>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<RoleToPermissionTenant>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<RoleToPermissionTenant>.SuccessAsync(await dbContext.RoleToPermissionTenants.FindAsync(id));
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<RoleToPermissionTenant>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<RoleToPermissionTenant>> InsertAsync([Body] RoleToPermissionTenant model)
        {
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateAt=DateTime.Now;
                model.CreateOperatorId = user?.Id;

                dbContext.RoleToPermissionTenants.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<RoleToPermissionTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<RoleToPermissionTenant>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<RoleToPermissionTenant>> UpdateAsync([Body] RoleToPermissionTenant model)
        {
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateAt = DateTime.Now;
                model.UpdateOperatorId = user?.Id;

                dbContext.RoleToPermissionTenants.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<RoleToPermissionTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<RoleToPermissionTenant>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
