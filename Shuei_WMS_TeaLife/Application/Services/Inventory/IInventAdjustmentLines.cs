using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.InventAdjustmentLines.BasePath)]
    public interface IInventAdjustmentLines : IRepository<Guid, InventAdjustmentLine>
    {
        //[Get(ApiRoutes.WarehouseShipmentLine.GetByMasterCodeAsync)]
        //Task<Result<List<ShippingCarrier>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
