
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
    public class RepositoryCurrencyPairSettingServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : ICurrencyPairSetting
    {
        public async Task<Result<List<CurrencyPairSetting>>> AddRangeAsync([Body] List<CurrencyPairSetting> model)
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

                await dbContext.CurrencyPairSettings.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<CurrencyPairSetting>>.SuccessAsync(model, "Add range CurrencyPairSetting successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<CurrencyPairSetting>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<CurrencyPairSetting>> DeleteRangeAsync([Body] List<CurrencyPairSetting> model)
        {
            try
            {
                dbContext.CurrencyPairSettings.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<CurrencyPairSetting>.SuccessAsync("Delete range CurrencyPairSetting successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CurrencyPairSetting>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<CurrencyPairSetting>> DeleteAsync([Body] CurrencyPairSetting model)
        {
            try
            {
                dbContext.CurrencyPairSettings.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<CurrencyPairSetting>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CurrencyPairSetting>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<CurrencyPairSetting>>> GetAllAsync()
        {
            try
            {
                return await Result<List<CurrencyPairSetting>>.SuccessAsync(await dbContext.CurrencyPairSettings.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<CurrencyPairSetting>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<CurrencyPairSetting>> GetByIdAsync([Path] int id)
        {
            try
            {
                return await Result<CurrencyPairSetting>.SuccessAsync(await dbContext.CurrencyPairSettings.FindAsync(id));
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CurrencyPairSetting>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<CurrencyPairSetting>> InsertAsync([Body] CurrencyPairSetting model)
        {
            try
            {
                dbContext.CurrencyPairSettings.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<CurrencyPairSetting>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CurrencyPairSetting>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<CurrencyPairSetting>> UpdateAsync([Body] CurrencyPairSetting model)
        {
            try
            {
                dbContext.CurrencyPairSettings.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<CurrencyPairSetting>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CurrencyPairSetting>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
