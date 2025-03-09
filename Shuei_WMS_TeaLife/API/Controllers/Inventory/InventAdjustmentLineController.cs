using API.Controllers.Base;
using Application.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Inventory
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventAdjustmentLineController : BaseController<Guid, InventAdjustmentLine>, IInventAdjustmentLines
    {
        readonly Repository _repository;

        public InventAdjustmentLineController(Repository repository = null) : base(repository.SInventAdjustmentLines)
        {
            _repository = repository;
        }
    }
}
