using API.Controllers.Base;
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
    public class ProductJanCodesController : BaseController<int, ProductJanCode>, IProductJanCodes
    {
        readonly Repository _repository;

        public ProductJanCodesController(Repository repository = null) : base(repository.SProductJanCodes)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.ProductJanCodes.GetByProductId)]
        public async Task<Result<List<ProductJanCode>>> GetByProductId([Path] int productId)
        {
            return await _repository.SProductJanCodes.GetByProductId(productId);
        }

        [HttpPost(ApiRoutes.ProductJanCodes.AddOrUpdateAsync)]
        public async Task<Result<List<ProductJanCode>>> AddOrUpdateAsync([Body] List<ProductJanCode> model)
        {
            return await _repository.SProductJanCodes.AddOrUpdateAsync(model);
        }
    }
}
