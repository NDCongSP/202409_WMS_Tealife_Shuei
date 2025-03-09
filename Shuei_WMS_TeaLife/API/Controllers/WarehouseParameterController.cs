using API.Controllers.Base;
using Application.Extentions;
using Application.Services;
using Application.Services.Base;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseParameterController : BaseController<Guid, WarehouseParameter>, IWarehouseParameters
    {
        readonly Repository _repository;

        public WarehouseParameterController(Repository repository = null!):base(repository.SWarehouseParameters)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehouseParameter.GetFirstOrDefault)]
        public Task<Result<WarehouseParameter>> GetFirstOrDefaultAsync()
        {
            return _repository.SWarehouseParameters.GetFirstOrDefaultAsync();
        }
    }
}
