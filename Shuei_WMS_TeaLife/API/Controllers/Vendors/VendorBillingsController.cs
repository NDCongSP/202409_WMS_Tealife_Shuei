using API.Controllers.Base;
using Application.Services.Base;
using Application.Services.Vendors;

using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Vendors
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VendorBillingsController : BaseController<int, VendorBilling>, IVendorBillings
    {
        readonly Repository _repository;
        public VendorBillingsController(Repository repository = null) : base(repository.SVendorBillings)
        {
            _repository = repository;
        }
    }
}
