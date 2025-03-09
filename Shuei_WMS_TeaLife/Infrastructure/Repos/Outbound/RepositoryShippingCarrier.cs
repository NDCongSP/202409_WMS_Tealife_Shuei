using Application.Extentions;
using Application.Services.Outbound;

using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;
using static Application.Extentions.ApiRoutes;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryShippingCarrier(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) :IShippingCarrier
    {
        public async Task<Result<List<ShippingCarrier>>> AddRangeAsync([Body] List<ShippingCarrier> model)
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

                await dbContext.ShippingCarriers.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<ShippingCarrier>>.SuccessAsync(model, "Add range ShippingCarrier successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ShippingCarrier>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ShippingCarrier>> DeleteRangeAsync([Body] List<ShippingCarrier> model)
        {
            try
            {
                dbContext.ShippingCarriers.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingCarrier>.SuccessAsync("Delete range ShippingCarrier successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ShippingCarrier>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ShippingCarrier>> DeleteAsync([Body] ShippingCarrier model)
        {
            try
            {
                dbContext.ShippingCarriers.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingCarrier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ShippingCarrier>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<ShippingCarrier>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ShippingCarrier>>.SuccessAsync(await dbContext.ShippingCarriers.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ShippingCarrier>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ShippingCarrier>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<ShippingCarrier>.SuccessAsync(await dbContext.ShippingCarriers.FindAsync(id));
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ShippingCarrier>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ShippingCarrier>> InsertAsync([Body] ShippingCarrier model)
        {
            try
            {
                //check required
                if (await CheckExistShippingCarrier(model))
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "The data exists.");
                    return await Result<ShippingCarrier>.FailAsync(JsonConvert.SerializeObject(err));
                }

                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateAt = DateTime.Now;
                model.CreateOperatorId = userInfo.Id;

                await dbContext.ShippingCarriers.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingCarrier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ShippingCarrier>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ShippingCarrier>> UpdateAsync([Body] ShippingCarrier model)
        {
            try
            {
                //check required
                if (await CheckExistShippingCarrier(model))
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "The data exists.");
                    return await Result<ShippingCarrier>.FailAsync(JsonConvert.SerializeObject(err));
                }

                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateAt = DateTime.Now;
                model.UpdateOperatorId = userInfo.Id;

                dbContext.ShippingCarriers.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingCarrier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ShippingCarrier>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        private async Task<bool> CheckExistShippingCarrier(ShippingCarrier shippingCarrier)
        {
            return await dbContext.ShippingCarriers.AnyAsync(x => x.Id != shippingCarrier.Id && x.ShippingCarrierCode.ToLower() == shippingCarrier.ShippingCarrierCode.ToLower());
        }
    }
}
