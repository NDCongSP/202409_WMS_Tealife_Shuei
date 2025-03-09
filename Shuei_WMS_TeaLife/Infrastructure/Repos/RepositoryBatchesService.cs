
using Application.DTOs;
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
    public class RepositoryBatchesService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IBatches
    {
        public async Task<Result<List<Batches>>> AddRangeAsync([Body] List<Batches> model)
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

                await dbContext.Batches.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<Batches>>.SuccessAsync(model, "Add range Batch numbers successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Batches>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Batches>> DeleteRangeAsync([Body] List<Batches> model)
        {
            try
            {
                dbContext.Batches.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Batches>.SuccessAsync("Delete range Batch Numbers successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Batches>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<Batches>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Batches>>.SuccessAsync(await dbContext.Batches.ToListAsync(), $"Successfull.");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Batches>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Batches>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var result = await dbContext.Batches.FindAsync(id);
                return await Result<Batches>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Batches>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Batches>> InsertAsync([Body] Batches model)
        {
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateAt = DateTime.Now;
                model.CreateOperatorId = user?.Id;

                await dbContext.Batches.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Batches>.SuccessAsync(model, $"Insert batch number sucessfull.");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Batches>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Batches>> UpdateAsync([Body] Batches model)
        {
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateAt = DateTime.Now;
                model.UpdateOperatorId = user?.Id;

                var dataUpdate = dbContext.Batches.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Batches>.SuccessAsync(model, $"Update batch number successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Batches>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Batches>> DeleteAsync([Body] Batches model)
        {
            try
            {
                dbContext.Batches.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Batches>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Batches>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Batches>> GetBatchByLotNo(GetBatchByLotNoDto data)
        {
            try
            {
                var batch = await dbContext.Batches.FirstOrDefaultAsync(x => x.TenantId == data.TenantId && x.ProductCode == data.ProductCode && x.LotNo == data.LotNo);
                if (batch == null)
                    return await Result<Batches>.FailAsync("Lotno not exist");
                return await Result<Batches>.SuccessAsync(batch);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Batches>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result> AddUpdateBatchByAdjusment(List<InventAdjustmentLine> data)
        {
            try
            {
                var adjusmentInfo = await dbContext.InventAdjustments.FirstOrDefaultAsync(x => x.AdjustmentNo == data.First().AdjustmentNo);
                if(adjusmentInfo == null)
                    return await Result.FailAsync("Inventory Adjustment not exist");
                var userInfo = contextAccessor.HttpContext?.User.FindFirst("UserId");
                //check exist in batches
                foreach (var line in data)
                {
                    var lotNo = String.IsNullOrWhiteSpace(line.LotNo) ? "N/A" : line.LotNo;
                    await dbContext.Batches.Where(x => x.TenantId == adjusmentInfo.TenantId && x.ProductCode == line.ProductCode && x.LotNo == lotNo).ExecuteDeleteAsync();
                    dbContext.Batches.Add(new Batches
                    {
                        Id = Guid.NewGuid(),
                        TenantId = adjusmentInfo.TenantId,
                        LotNo = lotNo,
                        ProductCode = line.ProductCode,
                        ExpirationDate = line.ExpirationDate,
                        CreateAt = DateTime.Now,
                        CreateOperatorId = userInfo == null ? "" : userInfo.Value,
                        IsDeleted = false
                    });
                }
                await dbContext.SaveChangesAsync();
                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
