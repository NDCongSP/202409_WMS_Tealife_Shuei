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
    public class InventTransferLineController : BaseController<Guid, InventTransferLine>, IInventTransferLines
    {
        readonly Repository _repository;

        public InventTransferLineController(Repository repository = null) : base(repository.SInventTransferLine)
        {
            _repository = repository;
        }
    }
}
