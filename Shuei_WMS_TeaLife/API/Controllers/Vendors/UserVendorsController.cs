using API.Controllers.Base;
using Application.Services.Base;
using Application.Services.Vendors;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Vendors
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserVendorsController : BaseController<string, UserVendor>,IUserVendors
    {
        readonly Repository _repository;
        public UserVendorsController(Repository repository = null) : base(repository.SUserVendors)
        {
            _repository = repository;
        }
    }
}
