using Application.Extentions;
using Application.Models;
using Application.Services;


using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryDevicesServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IDevices
    {
        public async Task<Result<List<Device>>> AddRangeAsync([Body] List<Device> model)
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

                await dbContext.Devices.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<Device>>.SuccessAsync(model, "Add range Device successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Device>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<Device>> DeleteRangeAsync([Body] List<Device> model)
        {
            try
            {
                dbContext.Devices.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Device>.SuccessAsync("Delete range Device successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Device>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<Device>> DeleteAsync([Body] Device model)
        {
            try
            {
                dbContext.Devices.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Device>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Device>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<List<Device>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Device>>.SuccessAsync(await dbContext.Devices.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Device>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<Device>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var result = await dbContext.Devices.FindAsync(id);
                return await Result<Device>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Device>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<Device>> GetByNameAsync(string name)
        {
            try
            {
                var result = await dbContext.Devices.Where(x => x.Name == name).FirstOrDefaultAsync();
                if (result == null)
                {
                    return await Result<Device>.FailAsync("Device not found");
                }
                else
                {
                    return await Result<Device>.SuccessAsync(result);
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Device>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<Device>> InsertAsync([Body] Device model)
        {
            try
            {
                var existCD = await dbContext.Devices.Where(x => x.Name == model.Name).FirstOrDefaultAsync();
                if (existCD != null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", $"Device name is already created.");
                    return await Result<Device>.FailAsync(JsonConvert.SerializeObject(err));
                }

                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateAt = DateTime.Now;
                model.CreateOperatorId = userInfo.Id;

                await dbContext.Devices.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Device>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Device>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<Device>> UpdateAsync([Body] Device model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateAt = DateTime.Now;
                model.UpdateOperatorId = userInfo.Id;

                var dataUpdate = dbContext.Devices.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Device>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Device>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<bool>> CheckNameExists(string name)
        {
            try
            {
                bool exists = await dbContext.Devices.AnyAsync(x => x.Name == name);
                return await Result<bool>.SuccessAsync(exists);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<bool>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

    }
}
