using Application.DTOs.Request.Account;
using Application.DTOs.Response.Account;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace WebUI.Pages.Components
{
    public partial class DialogCardPageAddNewUser1
    {
        private CreateAccountRequestDTO _model = new CreateAccountRequestDTO();
        List<CreateRoleRequestDTO> _roles = new List<CreateRoleRequestDTO>();
        private List<CreateRoleRequestDTO> _selectedRoles = []; 

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var result = await _authenServices.GetRolesAsync();

            foreach (var role in result)
            {
                _roles.Add(new CreateRoleRequestDTO() { Name = role.Name, Id = role.Id });
            }
            StateHasChanged();
        }

         async void Submit(CreateAccountRequestDTO arg)
        {
            var confirm = await _dialogService.Confirm($"Do you want to create a new account: {arg.UserName}?", "Create user", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Roles = _selectedRoles;

            var res = await _authenServices.CreateAccountAsync(arg);
            if (res.Flag)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = res.Message,
                    Duration = 2000
                });

                _dialogService.Close("Success");
            }
            else
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = res.Message,
                    Duration = 2000
                });
            }
        }


        async void RefreshData()
        {
            try
            {
                //_ovenId = int.TryParse(OvenId, out int value) ? value : 0;

                //var res = await _ft01Client.GetAllAsync();

                //if (res == null)
                //    return;
                //_ft01 = res.Data.ToList();

                //_ovenInfo = JsonConvert.DeserializeObject<OvensInfo>(_ft01.FirstOrDefault().C001).FirstOrDefault(x => x.Id == _ovenId);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = ex.Message,
                    Duration = 2000
                });
                return;
            }

            StateHasChanged();
        }

        void ShowTooltip(ElementReference elementReference, TooltipOptions options = null)
        {
            _tooltipService.Open(elementReference, "Full name", options);
        }
    }
}
