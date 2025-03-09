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
    public class InventBundleController : BaseController<Guid, InventBundle>, IInventBundle
    {
        readonly Repository _repository;

        public InventBundleController(Repository repository = null) : base(repository.SInventBundles)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.InventBundle.GetAllDTO)]
        public async Task<Result<List<InventBundleDTO>>> GetAllDTOAsync()
        {
            return await _repository.SInventBundles.GetAllDTOAsync();
        }
        [HttpGet(ApiRoutes.InventBundle.GetByTransNoDTOAsync)]
        public async Task<Result<InventBundleDTO>> GetByTransNoDTOAsync([Path] string transNo)
        {
            return await _repository.SInventBundles.GetByTransNoDTOAsync(transNo);
        }
        [HttpGet(ApiRoutes.InventBundle.GetAllMasterByBundleCode)]
        public async Task<Result<List<InventBundlesLineDTO>>> GetAllMasterByBundleCode([Path] string ProductBundleCode)
        {
            return await _repository.SInventBundles.GetAllMasterByBundleCode(ProductBundleCode);
        }
        [HttpGet(ApiRoutes.InventBundle.GetByIdDTO)]
        public async Task<Result<InventBundleDTO>> GetByIdDTOAsync([Path] Guid id)
        {
            return await _repository.SInventBundles.GetByIdDTOAsync(id);
        }

        [HttpPost(ApiRoutes.InventBundle.CompletedBundle)]
        public async Task<Result<bool>> CompletedInventBundleAsync([Body] Guid id)
        {
            return await _repository.SInventBundles.CompletedInventBundleAsync(id);
        }
        [HttpPost(ApiRoutes.InventBundle.CreatePutawayAsync)]
        public async Task<Result<bool>> CreatePutawayAsync([Path] Guid id)
        {
            return await _repository.SInventBundles.CreatePutawayAsync(id);
        }
        [HttpPost(ApiRoutes.InventBundle.UploadFromHandheldAsync)]
        public async Task<Result<bool>> UploadFromHandheldAsync([Body] InventBundleDTO InventBundle)
        {
            return await _repository.SInventBundles.UploadFromHandheldAsync(InventBundle);
        }

        [HttpPost(ApiRoutes.InventBundle.DeleteAllBundleLineAsync)]
        public async Task<Result<bool>> DeleteAllBundleLineAsync([Path] Guid id)
        {
            return await _repository.SInventBundles.DeleteAllBundleLineAsync(id);
        }
    }
}
