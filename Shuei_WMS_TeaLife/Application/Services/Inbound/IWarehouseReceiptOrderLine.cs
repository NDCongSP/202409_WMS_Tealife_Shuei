using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Services.Base;
using RestEase;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.WarehouseReceiptOrderLine.BasePath)]
    public interface IWarehouseReceiptOrderLine:IRepository<Guid, WarehouseReceiptOrderLine>
    {
        [Get(ApiRoutes.WarehouseReceiptOrderLine.GetByMasterCodeAsync)]
        Task<Result<List<WarehouseReceiptOrderLine>>> GetByMasterCodeAsync([Path] string receiptNo);

        [Get(ApiRoutes.WarehouseReceiptOrderLine.GetAllDTOAsync)]
        Task<Result<List<WarehouseReceiptOrderLineResponseDTO>>> GetAllDTOAsync();

        [Get(ApiRoutes.WarehouseReceiptOrderLine.GetByIdDTOAsync)]
        Task<Result<WarehouseReceiptOrderLineResponseDTO>> GetByIdDTOAsync([Path] Guid id);

        [Get(ApiRoutes.WarehouseReceiptOrderLine.GetByProductCodeAsync)]
        Task<Result<WarehouseReceiptOrderLine>> GetByProducCodetAsync([Path] string productCode);
    }
}
