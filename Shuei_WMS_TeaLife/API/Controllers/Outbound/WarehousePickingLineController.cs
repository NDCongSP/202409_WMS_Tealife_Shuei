using API.Controllers.Base;
using Application.DTOs;
using Application.Extentions;
using Application.Services.Outbound;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Outbound
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousePickingLineController : BaseController<Guid, WarehousePickingLine>, IWarehousePickingLine
    {
        readonly Repository _repository;

        public WarehousePickingLineController(Repository repository = null!) : base(repository.SWarehousePickingLine)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehousePickingLine.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehousePickingLine>>> GetByMasterCodeAsync([Path] string pickNo)
        {
            return await _repository.SWarehousePickingLine.GetByMasterCodeAsync(pickNo);
        }

        [HttpGet(ApiRoutes.WarehousePickingLine.GetPickingLineDTOAsync)]
        public async Task<Result<List<WarehousePickingLineDTO>>> GetPickingLineDTOAsync([Path] string pickNo)
        {
            return await _repository.SWarehousePickingLine.GetPickingLineDTOAsync(pickNo);
        }

        [HttpGet(ApiRoutes.WarehousePickingLine.GetShipmentsByPickAsync)]
        public async Task<Result<List<WarehousePickingShipmentDTO>>> GetShipmentsByPickAsync([Path] string pickNo)
        {
            return await _repository.SWarehousePickingLine.GetShipmentsByPickAsync(pickNo);
        }

        [HttpPut(ApiRoutes.WarehousePickingLine.UpdateWarehousePickingLinesAsync)]
        public async Task<Result> UpdateWarehousePickingLinesAsync([FromBody] List<WarehousePickingLineDTO> models)
        {
            return await _repository.SWarehousePickingLine.UpdateWarehousePickingLinesAsync(models);
        }
    }
}
