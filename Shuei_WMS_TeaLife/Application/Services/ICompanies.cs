using Application.Extentions;
using Application.Services.Base;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    [BasePath(ApiRoutes.CompanyTenant.BasePath)]
    public interface ICompanies : IRepository<Guid, CompanyTenant>
    {
    }
}
