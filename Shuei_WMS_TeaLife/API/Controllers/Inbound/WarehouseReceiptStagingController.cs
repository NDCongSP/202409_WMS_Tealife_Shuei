using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Services.Inbound;


using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseReceiptStagingController : BaseController<Guid, WarehouseReceiptStaging>, IWarehouseReceiptStaging
    {
        readonly Repository _repository;

        public WarehouseReceiptStagingController(Repository repository = null!) : base(repository.SWarehouseReceiptStagings)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehouseReceiptStaging.GetAllDTOAsync)]
        public async Task<Result<List<WarehouseReceiptStagingResponseDTO>>> GetAllDTOAsync()
        {
            return await _repository.SWarehouseReceiptStagings.GetAllDTOAsync();
        }
        [HttpGet(ApiRoutes.WarehouseReceiptStaging.GetByReceiptLineIdDTOAsync)]
        public async Task<Result<WarehouseReceiptStagingResponseDTO>> GetByReceiptLineIdDTOAsync([Path] Guid receiptLineId)
        {
            return await _repository.SWarehouseReceiptStagings.GetByReceiptLineIdDTOAsync(receiptLineId);
        }

        [HttpGet(ApiRoutes.WarehouseReceiptStaging.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehouseReceiptStaging>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            return await _repository.SWarehouseReceiptStagings.GetByMasterCodeAsync(receiptNo);
        }
        [HttpGet(ApiRoutes.WarehouseReceiptStaging.GetByReceiptLineIdAsync)]
        public async Task<Result<WarehouseReceiptStaging>> GetByReceiptLineIdAsync([Path] Guid receiptLineId)
        {
            return await _repository.SWarehouseReceiptStagings.GetByReceiptLineIdAsync(receiptLineId);
        }

        [HttpPost(ApiRoutes.WarehouseReceiptStaging.UploadProductErrorImageAsync)]
        public async Task<Result<string>> UploadProductErrorImageAsync([Body] List<UpdateReceiptImageRequestDTO> model)
        {
            return await _repository.SWarehouseReceiptStagings.UploadProductErrorImageAsync(model);
        }
    }
}
