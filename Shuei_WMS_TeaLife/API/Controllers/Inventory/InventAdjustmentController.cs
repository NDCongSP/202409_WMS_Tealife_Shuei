using API.Controllers.Base;
using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inventory
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventAdjustmentController : BaseController<Guid, InventAdjustment>, IInventAdjustment
    {
        readonly Repository _repository;

        public InventAdjustmentController(Repository repository = null) : base(repository.SInventAdjustments)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.InventAdjustment.GetAllDTO)]
        public async Task<Result<List<InventAdjustmentDTO>>> GetAllDTOAsync()
        {
            return await _repository.SInventAdjustments.GetAllDTOAsync();
        }
        [HttpGet(ApiRoutes.InventAdjustment.GetByAdjustmentNoDTO)]
        public async Task<Result<InventAdjustmentDTO>> GetByAdjustmentNoDTOAsync([Path] string adjustmentNo)
        {
            return await _repository.SInventAdjustments.GetByAdjustmentNoDTOAsync(adjustmentNo);
        }
        [HttpGet(ApiRoutes.InventAdjustment.GetByIdDTO)]
        public async Task<Result<InventAdjustmentDTO>> GetByIdDTOAsync([Path] Guid id)
        {
            return await _repository.SInventAdjustments.GetByIdDTOAsync(id);
        }

        [HttpPost(ApiRoutes.InventAdjustment.CompletedAdjustment)]
        public async Task<Result<bool>> CompletedInventAdjustmentAsync([Body] Guid id)
        {
            return await _repository.SInventAdjustments.CompletedInventAdjustmentAsync(id);
        }

        [HttpPost(ApiRoutes.InventAdjustment.DeleteAllAdjustmentLineAsync)]
        public async Task<Result<bool>> DeleteAllAdjustmentLineAsync([Path] Guid id)
        {
            return await _repository.SInventAdjustments.DeleteAllAdjustmentLineAsync(id);
        }
    }
}
