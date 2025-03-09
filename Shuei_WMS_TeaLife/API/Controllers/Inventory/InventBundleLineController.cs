using API.Controllers.Base;
using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inventory
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventBundleLineController : BaseController<Guid, InventBundleLine>, IInventBundleLines
    {
        readonly Repository _repository;

        public InventBundleLineController(Repository repository = null) : base(repository.SInventBundleLines)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.InventBundleLines.GetProductsByTransNo)]
        public async Task<Result<List<InventBundlesLineDTO>>> GetProductsByTransNo([Path] string TransNo)
        {
            return await _repository.SInventBundleLines.GetProductsByTransNo(TransNo);
        }

        [HttpGet(ApiRoutes.InventBundleLines.GetProductsByBundleCode)]
        public async Task<Result<List<InventBundlesLineDTO>>> GetProductsByBundleCode([Path] string BundleCode)
        {
            return await _repository.SInventBundleLines.GetProductsByBundleCode(BundleCode);
        }
    }
}
