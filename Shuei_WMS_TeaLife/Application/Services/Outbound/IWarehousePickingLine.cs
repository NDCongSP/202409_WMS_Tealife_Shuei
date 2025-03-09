using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehousePickingLine.BasePath)]
    public interface IWarehousePickingLine : IRepository<Guid, WarehousePickingLine>
    {
        [Get(ApiRoutes.WarehousePickingLine.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePickingLine>>> GetByMasterCodeAsync([Path] string pickNo);

        [Get(ApiRoutes.WarehousePickingLine.GetPickingLineDTOAsync)]
        Task<Result<List<WarehousePickingLineDTO>>> GetPickingLineDTOAsync([Path] string pickNo);

        [Get(ApiRoutes.WarehousePickingLine.GetShipmentsByPickAsync)]
        Task<Result<List<WarehousePickingShipmentDTO>>> GetShipmentsByPickAsync([Path] string pickNo);

        [Put(ApiRoutes.WarehousePickingLine.UpdateWarehousePickingLinesAsync)]
        Task<Result> UpdateWarehousePickingLinesAsync([Body] List<WarehousePickingLineDTO> models);

    }
}
