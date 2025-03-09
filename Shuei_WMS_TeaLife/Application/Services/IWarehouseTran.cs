using Application.DTOs;
using Application.DTOs.Request.Products;
using Application.Extentions;
using Application.Services.Base;
using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.WarehouseTran.BasePath)]
    public interface IWarehouseTran : IRepository<Guid, WarehouseTran>
    {
        [Get(ApiRoutes.WarehouseTran.GetAllProductInventoryInfo)]
        Task<Result<List<InventoryInfoDTO>>> GetAllProductInventoryInfoAsync();

        [Get(ApiRoutes.WarehouseTran.GetByProductCodeInventoryInfo)]
        Task<Result<InventoryInfoDTO>> GetByProductCodeInventoryInfoAsync([Path] string productCode);

        [Get(ApiRoutes.WarehouseTran.GetAllProductInventoryInfo)]
        Task<Result<List<InventoryInfoBinLotResponseDTO>>> GetAllProductInventoryInfoFlowBinLotAsync();

        [Get(ApiRoutes.WarehouseTran.GetByProductCodeInventoryInfoFlowBinLotAsync)]
        Task<Result<InventoryInfoBinLotResponseDTO>> GetByProductCodeInventoryInfoFlowBinLotAsync([Path] string productCode);

        [Post(ApiRoutes.WarehouseTran.GetInventoryInfoFlowBinLotFlowModelSearchAsync)]
        Task<Result<List<InventoryInfoBinLotResponseDTO>>> GetInventoryInfoFlowBinLotFlowModelSearchAsync([Body] InventoryInfoBinLotSearchRequestDTO model);

        [Post(ApiRoutes.WarehouseTran.GetProductInventoryInformationAsync)]
        Task<Result<List<InventoryHistoryDto>>> GetProductInventoryInformationAsync([Body] InventoryInformationSearchModel model);

        [Post(ApiRoutes.WarehouseTran.GetProductInventoryInformationDetailsAsync)]
        Task<Result<List<InventoryHistoryDetailsDto>>> GetProductInventoryInformationDetailsAsync([Body] InventoryInformationDetailsSearchModel model);

        [Post(ApiRoutes.WarehouseTran.GetInventoryAdjustmentDetailsAsync)]
        Task<Result<List<InventoryAdjustmentDetailsDto>>> GetInventoryAdjustmentDetailsAsync([Body] InventoryAdjustmentDetailsModel model);
        [Post(ApiRoutes.WarehouseTran.GetInventoryBundleAdjustmentDetailsAsync)]
        Task<Result<List<InventoryAdjustmentDetailsDto>>> GetInventoryBundleAdjustmentDetailsAsync([Body] InventoryAdjustmentDetailsModel model);
        [Post(ApiRoutes.WarehouseTran.UpdateInventoryAdjustmentAsync)]
        Task<Result<List<InventoryAdjustmentDetailsDto>>> UpdateInventoryAdjustmentAsync([Body] List<InventoryAdjustmentDetailsDto> model);
        [Post(ApiRoutes.WarehouseTran.GetInventoryAdjustmentAsync)]
        Task<Result<List<ProductDto>>> GetProductAdjustmentAsync([Body] InventoryAdjustmentDetailsModel model);

        [Post(ApiRoutes.WarehouseTran.GetQtyFromWarehouseTrans)]
        Task<Result<int>> GetQtyFromWarehouseTrans([Body] InventoryAdjustmentDetailsModel model);

        [Post(ApiRoutes.WarehouseTran.GetInventoryAdjustmentLinesDefaultProduct)]
        Task<Result<List<InventAdjustmentsLineDTO>>> GetInventoryAdjustmentLinesDefaultProduct([Body] InventoryAdjustmentDetailsModel model);

    }
}
