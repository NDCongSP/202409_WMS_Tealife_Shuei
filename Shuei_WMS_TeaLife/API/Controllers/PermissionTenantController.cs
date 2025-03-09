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
    public class PermissionTenantController : BaseController<Guid, PermissionsTenant>,IPermissionTenant
    {
        readonly Repository _repository;
        public PermissionTenantController(Repository repository = null!) : base(repository.SPermissionTenant)
        {
            _repository = repository;
        }
    }
}
