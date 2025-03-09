using Application.DTOs;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services.Base;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehouseShipment.BasePath)]
    public interface IWarehouseShipment : IRepository<Guid, WarehouseShipment>
    {
        [Post(ApiRoutes.WarehouseShipment.CreateAsync)]
        Task<Result<WarehouseShipmentDto>> CreateWarehouseShipmentAsync([Body] WarehouseShipmentDto model);
        
        [Post(ApiRoutes.WarehouseShipment.UpdateAsync)]
        Task<Result<WarehouseShipmentDto>> UpdateWarehouseShipmentAsync([Body] WarehouseShipmentDto model);
        
        [Post(ApiRoutes.WarehouseShipment.SearchAsync)]
        Task<Result<PageList<WarehouseShipmentDto>>> SearchWhShipments([Body]QueryModel<WarehouseShipmentSearchModel> model);

        [Get(ApiRoutes.WarehouseShipment.GetAsync)]
        Task<Result<WarehouseShipmentDto>> GetShipmentByIdAsync([Path] Guid id);

        [Post(ApiRoutes.WarehouseShipment.DeleteAsync)]
        Task<Result<bool>> DeleteShipmentAsync([Path] Guid id);

        [Post(ApiRoutes.WarehouseShipment.ConfirmShipmentAsync)]
        Task<Result<bool>> ConfirmShipmentAsync([Path] Guid id);

        [Post(ApiRoutes.WarehouseShipment.CreatePickingManualAsync)]
        Task<Result<string>> CreatePickingManualAsync([Body] SubmitCompletedShipmentDto data);

        [Post(ApiRoutes.WarehouseShipment.CreatePickingAsync)]
        Task<CreatePickingModel> CreatePickingAsync([Body]SubmitCompletedShipmentDto data);

        [Post(ApiRoutes.WarehouseShipment.AutoCreatePickingAsync)]
        Task<CreatePickingModel> CreateAutoPickingAsync([Path] string remarks, [Path] string tenantIds);

        [Post(ApiRoutes.WarehouseShipment.CheckMultipleShipmentCreatePicking)]
        Task<Result<bool>> CheckMultipleShipmentCreatePicking([Body] List<Guid> ids);

        [Post(ApiRoutes.WarehouseShipment.GetDeliveryNote)]
        Task<DeliveryNoteModel> GetDeliveryNote([Path] Guid id);
        [Post(ApiRoutes.WarehouseShipment.GetDeliveryNotes)]
        Task<List<DeliveryNoteModel>> GetDeliveryNotes([Body] List<Guid> ids);

        [Post(ApiRoutes.WarehouseShipment.CompletedShipmentAsync)]
        Task<Result<bool>> CompletedShipmentAsync([Path] Guid id);

        [Post(ApiRoutes.WarehouseShipment.CreateMovementAsync)]
        Task<Result<WarehouseShipmentDto>> CreateWarehouseMovementAsync([Body] WarehouseShipmentDto model);

        [Post(ApiRoutes.WarehouseShipment.UpdateMovementAsync)]
        Task<Result<WarehouseShipmentDto>> UpdateMovementAsync([Body] WarehouseShipmentDto model);

        [Post(ApiRoutes.WarehouseShipment.SearchMovementAsync)]
        Task<Result<PageList<WarehouseShipmentDto>>> SearchWhMovements([Body] QueryModel<WarehouseShipmentSearchModel> model);

        [Post(ApiRoutes.WarehouseShipment.ShipMovementAsync)]
        Task<Result<bool>> ShipMovementAsync([Path] Guid id);

        [Get(ApiRoutes.WarehouseShipment.GetProductByShipmentNoAsync)]
        Task<Result<IEnumerable<ProductDto>>> GetProductByShipmentNoAsync(string? code, string shipmentNo);

        [Get(ApiRoutes.WarehouseShipment.GetByMasterCodeAsync)]
        Task<Result<WarehouseShipment>> GetByMasterCodeAsync([Path] string shipmentNo);

        [Get(ApiRoutes.WarehouseShipment.GetShippingReport)]
        Task<Result<List<ShipmentReportDTO>>> GetShippingReportAsync();

        [Get(ApiRoutes.WarehouseShipment.GenerateShipmentFromOrder)]
        Task<Tuple<int, int, List<Tuple<string, string>>>> GenerateShipmentFromOrder([Path] string tenantIds);

        [Post(ApiRoutes.WarehouseShipment.DHLPickup)]
        Task<Result<bool>> DHLPickup([Body] List<Guid> ids);

        [Post(ApiRoutes.WarehouseShipment.CompletedShipments)]
        Task<Result> CompleteShipmentsAsync([Body] List<string> shipmentNos);

    }
}
