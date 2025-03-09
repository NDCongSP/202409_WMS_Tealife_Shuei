using Application.DTOs.Response;
using Application.Extentions;
using Application.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
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
    public class RepositoryCountryMasterServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : ICountryMaster
    {
        public async Task<Result<List<CountryMaster>>> AddRangeAsync([Body] List<CountryMaster> model)
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

                await dbContext.CountryMasters.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<CountryMaster>>.SuccessAsync(model, "Add range CountryMaster successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<CountryMaster>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<List<CountryMaster>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<CountryMaster>> DeleteAsync([Body] CountryMaster model)
        {
            try
            {
                dbContext.CountryMasters.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<CountryMaster>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CountryMaster>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<CountryMaster>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<CountryMaster>> DeleteRangeAsync([Body] List<CountryMaster> model)
        {
            try
            {
                dbContext.CountryMasters.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<CountryMaster>.SuccessAsync("Delete range country master successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CountryMaster>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<CountryMaster>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<CountryMaster>>> GetAllAsync()
        {
            try
            {
                return await Result<List<CountryMaster>>.SuccessAsync(await dbContext.CountryMasters.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<CountryMaster>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<List<CountryMaster>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<CountryMaster>> GetByIdAsync([Path] int id)
        {
            try
            {
                var result = await dbContext.CountryMasters.FindAsync(id);
                return await Result<CountryMaster>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CountryMaster>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<CountryMaster>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<CountryMaster>> InsertAsync([Body] CountryMaster model)
        {
            try
            {
                var existCD = await dbContext.CountryMasters.Where(x => x.CountryIso2 == model.CountryIso2 || x.CountryNameEn == model.CountryNameEn).FirstOrDefaultAsync();
                if (existCD != null)
                {
                    return await Result<CountryMaster>.FailAsync($"Country master code: {model.CountryIso2} is already created");
                }

                await dbContext.CountryMasters.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<CountryMaster>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CountryMaster>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<CountryMaster>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<CountryMaster>> UpdateAsync([Body] CountryMaster model)
        {
            try
            {
                var dataUpdate = dbContext.CountryMasters.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<CountryMaster>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CountryMaster>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<CountryMaster>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
