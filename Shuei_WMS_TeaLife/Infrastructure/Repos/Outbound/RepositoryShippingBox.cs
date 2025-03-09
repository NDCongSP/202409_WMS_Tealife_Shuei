using Application.DTOs;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Services.Outbound;

using Infrastructure.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    public class RepositoryShippingBox(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IShippingBox
    {
        public async Task<Result<List<ShippingBox>>> AddRangeAsync([Body] List<ShippingBox> model)
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

                await dbContext.ShippingBoxes.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<ShippingBox>>.SuccessAsync(model, "Add range ShippingBox successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ShippingBox>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ShippingBox>> DeleteRangeAsync([Body] List<ShippingBox> model)
        {
            try
            {
                dbContext.ShippingBoxes.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingBox>.SuccessAsync("Delete range ShippingBox successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ShippingBox>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ShippingBox>> DeleteAsync([Body] ShippingBox model)
        {
            try
            {
                dbContext.ShippingBoxes.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingBox>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ShippingBox>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<ShippingBox>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ShippingBox>>.SuccessAsync(await dbContext.ShippingBoxes.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ShippingBox>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }



        public async Task<Result<List<ShippingBoxDTO>>> GetAllWithCarrierAsync()
        {
            try
            {
                var shippingBoxes = await (from sb in dbContext.ShippingBoxes
                                           join sc in dbContext.ShippingCarriers
                                                on sb.ShippingCarrierId equals sc.Id into shippingCarriers
                                           from sc in shippingCarriers.DefaultIfEmpty()
                                           select new ShippingBoxDTO
                                           {
                                               Id = sb.Id,
                                               BoxName = sb.BoxName,
                                               BoxType = sb.BoxType,
                                               Length = sb.Length,
                                               Width = sb.Width,
                                               Height = sb.Height,
                                               MaxWeight = sb.MaxWeight,
                                               Status = sb.Status,
                                               ShippingCarrierId = sb.ShippingCarrierId,
                                               ShippingCarrierCode = sb.ShippingCarrierCode,
                                               ShippingCarrierName = sc != null ? sc.ShippingCarrierName : null
                                           })
                           .ToListAsync();


                return await Result<List<ShippingBoxDTO>>.SuccessAsync(shippingBoxes);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ShippingBoxDTO>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ShippingBox>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<ShippingBox>.SuccessAsync(await dbContext.ShippingBoxes.FindAsync(id));
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ShippingBox>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ShippingBox>> InsertAsync([Body] ShippingBox model)
        {
            try
            {
                //check required
                if (await CheckExistShippingBox(model))
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "The data exists.");
                    return await Result<ShippingBox>.FailAsync(JsonConvert.SerializeObject(err));
                }

                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateAt = DateTime.Now;
                model.CreateOperatorId = userInfo.Id;

                await dbContext.ShippingBoxes.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingBox>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ShippingBox>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ShippingBox>> UpdateAsync([Body] ShippingBox model)
        {
            try
            {
                //check required
                if (await CheckExistShippingBox(model))
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "The data exists.");
                    return await Result<ShippingBox>.FailAsync(JsonConvert.SerializeObject(err));
                }

                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateAt = DateTime.Now;
                model.CreateOperatorId = userInfo.Id;

                dbContext.ShippingBoxes.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ShippingBox>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ShippingBox>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        private async Task<bool> CheckExistShippingBox(ShippingBox shippingBox)
        {
            return await dbContext.ShippingBoxes.AnyAsync(x => x.Id != shippingBox.Id && x.BoxName.ToLower() == shippingBox.BoxName.ToLower() && x.ShippingCarrierCode == shippingBox.ShippingCarrierCode);
        }

        public async Task<Result<List<ShippingBox>>> GetByShippingCarrierCodeAsync([Path] string shippingCarrierCode)
        {
            try
            {
                var res = await dbContext.ShippingBoxes.Where(_ => _.ShippingCarrierCode == shippingCarrierCode).ToListAsync();

                if (res.Count() == 0)
                {

                }

                return await Result<List<ShippingBox>>.SuccessAsync(res, "Successful");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ShippingBox>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }



        public async Task<Result<List<ShippingCarrierDTO>>> GetAllShippingCarrierAsync()
        {
            try
            {
                var shippingCarriers = await (from sc in dbContext.ShippingCarriers
                                              select new ShippingCarrierDTO
                                              {
                                                  Id = sc.Id,
                                                  ShippingCarrierCode = sc.ShippingCarrierCode,
                                                  ShippingCarrierName = sc.ShippingCarrierName
                                              })
                           .ToListAsync();


                return await Result<List<ShippingCarrierDTO>>.SuccessAsync(shippingCarriers);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ShippingCarrierDTO>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
