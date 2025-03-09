using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class RepositoryTaskServices : ITaskModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessore;

        public RepositoryTaskServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessore)
        {
            _dbContext = dbContext;
            _contextAccessore = contextAccessore;
        }

        public async Task<Result<List<TaskReportDto>>> GetTaskReport()
        {
            var currentTime = DateTime.Now;

            var activeTasks = await (
                from task in _dbContext.TaskModels
                join tenant in _dbContext.TenantAuth 
                    on task.CompanyId equals tenant.TenantId
                where task.Status != 1
                select new 
                { 
                    TaskId = task.Id,
                    TenantId = task.CompanyId,
                    TenantName = tenant.TenantFullName 
                }).ToListAsync();

            var tasksWithChat = await _dbContext.TaskChatHistories
                .Select(x => x.TaskId)
                .Distinct()
                .ToListAsync();

            var result = activeTasks
                .GroupBy(x => new { x.TenantId, x.TenantName })
                .Select(g => new TaskReportDto
                {
                    TenantId = g.Key.TenantId,
                    TenantName = g.Key.TenantName,
                    Responsibility = g.Count(),
                    TaskIsOnHold = g.Count(x => !tasksWithChat.Contains(x.TaskId))
                })
                .ToList();

            return Result<List<TaskReportDto>>.Success(result);
        }
    }
}
