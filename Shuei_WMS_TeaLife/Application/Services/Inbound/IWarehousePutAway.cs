using Application.DTOs;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehousePutAway.BasePath)]
    public interface IWarehousePutAway : IRepository<Guid, WarehousePutAway>
    {
        [Get(ApiRoutes.WarehousePutAway.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePutAway>>> GetByMasterCodeAsync([Path] string putAwayNo);

        [Post(ApiRoutes.WarehousePutAway.InsertWarehousePutAwayOrder)]
        Task<Result<IEnumerable<WarehousePutAwayDto>>> InsertWarehousePutAwayOrder([Body] IEnumerable<WarehousePutAwayDto> request);

        [Get(ApiRoutes.WarehousePutAway.GetPutAwayAsync)]
        Task<Result<WarehousePutAwayDto>> GetPutAwayAsync(string PutAwayNo);

        [Post(ApiRoutes.WarehousePutAway.SyncHTData)]
        Task<Result<WarehousePutAwayDto>> SyncHTData([Body] WarehousePutAwayDto putAwayDto);

        [Post(ApiRoutes.WarehousePutAway.AdjustActionPutAway)]
        Task<Result<WarehousePutAwayDto>> AdjustActionPutAway([Body] WarehousePutAwayDto request);

        [Post(ApiRoutes.WarehousePutAway.UpdateWarehousePutAwaysStatus)]
        Task<Result<WarehousePutAway>> UpdateWarehousePutAwaysStatus([Body] WarehousePutAway request);

        [Get(ApiRoutes.WarehousePutAway.GetPutawayReportAsync)]
        Task<Result<List<PutawayReportDTO>>> GetPutawayReportAsync();

        [Post(ApiRoutes.WarehousePutAway.CompletePutawayFlow)]
        Task<Result<ResponseCompleteModel>> CompletePutawayFlow();
    }
}
