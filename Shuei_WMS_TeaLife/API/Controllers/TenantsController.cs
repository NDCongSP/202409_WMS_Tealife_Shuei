using API.Controllers.Base;
using Application.Extentions;
using Application.Models;
using Application.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : BaseController<int, TenantAuth>, ITenants
    {
        readonly Repository _repository;

        public TenantsController(Repository repository = null) : base(repository.STennats)
        {
            _repository = repository;
        }

    }
}
