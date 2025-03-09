using Application.Extentions;
using Application.Services.Inbound;


using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryArrivalInstructionDetailsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IArrivalInstructionDetails
    {
        public async Task<Result<List<ArrivalInstructionDetail>>> AddRangeAsync([Body] List<ArrivalInstructionDetail> model)
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

                await dbContext.ArrivalInstructionDetails.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<ArrivalInstructionDetail>>.SuccessAsync(model, "Add range ArrivalInstructionDetail successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<ArrivalInstructionDetail>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstructionDetail>> DeleteRangeAsync([Body] List<ArrivalInstructionDetail> model)
        {
            try
            {
                dbContext.ArrivalInstructionDetails.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstructionDetail>.SuccessAsync("Delete range ArrivalInstructionDetail successfull");
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstructionDetail>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstructionDetail>> DeleteAsync([Body] ArrivalInstructionDetail model)
        {
            try
            {
                dbContext.ArrivalInstructionDetails.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstructionDetail>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstructionDetail>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ArrivalInstructionDetail>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ArrivalInstructionDetail>>.SuccessAsync(await dbContext.ArrivalInstructionDetails.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ArrivalInstructionDetail>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstructionDetail>> GetByIdAsync([Path] int id)
        {
            try
            {
                return await Result<ArrivalInstructionDetail>.SuccessAsync(await dbContext.ArrivalInstructionDetails.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstructionDetail>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstructionDetail>> InsertAsync([Body] ArrivalInstructionDetail model)
        {
            try
            {
                await dbContext.ArrivalInstructionDetails.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstructionDetail>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstructionDetail>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstructionDetail>> UpdateAsync([Body] ArrivalInstructionDetail model)
        {
            try
            {
                dbContext.ArrivalInstructionDetails.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstructionDetail>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstructionDetail>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
