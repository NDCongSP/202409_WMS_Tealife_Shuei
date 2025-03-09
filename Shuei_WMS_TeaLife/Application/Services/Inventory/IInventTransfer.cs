using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.InventTransfer.BasePath)]
    public interface IInventTransfer : IRepository<Guid, InventTransfer>
    {
        [Get(ApiRoutes.InventTransfer.GetAllDTO)]
        Task<Result<List<InventTransfersDTO>>> GetAllDTO();

        [Get(ApiRoutes.InventTransfer.GetByIdDTO)]
        Task<Result<InventTransfersDTO>> GetByIdDTO([Path] Guid id);

        [Get(ApiRoutes.InventTransfer.GetByTransferNoDTO)]
        Task<Result<InventTransfersDTO>> GetByTransferNoDTO([Path] string transferNo);

        [Post(ApiRoutes.InventTransfer.CompletedTransfer)]
        Task<Result<bool>> CompletedInventTransfer([Path] Guid id);
    }
}
