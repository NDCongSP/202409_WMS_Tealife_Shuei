using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.Batches.BasePath)]
    public interface IBatches : IRepository<Guid, Batches>
    {
        [Post(ApiRoutes.Batches.GetBatchByLotNo)]
        Task<Result<Batches>> GetBatchByLotNo([Body]GetBatchByLotNoDto data);
        [Post(ApiRoutes.Batches.GetBatchByLotNo)]
        Task<Result> AddUpdateBatchByAdjusment([Body]List<InventAdjustmentLine> data);
    }
}
