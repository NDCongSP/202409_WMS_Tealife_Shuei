using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.InventTransferLines.BasePath)]
    public interface IInventTransferLines : IRepository<Guid, InventTransferLine>
    {
        //[Get(ApiRoutes.WarehouseShipmentLine.GetByMasterCodeAsync)]
        //Task<Result<List<ShippingCarrier>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
