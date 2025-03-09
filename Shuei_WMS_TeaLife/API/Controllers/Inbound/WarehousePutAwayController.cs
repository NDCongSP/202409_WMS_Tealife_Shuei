using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Services.Inbound;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousePutAwayController : BaseController<Guid, WarehousePutAway>, IWarehousePutAway
    {
        readonly Repository _repository;

        public WarehousePutAwayController(Repository repository = null!) : base(repository.SWarehousePutAways)
        {
            _repository = repository;
        }
        [HttpGet(ApiRoutes.WarehousePutAway.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePutAway>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            return await _repository.SWarehousePutAways.GetByMasterCodeAsync(putAwayNo);
        }

        [HttpPost(ApiRoutes.WarehousePutAway.InsertWarehousePutAwayOrder)]
        public async Task<Result<IEnumerable<WarehousePutAwayDto>>> InsertWarehousePutAwayOrder([Body] IEnumerable<WarehousePutAwayDto> request) => await _repository.SWarehousePutAways.InsertWarehousePutAwayOrder(request);

        [HttpGet(ApiRoutes.WarehousePutAway.GetPutAwayAsync)]
        public async Task<Result<WarehousePutAwayDto>> GetPutAwayAsync(string PutAwayNo) => await _repository.SWarehousePutAways.GetPutAwayAsync(PutAwayNo);

        [HttpPost(ApiRoutes.WarehousePutAway.SyncHTData)]
        public async Task<Result<WarehousePutAwayDto>> SyncHTData([Body]WarehousePutAwayDto putAwayDto) => await _repository.SWarehousePutAways.SyncHTData(putAwayDto);

        [HttpPost(ApiRoutes.WarehousePutAway.AdjustActionPutAway)]
        public async Task<Result<WarehousePutAwayDto>> AdjustActionPutAway([Body] WarehousePutAwayDto request) => await _repository.SWarehousePutAways.AdjustActionPutAway(request);
        [HttpPost(ApiRoutes.WarehousePutAway.UpdateWarehousePutAwaysStatus)]
        public async Task<Result<WarehousePutAway>> UpdateWarehousePutAwaysStatus([Body] WarehousePutAway request) => await _repository.SWarehousePutAways.UpdateWarehousePutAwaysStatus(request);

        [HttpGet(ApiRoutes.WarehousePutAway.GetPutawayReportAsync)]
        public async Task<Result<List<PutawayReportDTO>>> GetPutawayReportAsync()
        {

            return await _repository.SWarehousePutAways.GetPutawayReportAsync();
          
        }

        [HttpPost(ApiRoutes.WarehousePutAway.CompletePutawayFlow)]
        public async Task<Result<ResponseCompleteModel>> CompletePutawayFlow() => await _repository.SWarehousePutAways.CompletePutawayFlow();

    }
}
