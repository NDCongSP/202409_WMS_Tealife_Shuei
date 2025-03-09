using Application.DTOs;
using Application.DTOs.Request.shipment;
using Application.Extentions;
using Application.Services.Outbound;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Outbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PackingListController : ControllerBase, IPackingList
    {
        readonly Repository _repository;

        public PackingListController(Repository repository)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.PackingList.CompletePackingAsync)]
        public async Task<Result<string>> CompletePackingAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model)
        {
            return await _repository.SPackingList.CompletePackingAsync(model);
        }

        [HttpPost(ApiRoutes.PackingList.GetDataMasterAsync)]
        public async Task<Result<List<WarehousePackingListDto>>> GetDataMasterAsync([Body] PackingListSearchRequestDto model)
        {
            return await _repository.SPackingList.GetDataMasterAsync(model);
        }

        [HttpGet(ApiRoutes.PackingList.GetDataMasterByShipmentNoAsync)]
        public async Task<Result<WarehousePackingListDto>> GetDataMasterByShipmentNoAsync([Path] string shipmentNo)
        {
            return await _repository.SPackingList.GetDataMasterByShipmentNoAsync(shipmentNo);
        }

        [HttpGet(ApiRoutes.PackingList.GetDataMasterByTrackingNoAsync)]
        public async Task<Result<WarehousePackingListDto>> GetDataMasterByTrackingNoAsync([Path] string trackingNo)
        {
            return await _repository.SPackingList.GetDataMasterByTrackingNoAsync(trackingNo);
        }

        [HttpGet(ApiRoutes.PackingList.GetPackingLabelFilePathAsync)]
        public async Task<Result<OrderDispatch>> GetPackingLabelFilePathAsync([Path] string trackingNo)
        {
            return await _repository.SPackingList.GetPackingLabelFilePathAsync(trackingNo);
        }

        [HttpGet(ApiRoutes.PackingList.GetPdfAsBase64Async)]
        public async Task<Result<string>> GetPdfAsBase64Async([Path] string filePath)
        {
            return await _repository.SPackingList.GetPdfAsBase64Async(filePath);
        }

        [HttpPost(ApiRoutes.PackingList.UpdateOrderDispatchesFilePathAsync)]
        public async Task<Result<string>> UpdateOrderDispatchesFilePathAsync([Body] OrderDispatch model)
        {
            return await _repository.SPackingList.UpdateOrderDispatchesFilePathAsync(model);
        }

        [HttpPost(ApiRoutes.PackingList.UpdatePackedQtyAsync)]
        public async Task<Result<string>> UpdatePackedQtyAsync([Body] List<PackingListUpdatePackedQtyRequestDto> model)
        {
            return await _repository.SPackingList.UpdatePackedQtyAsync(model);
        }
    }
}
