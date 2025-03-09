
using Application.Extentions;
using Application.Services;
using Application.Services.Base;


using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Application.DTOs;
using Newtonsoft.Json;

namespace Infrastructure.Repos
{
    public class RepositoryUserToTenantServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor, UserManager<ApplicationUser> userManager) : IUserToTenant
    {
        public async Task<Result<List<UserToTenant>>> AddRangeAsync([Body] List<UserToTenant> model)
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

                var u2T = await dbContext.UserToTenants.Where(x => x.UserId == model.FirstOrDefault().UserId).ToListAsync();

                using (var tran = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var res = await DeleteRangeAsync(u2T);
                        if (!res.Succeeded)
                        {
                            await tran.RollbackAsync();

                            return await Result<List<UserToTenant>>.FailAsync(res.Messages.FirstOrDefault());
                        }

                        await dbContext.UserToTenants.AddRangeAsync(model);
                        await dbContext.SaveChangesAsync();

                        await tran.CommitAsync();
                        return await Result<List<UserToTenant>>.SuccessAsync(model, "Add range Tenant successfull");
                    }
                    catch (Exception ex)
                    {
                        await tran.RollbackAsync();
                        var err = new ErrorResponse();
                        err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");

                        return await Result<List<UserToTenant>>.FailAsync(JsonConvert.SerializeObject(err));
                    }
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<UserToTenant>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<UserToTenant>> DeleteRangeAsync([Body] List<UserToTenant> model)
        {
            try
            {
                dbContext.UserToTenants.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserToTenant>.SuccessAsync("Delete range Tenant successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<UserToTenant>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<UserToTenant>> DeleteAsync([Body] UserToTenant model)
        {
            try
            {
                dbContext.UserToTenants.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserToTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<UserToTenant>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<UserToTenant>>> GetAllAsync()
        {
            try
            {
                return await Result<List<UserToTenant>>.SuccessAsync(await dbContext.UserToTenants.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<UserToTenant>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<UserToTenant>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<UserToTenant>.SuccessAsync(await dbContext.UserToTenants.FindAsync(id));
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<UserToTenant>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<UserToTenant>> InsertAsync([Body] UserToTenant model)
        {
            try
            {
                await dbContext.UserToTenants.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserToTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<UserToTenant>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<UserToTenant>> UpdateAsync([Body] UserToTenant model)
        {
            try
            {
                dbContext.UserToTenants.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserToTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<UserToTenant>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        async Task<Result<List<UserToTenant>>> IUserToTenant.GetByUserIdAsync([Path] string userId)
        {
            try
            {
                return await Result<List<UserToTenant>>.SuccessAsync(await dbContext.UserToTenants.Where(x => x.UserId == userId).ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<UserToTenant>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public Task<List<UserDto>> GetUsersAsync() => userManager.Users.Select(x => new UserDto(x)).ToListAsync();
    }
}
