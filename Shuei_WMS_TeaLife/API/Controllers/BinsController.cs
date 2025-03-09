using API.Controllers.Base;
using Application.DTOs;
using Application.Extentions;
using Application.Models;
using Application.Services;


using Infrastructure.Data;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BinsController : BaseController<Guid, Bin>, IBins
    {
        readonly Repository _repository;

        public BinsController(Repository repository = null) : base(repository.SBins)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.Bins.AddOrUpdate)]
        public async Task<Result<List<Bin>>> AddOrUpdateAsync([Body] List<Bin> model)
        {
            return await _repository.SBins.AddOrUpdateAsync(model);
        }

        [HttpPost(ApiRoutes.Bins.GetByLocationId)]
        public async Task<Result<List<Bin>>> GetByLocationId([Path] Guid locationId)
        {
            return await _repository.SBins.GetByLocationId(locationId);
        }

        [HttpGet(ApiRoutes.Bins.GetLabelById)]
        public async Task<List<LabelInfoDto>> GetLabelByIdAsync([Path] string id)
        {
            return await _repository.SBins.GetLabelByIdAsync(id);
        }

        [HttpGet(ApiRoutes.Bins.GetLabelByLocationId)]
        public async Task<List<LabelInfoDto>> GetLabelByLocationIdAsync([Path] Guid locationId)
        {
            return await _repository.SBins.GetLabelByLocationIdAsync(locationId);
        }
    }
}
