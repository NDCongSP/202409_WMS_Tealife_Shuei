using Application.DTOs;
using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Application.Extentions;
using RestEase;

namespace Application.Services.Authen.UI
{
    [BaseAddress(ApiRoutes.Identity.BasePath)]
    public interface IAuthServices
    {
        Task<GeneralResponse> CreateSuperAdminAsync();
        Task<GeneralResponse> CreateAccountAsync(CreateAccountRequestDTO model);        
        Task<LoginResponse> LoginAccountAsync(LoginRequestDTO model);
        Task<LoginResponse> LoginAccountHTAsync(LoginRequestDTO model);
        Task<LoginResponse> RefreshTokenAsync();
        Task<GeneralResponse> CreateRoleAsysnc(CreateRoleRequestDTO model);
        Task<List<GetRoleResponseDTO>> GetRolesAsync();
        Task<List<GetUserWithRoleResponseDTO>> GetUsersWithRolesAsync();
        Task<GeneralResponse> ChangeUserRoleAsync(AssignUserRoleRequestDTO model);
        Task<GeneralResponse> ChangePassAsync(ChangePassRequestDTO model);
        Task<GeneralResponse> AssignUserRoleAsync(AssignUserRoleRequestDTO model);

        //Task<string> TryRefreshTokenAsync(RefreshTokenRequestDTO model);
        Task LogoutAsync();
        Task<GeneralResponse> DeleteUserAsync(UpdateDeleteRequestDTO model);
        Task<GeneralResponse> DeleteUserRoleAsync(AssignUserRoleRequestDTO model);
        Task<GeneralResponse> UpdateRoleAsync(UpdateDeleteRequestDTO model);
        Task<GeneralResponse> UpdateRoleDTOAsync(CreateRoleRequestDTO model);
        Task<GeneralResponse> UpdateUserInfoAsync(UpdateUserInfoRequestDTO model);
        Task<GetUserWithRoleResponseDTO> UserGetById(string id);
        Task<GeneralResponse> DeleteRoleAsync(UpdateDeleteRequestDTO model);
        Task<GetUserWithRoleResponseDTO> UserGetByEmailAsync(string email);

        Task<GetRoleResponseDTO> RoleGetById(string id);

        Task<string> GetReportBase64(string id);
        Task<string> GeneratePdf();
        Task<List<LabelInfoDto>> GetLabelByIdAsync(string id);
        Task<List<LabelInfoDto>> GetLabelsAllAsync();
    }
}
