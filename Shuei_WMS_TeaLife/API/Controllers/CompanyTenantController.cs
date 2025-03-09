using API.Controllers.Base;
using Application.Services;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyTenantController : BaseController<Guid, CompanyTenant>, ICompanies
    {
        readonly Repository _repository = new Repository();

        public CompanyTenantController(Repository repository = null!) : base(repository.SCompanies)
        {
            _repository = repository;
        }
    }
}
