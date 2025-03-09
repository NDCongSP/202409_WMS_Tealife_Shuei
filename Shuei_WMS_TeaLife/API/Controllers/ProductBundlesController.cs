using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request.Products;
using Application.Extentions;
using Application.Services;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductBundlesController : BaseController<int, ProductBundle>, IProductBundles
    {
        readonly Repository _repository;

        public ProductBundlesController(Repository repository = null!) : base(repository.SProductBundles)
        {
            _repository = repository;
        }
        [HttpGet(ApiRoutes.ProductBundle.GetAllDistinctAsync)]
        public Task<Result<List<ProductBundle>>> GetAllDistinctAsync()
        {
            return _repository.SProductBundles.GetAllDistinctAsync();
        }

        [HttpGet(ApiRoutes.ProductBundle.GetPlannedShipmentBundlesAsync)]
        public Task<Result<IEnumerable<ProductBundleDto>>> GetPlannedShipmentBundlesAsync()
        {
            return _repository.SProductBundles.GetPlannedShipmentBundlesAsync();
        }
        [HttpGet(ApiRoutes.ProductBundle.GetProductCodesByBundleCodeAsync)]
        public Task<Result<List<string>>> GetProductCodesByBundleCodeAsync([Path]string BundleCode)
        {
            return _repository.SProductBundles.GetProductCodesByBundleCodeAsync(BundleCode);
        }
    }
}
