
using Application.DTOs.Response;
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
    public class RepositoryProducCategorysServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IProductCategory
    {
        public async Task<Result<List<ProductCategory>>> AddRangeAsync([Body] List<ProductCategory> model)
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

                await dbContext.ProductCategories.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<ProductCategory>>.SuccessAsync(model, "Add range ProductCategories successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ProductCategory>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductCategory>> DeleteRangeAsync([Body] List<ProductCategory> model)
        {
            var err = new ErrorResponse();
            try
            {
                dbContext.ProductCategories.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductCategory>.SuccessAsync("Delete range ProductCategories successfull");
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductCategory>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductCategory>> DeleteAsync([Body] ProductCategory model)
        {
            var err = new ErrorResponse();
            try
            {
                dbContext.ProductCategories.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductCategory>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductCategory>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<ProductCategory>>> GetAllAsync()
        {
            var err = new ErrorResponse();
            try
            {
                return await Result<List<ProductCategory>>.SuccessAsync(await dbContext.ProductCategories.ToListAsync());
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ProductCategory>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductCategory>> GetByIdAsync([Path] int id)
        {
            var err = new ErrorResponse();
            try
            {
                var result = await dbContext.ProductCategories.FindAsync(id);
                return await Result<ProductCategory>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductCategory>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }


        public async Task<Result<ProductCategory>> InsertAsync([Body] ProductCategory model)
        {
            var err = new ErrorResponse();
            try
            {
                var existCD = await dbContext.ProductCategories.Where(x => x.CategoryName == model.CategoryName).FirstOrDefaultAsync();
                if (existCD != null)
                {
                    err.Errors.Add("Warning", "Category name is already created.");
                    return await Result<ProductCategory>.FailAsync(JsonConvert.SerializeObject(err));
                }

                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateAt = DateTime.Now;
                model.CreateOperatorId = userInfo.Id;

                await dbContext.ProductCategories.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductCategory>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductCategory>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<ProductCategory>> UpdateAsync([Body] ProductCategory model)
        {
            var err = new ErrorResponse();
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateAt = DateTime.Now;
                model.UpdateOperatorId = userInfo.Id;

                var dataUpdate = dbContext.ProductCategories.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<ProductCategory>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<ProductCategory>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
