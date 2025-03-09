using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.NumberSequences.BasePath)]
    public interface INumberSequences : IRepository<Guid, NumberSequences>
    {
        [Get(ApiRoutes.NumberSequences.GetNumberSequenceByType)]
        Task<Result<NumberSequences>> GetNumberSequenceByType([Path] string type);

        [Post(ApiRoutes.NumberSequences.IncreaseNumberSequenceByType)]
        Task<Result<bool>> IncreaseNumberSequenceByType([Path] string type);

        [Get(ApiRoutes.NumberSequences.GetNoByType)]
        Task<string> GetNoByType([Path] string type);

    }
}
