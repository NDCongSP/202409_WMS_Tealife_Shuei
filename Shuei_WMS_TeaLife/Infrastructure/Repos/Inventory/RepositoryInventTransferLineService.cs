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
    public class RepositoryInventTransferLineService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IInventTransferLines
    {
        public async Task<Result<List<InventTransferLine>>> AddRangeAsync([Body] List<InventTransferLine> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;

                    if (await CheckExist(item)) return await Result<List<InventTransferLine>>.FailAsync($"{item.TransferNo}|{item.FromBin}|{item.ToBin}|{item.FromLotNo}|{item.ToLotNo} Is Existed");
                }

                await dbContext.InventTransferLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<InventTransferLine>>.SuccessAsync(model, "Add range InventAdjustment line successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<InventTransferLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransferLine>> DeleteRangeAsync([Body] List<InventTransferLine> model)
        {
            try
            {
                dbContext.InventTransferLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventTransferLine>.SuccessAsync("Delete range InventAdjustment line successfull");
            }
            catch (Exception ex)
            {
                return await Result<InventTransferLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransferLine>> DeleteAsync([Body] InventTransferLine model)
        {
            try
            {
                dbContext.InventTransferLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventTransferLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventTransferLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<InventTransferLine>>> GetAllAsync()
        {
            try
            {
                return await Result<List<InventTransferLine>>.SuccessAsync(await dbContext.InventTransferLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<InventTransferLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransferLine>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<InventTransferLine>.SuccessAsync(await dbContext.InventTransferLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<InventTransferLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransferLine>> InsertAsync([Body] InventTransferLine model)
        {
            try
            {
                //check required
                //if (await CheckExist(model))
                //    return await Result<InventTransferLine>.FailAsync($"{model.TransferNo}|{model.FromBin}|{model.ToBin}|{model.FromLotNo}|{model.ToLotNo} Is Existed");
                await dbContext.InventTransferLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventTransferLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventTransferLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<InventTransferLine>> UpdateAsync([Body] InventTransferLine model)
        {
            try
            {
               
                var existingLine = await dbContext.InventTransferLines.FindAsync(model.Id);

                if (existingLine == null)
                    return await Result<InventTransferLine>.FailAsync($"{model.TransferNo}|{model.FromBin}|{model.ToBin}|{model.FromLotNo}|{model.ToLotNo} could be not found.");

            
                dbContext.Entry(existingLine).State = EntityState.Detached;

               
                dbContext.Entry(model).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();

                return await Result<InventTransferLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventTransferLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        private async Task<bool> CheckExist(InventTransferLine model)
        {
            return await dbContext.InventTransferLines.AnyAsync(x => x.TransferNo == model.TransferNo && x.FromBin == model.FromBin && x.ToBin == model.ToBin
                                                                    && x.FromLotNo == model.ToLotNo);
        }
    }
}
