using Application.DTOs.Request.Account;
using Application.DTOs.Response.Account;
using Application.DTOs.Response;
using RestEase;
using Application.Extentions;
using Application.DTOs;


namespace Application.Services.Authen
{
    public interface IAccount
    {
        Task<GeneralResponse> CreateSuperAdminAsync();
        Task<GeneralResponse> CreateAccountAsync([Body] CreateAccountRequestDTO model);
        Task<LoginResponse> LoginAccountAsync([Body] LoginRequestDTO model);
        Task<LoginResponse> LoginAccountHTAsync([Body] LoginRequestDTO model);
        Task<LoginResponse> RefreshTokenAsync([Body] RefreshTokenRequestDTO model);
        Task<GeneralResponse> CreateRoleAsync([Body] CreateRoleRequestDTO model);
        Task<List<GetRoleResponseDTO>> GetRolesAsync();
        Task<List<GetUserWithRoleResponseDTO>> GetUsersWithRolesAsync();
        Task<GeneralResponse> ChangeUserRoleAsync([Body] AssignUserRoleRequestDTO model);
        Task<GeneralResponse> ChangePassAsync([Body] ChangePassRequestDTO model);
        Task<GeneralResponse> AssignUserRoleAsync([Body] AssignUserRoleRequestDTO model);
        Task<GeneralResponse> DeleteUserAsync([Body] UpdateDeleteRequestDTO model);

        Task<GeneralResponse> DeleteUserRoleAsync([Body] AssignUserRoleRequestDTO model);
        Task<GeneralResponse> UpdateRoleAsync([Body] UpdateDeleteRequestDTO model);
        Task<GeneralResponse> UpdateRoleDTOAsync([Body] CreateRoleRequestDTO model);

        Task<GeneralResponse> UpdateUserInfoAsync([Body]UpdateUserInfoRequestDTO model);
        Task<GetUserWithRoleResponseDTO> UserGetById([Path] string id);
        Task<GeneralResponse> DeleteRoleAsync([Body] UpdateDeleteRequestDTO model);
        Task<GetUserWithRoleResponseDTO> UserGetByEmailAsync([Path] string email);

        [Get(ApiRoutes.Identity.RoleGetById)]
        Task<GetRoleResponseDTO> RoleGetById([Path] string id);

        [Get(ApiRoutes.Identity.GetReportBase64)]
        Task<string> GetReportBase64([Path] string id);
        [Get(ApiRoutes.Identity.GeneratePdf)]
        Task<string> GeneratePdf();
        [Get(ApiRoutes.Identity.GetLabelById)]
        Task<List<LabelInfoDto>> GetLabelByIdAsync([Path] string id );
        [Get(ApiRoutes.Identity.GetLabelsAll)]
        Task<List<LabelInfoDto>> GetLabelsAllAsync();

        [Get(ApiRoutes.Identity.JobApi)]
        Task<Permissions> JobApi();
    }
}
