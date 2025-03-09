using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Application.Extentions;
using Application.Services.Authen;
using Application.Services.Authen.UI;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace API.Controllers
{
    [Authorize]
    [Route($"api/[controller]")]
    [ApiController]
    public class AccountController(IAccount account) : ControllerBase
    {
        //[Authorize(Policy ="Admin")]
        [HttpPost(ApiRoutes.Identity.CreateAccount)]
        public async Task<ActionResult<GeneralResponse>> CreateAccountAsync(CreateAccountRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.CreateAccountAsync(model));
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<ActionResult<GeneralResponse>> LoginAccountAsync(LoginRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.LoginAccountAsync(model));
        }
        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.LoginHt)]
        public async Task<ActionResult<GeneralResponse>> LoginAccountHtAsync(LoginRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.LoginAccountHTAsync(model));
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.RefreshToken)]
        public async Task<ActionResult<GeneralResponse>> RefreshTokenAsync(RefreshTokenRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.RefreshTokenAsync(model));
        }

        [HttpPost(ApiRoutes.Identity.CreateRole)]
        public async Task<ActionResult<GeneralResponse>> CreateRoleAsync(CreateRoleRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.CreateRoleAsync(model));
        }

        [HttpGet(ApiRoutes.Identity.RoleList)]
        public async Task<ActionResult<IEnumerable<GetRoleResponseDTO>>> GetRoleAsync()
        {
            return Ok(await account.GetRolesAsync());
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Identity.CreateSuperAdminAccount)]
        public async Task<ActionResult<GeneralResponse>> CreateSupperAdminAsync()
        {
            return Ok(await account.CreateSuperAdminAsync());
        }

        [HttpGet(ApiRoutes.Identity.UserWithRole)]
        public async Task<ActionResult<List<GeneralResponse>>> GetUserWithRoleAsync()
        {
            return Ok(await account.GetUsersWithRolesAsync());
        }

        [HttpPost(ApiRoutes.Identity.ChangePassword)]
        public async Task<ActionResult<GeneralResponse>> ChangePassAsync(ChangePassRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.ChangePassAsync(model));
        }

        [HttpPost(ApiRoutes.Identity.ChangeUserRole)]
        public async Task<ActionResult<GeneralResponse>> ChangeRoleAsync(AssignUserRoleRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.ChangeUserRoleAsync(model));
        }

        [HttpPost(ApiRoutes.Identity.AssignUserRole)]
        public async Task<ActionResult<GeneralResponse>> AssignUserRoleAsync(AssignUserRoleRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.AssignUserRoleAsync(model));
        }

        [HttpPost(ApiRoutes.Identity.DeleteUser)]
        public async Task<ActionResult<GeneralResponse>> DeleteUserAsync(UpdateDeleteRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.DeleteUserAsync(model));
        }

        [HttpPost(ApiRoutes.Identity.DeleteUserRole)]
        public async Task<ActionResult<GeneralResponse>> DeleteUserRoleAsync(AssignUserRoleRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.DeleteUserRoleAsync(model));
        }

        [HttpPost(ApiRoutes.Identity.UpdateRole)]
        public async Task<ActionResult<GeneralResponse>> UpdateRoleAsync(UpdateDeleteRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.UpdateRoleAsync(model));
        }

        [HttpPost(ApiRoutes.Identity.UpdateRoleDTO)]
        public async Task<ActionResult<GeneralResponse>> UpdateRoleDTOAsync(CreateRoleRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.UpdateRoleDTOAsync(model));
        }

        [HttpPost(ApiRoutes.Identity.UpdateUserInfo)]
        public async Task<ActionResult<GeneralResponse>> UpdateUserInfoAsync(UpdateUserInfoRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.UpdateUserInfoAsync(model));
        }

        [HttpGet(ApiRoutes.Identity.UserGetById)]
        public async Task<ActionResult<GeneralResponse>> UserGetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Model cannot be null");

            return Ok(await account.UserGetById(id));
        }

        [HttpGet(ApiRoutes.Identity.UserGetByEmail)]
        public async Task<ActionResult<GeneralResponse>> UserGetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Model cannot be null");

            return Ok(await account.UserGetByEmailAsync(email));
        }

        [HttpPost(ApiRoutes.Identity.DeleteRole)]
        public async Task<ActionResult<GeneralResponse>> DeleteRoleAsync(UpdateDeleteRequestDTO model)
        {
            if (model == null) return BadRequest("Model cannot be null");

            return Ok(await account.DeleteRoleAsync(model));
        }

        [HttpGet(ApiRoutes.Identity.RoleGetById)]
        public async Task<ActionResult<GeneralResponse>> RoleGetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Id cannot be null");

            return Ok(await account.RoleGetById(id));
        }

        [HttpGet(ApiRoutes.Identity.GetReportBase64)]
        public async Task<ActionResult<string>> GetReportBase64(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Id cannot be null");

            return Ok(await account.GetReportBase64(id));
        }

        [HttpGet(ApiRoutes.Identity.GeneratePdf)]
        public async Task<ActionResult<string>> GeneratePdf()
        {
            return Ok(await account.GeneratePdf());
        }

        [HttpGet(ApiRoutes.Identity.GetLabelById)]
        public async Task<ActionResult<string>> GetLabelByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest("Id cannot be null");
            return Ok(await account.GetLabelByIdAsync(id));
        }

        [HttpGet(ApiRoutes.Identity.GetLabelsAll)]
        public async Task<ActionResult<string>> GetLabelsAllAsync()
        {
            return Ok(await account.GetLabelsAllAsync());
        }

        [AllowAnonymous]
        [HttpGet(ApiRoutes.Identity.JobApi)]
        public async Task<ActionResult<Permissions>> JobApi()
        {
            return Ok(await account.JobApi());
        }
    }
}
