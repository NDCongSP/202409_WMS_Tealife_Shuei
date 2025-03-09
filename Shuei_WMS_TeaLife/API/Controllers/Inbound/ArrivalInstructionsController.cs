using API.Controllers.Base;
using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services.Inbound;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Inbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArrivalInstructionsController : BaseController<int, ArrivalInstruction>,IArrivalInstructions
    {
        readonly Repository _repository;

        public ArrivalInstructionsController(Repository repository = null!):base(repository.SArrivalInstructions)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.ArrivalInstructions.SearchAsync)]
        public async Task<Result<PageList<ArrivalInstructionDto>>> SearchArrivalInstructions([Body] QueryModel<ReceivePlanSearchModel> model)
        {
            return await _repository.SArrivalInstructions.SearchArrivalInstructions(model);
        }
    }
}
