using Application.DTOs;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryCompaniesServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : ICompanies
    {
        public async Task<Result<List<CompanyTenant>>> AddRangeAsync([Body] List<CompanyTenant> model)
        {
            var err = new ErrorResponse();
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                //foreach (var item in model)
                //{
                //    item.CreateAt = DateTime.Now;
                //    item.CreateOperatorId = userInfo?.Id;
                //    item.Status = EnumStatus.Activated;

                //}

                await dbContext.Companies.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<CompanyTenant>>.SuccessAsync(model, "Add range Tenant successfull");
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<CompanyTenant>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<List<CompanyTenant>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<CompanyTenant>> DeleteAsync([Body] CompanyTenant model)
        {
            var err = new ErrorResponse();
            try
            {
                dbContext.Companies.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<CompanyTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CompanyTenant>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<CompanyTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<CompanyTenant>> DeleteRangeAsync([Body] List<CompanyTenant> model)
        {
            var err = new ErrorResponse();
            try
            {
                dbContext.Companies.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<CompanyTenant>.SuccessAsync("Delete range Tenant successfull");
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CompanyTenant>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<CompanyTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<CompanyTenant>>> GetAllAsync()
        {
            var err = new ErrorResponse();
            try
            {
                return await Result<List<CompanyTenant>>.SuccessAsync(await dbContext.Companies.ToListAsync());
            }
            catch (Exception ex)
            {
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<CompanyTenant>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<List<CompanyTenant>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<CompanyTenant>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                var result = await dbContext.Companies.FindAsync(id);
                return await Result<CompanyTenant>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CompanyTenant>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<CompanyTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<CompanyTenant>> InsertAsync([Body] CompanyTenant model)
        {
            try
            {
                await dbContext.Companies.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<CompanyTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CompanyTenant>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<CompanyTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<CompanyTenant>> UpdateAsync([Body] CompanyTenant model)
        {
            try
            {
                var dataUpdate = dbContext.Companies.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<CompanyTenant>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<CompanyTenant>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<CompanyTenant>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
