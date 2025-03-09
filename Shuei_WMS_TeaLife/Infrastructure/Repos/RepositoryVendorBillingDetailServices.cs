
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
    public class RepositoryVendorBillingDetailServices(ApplicationDbContext dbContext,IHttpContextAccessor  contextAccessor) : IVendorBillingDetail
    {
        public async Task<Result<List<VendorBillingDetail>>> AddRangeAsync([Body] List<VendorBillingDetail> model)
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

                await dbContext.VendorBillingDetails.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<VendorBillingDetail>>.SuccessAsync(model, "Add range VendorBillingDetail successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<VendorBillingDetail>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<VendorBillingDetail>> DeleteRangeAsync([Body] List<VendorBillingDetail> model)
        {
            try
            {
                dbContext.VendorBillingDetails.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<VendorBillingDetail>.SuccessAsync("Delete range VendorBillingDetail successfull");
            }
            catch (Exception ex)
            {
                return await Result<VendorBillingDetail>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<VendorBillingDetail>> DeleteAsync([Body] VendorBillingDetail model)
        {
            try
            {
                dbContext.VendorBillingDetails.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<VendorBillingDetail>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<VendorBillingDetail>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<VendorBillingDetail>>> GetAllAsync()
        {
            try
            {
                return await Result<List<VendorBillingDetail>>.SuccessAsync(await dbContext.VendorBillingDetails.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<VendorBillingDetail>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<VendorBillingDetail>> GetByIdAsync([Path] int id)
        {
            try
            {
                return await Result<VendorBillingDetail>.SuccessAsync(await dbContext.VendorBillingDetails.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<VendorBillingDetail>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<VendorBillingDetail>> InsertAsync([Body] VendorBillingDetail model)
        {
            try
            {
                dbContext.VendorBillingDetails.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<VendorBillingDetail>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<VendorBillingDetail>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<VendorBillingDetail>> UpdateAsync([Body] VendorBillingDetail model)
        {
            try
            {
                dbContext.VendorBillingDetails.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<VendorBillingDetail>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<VendorBillingDetail>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
