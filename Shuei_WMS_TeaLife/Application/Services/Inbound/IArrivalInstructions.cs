using Application.DTOs.Request;
using Application.DTOs;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services.Base;

using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Inbound
{
    [BasePath(ApiRoutes.ArrivalInstructions.BasePath)]
    public interface IArrivalInstructions : IRepository<int, ArrivalInstruction>
    {
        [Post(ApiRoutes.ArrivalInstructions.SearchAsync)]
        Task<Result<PageList<ArrivalInstructionDto>>> SearchArrivalInstructions([Body] QueryModel<ReceivePlanSearchModel> model);
    }
}
