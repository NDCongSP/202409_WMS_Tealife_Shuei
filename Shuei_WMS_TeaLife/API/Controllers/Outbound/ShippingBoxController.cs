using API.Controllers.Base;
using Application.DTOs;
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
    public class ShippingBoxController : BaseController<Guid, ShippingBox>, IShippingBox
    {
        readonly Repository _repository;

        public ShippingBoxController(Repository repository = null!) : base(repository.SShippingBox)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.ShippingBox.GetByShippingCarrierCodeAsync)]
        public async Task<Result<List<ShippingBox>>> GetByShippingCarrierCodeAsync([Path] string shippingCarrierCode)
        {
            return await _repository.SShippingBox.GetByShippingCarrierCodeAsync(shippingCarrierCode);
        }

        [HttpGet(ApiRoutes.ShippingBox.GetAllShippingCarrierAsync)]
        public async Task<Result<List<ShippingCarrierDTO>>> GetAllShippingCarrierAsync()
        {
            return await _repository.SShippingBox.GetAllShippingCarrierAsync();
        }

        [HttpGet(ApiRoutes.ShippingBox.GetAllWithCarrierAsync)]
        public async Task<Result<List<ShippingBoxDTO>>> GetAllWithCarrierAsync()
        {
            return await _repository.SShippingBox.GetAllWithCarrierAsync();
        }
    }
}
