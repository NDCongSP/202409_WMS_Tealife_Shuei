
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
    public class RepositoryRoleToPermissionsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IRoleToPermissions
    {
        public async Task<Result<List<RoleToPermission>>> AddRangeAsync([Body] List<RoleToPermission> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo!.Id;
                    item.Status = EnumStatus.Activated;

                }

                await dbContext.RoleToPermissions.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<RoleToPermission>>.SuccessAsync(model, "Add range RoleToPermissions successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<RoleToPermission>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<RoleToPermission>> DeleteRangeAsync([Body] List<RoleToPermission> model)
        {
            try
            {
                dbContext.RoleToPermissions.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<RoleToPermission>.SuccessAsync("Delete range RoleToPermissions successfull");
            }
            catch (Exception ex)
            {
                return await Result<RoleToPermission>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<RoleToPermission>> DeleteAsync([Body] RoleToPermission model)
        {
            try
            {
                dbContext.RoleToPermissions.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<RoleToPermission>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<RoleToPermission>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<RoleToPermission>>> GetAllAsync()
        {
            try
            {
                return await Result<List<RoleToPermission>>.SuccessAsync(await dbContext.RoleToPermissions.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<RoleToPermission>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<RoleToPermission>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<RoleToPermission>.SuccessAsync(await dbContext.RoleToPermissions.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<RoleToPermission>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<RoleToPermission>> InsertAsync([Body] RoleToPermission model)
        {
            try
            {
                dbContext.RoleToPermissions.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<RoleToPermission>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<RoleToPermission>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<RoleToPermission>> UpdateAsync([Body] RoleToPermission model)
        {
            try
            {
                dbContext.RoleToPermissions.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<RoleToPermission>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<RoleToPermission>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<RoleToPermission>>> GetByPermissionsIdAsync([Path] string id)
        {
            try
            {
                var response = await dbContext.RoleToPermissions.Where(x => x.PermissionId == Guid.Parse(id)).ToListAsync();
                return await Result<List<RoleToPermission>>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                return await Result<List<RoleToPermission>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
