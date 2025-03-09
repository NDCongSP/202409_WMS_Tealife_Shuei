using API.Controllers.Base;
using Application.DTOs;
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
    public class WarehouseReceiptOrderLineController : BaseController<Guid, WarehouseReceiptOrderLine>, IWarehouseReceiptOrderLine
    {
        readonly Repository _repository;

        public WarehouseReceiptOrderLineController(Repository repository = null!) : base(repository.SWarehouseReceiptOrderLines)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehouseReceiptOrderLine.GetAllDTOAsync)]
        public async Task<Result<List<WarehouseReceiptOrderLineResponseDTO>>> GetAllDTOAsync()
        {
            return await _repository.SWarehouseReceiptOrderLines.GetAllDTOAsync();
        }
        [HttpGet(ApiRoutes.WarehouseReceiptOrderLine.GetByIdDTOAsync)]
        public async Task<Result<WarehouseReceiptOrderLineResponseDTO>> GetByIdDTOAsync([Path] Guid id)
        {
            return await _repository.SWarehouseReceiptOrderLines.GetByIdDTOAsync(id);
        }

        [HttpGet(ApiRoutes.WarehouseReceiptOrderLine.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehouseReceiptOrderLine>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            return await _repository.SWarehouseReceiptOrderLines.GetByMasterCodeAsync(receiptNo);
        }

        [HttpGet(ApiRoutes.WarehouseReceiptOrderLine.GetByProductCodeAsync)]
        public async Task<Result<WarehouseReceiptOrderLine>> GetByProducCodetAsync([Path] string productCode)
        {
            return await _repository.SWarehouseReceiptOrderLines.GetByProducCodetAsync(productCode);
        }
    }
}
