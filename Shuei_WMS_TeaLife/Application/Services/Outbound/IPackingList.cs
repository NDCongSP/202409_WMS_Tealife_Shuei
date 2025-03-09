using Application.DTOs;
using Application.DTOs.Request.shipment;
using Application.Extentions;
using RestEase;

namespace Application.Services.Outbound
{
    [BasePath(ApiRoutes.PackingList.BasePath)]
    public interface IPackingList
    {
        [Post(ApiRoutes.PackingList.GetDataMasterAsync)]
        Task<Result<List<WarehousePackingListDto>>> GetDataMasterAsync([Body] PackingListSearchRequestDto model);

        [Post(ApiRoutes.PackingList.UpdatePackedQtyAsync)]
        Task<Result<string>> UpdatePackedQtyAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model);

        [Post(ApiRoutes.PackingList.CompletePackingAsync)]
        Task<Result<string>> CompletePackingAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model);

        [Get(ApiRoutes.PackingList.GetDataMasterByTrackingNoAsync)]
        Task<Result<WarehousePackingListDto>> GetDataMasterByTrackingNoAsync([Path] string trackingNo);

        [Get(ApiRoutes.PackingList.GetDataMasterByShipmentNoAsync)]
        Task<Result<WarehousePackingListDto>> GetDataMasterByShipmentNoAsync([Path] string shipmentNo);

        [Get(ApiRoutes.PackingList.GetPdfAsBase64Async)]
        Task<Result<string>> GetPdfAsBase64Async([Path] string filePath);
        [Get(ApiRoutes.PackingList.GetPackingLabelFilePathAsync)]
        Task<Result<OrderDispatch>> GetPackingLabelFilePathAsync([Path] string trackingNo);

        [Post(ApiRoutes.PackingList.UpdateOrderDispatchesFilePathAsync)]
        Task<Result<string>> UpdateOrderDispatchesFilePathAsync([Body] OrderDispatch model);
    }
}
