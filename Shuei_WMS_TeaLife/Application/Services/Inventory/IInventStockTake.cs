using Application.DTOs.Request;
using Application.DTOs;
using Application.Extentions;
using Application.Extentions.Pagings;
using RestEase;
using Application.Services.Base;

namespace Application.Services.Inventory
{
    [BasePath(ApiRoutes.InventStockTake.BasePath)]
    public interface IInventStockTake : IRepository<Guid, InventStockTake>
    {
        [Get(ApiRoutes.InventStockTake.GetAll)]
        Task<Result<List<InventStockTakeDto>>> GetAll();

        [Get(ApiRoutes.InventStockTake.GetByStockTakeNo)]
        Task<Result<InventStockTakeDto>> GetByStockTakeNo(string StockTakeNo);

        [Post(ApiRoutes.InventStockTake.Insert)]
        Task<Result<InventStockTakeDto>> Insert([Body] InventStockTakeDto dto);

        [Post(ApiRoutes.InventStockTake.Update)]
        Task<Result<InventStockTakeDto>> Update([Body] InventStockTakeDto dto);

        [Post(ApiRoutes.InventStockTake.Delete)]
        Task<Result<bool>> Delete(Guid id);

        [Post(ApiRoutes.InventStockTake.GetStockTakeAsync)]
        Task<Result<List<InventStockTakeDto>>> GetStockTakeAsync([Body] InventStockTakeSearchModel model);

        [Post(ApiRoutes.InventStockTake.CompleteInventStockTake)]
        Task<Result<bool>> CompleteInventStockTake([Body] InventStockTakeDto dto);

        [Get(ApiRoutes.InventStockTake.GetStockTakeLineByIdAsync)]
        Task<Result<List<InventStockTakeLineDto>>> GetStockTakeLineByIdAsync([Path] string StockTakeNo);
        
        [Post(ApiRoutes.InventStockTake.CheckProductExistenceinStocktake)]
        Task<Result<bool>> CheckProductExistenceinStocktake([Body] InventStockTakeLineDto InventStockTakeLineDtos);

        [Get(ApiRoutes.InventStockTake.GetNewStocktakeNo)]
        Task<Result<string>> GetNewStocktakeNo();

        [Get(ApiRoutes.InventStockTake.GetCurrentUserAsync)]
        Task<Result<string>> GetCurrentUserAsync();
    }
}
