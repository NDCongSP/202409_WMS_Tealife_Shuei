
using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Extentions.Pagings;
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
using static Application.Extentions.ApiRoutes;

namespace Infrastructure.Repos
{
    public class RepositoryArrivalInstructionServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IArrivalInstructions
    {
        public async Task<Result<List<ArrivalInstruction>>> AddRangeAsync([Body] List<ArrivalInstruction> model)
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

                await dbContext.ArrivalInstructions.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<ArrivalInstruction>>.SuccessAsync(model, "Add range ArrivalInstructions successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<ArrivalInstruction>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstruction>> DeleteRangeAsync([Body] List<ArrivalInstruction> model)
        {
            try
            {
                dbContext.ArrivalInstructions.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstruction>.SuccessAsync("Delete range ArrivalInstructions successfull");
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstruction>> DeleteAsync([Body] ArrivalInstruction model)
        {
            try
            {
                dbContext.ArrivalInstructions.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstruction>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ArrivalInstruction>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ArrivalInstruction>>.SuccessAsync(await dbContext.ArrivalInstructions.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ArrivalInstruction>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<ArrivalInstruction>> GetByIdAsync([Path] int id)
        {
            try
            {
                return await Result<ArrivalInstruction>.SuccessAsync(await dbContext.ArrivalInstructions.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstruction>> InsertAsync([Body] ArrivalInstruction model)
        {
            try
            {
                await dbContext.ArrivalInstructions.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstruction>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ArrivalInstruction>> UpdateAsync([Body] ArrivalInstruction model)
        {
            try
            {
                dbContext.ArrivalInstructions.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ArrivalInstruction>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ArrivalInstruction>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<PageList<ArrivalInstructionDto>>> SearchArrivalInstructions([Body] QueryModel<ReceivePlanSearchModel> model)
        {
            try
            {
                var data = model.Entity ?? new ReceivePlanSearchModel();
                if (data.ReceiveDateFrom != default)
                    data.ReceiveDateFrom = new DateTime(data.ReceiveDateFrom.Value.Year, data.ReceiveDateFrom.Value.Month, data.ReceiveDateFrom.Value.Day, 0, 0, 0);
                if (data.ReceiveDateTo != default)
                    data.ReceiveDateTo = new DateTime(data.ReceiveDateTo.Value.Year, data.ReceiveDateTo.Value.Month, data.ReceiveDateTo.Value.Day, 23, 59, 59);
                var query = from ai in dbContext.ArrivalInstructions
                            join p in dbContext.Products
                            on ai.ProductCode equals p.ProductCode into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            join s in dbContext.Suppliers 
                            on p.SupplierId equals s.Id into sGroup
                            from s in sGroup.DefaultIfEmpty()
                            join wro in dbContext.WarehouseReceiptOrders
                            on ai.Id equals wro.ScheduledArrivalNumber into wroGroup
                            from wro in wroGroup.DefaultIfEmpty()
                            where ai.IsDeleted == false
                            && (string.IsNullOrEmpty(data.ProductCode) || ai.ProductCode == data.ProductCode)
                            && (data.ArrivalNo == default || ai.Id == data.ArrivalNo)
                            && (data.ReceiveDateFrom == default || ai.ScheduledArrivalDate >= data.ReceiveDateFrom)
                            && (data.ReceiveDateTo == default || ai.ScheduledArrivalDate <= data.ReceiveDateTo)
                            && (data.SupplierId == default || s.Id == data.SupplierId)
                            select new ArrivalInstructionDto
                            {
                                Id = ai.Id,
                                ReceiptNo = wro != null ? wro.ReceiptNo : null,
                                ScheduledArrivalNumber = ai.Id,
                                ScheduledArrivalDate = ai.ScheduledArrivalDate,
                                SupplierName = s != null ? s.SupplierName : "",
                                ProductCode = ai.ProductCode,
                                ProductName = p != null ? p.ProductName : "",
                                Quantity = ai.Quantity
                            };
                var pagedList = PageList<ArrivalInstructionDto>.PagedResult(query, model.PageNumber, model.PageSize);
                return await Result<PageList<ArrivalInstructionDto>>.SuccessAsync(pagedList);
            }
            catch (Exception ex)
            {
                return await Result<PageList<ArrivalInstructionDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
