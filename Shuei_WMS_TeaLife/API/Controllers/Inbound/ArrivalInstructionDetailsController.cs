using API.Controllers.Base;
using Application.Services.Inbound;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Inbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ArrivalInstructionDetailsController : BaseController<int,ArrivalInstructionDetail>,IArrivalInstructionDetails
    {
        readonly Repository _repository;

        public ArrivalInstructionDetailsController(Repository repository = null!) : base(repository.SArrivalInstructionDetails)
        {
            _repository = repository;
        }
    }
}
