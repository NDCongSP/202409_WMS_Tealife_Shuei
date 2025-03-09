
using Application.Extentions;
using Application.Services;


using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using Newtonsoft.Json;

namespace Infrastructure.Repos
{
    public class RepositorySequenceNumberServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : INumberSequences
    {
        public async Task<Result<List<NumberSequences>>> AddRangeAsync([Body] List<NumberSequences> model)
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

                await dbContext.SequencesNumber.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<NumberSequences>>.SuccessAsync(model, "Add range Sequence numbers successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<NumberSequences>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<NumberSequences>> DeleteRangeAsync([Body] List<NumberSequences> model)
        {
            try
            {
                dbContext.SequencesNumber.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<NumberSequences>.SuccessAsync("Delete range Sequence Numbers successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<NumberSequences>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<NumberSequences>>> GetAllAsync()
        {
            try
            {
                return await Result<List<NumberSequences>>.SuccessAsync(await dbContext.SequencesNumber.ToListAsync(), $"Successfull.");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<NumberSequences>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<NumberSequences>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var result = await dbContext.SequencesNumber.FindAsync(id);
                return await Result<NumberSequences>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<NumberSequences>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }


        public async Task<Result<NumberSequences>> InsertAsync([Body] NumberSequences model)
        {
            try
            {
                var existPrefix = await dbContext.SequencesNumber.Where(x => x.Prefix == model.Prefix).FirstOrDefaultAsync();
                if (existPrefix != null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "The data exist.");
                    return await Result<NumberSequences>.FailAsync(JsonConvert.SerializeObject(err));
                }

                var existJournal = await dbContext.SequencesNumber.Where(x => x.JournalType == model.JournalType).FirstOrDefaultAsync();
                if (existJournal != null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "The data exist.");
                    return await Result<NumberSequences>.FailAsync(JsonConvert.SerializeObject(err));
                }

                await dbContext.SequencesNumber.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<NumberSequences>.SuccessAsync(model, $"Insert sequence number sucessfull.");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<NumberSequences>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
        public async Task<Result<NumberSequences>> UpdateAsync([Body] NumberSequences model)
        {
            try
            {
                var dataUpdate = dbContext.SequencesNumber.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<NumberSequences>.SuccessAsync(model, $"Update sequence number successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<NumberSequences>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<NumberSequences>> DeleteAsync([Body] NumberSequences model)
        {
            try
            {
                dbContext.SequencesNumber.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<NumberSequences>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<NumberSequences>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<NumberSequences>> GetNumberSequenceByType([Path] string type)
        {
            try
            {
                var result = await dbContext.SequencesNumber.FirstOrDefaultAsync(x=> x.JournalType == type);
                return await Result<NumberSequences>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<NumberSequences>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<string> GetNoByType([Path] string type)
        {
            try
            {
                var result = await dbContext.SequencesNumber.FirstOrDefaultAsync(x => x.JournalType == type);
                return $"{result.Prefix}{result.CurrentSequenceNo.ToString().PadLeft((int)result.SequenceLength, '0')}"; ;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task<Result<bool>> IncreaseNumberSequenceByType([Path] string type)
        {
            try
            {
                var result = await dbContext.SequencesNumber.Where(x => x.JournalType == type)
                    .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.CurrentSequenceNo, xx => xx.CurrentSequenceNo + 1));
                return await Result<bool>.SuccessAsync(true);
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
