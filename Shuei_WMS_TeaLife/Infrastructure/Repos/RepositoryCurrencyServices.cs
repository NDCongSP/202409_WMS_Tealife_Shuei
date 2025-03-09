using Application.DTOs.Response;
using Application.Extentions;
using Application.Services;

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
    public class RepositoryCurrencyServices(ApplicationDbContext dbContext,IHttpContextAccessor contextAccessor) : ICurrency
    {
        public async Task<Result<List<Currency>>> AddRangeAsync([Body] List<Currency> model)
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

                await dbContext.Currencies.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<Currency>>.SuccessAsync(model, "Add range Currency successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Currency>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<List<Currency>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Currency>> DeleteRangeAsync([Body] List<Currency> model)
        {
            try
            {
                dbContext.Currencies.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Currency>.SuccessAsync("Delete range Currency successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Currency>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<Currency>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Currency>> DeleteAsync([Body] Currency model)
        {
            try
            {
                dbContext.Currencies.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Currency>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Currency>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<Currency>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Currency>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Currency>>.SuccessAsync(await dbContext.Currencies.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Currency>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<List<Currency>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Currency>> GetByIdAsync([Path] string id)
        {
            try
            {
                return await Result<Currency>.SuccessAsync(await dbContext.Currencies.FindAsync(id));
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Currency>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<Currency>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Currency>> InsertAsync([Body] Currency model)
        {
            try
            {
                dbContext.Currencies.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Currency>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Currency>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<Currency>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Currency>> UpdateAsync([Body] Currency model)
        {
            try
            {
                dbContext.Currencies.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Currency>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Currency>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<Currency>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
