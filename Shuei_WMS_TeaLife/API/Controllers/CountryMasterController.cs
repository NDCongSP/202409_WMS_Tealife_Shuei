using API.Controllers.Base;
using Application.Services;
using Application.Services.Base;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CountryMasterController : BaseController<int, CountryMaster>, ICountryMaster
    {
        readonly Repository _repository;
        public CountryMasterController(Repository repository = null) : base(repository.SCountryMaster)
        {
            _repository = repository;
        }
    }
}
