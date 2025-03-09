using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.ProductJanCodes.BasePath)]
    public interface IProductJanCodes : IRepository<int, ProductJanCode>
    {
        [Post(ApiRoutes.ProductJanCodes.GetByProductId)]
        Task<Result<List<ProductJanCode>>> GetByProductId([Path] int productId);

        [Post(ApiRoutes.ProductJanCodes.AddOrUpdateAsync)]
        Task<Result<List<ProductJanCode>>> AddOrUpdateAsync([Body] List<ProductJanCode> model);
    }
}
