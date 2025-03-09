using Application.DTOs;
using Application.Extentions;
using Application.Services.Inbound;



using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryWarehousePutAwayLineServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehousePutAwayLine
    {
        public async Task<Result<List<WarehousePutAwayLine>>> AddRangeAsync([Body] List<WarehousePutAwayLine> model)
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

                await dbContext.WarehousePutAwayLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<WarehousePutAwayLine>>.SuccessAsync(model, "Add range WarehousePutAwayLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAwayLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayLine>> DeleteRangeAsync([Body] List<WarehousePutAwayLine> model)
        {
            try
            {
                dbContext.WarehousePutAwayLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAwayLine>.SuccessAsync("Delete range WarehousePutAwayLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayLine>> DeleteAsync([Body] WarehousePutAwayLine model)
        {
            try
            {
                dbContext.WarehousePutAwayLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAwayLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePutAwayLine>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehousePutAwayLine>>.SuccessAsync(await dbContext.WarehousePutAwayLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAwayLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayLine>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePutAwayLine>.SuccessAsync(await dbContext.WarehousePutAwayLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayLine>> InsertAsync([Body] WarehousePutAwayLine model)
        {
            try
            {
                await dbContext.WarehousePutAwayLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAwayLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayLine>> UpdateAsync([Body] WarehousePutAwayLine model)
        {
            try
            {
                dbContext.WarehousePutAwayLines.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehousePutAwayLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePutAwayLine>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            try
            {
                return await Result<List<WarehousePutAwayLine>>.SuccessAsync(await dbContext.WarehousePutAwayLines.Where(x => x.PutAwayNo == putAwayNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAwayLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<List<LabelInfoDto>> GetLabelByIdAsync([Path] Guid id)
        {
            try
            {
                var dataInfo = dbContext.WarehousePutAwayLines.Where(x => x.Id == id)
                    .Join(dbContext.Products,  x => x.ProductCode, y => y.ProductCode, (x, y) => new
                        {
                            putAway = x,
                            product = y
                        }).FirstOrDefault();
                if (dataInfo == null) return null;

                List<LabelInfoDto> res = new List<LabelInfoDto>();
                
                res.Add(new LabelInfoDto()
                {
                    Title = "Put Away",
                    QrValue = GlobalVariable.GenerateQRCode($"{dataInfo.putAway.ProductCode}:JANCode:{dataInfo.putAway.LotNo}"),
                    Title1 = "Product Code:",
                    Content1 = dataInfo.putAway.ProductCode,
                    Title2 = "JAN Code:",
                    Content2 = dataInfo.product.JanCode,
                    Title3 = "LOT:",
                    Content3 = dataInfo.putAway.LotNo,
                    Title4 = "ExpiryDate",
                    Content4 = dataInfo.putAway.ExpirationDate == default ? "" : dataInfo.putAway.ExpirationDate.Value.ToString("dd/MM/yyyy")
                });

                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<LabelInfoDto>> GetLabelByPutAwayNo([Path] string putAwayNo)
        {
            try
            {
                var dataInfo = await dbContext.WarehousePutAwayLines.Where(m => m.PutAwayNo == putAwayNo).ToListAsync();
                if (dataInfo == null) return null;

                List<LabelInfoDto> res = new List<LabelInfoDto>();

                int index = 1;
                
                foreach (var item in dataInfo)
                {
                    res.Add(new LabelInfoDto()
                    {
                        Title = "Put Away",
                        QrValue = GlobalVariable.GenerateQRCode($"{item.ProductCode}:JANCode:{item.LotNo}"),
                        Title1 = "Product Code:",
                        Content1 = item.ProductCode,
                        Title2 = "JAN Code:",
                        Content2 ="",
                        Title3="LOT:",
                        Content3=item.LotNo,
                        Title4= "ExpiryDate",
                        Content4=""
                    });

                    index += 1;
                }

                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

}
