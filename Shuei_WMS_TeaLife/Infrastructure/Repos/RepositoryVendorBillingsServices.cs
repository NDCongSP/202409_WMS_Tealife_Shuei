
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
    public class RepositoryVendorBillingsServices(ApplicationDbContext dbContext,IHttpContextAccessor  contextAccessor) : IVendorBillings
    {
        public async Task<Result<List<VendorBilling>>> AddRangeAsync([Body] List<VendorBilling> model)
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

                await dbContext.VendorBillings.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<VendorBilling>>.SuccessAsync(model, "Add range VendorBillingDetail successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<VendorBilling>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<VendorBilling>> DeleteRangeAsync([Body] List<VendorBilling> model)
        {
            try
            {
                dbContext.VendorBillings.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<VendorBilling>.SuccessAsync("Delete range VendorBilling successfull");
            }
            catch (Exception ex)
            {
                return await Result<VendorBilling>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<VendorBilling>> DeleteAsync([Body] VendorBilling model)
        {
            try
            {
                dbContext.VendorBillings.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<VendorBilling>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<VendorBilling>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<VendorBilling>>> GetAllAsync()
        {
            try
            {
                return await Result<List<VendorBilling>>.SuccessAsync(await dbContext.VendorBillings.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<VendorBilling>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<VendorBilling>> GetByIdAsync([Path] int id)
        {
            try
            {
                return await Result<VendorBilling>.SuccessAsync(await dbContext.VendorBillings.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<VendorBilling>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<VendorBilling>> InsertAsync([Body] VendorBilling model)
        {
            try
            {
                dbContext.VendorBillings.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<VendorBilling>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<VendorBilling>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<VendorBilling>> UpdateAsync([Body] VendorBilling model)
        {
            try
            {
                dbContext.VendorBillings.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<VendorBilling>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<VendorBilling>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
