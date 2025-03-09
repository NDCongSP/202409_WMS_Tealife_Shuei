
using Application.Extentions;
using Application.Models;
using Application.Services;


using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Newtonsoft.Json;

namespace Infrastructure.Repos
{
    public class RepositoryLocationsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : ILocations
    {
        public async Task<Result<List<Location>>> AddRangeAsync([Body] List<Location> model)
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

                await dbContext.Locations.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<Location>>.SuccessAsync(model, "Add range Tenant successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Location>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Location>> DeleteRangeAsync([Body] List<Location> model)
        {
            try
            {
                dbContext.Locations.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Location>.SuccessAsync("Delete range Tenant successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Location>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<Location>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Location>>.SuccessAsync(await dbContext.Locations.ToListAsync(), $"Successfull.");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Location>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Location>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var result = await dbContext.Locations.FindAsync(id);
                return await Result<Location>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Location>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }


        public async Task<Result<Location>> InsertAsync([Body] Location model)
        {
            try
            {
                var existCD = await dbContext.Locations.Where(x => x.LocationCD == model.LocationCD).FirstOrDefaultAsync();
                if (existCD != null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "The data exists.");
                    return await Result<Location>.FailAsync(JsonConvert.SerializeObject(err));
                }

                var existName = await dbContext.Locations.Where(x => x.LocationName == model.LocationName).FirstOrDefaultAsync();
                if (existName != null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "The data exists.");
                    return await Result<Location>.FailAsync(JsonConvert.SerializeObject(err));
                }

                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateAt = DateTime.Now;
                model.CreateOperatorId = userInfo.Id;

                await dbContext.Locations.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Location>.SuccessAsync(model, $"Insert location {model.LocationName} sucessfull.");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Location>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<List<ProductJanCode>>> GetByProductId(int productId)
        {
            try
            {
                return await Result<List<ProductJanCode>>.SuccessAsync(await dbContext.ProductJanCodes.Where(m => m.ProductId == productId).ToListAsync());

            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ProductJanCode>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<Location>> UpdateAsync([Body] Location model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateAt = DateTime.Now;
                model.UpdateOperatorId = userInfo.Id;

                var dataUpdate = dbContext.Locations.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Location>.SuccessAsync(model, $"Update location {model.LocationName} successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Location>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Location>> DeleteAsync([Body] Location model)
        {
            try
            {
                dbContext.Locations.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Location>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Location>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
