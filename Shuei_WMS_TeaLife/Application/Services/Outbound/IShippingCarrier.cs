using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.ShippingCarier.BasePath)]
    public interface IShippingCarrier : IRepository<Guid, ShippingCarrier>
    {
        //[Get(ApiRoutes.WarehouseShipmentLine.GetByMasterCodeAsync)]
        //Task<Result<List<ShippingCarrier>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
