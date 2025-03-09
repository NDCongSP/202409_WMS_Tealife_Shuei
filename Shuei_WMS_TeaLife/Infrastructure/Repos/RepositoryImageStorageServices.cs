using Application.DTOs;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Models;
using Application.Services;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class RepositoryImageStorageServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IImageStorage
    {
        public async Task<Result<List<ImageStorage>>> GetByResourceIdAndTypeAsync([Body] ImageStorageSearchRequestDTO model)
        {
            try
            {
                string resourceId = string.Empty;

                foreach (var item in model.ResourceId)
                {
                    resourceId = $"{resourceId},{item}";
                }
                var resImage = await dbContext.Database.SqlQueryRaw<ImageStorage>("sp_getImageStorageByLiatResources @resourceId = {0},@type = {1}", resourceId, (int)model.Type)
                            .ToListAsync();

                if (resImage.Count == 0)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Could be not found data.");
                    return await Result<List<ImageStorage>>.FailAsync(JsonConvert.SerializeObject(err));
                }

                return await Result<List<ImageStorage>>.SuccessAsync(resImage, "Successful");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<List<ImageStorage>>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
