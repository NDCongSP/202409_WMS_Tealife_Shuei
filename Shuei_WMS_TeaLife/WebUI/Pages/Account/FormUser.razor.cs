using Application.DTOs.Request.Account;
using Application.DTOs.Response.Account;
using Application.Extentions;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using WebUI.Pages.Account.Dto;
using static Application.Extentions.ConstantExtention;

namespace WebUI.Pages.Account
{
    public partial class FormUser
    {
        [Parameter] public required string Mode { get; set; }
        [Parameter] public GetUserWithRoleResponseDTO? UserDto { get; set; }

        private List<object> tenants;
        GetUserWithRoleResponseDTO _userModel = new GetUserWithRoleResponseDTO();
        private List<GetRoleResponseDTO> _filteredRoles = new List<GetRoleResponseDTO>();
        private List<GetRoleResponseDTO> _roles = new List<GetRoleResponseDTO>();
        private List<CreateRoleRequestDTO> selectedRoles = new List<CreateRoleRequestDTO>();
        private FormUserDto userDto = new FormUserDto();
        private string searchTerm = string.Empty;
        private bool isLoading = true;

        protected async override void OnInitialized()
        {
            try
            {
                _roles = await _authenServices.GetRolesAsync();

                _filteredRoles = new List<GetRoleResponseDTO>(_roles);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
        private async Task AddNewItemAsync()
        {
            if (userDto != null)
            {
                var payload = new CreateAccountRequestDTO();
                payload.UserName = userDto.UserName;
                payload.FullName = userDto.FullName;
                payload.Password = userDto.Password;
                payload.ConfirmPassword = userDto.Password;
                payload.Email = userDto.Email;
                payload.Status = userDto.Status;
                payload.Roles = selectedRoles;

                await _authenServices.CreateAccountAsync(payload);
            }
        }

        private async Task EditItemAsync(string userName) { }

        private async Task SaveUser()
        {
            if (Mode == ViewMode.Create)
            {
                await AddNewItemAsync();

                _navigation.NavigateTo("/");
            }
            else if (Mode == ViewMode.Edit)
            {
                await EditItemAsync("");
            }
        }

        private void AddRecord()
        {
            // Logic to add a record to the tenant table
        }

        private void FilterUsers(ChangeEventArgs e)
        {
            searchTerm = e.Value.ToString();
            _filteredRoles = _roles.Where(role => role.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            StateHasChanged();
        }

        private void UpdateSelection(CreateRoleRequestDTO role, object value)
        {
            if ((bool)value)
            {
                selectedRoles.Clear();
                selectedRoles.Add(role);
            }
            else
            {
                selectedRoles.RemoveAll(r => r.Id == role.Id);
            }
        }
    }
}
