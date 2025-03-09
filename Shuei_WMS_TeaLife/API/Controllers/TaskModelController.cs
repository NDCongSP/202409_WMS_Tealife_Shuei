using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskModelController : ControllerBase, ITaskModel
    {
        readonly Repository _repository;

        public TaskModelController(Repository repository)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.TaskModel.GetTaskReport)]
        public async Task<Result<List<TaskReportDto>>> GetTaskReport() => await _repository.STask.GetTaskReport();
    }
}
