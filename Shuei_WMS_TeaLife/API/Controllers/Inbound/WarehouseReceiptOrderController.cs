using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services.Inbound;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseReceiptOrderController : BaseController<Guid, WarehouseReceiptOrder>,IWarehouseReceiptOrder
    {
        readonly Repository _repository;

        public WarehouseReceiptOrderController(Repository repository = null!):base(repository.SWarehouseReceiptOrders)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehouseReceiptOrder.GetByMasterCodeAsync)]
        public async Task<Result<List<WarehouseReceiptOrder>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            return await _repository.SWarehouseReceiptOrders.GetByMasterCodeAsync(receiptNo);
        }

        [HttpPost(ApiRoutes.WarehouseReceiptOrder.InsertWarehouseReceiptOrder)]
        public async Task<Result<WarehouseReceiptOrderDto>> InsertWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request) => await _repository.SWarehouseReceiptOrders.InsertWarehouseReceiptOrder(request);
        
        [HttpPost(ApiRoutes.WarehouseReceiptOrder.UpdateWarehouseReceiptOrder)]
        public async Task<Result<WarehouseReceiptOrderDto>> UpdateWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request) => await _repository.SWarehouseReceiptOrders.UpdateWarehouseReceiptOrder(request);

        [HttpGet(ApiRoutes.WarehouseReceiptOrder.GetReceiptOrderAsync)]
        public async Task<Result<WarehouseReceiptOrderDto>> GetReceiptOrderAsync([Path] string receiptNo) => await _repository.SWarehouseReceiptOrders.GetReceiptOrderAsync(receiptNo);

        [HttpGet(ApiRoutes.WarehouseReceiptOrder.GetReceiptOrderListAsync)]
        public async Task<Result<List<WarehouseReceiptOrderDto>>> GetReceiptOrderListAsync() => await _repository.SWarehouseReceiptOrders.GetReceiptOrderListAsync();

        [HttpPost(ApiRoutes.WarehouseReceiptOrder.SyncHTData)]
        public async Task<Result<WarehouseReceiptOrderDto>> SyncHTData([Body] WarehouseReceiptOrderDto receiptDto) => await _repository.SWarehouseReceiptOrders.SyncHTData(receiptDto);
        [HttpPost(ApiRoutes.WarehouseReceiptOrder.CreateLineFromArrivalNo)]
        public async Task<Result<WarehouseReceiptOrderDto>> CreateLineFromArrivalNo([Body] WarehouseReceiptOrderDto receiptDto) => await _repository.SWarehouseReceiptOrders.CreateLineFromArrivalNo(receiptDto);

        [HttpPost(ApiRoutes.WarehouseReceiptOrder.AdjustActionReceiptOrder)]
        public async Task<Result<WarehouseReceiptOrderDto>> AdjustActionReceiptOrder([Body] WarehouseReceiptOrderDto request) => await _repository.SWarehouseReceiptOrders.AdjustActionReceiptOrder(request);

        [HttpPost(ApiRoutes.WarehouseReceiptOrder.CreateReceiptFromReceiptPlan)]
        public Task<Result<string>> CreateReceiptFromReceiptPlan([Path] int arrivalNo)
        {
            return _repository.SWarehouseReceiptOrders.CreateReceiptFromReceiptPlan(arrivalNo);
        }
        
        [HttpPost(ApiRoutes.WarehouseReceiptOrder.SearchReceiptOrderListAsync)]
        public async Task<Result<PageList<WarehouseReceiptOrderDto>>> SearchReceiptOrderListAsync([Body] QueryModel<WarehouseReceiptSearchModel> model) => await _repository.SWarehouseReceiptOrders.SearchReceiptOrderListAsync(model);

        [HttpPost(ApiRoutes.WarehouseReceiptOrder.GenerateReceiptFromReceiptPlan)]
        public async Task<Tuple<int, int>> GenerateReceiptFromReceiptPlan([Path] string tenantIds)
        {
            return await _repository.SWarehouseReceiptOrders.GenerateReceiptFromReceiptPlan(tenantIds);
        }

        [HttpGet(ApiRoutes.WarehouseReceiptOrder.GetReceiptReportAsync)]
        public async Task<Result<List<ReceiptReportDTO>>> GetReceiptReportAsync()
        {

            return await _repository.SWarehouseReceiptOrders.GetReceiptReportAsync();

        }

        [HttpPost(ApiRoutes.WarehouseReceiptOrder.CompleteAndPutAwayReceipts)]
        public async Task<Result<ResponseCompleteModel>> CompleteAndPutAwayReceipts() => await _repository.SWarehouseReceiptOrders.CompleteAndPutAwayReceipts();
    }
}
