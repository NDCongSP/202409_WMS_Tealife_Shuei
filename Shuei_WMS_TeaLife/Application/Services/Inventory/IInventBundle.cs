using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.InventBundle.BasePath)]
    public interface IInventBundle : IRepository<Guid, InventBundle>
    {
        [Get(ApiRoutes.InventBundle.GetAllDTO)]
        Task<Result<List<InventBundleDTO>>> GetAllDTOAsync();

        [Get(ApiRoutes.InventBundle.GetByIdDTO)]
        Task<Result<InventBundleDTO>> GetByIdDTOAsync([Path] Guid id);

        [Get(ApiRoutes.InventBundle.GetByTransNoDTOAsync)]
        Task<Result<InventBundleDTO>> GetByTransNoDTOAsync([Path] string transNo);

        [Get(ApiRoutes.InventBundle.GetAllMasterByBundleCode)]
        Task<Result<List<InventBundlesLineDTO>>> GetAllMasterByBundleCode([Path] string ProductBundleCode);
        [Post(ApiRoutes.InventBundle.CompletedBundle)]
        Task<Result<bool>> CompletedInventBundleAsync([Body] Guid id);
        [Post(ApiRoutes.InventBundle.CreatePutawayAsync)]
        Task<Result<bool>> CreatePutawayAsync([Path] Guid id);

        [Post(ApiRoutes.InventBundle.UploadFromHandheldAsync)]
        Task<Result<bool>> UploadFromHandheldAsync([Body] InventBundleDTO InventBundle);

        [Post(ApiRoutes.InventBundle.DeleteAllBundleLineAsync)]
        Task<Result<bool>> DeleteAllBundleLineAsync([Path] Guid id);
    }
}
