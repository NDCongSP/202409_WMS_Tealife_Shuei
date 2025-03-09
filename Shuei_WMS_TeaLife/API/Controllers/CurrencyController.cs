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
    public class CurrencyController : BaseController<string, Currency>,ICurrency
    {
        readonly Repository _repository;

        public CurrencyController(Repository repository = null!) : base(repository.SCurrency)
        {
            _repository = repository;
        }
    }
}
