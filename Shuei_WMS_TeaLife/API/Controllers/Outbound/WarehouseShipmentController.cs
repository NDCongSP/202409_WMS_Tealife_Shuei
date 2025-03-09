using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Extentions.Pagings;
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
    public class WarehouseShipmentController : BaseController<Guid, WarehouseShipment>, IWarehouseShipment
    {
        readonly Repository _repository;

        public WarehouseShipmentController(Repository repository = null!) : base(repository.SWarehouseShipment)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.WarehouseShipment.CreateAsync)]
        public async Task<Result<WarehouseShipmentDto>> CreateWarehouseShipmentAsync([Body] WarehouseShipmentDto model)
        {
            return await _repository.SWarehouseShipment.CreateWarehouseShipmentAsync(model);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.SearchAsync)]
        public async Task<Result<PageList<WarehouseShipmentDto>>> SearchWhShipments(QueryModel<WarehouseShipmentSearchModel> model)
        {
            return await _repository.SWarehouseShipment.SearchWhShipments(model);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.UpdateAsync)]
        public async Task<Result<WarehouseShipmentDto>> UpdateWarehouseShipmentAsync(WarehouseShipmentDto model)
        {
            return await _repository.SWarehouseShipment.UpdateWarehouseShipmentAsync(model);
        }

        [HttpGet(ApiRoutes.WarehouseShipment.GetAsync)]
        public async Task<Result<WarehouseShipmentDto>> GetShipmentByIdAsync([Path] Guid id)
        {
            return await _repository.SWarehouseShipment.GetShipmentByIdAsync(id);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.DeleteAsync)]
        public async Task<Result<bool>> DeleteShipmentAsync([Path] Guid id)
        {
            return await _repository.SWarehouseShipment.DeleteShipmentAsync(id);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.ConfirmShipmentAsync)]
        public async Task<Result<bool>> ConfirmShipmentAsync([Path] Guid id)
        {
            return await _repository.SWarehouseShipment.ConfirmShipmentAsync(id);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.CreatePickingAsync)]
        public async Task<CreatePickingModel> CreatePickingAsync([Body] SubmitCompletedShipmentDto data)
        {
            return await _repository.SWarehouseShipment.CreatePickingAsync(data);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.CheckMultipleShipmentCreatePicking)]
        public async Task<Result<bool>> CheckMultipleShipmentCreatePicking([Body] List<Guid> ids)
        {
            return await _repository.SWarehouseShipment.CheckMultipleShipmentCreatePicking(ids);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.GetDeliveryNote)]
        public async Task<DeliveryNoteModel> GetDeliveryNote([Path] Guid id)
        {
            return await _repository.SWarehouseShipment.GetDeliveryNote(id);
        }
        [HttpPost(ApiRoutes.WarehouseShipment.GetDeliveryNotes)]
        public async Task<List<DeliveryNoteModel>> GetDeliveryNotes([Body] List<Guid> ids)
        {
            return await _repository.SWarehouseShipment.GetDeliveryNotes(ids);
        }
        [HttpPost(ApiRoutes.WarehouseShipment.CompletedShipmentAsync)]
        public async Task<Result<bool>> CompletedShipmentAsync([Path] Guid id)
        {
            return await _repository.SWarehouseShipment.CompletedShipmentAsync(id);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.CreateMovementAsync)]
        public async Task<Result<WarehouseShipmentDto>> CreateWarehouseMovementAsync([Body] WarehouseShipmentDto model)
        {
            return await _repository.SWarehouseShipment.CreateWarehouseMovementAsync(model);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.UpdateMovementAsync)]
        public async Task<Result<WarehouseShipmentDto>> UpdateMovementAsync([Body] WarehouseShipmentDto model)
        {
            return await _repository.SWarehouseShipment.UpdateMovementAsync(model);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.SearchMovementAsync)]
        public async Task<Result<PageList<WarehouseShipmentDto>>> SearchWhMovements([Body] QueryModel<WarehouseShipmentSearchModel> model)
        {
            return await _repository.SWarehouseShipment.SearchWhMovements(model);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.ShipMovementAsync)]
        public async Task<Result<bool>> ShipMovementAsync([Path] Guid id)
        {
            return await _repository.SWarehouseShipment.ShipMovementAsync(id);
        }

        [HttpGet(ApiRoutes.WarehouseShipment.GetProductByShipmentNoAsync)]
        public async Task<Result<IEnumerable<ProductDto>>> GetProductByShipmentNoAsync(string? code, string shipmentNo) => await _repository.SWarehouseShipment.GetProductByShipmentNoAsync(code, shipmentNo);

        [HttpGet(ApiRoutes.WarehouseShipment.GetByMasterCodeAsync)]
        public async Task<Result<WarehouseShipment>> GetByMasterCodeAsync([Path] string shipmentNo)
        {
            return await _repository.SWarehouseShipment.GetByMasterCodeAsync(shipmentNo);
        }

        [HttpGet(ApiRoutes.WarehouseShipment.GetShippingReport)]
        public async Task<Result<List<ShipmentReportDTO>>> GetShippingReportAsync()
        {
            return await _repository.SWarehouseShipment.GetShippingReportAsync();
        }

        [HttpPost(ApiRoutes.WarehouseShipment.GenerateShipmentFromOrder)]
        public async Task<Tuple<int, int, List<Tuple<string, string>>>> GenerateShipmentFromOrder([Path] string tenantIds)
        {
            return await _repository.SWarehouseShipment.GenerateShipmentFromOrder(tenantIds);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.DHLPickup)]
        public async Task<Result<bool>> DHLPickup([Body] List<Guid> ids)
        {
            return await _repository.SWarehouseShipment.DHLPickup(ids);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.CompletedShipments)]
        public async Task<Result> CompleteShipmentsAsync([Body] List<string> shipmentNos)
        {
            return await _repository.SWarehouseShipment.CompleteShipmentsAsync(shipmentNos);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.AutoCreatePickingAsync)]
        public async Task<CreatePickingModel> CreateAutoPickingAsync([Path] string remarks, [Path] string tenantIds)
        {
            return await _repository.SWarehouseShipment.CreateAutoPickingAsync(remarks, tenantIds);
        }

        [HttpPost(ApiRoutes.WarehouseShipment.CreatePickingManualAsync)]
        public async Task<Result<string>> CreatePickingManualAsync([Body]SubmitCompletedShipmentDto data)
        {
            return await _repository.SWarehouseShipment.CreatePickingManualAsync(data);
        }
    }
}
