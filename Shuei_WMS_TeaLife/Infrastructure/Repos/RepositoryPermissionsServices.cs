using Application.DTOs.Response.Account;

using Application.Extentions;
using Application.Models;
using Application.Services.Authen;
using Dapper;

using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryPermissionsServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IPermissions
    {
        public async Task<Result<List<Permissions>>> AddRangeAsync([Body] List<Permissions> model)
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

                await dbContext.Permissions.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<Permissions>>.SuccessAsync(model, "Add range Permissions successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Permissions>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Permissions>> DeleteRangeAsync([Body] List<Permissions> model)
        {
            try
            {
                dbContext.Permissions.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<Permissions>.SuccessAsync("Delete range Permissions successfull");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Permissions>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<PermissionsListResponseDTO>> AddOrEditAsync([Body] PermissionsListResponseDTO model)
        {
            try
            {
                using (var transaction = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        //lay thong tin user
                        var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                        if (model.Id == Guid.Empty)
                        {
                            var check = await dbContext.Permissions.FirstOrDefaultAsync(x => x.Name == model.Name);

                            if (check != null)
                            {
                                transaction.Rollback();
                                var err = new ErrorResponse();
                                err.Errors.Add("Warning", "The data exists.");
                                return await Result<PermissionsListResponseDTO>.FailAsync(JsonConvert.SerializeObject(err));
                            }

                            //add new permission
                            var newPermission = new Permissions()
                            {
                                Id = Guid.NewGuid(),
                                Name = model.Name,
                                Description = model.Description,
                                CreateAt = DateTime.Now,
                                CreateOperatorId = userInfo.Id,
                                Status = EnumStatus.Activated,
                            };
                            await dbContext.Permissions.AddAsync(newPermission);

                            //check assigned, if have then insert to RoleToPermision entity
                            if (model.AssignedToRoles.Count > 0)
                            {
                                var assignToRole = new List<RoleToPermission>();

                                foreach (var item in model.AssignedToRoles)
                                {
                                    assignToRole.Add(new RoleToPermission()
                                    {
                                        Id = Guid.NewGuid(),
                                        RoleId = Guid.Parse(item.Id),
                                        RoleName = item.Name,
                                        PermissionId = newPermission.Id,
                                        PermisionName = newPermission.Name,
                                        PermisionDescription = newPermission.Description,
                                        CreateAt = DateTime.Now,
                                        CreateOperatorId = userInfo.Id,
                                        Status = EnumStatus.Activated,
                                    });
                                }

                                await dbContext.RoleToPermissions.AddRangeAsync(assignToRole);
                            }
                        }
                        else
                        {
                            //update permission
                            var updatePermission = new Permissions()
                            {
                                Id = model.Id,
                                Name = model.Name,
                                Description = model.Description,
                                CreateAt = model.CreateAt,
                                CreateOperatorId = model.CreateOperatorId,
                                Status = EnumStatus.Activated,
                                UpdateAt = DateTime.Now,
                                UpdateOperatorId = userInfo.Id,
                            };
                            dbContext.Permissions.Update(updatePermission);

                            //check assigned, if have then insert to RoleToPermision entity
                            //xoa het assign to role hien tai
                            var rtp = dbContext.RoleToPermissions.Where(x => x.PermissionId == model.Id).ToList();
                            dbContext.RoleToPermissions.RemoveRange(rtp);
                            if (model.AssignedToRoles.Count > 0)
                            {
                                var assignToRole = new List<RoleToPermission>();

                                foreach (var item in model.AssignedToRoles)
                                {
                                    assignToRole.Add(new RoleToPermission()
                                    {
                                        Id = Guid.NewGuid(),
                                        RoleId = Guid.Parse(item.Id),
                                        RoleName = item.Name,
                                        PermissionId = model.Id,
                                        PermisionName = model.Name,
                                        PermisionDescription = model.Description,
                                        CreateAt = model.CreateAt,
                                        CreateOperatorId = model.CreateOperatorId,
                                        Status = EnumStatus.Activated,
                                        UpdateAt = DateTime.Now,
                                        UpdateOperatorId = userInfo.Id,
                                    });
                                }

                                await dbContext.RoleToPermissions.AddRangeAsync(assignToRole);
                            }
                        }

                        await dbContext.SaveChangesAsync();

                        // Commit the transaction
                        await transaction.CommitAsync();

                        return await Result<PermissionsListResponseDTO>.SuccessAsync(model, "Create permission and assign to roles successfully");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if an error occurs
                        transaction.Rollback();
                      throw new Exception($"Transaction failed: {ex.Message}{Environment.NewLine}{ex.InnerException}");
                    }
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<PermissionsListResponseDTO>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Permissions>> DeleteAsync([Body] Permissions model)
        {
            try
            {
                dbContext.Permissions.Remove(model);

                #region Remove permission in role
                var rtp = dbContext.RoleToPermissions.Where(x => x.PermissionId == model.Id).ToList();
                dbContext.RoleToPermissions.RemoveRange(rtp);
                #endregion

                await dbContext.SaveChangesAsync();
                return await Result<Permissions>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Permissions>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<Permissions>>> GetAllAsync()
        {
            try
            {
                return await Result<List<Permissions>>.SuccessAsync(await dbContext.Permissions.ToListAsync());
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<Permissions>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<List<PermissionsListResponseDTO>>> GetAllPermissionWithAssignedRoleAsync()
        {
            try
            {
                var c = dbContext.GetConnectionString();

                var allPermission = new List<PermissionsListModel>();
                using (var connection = new SqlConnection(dbContext.Database.GetConnectionString()))
                {
                    allPermission = connection.Query<PermissionsListModel>($"SELECT _per.*,_roleToPer.RoleId,_roleToPer.RoleName " +
                               $"FROM wms.[Permissions] _per " +
                               $"LEFT JOIN wms.RoleToPermission _roleToPer ON _roleToPer.PermissionId = _per.Id")
                   .ToList();
                }

                var allPermissionGroup = allPermission.GroupBy(x => x.Id)
                    .Select(g => new
                    {
                        PermissionId = g.Key,
                        Name = g.FirstOrDefault().Name,
                        Description = g.FirstOrDefault().Description,
                        CreateAt = g.FirstOrDefault().CreateAt,
                        CreateOperatorId = g.FirstOrDefault().CreateOperatorId,
                        UpdateAt = g.FirstOrDefault().UpdateAt,
                        UpdateOperatorId = g.FirstOrDefault().UpdateOperatorId,
                        Status = g.FirstOrDefault().Status,
                        AssignedToRoles = g.ToList()
                    });

                List<PermissionsListResponseDTO> responseToClient = new List<PermissionsListResponseDTO>();
                foreach (var item in allPermissionGroup)
                {
                    //get list role assigned by permission
                    List<GetRoleResponseDTO> assignedRoles = new List<GetRoleResponseDTO>();
                    foreach (var role in item.AssignedToRoles)
                    {
                        assignedRoles.Add(new GetRoleResponseDTO()
                        {
                            Id = role.RoleId.ToString(),
                            Name = role.RoleName,
                        });
                    }

                    responseToClient.Add(new PermissionsListResponseDTO()
                    {
                        Id = item.PermissionId,
                        Name = item.Name,
                        Description = item.Description,
                        CreateAt = item.CreateAt,
                        CreateOperatorId = item.CreateOperatorId,
                        UpdateAt = item.UpdateAt,
                        UpdateOperatorId = item.UpdateOperatorId,
                        Status = item.Status,
                        AssignedToRoles = assignedRoles,
                    });
                }

                return await Result<List<PermissionsListResponseDTO>>.SuccessAsync(responseToClient);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<PermissionsListResponseDTO>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Permissions>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<Permissions>.SuccessAsync(await dbContext.Permissions.FindAsync(id));
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Permissions>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Permissions>> InsertAsync([Body] Permissions model)
        {
            try
            {
                var existCD = await dbContext.Permissions.Where(x => x.Name == model.Name).FirstOrDefaultAsync();
                if (existCD != null)
                {
                    return await Result<Permissions>.FailAsync($"Permission name: {model.Name} is already created");
                }

                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.CreateAt = DateTime.Now;
                model.CreateOperatorId = userInfo.Id;

                await dbContext.Permissions.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<Permissions>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Permissions>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<Permissions>> UpdateAsync([Body] Permissions model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                model.UpdateAt = DateTime.Now;
                model.UpdateOperatorId = userInfo.Id;

                dbContext.Permissions.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<Permissions>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<Permissions>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
