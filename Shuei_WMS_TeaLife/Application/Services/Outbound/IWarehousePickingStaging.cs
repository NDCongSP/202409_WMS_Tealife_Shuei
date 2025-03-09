using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehousePickingStaging.BasePath)]
    public interface IWarehousePickingStaging : IRepository<Guid, WarehousePickingStaging>
    {
        [Get(ApiRoutes.WarehousePickingStaging.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePickingStaging>>> GetByMasterCodeAsync([Path] string pickNo);
    }
}
