using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.InventBundleLines.BasePath)]
    public interface IInventBundleLines : IRepository<Guid, InventBundleLine>
    {
        [Get(ApiRoutes.InventBundleLines.GetProductsByTransNo)]
        Task<Result<List<InventBundlesLineDTO>>> GetProductsByTransNo([Path] string TransNo);
        [Get(ApiRoutes.InventBundleLines.GetProductsByBundleCode)]
        Task<Result<List<InventBundlesLineDTO>>> GetProductsByBundleCode([Path] string BundleCode);
    }
}
