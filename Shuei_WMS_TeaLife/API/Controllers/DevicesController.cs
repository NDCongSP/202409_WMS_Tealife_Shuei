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
    public class DevicesController : BaseController<Guid, Device>, IDevices
    {
        readonly Repository _repository;

        public DevicesController(Repository repository = null) : base(repository.SDevices)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.Devices.GetByNameAsync)]
        public async Task<Result<Device>> GetByNameAsync(string name) => await _repository.SDevices.GetByNameAsync(name);

        [HttpGet(ApiRoutes.Devices.CheckNameExists)]
        public async Task<Result<bool>> CheckNameExists(string name) => await _repository.SDevices.CheckNameExists(name);
    }
}
