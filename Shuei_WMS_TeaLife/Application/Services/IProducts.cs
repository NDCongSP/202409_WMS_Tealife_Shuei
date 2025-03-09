using Application.DTOs;
using Application.DTOs.Request.Products;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.Product.BasePath)]
    public interface IProducts : IRepository<int, Product>
    {
        [Post(ApiRoutes.Product.UploadProductImage)]
        Task<Result<string>> UploadProductImage([Body] ImageInfoDTO model);


        [Get(ApiRoutes.Product.GetProductListAsync)]
        Task<Result<IEnumerable<ProductDto>>> GetProductListAsync();

        [Get(ApiRoutes.Product.GetByProductCodeAsync)]
        Task<Result<ProductDto>> GetByProductCodeAsync(string code);

        [Get(ApiRoutes.Product.SearchByProductCodeAsync)]

        Task<Result<IEnumerable<ProductDto>>> SearchByProductCodeAsync(string? code);

        [Get(ApiRoutes.Product.AutocompleteProductAsync)]
        Task<Result<IEnumerable<ProductDto>>> AutocompleteProductAsync(string? keyword, int tenantId);

        [Post(ApiRoutes.Product.GetAllDtoAsync)]
        Task<Result<List<ProductsDTO>>> GetAllDtoAsync([Body] ProductSearchRequestDTO model);

        [Get(ApiRoutes.Product.GetByIdDtoAsync)]
        Task<Result<ProductsDTO>> GetByIdDtoAsync([Path] int id);

        [Post(ApiRoutes.Product.InsertDtoAsync)]
        Task<Result<ProductAddUpdateRequestDTO>> InsertDtoAsync([Body] ProductAddUpdateRequestDTO model);
        [Post(ApiRoutes.Product.UpdateDtoAsync)]
        Task<Result<ProductAddUpdateRequestDTO>> UpdateDtoAsync([Body] ProductAddUpdateRequestDTO model);

        [Delete(ApiRoutes.Product.DeleteDtoAsync)]
        Task<Result<string>> DeleteDtoAsync([Path] int id);


        [Get(ApiRoutes.Product.GetProductCategoriesAsync)]
        Task<Result<List<ProductCategory>>> GetProductCategoriesAsync();

        [Get(ApiRoutes.Product.GetByUnitAsync)]
        Task<Result<Product>> GetByUnitAsync([Path] int unitId);
        [Get(ApiRoutes.Product.GetByCatetgoryAsync)]
        Task<Result<Product>> GetByCatetgoryAsync([Path] int categoryId);
        [Get(ApiRoutes.Product.GetBySupplierAsync)]
        Task<Result<Product>> GetBySupplierAsync([Path] int supplierId);
    }
}
