using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request.Products;
using Application.Extentions;
using Application.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseTranController : BaseController<Guid, WarehouseTran>, IWarehouseTran
    {
        readonly Repository _repository;

        public WarehouseTranController(Repository repository = null!):base(repository.SWarehouseTrans)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.WarehouseTran.GetAllProductInventoryInfo)]
        public async Task<Result<List<InventoryInfoDTO>>> GetAllProductInventoryInfoAsync()
        {
            return await _repository.SWarehouseTrans.GetAllProductInventoryInfoAsync();
        }

        [HttpGet(ApiRoutes.WarehouseTran.GetByProductCodeInventoryInfo)]
        public async Task<Result<InventoryInfoDTO>> GetByProductCodeInventoryInfoAsync([Path] string productCode)
        {
            return await _repository.SWarehouseTrans.GetByProductCodeInventoryInfoAsync(productCode);
        }

        [HttpGet(ApiRoutes.WarehouseTran.GetAllProductInventoryInfoFlowBinLotAsync)]
        public async Task<Result<List<InventoryInfoBinLotResponseDTO>>> GetAllProductInventoryInfoFlowBinLotAsync()
        {
            return await _repository.SWarehouseTrans.GetAllProductInventoryInfoFlowBinLotAsync();
        }

        [HttpGet(ApiRoutes.WarehouseTran.GetByProductCodeInventoryInfoFlowBinLotAsync)]
        public async Task<Result<InventoryInfoBinLotResponseDTO>> GetByProductCodeInventoryInfoFlowBinLotAsync([Path] string productCode)
        {
            return await _repository.SWarehouseTrans.GetByProductCodeInventoryInfoFlowBinLotAsync(productCode);
        }

        [HttpPost(ApiRoutes.WarehouseTran.GetInventoryInfoFlowBinLotFlowModelSearchAsync)]
        public async Task<Result<List<InventoryInfoBinLotResponseDTO>>> GetInventoryInfoFlowBinLotFlowModelSearchAsync([Body] InventoryInfoBinLotSearchRequestDTO model)
        {
            return await _repository.SWarehouseTrans.GetInventoryInfoFlowBinLotFlowModelSearchAsync(model);
        }


        [HttpPost(ApiRoutes.WarehouseTran.GetProductInventoryInformationAsync)]
        public async Task<Result<List<InventoryHistoryDto>>> GetProductInventoryInformationAsync([Body] InventoryInformationSearchModel model) => await _repository.SWarehouseTrans.GetProductInventoryInformationAsync(model);


        [HttpPost(ApiRoutes.WarehouseTran.GetProductInventoryInformationDetailsAsync)]
        public async Task<Result<List<InventoryHistoryDetailsDto>>> GetProductInventoryInformationDetailsAsync([Body] InventoryInformationDetailsSearchModel model) => await _repository.SWarehouseTrans.GetProductInventoryInformationDetailsAsync(model);


        [HttpPost(ApiRoutes.WarehouseTran.GetInventoryAdjustmentDetailsAsync)]
        public async Task<Result<List<InventoryAdjustmentDetailsDto>>> GetInventoryAdjustmentDetailsAsync([Body] InventoryAdjustmentDetailsModel model) => await _repository.SWarehouseTrans.GetInventoryAdjustmentDetailsAsync(model);

        [HttpPost(ApiRoutes.WarehouseTran.GetInventoryBundleAdjustmentDetailsAsync)]
        public async Task<Result<List<InventoryAdjustmentDetailsDto>>> GetInventoryBundleAdjustmentDetailsAsync([Body] InventoryAdjustmentDetailsModel model) => await _repository.SWarehouseTrans.GetInventoryBundleAdjustmentDetailsAsync(model);

        [HttpPost(ApiRoutes.WarehouseTran.UpdateInventoryAdjustmentAsync)]
        public async Task<Result<List<InventoryAdjustmentDetailsDto>>> UpdateInventoryAdjustmentAsync([Body] List<InventoryAdjustmentDetailsDto> list) => await _repository.SWarehouseTrans.UpdateInventoryAdjustmentAsync(list);

        [HttpPost(ApiRoutes.WarehouseTran.GetInventoryAdjustmentAsync)]
        public async Task<Result<List<ProductDto>>> GetProductAdjustmentAsync([Body] InventoryAdjustmentDetailsModel model) => await _repository.SWarehouseTrans.GetProductAdjustmentAsync(model);

        [HttpPost(ApiRoutes.WarehouseTran.GetQtyFromWarehouseTrans)]
        public async Task<Result<int>> GetQtyFromWarehouseTrans([Body] InventoryAdjustmentDetailsModel model) => await _repository.SWarehouseTrans.GetQtyFromWarehouseTrans(model);
        [HttpPost(ApiRoutes.WarehouseTran.GetInventoryAdjustmentLinesDefaultProduct)]
        public async Task<Result<List<InventAdjustmentsLineDTO>>> GetInventoryAdjustmentLinesDefaultProduct([Body] InventoryAdjustmentDetailsModel model) => await _repository.SWarehouseTrans.GetInventoryAdjustmentLinesDefaultProduct(model);

    }
}
