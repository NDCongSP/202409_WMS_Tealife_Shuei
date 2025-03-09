using Radzen.Blazor;
using Radzen;
using System.Reflection;
using Application.DTOs.Response.Account;
using WebUIFinal.Pages.Components;
using Application.DTOs.Request.Account;
using Microsoft.AspNetCore.Components;
using Application.DTOs.Request.Products;
using Application.DTOs.Request;

namespace WebUIFinal.Pages.Account
{
    public partial class UserManager
    {
        List<GetUserWithRoleResponseDTO> _users = new List<GetUserWithRoleResponseDTO>();
        List<GetUserWithRoleResponseDTO> _userSearch = new List<GetUserWithRoleResponseDTO>();
        GetUserWithRoleResponseDTO _userModel = new GetUserWithRoleResponseDTO();

        CreateAccountRequestDTO _registerModel = new CreateAccountRequestDTO();
        string _roleSelect;
        List<string> _role = new List<string>() { "User", "Operator" };

        RadzenDataGrid<GetUserWithRoleResponseDTO> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        UserSearchRequestDTO _searchModel = new UserSearchRequestDTO();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizer["DisplayPage"] + " {0} " + _localizer["Of"] + " {1} <b>(" + _localizer["Total"] + " {2} " + _localizer["Records"] + ")</b>";

            await RefreshDataAsync();
        }

        async Task DeleteItemAsync(UpdateDeleteRequestDTO model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Delete"]} {_localizer["User"]}: {model.Name}?", $"{_localizer["Delete"]} {_localizer["User"]}", new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _authenServices.DeleteUserAsync(model);

                if (res.Flag)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizer["Success"],
                        Detail = res.Message,
                        Duration = 5000
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
                        Summary = _localizer["Error"],
                        Detail = res.Message,
                        Duration = 5000
                    });
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });

                return;
            }
        }

        async Task ViewItemAsync(string id)
        {

            _navigation.NavigateTo($"/adduser/{_localizer["Detail.View"]} {_localizer["User"]}|{id}|");
        }
        async Task EditItemAsync(string id)
        {

            _navigation.NavigateTo($"/adduser/{_localizer["Detail.Edit"]} {_localizer["User"]}|{id}|");
        }

        async Task AddNewItemAsync()
        {
            _navigation.NavigateTo($"/adduser/{_localizer["Detail.Create"]} {_localizer["User"]}");
            //var res = await _dialogService.OpenAsync<DialogCardPageAddNewUser>($"Create new user",
            //        new Dictionary<string, object>() { },
            //        new DialogOptions()
            //        {
            //            Width = "1000px",
            //            Height = "800px",
            //            Resizable = true,
            //            Draggable = true,
            //            CloseDialogOnOverlayClick = true
            //        });

            //if (res == "Success")
            //{
            //    RefreshDataAsync();
            //}
        }

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
                _users = new List<GetUserWithRoleResponseDTO>();

                if (res != null)
                    _users.AddRange(res.Where(_ => _.Roles.FirstOrDefault()?.Name != "Warehouse API"));
                _userSearch = _users;

                //await _profileGrid.RefreshDataAsync();

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }
        }

        async void ReloadData(UserSearchRequestDTO arg)
        {
            _userSearch = _users.Where(x => (string.IsNullOrEmpty(arg.Keyword)
                                        || x.Email.ToLower().Contains(arg.Keyword.ToLower())
                                        || x.UserName.ToLower().Contains(arg.Keyword.ToLower())
                                        || x.FullName.ToLower().Contains(arg.Keyword.ToLower())) &&
                                        (string.IsNullOrEmpty(arg.RoleID) || x.Roles.Any(xx => xx.Name.ToLower() == arg.RoleID.ToLower()))).ToList();
            StateHasChanged();
        }
        async Task ClearFilter()
        {
            _searchModel = new UserSearchRequestDTO();
            ReloadData(_searchModel);
        }
    }
}
