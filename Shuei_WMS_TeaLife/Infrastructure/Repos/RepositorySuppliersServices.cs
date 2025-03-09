using Application.DTOs;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Services.Suppliers;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositorySuppliersServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : ISuppliers
    {
        public async Task<Result<List<Supplier>>> AddRangeAsync([Body] List<Supplier> model)
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

                await dbContext.Suppliers.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<Supplier>>.SuccessAsync(model, "Add range Supplier successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Supplier>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<List<Supplier>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Supplier>> DeleteRangeAsync([Body] List<Supplier> model)
        {
            try
            {
                dbContext.Suppliers.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Supplier>.SuccessAsync("Delete range Supplier successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Supplier>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<Supplier>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Supplier>>.SuccessAsync(await dbContext.Suppliers.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Supplier>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<List<Supplier>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Supplier>> GetByIdAsync([Path] int id)
        {
            try
            {
                var result = await dbContext.Suppliers.FindAsync(id);
                return await Result<Supplier>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Supplier>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


        public async Task<Result<Supplier>> InsertAsync([Body] Supplier model)
        {
            try
            {
                var existCD = await dbContext.Suppliers.Where(x => x.SupplierId == model.SupplierId).FirstOrDefaultAsync();
                if (existCD != null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Suppliers Id is already created.");
                    return await Result<Supplier>.FailAsync(JsonConvert.SerializeObject(err));
                }

                var existName = await dbContext.Suppliers.Where(x => x.SupplierName == model.SupplierName).FirstOrDefaultAsync();
                if (existName != null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Suppliers name is already created.");
                    return await Result<Supplier>.FailAsync(JsonConvert.SerializeObject(err));
                    //return await Result<Supplier>.FailAsync($"Supplier name: {model.SupplierName} is already created");

                }

                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateAt = DateTime.Now;
                model.CreateOperatorId = userInfo.Id;

                await dbContext.Suppliers.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Supplier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Supplier>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<Supplier>> UpdateAsync([Body] Supplier model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateAt = DateTime.Now;
                model.UpdateOperatorId = userInfo.Id;

                var dataUpdate = dbContext.Suppliers.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Supplier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Supplier>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<Supplier>> DeleteAsync([Body] Supplier model)
        {
            try
            {
                dbContext.Suppliers.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<Supplier>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Supplier>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<Supplier>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<SupplierTenantDTO>>> GetSupplierWithTenantAsync()
        {
            try
            {
                var suppliers = await dbContext.Suppliers.ToListAsync(); // Lấy danh sách tất cả Supplier
                var supplierTenantDTOs = new List<SupplierTenantDTO>();


                var leftJoinResult = from a in dbContext.Suppliers
                                     join b in dbContext.Companies on a.CompanyId equals b.AuthPTenantId into joinedB
                                     from subB in joinedB.DefaultIfEmpty()
                                     select new
                                     {
                                         Suppliers = a,
                                         TenantName = subB != null ? subB.FullName : string.Empty
                                     };

                // Cấu hình AutoMapper
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Supplier, SupplierTenantDTO>();
                });
                var mapper = config.CreateMapper();

                foreach (var item in leftJoinResult)
                {
                    var d= mapper.Map<SupplierTenantDTO>(item.Suppliers);
                    d.TenantName = item.TenantName;

                    supplierTenantDTOs.Add(d);
                }

                return await Result<List<SupplierTenantDTO>>.SuccessAsync(supplierTenantDTOs);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<SupplierTenantDTO>>.FailAsync(JsonConvert.SerializeObject(err));
                //return await Result<List<SupplierTenantDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
