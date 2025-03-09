using API.Controllers.Base;
using Application.Services.Authen;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleToPermissionTenantController : BaseController<Guid, RoleToPermissionTenant>, IRoleToPermissionTenant
    {
        readonly Repository _repository;
        public RoleToPermissionTenantController(Repository repository = null!) : base(repository.SRoleToPermissionTenant)
        {
            _repository = repository;
        }
    }
}
