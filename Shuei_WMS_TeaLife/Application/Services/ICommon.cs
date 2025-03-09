using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using RestEase;

namespace Application.Services
{
    [BasePath(ApiRoutes.Common.BasePath)]
    public interface ICommon
    {
        [Post(ApiRoutes.Common.UpdateHHTStatusAsync)]
        Task<Result<HhtUpdateStatusRequestDTO>> UpdateHHTStatusAsync([Body] HhtUpdateStatusRequestDTO model);

        [Post(ApiRoutes.Common.GetLaterPaymentAsync)]
        Task<Result<string>> GetLaterPaymentAsync([Body] LaterPayRequest model);

        [Get(ApiRoutes.Common.GetDataForExportCsvAsync)]
        Task<Result<string>> GetDataForExportCsvAsync();

        [Post(ApiRoutes.Common.ImportCsvAsync1)]
        Task<Result<string>> ImportCsvAndUpdateTrackingNoAsync1([Body]IFormFile file);

        [Post(ApiRoutes.Common.ImportCsvAsync)]
        Task<Result<string>> ImportCsvAndUpdateTrackingNoAsync([Body] List<ImportCSVForJPYPModel> models);
    }
}
