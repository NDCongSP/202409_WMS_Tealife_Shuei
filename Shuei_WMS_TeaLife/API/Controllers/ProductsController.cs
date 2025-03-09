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
    public class ProductsController : BaseController<int, Product>, IProducts
    {
        readonly Repository _repository;

        public ProductsController(Repository repository = null!) : base(repository.SProducts)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.Product.UploadProductImage)]
        public async Task<Result<string>> UploadProductImage([Body] ImageInfoDTO model)
        {
            return await _repository.SProducts.UploadProductImage(model);
        }

        [HttpGet(ApiRoutes.Product.GetProductListAsync)]
        public async Task<Result<IEnumerable<ProductDto>>> GetProductListAsync() => await _repository.SProducts.GetProductListAsync();

        [HttpGet(ApiRoutes.Product.GetByProductCodeAsync)]
        public async Task<Result<ProductDto>> GetByProductCodeAsync(string code) => await _repository.SProducts.GetByProductCodeAsync(code);

        [HttpGet(ApiRoutes.Product.SearchByProductCodeAsync)]
        public async Task<Result<IEnumerable<ProductDto>>> SearchByProductCodeAsync(string code) => await _repository.SProducts.SearchByProductCodeAsync(code);
        

        [HttpGet(ApiRoutes.Product.GetProductCategoriesAsync)]
        public async Task<Result<List<ProductCategory>>> GetProductCategoriesAsync() => await _repository.SProducts.GetProductCategoriesAsync();


        [HttpGet(ApiRoutes.Product.AutocompleteProductAsync)]
        public async Task<Result<IEnumerable<ProductDto>>> AutocompleteProductAsync(string? keyword, int tenantId)
        {
            return await _repository.SProducts.AutocompleteProductAsync(keyword, tenantId);
        }

        [HttpPost(ApiRoutes.Product.GetAllDtoAsync)]
        public async Task<Result<List<ProductsDTO>>> GetAllDtoAsync([Body] ProductSearchRequestDTO model)
        {
            return await _repository.SProducts.GetAllDtoAsync(model);
        }
        [HttpGet(ApiRoutes.Product.GetByIdDtoAsync)]
        public async Task<Result<ProductsDTO>> GetByIdDtoAsync([Path] int id)
        {
            return await _repository.SProducts.GetByIdDtoAsync(id);
        }

        [HttpPost(ApiRoutes.Product.InsertDtoAsync)]
        public async Task<Result<ProductAddUpdateRequestDTO>> InsertDtoAsync([Body] ProductAddUpdateRequestDTO model)
        {
            return await _repository.SProducts.InsertDtoAsync(model);
        }
        [HttpPost(ApiRoutes.Product.UpdateDtoAsync)]
        public async Task<Result<ProductAddUpdateRequestDTO>> UpdateDtoAsync([Body] ProductAddUpdateRequestDTO model)
        {
            return await _repository.SProducts.UpdateDtoAsync(model);
        }
        [HttpDelete(ApiRoutes.Product.DeleteDtoAsync)]
        public async Task<Result<string>> DeleteDtoAsync([Path] int id)
        {
            return await _repository.SProducts.DeleteDtoAsync(id);
        }

        [HttpGet(ApiRoutes.Product.GetByUnitAsync)]
        public async Task<Result<Product>> GetByUnitAsync([Path] int unitId)
        {
            return await _repository.SProducts.GetByUnitAsync(unitId);
        }
        [HttpGet(ApiRoutes.Product.GetByCatetgoryAsync)]
        public async Task<Result<Product>> GetByCatetgoryAsync([Path] int categoryId)
        {
            return await _repository.SProducts.GetByCatetgoryAsync(categoryId);
        }
        [HttpGet(ApiRoutes.Product.GetBySupplierAsync)]
        public async Task<Result<Product>> GetBySupplierAsync([Path] int supplierId)
        {
            return await _repository.SProducts.GetBySupplierAsync(supplierId);
        }
    }
}