using Application.Extentions;
using Application.Services;
using Application.Services.Outbound;

using Infrastructure.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using RestEase;
using System.Linq;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryInventAdjustmentLineService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor, IBatches batchesServices) : IInventAdjustmentLines
    {
        public async Task<Result<List<InventAdjustmentLine>>> AddRangeAsync([Body] List<InventAdjustmentLine> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);
                //get 
                var lstBatches = new List<Batches>();
                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;
                    if (await CheckExist(item)) return await Result<List<InventAdjustmentLine>>.FailAsync($"{item.AdjustmentNo}|{item.ProductCode}|{item.LotNo}|{item.Qty} Is Existed");
                }
                

                await dbContext.InventAdjustmentLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<InventAdjustmentLine>>.SuccessAsync(model, "Add range InventAdjustmentLine line successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<InventAdjustmentLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustmentLine>> DeleteRangeAsync([Body] List<InventAdjustmentLine> model)
        {
            try
            {
                dbContext.InventAdjustmentLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventAdjustmentLine>.SuccessAsync("Delete range InventAdjustmentLine line successfull");
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustmentLine>> DeleteAsync([Body] InventAdjustmentLine model)
        {
            try
            {
                dbContext.InventAdjustmentLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventAdjustmentLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<InventAdjustmentLine>>> GetAllAsync()
        {
            try
            {
                return await Result<List<InventAdjustmentLine>>.SuccessAsync(await dbContext.InventAdjustmentLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<InventAdjustmentLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustmentLine>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<InventAdjustmentLine>.SuccessAsync(await dbContext.InventAdjustmentLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustmentLine>> InsertAsync([Body] InventAdjustmentLine model)
        {
            try
            {
                //check required
                if (await CheckExist(model))
                    return await Result<InventAdjustmentLine>.FailAsync($"{model.AdjustmentNo}|{model.ProductCode}|{model.LotNo}|{model.Qty} Is Existed");
                await dbContext.InventAdjustmentLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
               

                return await Result<InventAdjustmentLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventAdjustmentLine>> UpdateAsync([Body] InventAdjustmentLine model)
        {
            try
            {
                //check required
                if (await dbContext.InventAdjustmentLines.AsNoTracking().FirstAsync(x => x.Id == model.Id) == null)
                    return await Result<InventAdjustmentLine>.FailAsync($"{model.AdjustmentNo}|{model.ProductCode}|{model.LotNo}|{model.Qty} could be not found.");
                dbContext.InventAdjustmentLines.Update(model);
                await dbContext.SaveChangesAsync();
                
                return await Result<InventAdjustmentLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventAdjustmentLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        private async Task<bool> CheckExist(InventAdjustmentLine model)
        {
            return await dbContext.InventAdjustmentLines.AnyAsync(x => x.AdjustmentNo == model.AdjustmentNo && x.ProductCode == model.ProductCode && x.LotNo == model.LotNo
                                                                    && x.Qty == model.Qty);
        }
    }
}
