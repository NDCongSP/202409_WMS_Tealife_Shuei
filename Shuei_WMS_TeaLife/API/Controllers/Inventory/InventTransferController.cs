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
    public class InventTransferController : BaseController<Guid, InventTransfer>, IInventTransfer
    {
        readonly Repository _repository;

        public InventTransferController(Repository repository = null) : base(repository.SInventTransfer)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.InventTransfer.CompletedTransfer)]
        public async Task<Result<bool>> CompletedInventTransfer([Path] Guid id)
        {
            return await _repository.SInventTransfer.CompletedInventTransfer(id);
        }

        [HttpGet(ApiRoutes.InventTransfer.GetAllDTO)]
        public async Task<Result<List<InventTransfersDTO>>> GetAllDTO()
        {
            return await _repository.SInventTransfer.GetAllDTO();
        }

        [HttpGet(ApiRoutes.InventTransfer.GetByIdDTO)]
        public async Task<Result<InventTransfersDTO>> GetByIdDTO([Path] Guid id)
        {
            return await _repository.SInventTransfer.GetByIdDTO(id);
        }

        [HttpGet(ApiRoutes.InventTransfer.GetByTransferNoDTO)]
        public async Task<Result<InventTransfersDTO>> GetByTransferNoDTO([Path] string transferNo)
        {
            return await _repository.SInventTransfer.GetByTransferNoDTO(transferNo);
        }
    }
}
