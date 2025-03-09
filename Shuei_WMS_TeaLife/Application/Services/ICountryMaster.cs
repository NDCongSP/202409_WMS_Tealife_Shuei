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
    [BasePath(ApiRoutes.CountryMaster.BasePath)]
    public interface ICountryMaster : IRepository<int, CountryMaster>
    {

    }
}
