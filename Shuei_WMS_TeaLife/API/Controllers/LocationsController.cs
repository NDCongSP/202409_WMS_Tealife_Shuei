using API.Controllers.Base;
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
    public class LocationsController : BaseController<Guid, Location>, ILocations
    {
        readonly Repository _repository;

        public LocationsController(Repository repository = null) : base(repository.SLocations)
        {
            _repository = repository;
        }
    }
}
