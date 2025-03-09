using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategories category) : ControllerBase
    {
        [HttpGet(ApiRoutes.Categories.GetUsers)]
        public async Task<ActionResult<Result<List<SelectListItem>>>> GetUserDropdown()
        {
            var result = await category.GetUserDropdown();
            return Ok(result);
        }

        [HttpGet(ApiRoutes.Categories.GetBinByLocationId)]
        //System.Web.Mvc.SelectListItem
        public async Task<ActionResult<Result<List<SelectListItem>>>> GetBinByLocation(string locationId)
        {
            var result = await category.GetBinByLocation(locationId);
            return Ok(result);
        }

        [HttpGet(ApiRoutes.Categories.GetOrders)]
        public async Task<ActionResult<List<OrderSelectList>>> GetOrders(string? orderId = default)
        {
            var result = await category.GetOrders(orderId);
            return Ok(result);
        }
    }
}
