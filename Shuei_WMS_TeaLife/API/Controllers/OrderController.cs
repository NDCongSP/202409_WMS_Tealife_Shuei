using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase, IOrder
    {
        readonly Repository _repository;

        public OrderController(Repository repository = null)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.Order.GetOrderReport)]
        public async Task<Result<List<OrderReportDto>>> GetOrderReport() => await _repository.SOrder.GetOrderReport();
    }
}
