using Application.Extentions;
using Application.Services.Base;

using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Vendors
{
    [BasePath(ApiRoutes.VendorBillings.BasePath)]
    public interface IVendorBillingDetail : IRepository<int, VendorBillingDetail>
    {
    }
}
