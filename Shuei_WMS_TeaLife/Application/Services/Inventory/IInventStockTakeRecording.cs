using Application.DTOs;
using Application.DTOs.Request.StockTake;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.InventStockTakeRecording.BasePath)]
    public interface IInventStockTakeRecording : IRepository<Guid, InventStockTakeRecording>
    {
        [Get(ApiRoutes.InventStockTakeRecording.GetAllDTO)]
        Task<Result<List<InventStockTakeRecordingDTO>>> GetAllDTOAsync();

        [Get(ApiRoutes.InventStockTakeRecording.GetByIdDTO)]
        Task<Result<InventStockTakeRecordingDTO>> GetByIdDTOAsync([Path] Guid id);

        [Get(ApiRoutes.InventStockTakeRecording.GetListByStockTakeNoDTOAsync)]
        Task<Result<List<InventStockTakeRecordingDTO>>> GetListByStockTakeNoDTOAsync([Path] string StockTakeNo);

        [Put(ApiRoutes.InventStockTakeRecording.GetStockTakeRecordingAsync)]
        Task<Result<List<InventStockTakeRecordingDTO>>> GetStockTakeRecordingAsync([Body] StockTakeRecordingSearchRequestDto model);

        [Post(ApiRoutes.InventStockTakeRecording.CreateStockTakeRecordingAsync)]
        Task<Result> CreateStockTakeRecordingAsync([Body] InventStockTakeWithDetailsDTO model);

        [Delete(ApiRoutes.InventStockTakeRecording.DeleteStockTakeRecordingAsync)]
        Task<Result> DeleteStockTakeRecordingAsync([Path] Guid StockTakeRecordingId);

        [Patch(ApiRoutes.InventStockTakeRecording.CompleteStockTakeRecordingAsync)]
        Task<Result> CompleteStockTakeRecordingAsync([Path] Guid StockTakeRecordingId);

        // lines
        [Get(ApiRoutes.InventStockTakeRecording.GetLineByStockTakeRecordingIdAsync)]
        Task<Result<List<InventStockTakeRecordingLine>>> GetLineByStockTakeRecordingIdAsync([Path] Guid StockTakeRecordingId);

        [Get(ApiRoutes.InventStockTakeRecording.GetLineByStockTakeNoDTOAsync)]
        Task<Result<List<InventStockTakeRecordingLine>>> GetLineByStockTakeNoDTOAsync([Path] string StockTakeNo);

        [Put(ApiRoutes.InventStockTakeRecording.UpdateStockTakeRecordingLinesAsync)]
        Task<Result> UpdateStockTakeRecordingLinesAsync([Body] List<InventStockTakeRecordingLineDtos> models);
    }
}
