using Application.DTOs;
using Application.Extentions;
using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.TaskModel.BasePath)]
    public interface ITaskModel
    {
        [Get(ApiRoutes.TaskModel.GetTaskReport)]
        Task<Result<List<TaskReportDto>>> GetTaskReport();
    }
}
