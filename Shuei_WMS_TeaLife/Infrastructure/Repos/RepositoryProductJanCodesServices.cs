
using Application.DTOs.Response;
using Application.Extentions;
using Application.Services;

using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryProductJanCodesServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IProductJanCodes
    {
        public async Task<Result<List<ProductJanCode>>> AddRangeAsync([Body] List<ProductJanCode> model)
        {
            var err = new ErrorResponse();
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

                await dbContext.ProductJanCodes.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<ProductJanCode>>.SuccessAsync(model, "Add range ProductJanCodes successfull");
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ProductJanCode>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductJanCode>> DeleteRangeAsync([Body] List<ProductJanCode> model)
        {
            var err = new ErrorResponse();
            try
            {
                dbContext.ProductJanCodes.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductJanCode>.SuccessAsync("Delete range ProductJanCodes successfull");
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductJanCode>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<ProductJanCode>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ProductJanCode>>.SuccessAsync(await dbContext.ProductJanCodes.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ProductJanCode>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductJanCode>> GetByIdAsync([Path] int id)
        {
            var err=new ErrorResponse();
            try
            {
                var result = await dbContext.ProductJanCodes.FindAsync(id);
                return await Result<ProductJanCode>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductJanCode>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductJanCode>> InsertAsync([Body] ProductJanCode model)
        {
            var err = new ErrorResponse();
            try
            {
                var existCD = await dbContext.ProductJanCodes.Where(x => x.ProductId == model.ProductId && x.JanCode == model.JanCode).FirstOrDefaultAsync();
                if (existCD != null)
                {
                    return await Result<ProductJanCode>.FailAsync($"Product JAN code: {model.JanCode} is already created");
                }

                await dbContext.ProductJanCodes.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductJanCode>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductJanCode>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductJanCode>> UpdateAsync([Body] ProductJanCode model)
        {
            var err = new ErrorResponse();
            try
            {
                var dataUpdate = dbContext.ProductJanCodes.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductJanCode>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductJanCode>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }


        public async Task<Result<List<ProductJanCode>>> GetByProductId([Path] int productId)
        {
            var err = new ErrorResponse();
            try
            {
                return await Result<List<ProductJanCode>>.SuccessAsync(await dbContext.ProductJanCodes.Where(m => m.ProductId == productId).ToListAsync());

            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ProductJanCode>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductJanCode>> DeleteAsync([Body] ProductJanCode model)
        {
            var err = new ErrorResponse();
            try
            {
                dbContext.ProductJanCodes.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductJanCode>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductJanCode>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<ProductJanCode>>> AddOrUpdateAsync([Body] List<ProductJanCode> model)
        {
            var err = new ErrorResponse();
            try
            {
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                dbContext.ProductJanCodes.UpdateRange(model);
                await dbContext.SaveChangesAsync();

                return await Result<List<ProductJanCode>>.SuccessAsync(model, $"Successfull");
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ProductJanCode>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
