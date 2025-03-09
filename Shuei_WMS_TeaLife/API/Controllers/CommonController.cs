using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Models;
using Application.Services;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase, ICommon
    {
        protected Repository _repository;

        public CommonController(Repository repository)
        {
            _repository = repository;
        }

        [HttpGet(ApiRoutes.Common.GetDataForExportCsvAsync)]
        public async Task<Result<string>> GetDataForExportCsvAsync()
        {
            return await _repository.SCommon.GetDataForExportCsvAsync();
        }

        [HttpPost(ApiRoutes.Common.GetLaterPaymentAsync)]
        public async Task<Result<string>> GetLaterPaymentAsync([Body] LaterPayRequest model)
        {
            return await _repository.SCommon.GetLaterPaymentAsync(model);
        }

        [HttpPost(ApiRoutes.Common.ImportCsvAsync)]
        public async Task<Result<string>> ImportCsvAndUpdateTrackingNoAsync([Body] List<ImportCSVForJPYPModel> models)
        {
            return await _repository.SCommon.ImportCsvAndUpdateTrackingNoAsync(models);
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Common.ImportCsvAsync1)]
        public async Task<Result<string>> ImportCsvAndUpdateTrackingNoAsync1(IFormFile file)
        {
            return await _repository.SCommon.ImportCsvAndUpdateTrackingNoAsync1(file);
        }

        [HttpPost(ApiRoutes.Common.UpdateHHTStatusAsync)]
        public async Task<Result<HhtUpdateStatusRequestDTO>> UpdateHHTStatusAsync([Body] HhtUpdateStatusRequestDTO model)
        {
            return await _repository.SCommon.UpdateHHTStatusAsync(model);
        }
    }
}
