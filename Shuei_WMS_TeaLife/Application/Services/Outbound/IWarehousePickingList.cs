using Application.DTOs;
using Application.DTOs.Request;
using Application.DTOs.Request.Picking;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.WarehousePickingList.BasePath)]
    public interface IWarehousePickingList : IRepository<Guid, WarehousePickingList>
    {
        [Get(ApiRoutes.WarehousePickingList.GetByMasterCodeAsync)]
        Task<Result<List<WarehousePickingList>>> GetByMasterCodeAsync([Path] string pickNo);

        [Put(ApiRoutes.WarehousePickingList.GetWarehousePickingDTOAsync)]
        Task<Result<List<WarehousePickingDTO>>> GetWarehousePickingDTOAsync([Body] QueryModel<PickingListSearchRequestDto> model);

        [Delete(ApiRoutes.WarehousePickingList.DeletePickingAsync)]
        Task<Result> DeletePickingAsync([Path] string pickNo);

        [Patch(ApiRoutes.WarehousePickingList.CompletePickingAsync)]
        Task<Result> CompletePickingAsync([Path] string pickNo);

        [Put(ApiRoutes.WarehousePickingList.SyncToHTAsync)]
        Task<Result<List<WarehousePickingLineDTO>>> SyncToHTTmpAsync([Body] List<WarehousePickingLineDTO> model);

        [Get(ApiRoutes.WarehousePickingList.GetBarcodeDataAsync)]
        Task<string> GetBarcodeDataAsync([Path] string No);

        [Post(ApiRoutes.WarehousePickingList.GetDataCoverSheetNDeliveryNote)]
        Task<List<CoverSheetNDeliveryNoteDto>> GetDataCoverSheetNDeliveryNote([Body] List<string> ids);

        [Post(ApiRoutes.WarehousePickingList.CompletedPickings)]
        Task<Result> CompletePickingsAsync([Body] List<string> pickNos);

        [Post(ApiRoutes.WarehousePickingList.AutoCompletedPickings)]
        Task<Result<ResponseCompleteModel>> AutoCompletePickingsAsync();
    }
}
