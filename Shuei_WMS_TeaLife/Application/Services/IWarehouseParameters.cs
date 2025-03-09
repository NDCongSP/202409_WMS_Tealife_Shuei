using Application.Extentions;
using Application.Services.Base;
using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.WarehouseParameter.BasePath)]
    public interface IWarehouseParameters : IRepository<Guid, WarehouseParameter>
    {
        [Get(ApiRoutes.WarehouseParameter.GetFirstOrDefault)]
        Task<Result<WarehouseParameter>> GetFirstOrDefaultAsync();
    }
}
