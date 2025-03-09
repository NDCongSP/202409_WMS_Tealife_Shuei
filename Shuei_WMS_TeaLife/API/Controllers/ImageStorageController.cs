using Application.DTOs.Request;
using Application.Extentions;
using Application.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImageStorageController : ControllerBase, IImageStorage
    {
        readonly Repository _repository;

        public ImageStorageController(Repository repository)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.ImageStorage.GetByResourceIdAndTypeAsync)]
        public async Task<Result<List<ImageStorage>>> GetByResourceIdAndTypeAsync([Body] ImageStorageSearchRequestDTO model)
        {
            return await _repository.SImageStorage.GetByResourceIdAndTypeAsync(model);
        }
    }
}
