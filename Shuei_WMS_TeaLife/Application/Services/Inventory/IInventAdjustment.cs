using Application.DTOs;
using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.InventAdjustment.BasePath)]
    public interface IInventAdjustment : IRepository<Guid, InventAdjustment>
    {
        [Get(ApiRoutes.InventAdjustment.GetAllDTO)]
        Task<Result<List<InventAdjustmentDTO>>> GetAllDTOAsync();

        [Get(ApiRoutes.InventAdjustment.GetByIdDTO)]
        Task<Result<InventAdjustmentDTO>> GetByIdDTOAsync([Path] Guid id);

        [Get(ApiRoutes.InventAdjustment.GetByAdjustmentNoDTO)]
        Task<Result<InventAdjustmentDTO>> GetByAdjustmentNoDTOAsync([Path] string adjustmentNo);
        [Post(ApiRoutes.InventAdjustment.CompletedAdjustment)]
        Task<Result<bool>> CompletedInventAdjustmentAsync([Path] Guid id);

        [Post(ApiRoutes.InventAdjustment.DeleteAllAdjustmentLineAsync)]
        Task<Result<bool>> DeleteAllAdjustmentLineAsync([Path] Guid id);
    }
}
