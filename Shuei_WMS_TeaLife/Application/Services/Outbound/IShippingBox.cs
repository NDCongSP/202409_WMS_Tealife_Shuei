using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.ShippingBox.BasePath)]
    public interface IShippingBox : IRepository<Guid, ShippingBox>
    {
        [Get(ApiRoutes.ShippingBox.GetByShippingCarrierCodeAsync)]
        Task<Result<List<ShippingBox>>> GetByShippingCarrierCodeAsync([Path] string shippingCarrierCode);

        [Get(ApiRoutes.ShippingBox.GetAllShippingCarrierAsync)]
        Task<Result<List<ShippingCarrierDTO>>> GetAllShippingCarrierAsync();

        [Get(ApiRoutes.ShippingBox.GetAllWithCarrierAsync)]
        Task<Result<List<ShippingBoxDTO>>> GetAllWithCarrierAsync();


    }
}
