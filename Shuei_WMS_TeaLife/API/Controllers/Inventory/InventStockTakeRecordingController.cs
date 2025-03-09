using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request.Picking;
using Application.DTOs.Request.StockTake;
using Application.Extentions;
using Application.Services;
using FBT.ShareModels.WMS;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inventory
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventStockTakeRecordingController : BaseController<Guid, InventStockTakeRecording>, IInventStockTakeRecording
    {
        readonly Repository _repository;

        public InventStockTakeRecordingController(Repository repository = null) : base(repository.SInventStockTakeRecordings)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.InventStockTakeRecording.GetAllDTO)]
        public async Task<Result<List<InventStockTakeRecordingDTO>>> GetAllDTOAsync()
        {
            return await _repository.SInventStockTakeRecordings.GetAllDTOAsync();
        }

        [HttpGet(ApiRoutes.InventStockTakeRecording.GetByIdDTO)]
        public async Task<Result<InventStockTakeRecordingDTO>> GetByIdDTOAsync([Path] Guid id)
        {
            return await _repository.SInventStockTakeRecordings.GetByIdDTOAsync(id);
        }

        [HttpGet(ApiRoutes.InventStockTakeRecording.GetListByStockTakeNoDTOAsync)]
        public async Task<Result<List<InventStockTakeRecordingDTO>>> GetListByStockTakeNoDTOAsync([Path] string StockTakeNo)
        {
            return await _repository.SInventStockTakeRecordings.GetListByStockTakeNoDTOAsync(StockTakeNo);
        }

        [HttpPut(ApiRoutes.InventStockTakeRecording.GetStockTakeRecordingAsync)]
        public async Task<Result<List<InventStockTakeRecordingDTO>>> GetStockTakeRecordingAsync([Body] StockTakeRecordingSearchRequestDto model)
        {
            return await _repository.SInventStockTakeRecordings.GetStockTakeRecordingAsync(model);
        }

        [HttpPost(ApiRoutes.InventStockTakeRecording.CreateStockTakeRecordingAsync)]
        public async Task<Result> CreateStockTakeRecordingAsync([Body] InventStockTakeWithDetailsDTO model)
        {
            return await _repository.SInventStockTakeRecordings.CreateStockTakeRecordingAsync(model);
        }

        [HttpDelete(ApiRoutes.InventStockTakeRecording.DeleteStockTakeRecordingAsync)]
        public async Task<Result> DeleteStockTakeRecordingAsync([Path] Guid StockTakeRecordingId)
        {
            return await _repository.SInventStockTakeRecordings.DeleteStockTakeRecordingAsync(StockTakeRecordingId);
        }

        [HttpPatch(ApiRoutes.InventStockTakeRecording.CompleteStockTakeRecordingAsync)]
        public async Task<Result> CompleteStockTakeRecordingAsync([Path] Guid StockTakeRecordingId)
        {
            return await _repository.SInventStockTakeRecordings.CompleteStockTakeRecordingAsync(StockTakeRecordingId);
        }

        // lines
        [HttpGet(ApiRoutes.InventStockTakeRecording.GetLineByStockTakeRecordingIdAsync)]
        public async Task<Result<List<InventStockTakeRecordingLine>>> GetLineByStockTakeRecordingIdAsync([Path] Guid StockTakeRecordingId)
        {
            return await _repository.SInventStockTakeRecordings.GetLineByStockTakeRecordingIdAsync(StockTakeRecordingId);
        }

        [HttpGet(ApiRoutes.InventStockTakeRecording.GetLineByStockTakeNoDTOAsync)]
        async Task<Result<List<InventStockTakeRecordingLine>>> IInventStockTakeRecording.GetLineByStockTakeNoDTOAsync([Path] string StockTakeNo)
        {
            return await _repository.SInventStockTakeRecordings.GetLineByStockTakeNoDTOAsync(StockTakeNo);
        }

        [HttpPut(ApiRoutes.InventStockTakeRecording.UpdateStockTakeRecordingLinesAsync)]
        public async Task<Result> UpdateStockTakeRecordingLinesAsync([FromBody] List<InventStockTakeRecordingLineDtos> models)
        {
            return await _repository.SInventStockTakeRecordings.UpdateStockTakeRecordingLinesAsync(models);
        }
    }
}
