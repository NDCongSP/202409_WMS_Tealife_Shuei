using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services.Outbound;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Outbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnOrderController : BaseController<Guid, ReturnOrder>, IReturnOrder
    {
        private readonly Repository _repository;

        public ReturnOrderController(Repository repository) : base(repository.SReturnOrder)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.ReturnOrder.GetAllReturnOrdersAsync)]
        public async Task<Result<List<ReturnOrderDto>>> GetAllReturnOrdersAsync() => await _repository.SReturnOrder.GetAllReturnOrdersAsync();

        [HttpGet(ApiRoutes.ReturnOrder.GetReturnOrderByReturnNoAsync)]
        public async Task<Result<ReturnOrderDto>> GetReturnOrderByReturnNoAsync(string returnOrderNo) => await _repository.SReturnOrder.GetReturnOrderByReturnNoAsync(returnOrderNo);

        [HttpPost(ApiRoutes.ReturnOrder.InsertReturnOrderAsync)]
        public async Task<Result<ReturnOrderDto>> InsertReturnOrderAsync([Body] ReturnOrderDto dto) => await _repository.SReturnOrder.InsertReturnOrderAsync(dto);

        [HttpPost(ApiRoutes.ReturnOrder.UpdateReturnOrderAsync)]
        public async Task<Result<ReturnOrderDto>> UpdateReturnOrderAsync([Body] ReturnOrderDto dto) => await _repository.SReturnOrder.UpdateReturnOrderAsync(dto);

        [HttpPost(ApiRoutes.ReturnOrder.DeleteReturnOrderAsync)]
        public async Task<Result<bool>> DeleteReturnOrderAsync([Path] Guid id) => await _repository.SReturnOrder.DeleteReturnOrderAsync(id);

        [HttpPost(ApiRoutes.ReturnOrder.SearchReturnOrder)]
        public async Task<Result<PageList<ReturnOrderDto>>> SearchReturnOrder([Body] QueryModel<ReturnOrderSearchModel> model) => await _repository.SReturnOrder.SearchReturnOrder(model);
        [HttpGet(ApiRoutes.ReturnOrder.GetReturnByShipmentNo)]
        public async Task<Result<List<ReturnOrderDto>>> GetReturnByShipmentNo(string shipmentNo) => await _repository.SReturnOrder.GetReturnByShipmentNo(shipmentNo);

    }
}
