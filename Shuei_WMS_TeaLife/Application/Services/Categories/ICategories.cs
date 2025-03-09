using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;
using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.Categories.BasePath)]
    public interface ICategories
    {
        [Get(ApiRoutes.Categories.GetUsers)]
        Task<Result<List<SelectListItem>>> GetUserDropdown();
        
        [Get(ApiRoutes.Categories.GetBinByLocationId)]
        Task<Result<List<SelectListItem>>> GetBinByLocation(string? locationId = default);

        [Get(ApiRoutes.Categories.GetOrders)]
        Task<List<OrderSelectList>> GetOrders(string? orderId = default);
    }
}
