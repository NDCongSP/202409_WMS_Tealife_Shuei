using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    [BasePath(ApiRoutes.ProductBundle.BasePath)]
    public interface IProductBundles : IRepository<int, ProductBundle>
    {
        [Get(ApiRoutes.ProductBundle.GetPlannedShipmentBundlesAsync)]
        Task<Result<IEnumerable<ProductBundleDto>>> GetPlannedShipmentBundlesAsync();
        [Get(ApiRoutes.ProductBundle.GetAllDistinctAsync)]
        Task<Result<List<ProductBundle>>> GetAllDistinctAsync();
        [Get(ApiRoutes.ProductBundle.GetProductCodesByBundleCodeAsync)]
        Task<Result<List<String>>> GetProductCodesByBundleCodeAsync([Path] string BundleCode);


    }
}
