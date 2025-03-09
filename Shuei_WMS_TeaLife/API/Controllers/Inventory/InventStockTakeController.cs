using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services.Inventory;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inventory
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventStockTakeController : BaseController<Guid, InventStockTake>, IInventStockTake
    {
        private readonly Repository _repository;

        public InventStockTakeController(Repository repository) : base(repository.SStockTake)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.InventStockTake.GetAll)]
        public async Task<Result<List<InventStockTakeDto>>> GetAll() => await _repository.SStockTake.GetAll();

        [HttpGet(ApiRoutes.InventStockTake.GetByStockTakeNo)]
        public async Task<Result<InventStockTakeDto>> GetByStockTakeNo(string StockTakeNo) => await _repository.SStockTake.GetByStockTakeNo(StockTakeNo);

        [HttpPost(ApiRoutes.InventStockTake.Delete)]
        public async Task<Result<bool>> Delete(Guid id) => await _repository.SStockTake.Delete(id);

        [HttpPost(ApiRoutes.InventStockTake.Insert)]
        public async Task<Result<InventStockTakeDto>> Insert([Body] InventStockTakeDto dto) => await _repository.SStockTake.Insert(dto);

        [HttpPost(ApiRoutes.InventStockTake.GetStockTakeAsync)]
        public async Task<Result<List<InventStockTakeDto>>> GetStockTakeAsync([Body] InventStockTakeSearchModel model) => await _repository.SStockTake.GetStockTakeAsync(model);

        [HttpPost(ApiRoutes.InventStockTake.Update)]
        public async Task<Result<InventStockTakeDto>> Update([Body] InventStockTakeDto dto) => await _repository.SStockTake.Update(dto);
        
        [HttpPost(ApiRoutes.InventStockTake.CompleteInventStockTake)]
        public async Task<Result<bool>> CompleteInventStockTake([Body] InventStockTakeDto dto) => await _repository.SStockTake.CompleteInventStockTake(dto);

        [HttpGet(ApiRoutes.InventStockTake.GetStockTakeLineByIdAsync)]
        public async Task<Result<List<InventStockTakeLineDto>>> GetStockTakeLineByIdAsync(string StockTakeNo) => await _repository.SStockTake.GetStockTakeLineByIdAsync(StockTakeNo);

        [HttpPost(ApiRoutes.InventStockTake.CheckProductExistenceinStocktake)]
        public async Task<Result<bool>> CheckProductExistenceinStocktake([Body] InventStockTakeLineDto InventStockTakeLineDtos)
        {
            return await _repository.SStockTake.CheckProductExistenceinStocktake(InventStockTakeLineDtos);
        }

        [HttpGet(ApiRoutes.InventStockTake.GetNewStocktakeNo)]
        public async Task<Result<string>> GetNewStocktakeNo() => await _repository.SStockTake.GetNewStocktakeNo();

        [HttpGet(ApiRoutes.InventStockTake.GetCurrentUserAsync)]
        public async Task<Result<string>> GetCurrentUserAsync() => await _repository.SStockTake.GetCurrentUserAsync();
    }
}
