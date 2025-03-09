using Application.DTOs;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services.Base;
using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehouseReceiptOrder.BasePath)]
    public interface IWarehouseReceiptOrder:IRepository<Guid, WarehouseReceiptOrder>
    {
        [Get(ApiRoutes.WarehouseReceiptOrder.GetByMasterCodeAsync)]
        Task<Result<List<WarehouseReceiptOrder>>> GetByMasterCodeAsync([Path] string receiptNo);

        [Post(ApiRoutes.WarehouseReceiptOrder.InsertWarehouseReceiptOrder)]
        Task<Result<WarehouseReceiptOrderDto>> InsertWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request);

        [Post(ApiRoutes.WarehouseReceiptOrder.UpdateWarehouseReceiptOrder)]
        Task<Result<WarehouseReceiptOrderDto>> UpdateWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request);

        [Get(ApiRoutes.WarehouseReceiptOrder.GetReceiptOrderAsync)]
        Task<Result<WarehouseReceiptOrderDto>> GetReceiptOrderAsync([Path] string receiptNo);

        [Get(ApiRoutes.WarehouseReceiptOrder.GetReceiptOrderListAsync)]
        Task<Result<List<WarehouseReceiptOrderDto>>> GetReceiptOrderListAsync();

        [Post(ApiRoutes.WarehouseReceiptOrder.SyncHTData)]
        Task<Result<WarehouseReceiptOrderDto>> SyncHTData([Body] WarehouseReceiptOrderDto receiptDto);
        [Post(ApiRoutes.WarehouseReceiptOrder.CreateLineFromArrivalNo)]
        Task<Result<WarehouseReceiptOrderDto>> CreateLineFromArrivalNo([Body] WarehouseReceiptOrderDto receiptDto);

        [Post(ApiRoutes.WarehouseReceiptOrder.AdjustActionReceiptOrder)]
        Task<Result<WarehouseReceiptOrderDto>> AdjustActionReceiptOrder([Body] WarehouseReceiptOrderDto request);

        [Post(ApiRoutes.WarehouseReceiptOrder.CreateReceiptFromReceiptPlan)]
        Task<Result<string>> CreateReceiptFromReceiptPlan([Path] int arrivalNo);
        
        [Post(ApiRoutes.WarehouseReceiptOrder.SearchReceiptOrderListAsync)]
        Task<Result<PageList<WarehouseReceiptOrderDto>>> SearchReceiptOrderListAsync([Body] QueryModel<WarehouseReceiptSearchModel> model);
        [Post(ApiRoutes.WarehouseReceiptOrder.GenerateReceiptFromReceiptPlan)]
        Task<Tuple<int, int>> GenerateReceiptFromReceiptPlan([Path]string tenantIds);

        [Get(ApiRoutes.WarehouseReceiptOrder.GetReceiptReportAsync)]
        Task<Result<List<ReceiptReportDTO>>> GetReceiptReportAsync();

        [Post(ApiRoutes.WarehouseReceiptOrder.CompleteAndPutAwayReceipts)]
        Task<Result<ResponseCompleteModel>> CompleteAndPutAwayReceipts();
    }
}
