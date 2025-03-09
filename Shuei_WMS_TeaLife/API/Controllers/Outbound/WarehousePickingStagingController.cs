using API.Controllers.Base;
using Application.Extentions;
using Application.Services.Outbound;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Outbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousePickingStagingController : BaseController<Guid, WarehousePickingStaging>, IWarehousePickingStaging
    {
        readonly Repository _repository;

        public WarehousePickingStagingController(Repository repository = null!) : base(repository.SWarehousePickingStaging)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehousePickingStaging.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePickingStaging>>> GetByMasterCodeAsync([Path] string pickNo)
        {
            return await _repository.SWarehousePickingStaging.GetByMasterCodeAsync(pickNo);
        }
    }
}
