using API.Controllers.Base;
using Application.DTOs.Response.Account;
using Application.Extentions;
using Application.Services.Authen;
using Application.Services.Base;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : BaseController<Guid, Permissions>,IPermissions
    {
        readonly Repository _repository;
        public PermissionsController(Repository repository = null) : base(repository.SPermissions)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.Permissions.AddOrEdit)]
        public async Task<Result<PermissionsListResponseDTO>> AddOrEditAsync([Body] PermissionsListResponseDTO model)
        {
            return await _repository.SPermissions.AddOrEditAsync(model);
        }

        [HttpGet(ApiRoutes.Permissions.GetAllPermissionWithAssignedRole)]
        public async Task<Result<List<PermissionsListResponseDTO>>> GetAllPermissionWithAssignedRoleAsync()
        {
            return await _repository.SPermissions.GetAllPermissionWithAssignedRoleAsync();
        }
    }
}
