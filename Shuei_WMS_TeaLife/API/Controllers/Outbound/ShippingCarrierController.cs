using API.Controllers.Base;
using Application.Extentions;
using Application.Services.Outbound;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers.Outbound
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingCarrierController : BaseController<Guid, ShippingCarrier>, IShippingCarrier
    {
        readonly Repository _repository;

        public ShippingCarrierController(Repository repository = null!):base(repository.SShippingCarrier) 
        {
            _repository = repository;
        }
    }
}
