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
    [BasePath(ApiRoutes.Bins.BasePath)]
    public interface IBins : IRepository<Guid, Bin>
    {
        [Post(ApiRoutes.Bins.GetByLocationId)]
        Task<Result<List<Bin>>> GetByLocationId([Path] Guid locationId);

        [Post(ApiRoutes.Bins.AddOrUpdate)]
        Task<Result<List<Bin>>> AddOrUpdateAsync([Body] List<Bin> model);

        [Get(ApiRoutes.Bins.GetLabelById)]
        Task<List<LabelInfoDto>> GetLabelByIdAsync([Path] string id);
        [Get(ApiRoutes.Bins.GetLabelByLocationId)]
        Task<List<LabelInfoDto>> GetLabelByLocationIdAsync([Path] Guid locationId);
    }
}
