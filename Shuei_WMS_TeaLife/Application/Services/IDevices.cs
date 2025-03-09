using Application.DTOs;
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
    [BasePath(ApiRoutes.Devices.BasePath)]
    public interface IDevices : IRepository<Guid, Device>
    {
        [Get(ApiRoutes.Devices.GetByNameAsync)]
        Task<Result<Device>> GetByNameAsync([Path] string name);
        [Get(ApiRoutes.Devices.CheckNameExists)]
        Task<Result<bool>> CheckNameExists([Path] string name);
    }
}
