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
    public class VendorBillingDetailController : BaseController<int, VendorBillingDetail>,IVendorBillingDetail
    {
        readonly Repository _repository;
        public VendorBillingDetailController(Repository repository = null) : base(repository.SVendorBillingDetail)
        {
            _repository = repository;
        }
    }
}
