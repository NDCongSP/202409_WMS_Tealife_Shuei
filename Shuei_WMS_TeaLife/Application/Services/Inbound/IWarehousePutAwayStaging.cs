using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehousePutAwayStaging.BasePath)]
    public interface IWarehousePutAwayStaging : IRepository<Guid, WarehousePutAwayStaging>
    {
        [Get(ApiRoutes.WarehousePutAwayStaging.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePutAwayStaging>>> GetByMasterCodeAsync([Path] string putAwayNo);
        
        [Get(ApiRoutes.WarehousePutAwayStaging.GetByPutAwayLineIdAsync)]
        Task<Result<List<WarehousePutAwayStaging>>> GetByPutAwayLineIdAsync(Guid putAwayLineId);
    }
}
