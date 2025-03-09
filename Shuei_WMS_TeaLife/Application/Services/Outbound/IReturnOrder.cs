using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services.Base;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.ReturnOrder.BasePath)]
    public interface IReturnOrder : IRepository<Guid, ReturnOrder>
    {
        [Get(ApiRoutes.ReturnOrder.GetAllReturnOrdersAsync)]
        Task<Result<List<ReturnOrderDto>>> GetAllReturnOrdersAsync();

        [Get(ApiRoutes.ReturnOrder.GetReturnOrderByReturnNoAsync)]
        Task<Result<ReturnOrderDto>> GetReturnOrderByReturnNoAsync(string returnOrderNo);

        [Post(ApiRoutes.ReturnOrder.InsertReturnOrderAsync)]
        Task<Result<ReturnOrderDto>> InsertReturnOrderAsync([Body] ReturnOrderDto dto);

        [Post(ApiRoutes.ReturnOrder.UpdateReturnOrderAsync)]
        Task<Result<ReturnOrderDto>> UpdateReturnOrderAsync([Body] ReturnOrderDto dto);

        [Post(ApiRoutes.ReturnOrder.DeleteReturnOrderAsync)]
        Task<Result<bool>> DeleteReturnOrderAsync([Path] Guid id);

        [Post(ApiRoutes.ReturnOrder.SearchReturnOrder)]
        Task<Result<PageList<ReturnOrderDto>>> SearchReturnOrder([Body] QueryModel<ReturnOrderSearchModel> model);

        [Get(ApiRoutes.ReturnOrder.GetReturnByShipmentNo)]
        Task<Result<List<ReturnOrderDto>>> GetReturnByShipmentNo(string shipmentNo);
    }
}
