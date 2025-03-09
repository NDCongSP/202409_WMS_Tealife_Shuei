
using Application.Extentions;
using Application.Services.Vendors;


using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryUserVendorServices(ApplicationDbContext dbContext,IHttpContextAccessor  contextAccessor) : IUserVendors
    {
        public async Task<Result<List<UserVendor>>> AddRangeAsync([Body] List<UserVendor> model)
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

                await dbContext.UserVendors.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<UserVendor>>.SuccessAsync(model, "Add range UserVendor successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<UserVendor>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<UserVendor>> DeleteRangeAsync([Body] List<UserVendor> model)
        {
            try
            {
                dbContext.UserVendors.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserVendor>.SuccessAsync("Delete range UserVendor successfull");
            }
            catch (Exception ex)
            {
                return await Result<UserVendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<UserVendor>> DeleteAsync([Body] UserVendor model)
        {
            try
            {
                dbContext.UserVendors.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserVendor>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<UserVendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<UserVendor>>> GetAllAsync()
        {
            try
            {
                return await Result<List<UserVendor>>.SuccessAsync(await dbContext.UserVendors.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<UserVendor>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<UserVendor>> GetByIdAsync([Path] string id)
        {
            try
            {
                return await Result<UserVendor>.SuccessAsync(await dbContext.UserVendors.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<UserVendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<UserVendor>> InsertAsync([Body] UserVendor model)
        {
            try
            {
                dbContext.UserVendors.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserVendor>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<UserVendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<UserVendor>> UpdateAsync([Body] UserVendor model)
        {
            try
            {
                dbContext.UserVendors.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<UserVendor>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<UserVendor>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
