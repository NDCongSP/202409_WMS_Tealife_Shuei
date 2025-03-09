using Application.DTOs.Request;
using Application.Extentions;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    [BasePath(ApiRoutes.ImageStorage.BasePath)]
    public interface IImageStorage
    {
        [Post(ApiRoutes.ImageStorage.GetByResourceIdAndTypeAsync)]
        Task<Result<List<ImageStorage>>> GetByResourceIdAndTypeAsync([Body] ImageStorageSearchRequestDTO model);
    }
}
