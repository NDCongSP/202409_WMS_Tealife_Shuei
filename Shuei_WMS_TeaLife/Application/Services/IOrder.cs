using Application.DTOs;
using Application.Extentions;
using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.Order.BasePath)]
    public interface IOrder
    {
        [Get(ApiRoutes.Order.GetOrderReport)]
        Task<Result<List<OrderReportDto>>> GetOrderReport();
    }
}
