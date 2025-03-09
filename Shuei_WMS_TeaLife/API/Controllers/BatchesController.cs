using API.Controllers.Base;
using Application.DTOs;
using Application.Extentions;
using Application.Services;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BatchesController : BaseController<Guid, Batches>, IBatches
    {
        private readonly Repository _repository;

        public BatchesController(Repository repository = null) : base(repository.SBatches)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.Batches.SaveBatchByLotNo)]
        public async Task<Result> AddUpdateBatchByAdjusment([Body] List<InventAdjustmentLine> data)
        {
            return await _repository.SBatches.AddUpdateBatchByAdjusment(data);
        }

        [HttpPost(ApiRoutes.Batches.GetBatchByLotNo)]
        public async Task<Result<Batches>> GetBatchByLotNo([FromBody] GetBatchByLotNoDto data)
        {
            return await _repository.SBatches.GetBatchByLotNo(data);
        }
    }
}
