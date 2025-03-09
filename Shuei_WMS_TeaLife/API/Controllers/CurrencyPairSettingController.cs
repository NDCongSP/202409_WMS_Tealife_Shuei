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
    public class CurrencyPairSettingController : BaseController<int, CurrencyPairSetting>, ICurrencyPairSetting
    {
        readonly Repository _repository;

        public CurrencyPairSettingController(Repository repository = null!) : base(repository.SCurrencyPairSetting)
        {
            _repository = repository;
        }
    }
}
