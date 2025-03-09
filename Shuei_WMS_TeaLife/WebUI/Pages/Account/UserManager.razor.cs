using Application.DTOs.Request.Account;
using Application.DTOs.Response.Account;
using Radzen;
using Radzen.Blazor;

namespace WebUI.Pages.Account
{
    public partial class UserManager
    {
        List<GetUserWithRoleResponseDTO> _users = new List<GetUserWithRoleResponseDTO>();
        GetUserWithRoleResponseDTO _userModel = new GetUserWithRoleResponseDTO();

        CreateAccountRequestDTO _registerModel = new CreateAccountRequestDTO();
        string _roleSelect;
        List<string> _role = new List<string>() { "User", "Operator" };

        RadzenDataGrid<GetUserWithRoleResponseDTO> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        protected override async Task OnInitializedAsync()
        {
            await RefreshDataAsync();
        }

        async Task DeleteItemAsync(UpdateDeleteRequestDTO model)
        {
            try
            {
                //var confirm = await _dialogService.Confirm($"Bạn chắc chắn muốn xóa user: {model.Name}", "Xóa tài khoản", new ConfirmOptions()
                //{
                //    OkButtonText = "Yes",
                //    CancelButtonText = "No",
                //    AutoFocusFirstElement = true,
                //});

                //if (confirm == null || confirm == false) return;

                var res = await _authenServices.DeleteUserAsync(model);

                if (res.Flag)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = res.Message,
                        Duration = 2000
                    });

                    _registerModel = null;
                    _registerModel = new CreateAccountRequestDTO();

                    RefreshDataAsync();
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
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = ex.Message,
                    Duration = 2000
                });

                return;
            }
        }

        //async Task EditItemAsync(string userName) { }

        //async Task AddNewItemAsync()
        //{
        //    var res = await _dialogService.OpenAsync<DialogCardPageAddNewUser>($"Create new user",
        //            new Dictionary<string, object>() { },
        //            new DialogOptions()
        //            {
        //                Width = "1000px",
        //                Height = "800px",
        //                Resizable = true,
        //                Draggable = true,
        //                CloseDialogOnOverlayClick = true
        //            });

        //    if (res == "Success")
        //    {
        //        RefreshDataAsync();
        //    }
        //}

        async Task RefreshDataAsync()
        {
            try
            {
                _role = null; _role = new List<string>();
                var resultRole = await _authenServices.GetRolesAsync();
                if (resultRole != null || resultRole.Count > 0)
                {
                    foreach (var item in resultRole)
                    {
                        _role.Add(item.Name);
                    }
                }

                var res = await _authenServices.GetUsersWithRolesAsync();
                _users = null;
                _users = new List<GetUserWithRoleResponseDTO>();

                if (res != null)
                    _users.AddRange(res);

                await _profileGrid.RefreshDataAsync();

                StateHasChanged();
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
        }
    }
}
