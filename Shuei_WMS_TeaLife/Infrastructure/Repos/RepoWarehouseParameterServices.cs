using Application.Extentions;
using Application.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepoWarehouseParameterServices(ApplicationDbContext _dbContext, IHttpContextAccessor contextAccessor) : IWarehouseParameters
    {
        public async Task<Result<List<WarehouseParameter>>> AddRangeAsync([Body] List<WarehouseParameter> model)
        {
            try
            {
                await _dbContext.WarehouseParameters.AddRangeAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<List<WarehouseParameter>>.SuccessAsync(model, "Add range successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<WarehouseParameter>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseParameter>> DeleteAsync([Body] WarehouseParameter model)
        {
            try
            {
                _dbContext.WarehouseParameters.Remove(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehouseParameter>.SuccessAsync("");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseParameter>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseParameter>> DeleteRangeAsync([Body] List<WarehouseParameter> model)
        {
            try
            {
                _dbContext.WarehouseParameters.RemoveRange(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehouseParameter>.SuccessAsync("");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseParameter>.FailAsync(JsonConvert.SerializeObject(err)); 
            }
        }

        public async Task<Result<List<WarehouseParameter>>> GetAllAsync()
        {
            try
            {
                var parameters = await _dbContext.WarehouseParameters.ToListAsync();
                return await Result<List<WarehouseParameter>>.SuccessAsync(parameters);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<WarehouseParameter>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseParameter>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var parameter = await _dbContext.WarehouseParameters.FirstOrDefaultAsync(x => x.Id == id);
                return await Result<WarehouseParameter>.SuccessAsync(parameter);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseParameter>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseParameter>> InsertAsync([Body] WarehouseParameter model)
        {
            try
            {
                await _dbContext.WarehouseParameters.AddAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehouseParameter>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseParameter>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseParameter>> UpdateAsync([Body] WarehouseParameter model)
        {
            try
            {
                _dbContext.WarehouseParameters.Update(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehouseParameter>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseParameter>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<WarehouseParameter>> GetFirstOrDefaultAsync()
        {
            try
            {
                var parameter = await _dbContext.WarehouseParameters.FirstOrDefaultAsync(x => x.Status == 1);
                return await Result<WarehouseParameter>.SuccessAsync(parameter);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<WarehouseParameter>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
