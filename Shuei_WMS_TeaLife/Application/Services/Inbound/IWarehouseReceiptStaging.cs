using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Services.Base;
using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehouseReceiptStaging.BasePath)]
    public interface IWarehouseReceiptStaging:IRepository<Guid, WarehouseReceiptStaging>
    {
        [Get(ApiRoutes.WarehouseReceiptStaging.GetByMasterCodeAsync)]
        Task<Result<List<WarehouseReceiptStaging>>> GetByMasterCodeAsync([Path] string receiptNo);

        [Get(ApiRoutes.WarehouseReceiptStaging.GetByReceiptLineIdAsync)]
        Task<Result<WarehouseReceiptStaging>> GetByReceiptLineIdAsync([Path] Guid receiptLineId);

        [Post(ApiRoutes.WarehouseReceiptStaging.UploadProductErrorImageAsync)]
        Task<Result<string>> UploadProductErrorImageAsync([Body] List<UpdateReceiptImageRequestDTO> model);

        [Get(ApiRoutes.WarehouseReceiptStaging.GetAllDTOAsync)]
        Task<Result<List<WarehouseReceiptStagingResponseDTO>>> GetAllDTOAsync();

        [Get(ApiRoutes.WarehouseReceiptStaging.GetByReceiptLineIdDTOAsync)]
        Task<Result<WarehouseReceiptStagingResponseDTO>> GetByReceiptLineIdDTOAsync([Path] Guid receiptLineId);
    }
}
