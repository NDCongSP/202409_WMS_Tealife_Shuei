using Application.DTOs.Response.Account;
using Radzen.Blazor;
using Radzen;
using WebUIFinal.Pages.Components;
using Microsoft.AspNetCore.Identity;
using Application.DTOs.Request.Account;

namespace WebUIFinal.Pages.Account
{
    public partial class Roles
    {
        List<GetRoleResponseDTO> _dataGrid = null;
        RadzenDataGrid<GetRoleResponseDTO> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizer["DisplayPage"] + " {0} " + _localizer["Of"] + " {1} <b>(" + _localizer["Total"] + " {2} " + _localizer["Records"] + ")</b>";

            await RefreshDataAsync();
        }

        async Task DeleteItemAsync(GetRoleResponseDTO model)
        {
            try
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Delete"] + _localizer["Role"] + $": {model.Name}?", _localizer["Delete"] + " " + _localizer["Role"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _authenServices.DeleteRoleAsync(new UpdateDeleteRequestDTO() { Id = model.Id, Name = model.Name });

                if (res.Flag)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                         Summary = _localizer["Success"],
                        Detail = res.Message,
                        Duration = 5000
                    });

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

        async Task EditItemAsync(string id)
        {
            _navigation.NavigateTo($"/addRole/{_localizer["Detail.Edit"]} {_localizer["Role"]}|{id}");
            //var res = await _dialogService.OpenAsync<DialogCardPageAddNewRole>($"Edit Role",
            //       new Dictionary<string, object>() { { "_model", new CreateRoleRequestDTO() { Id = model.Id, Name = model.Name } } },
            //       new DialogOptions()
            //       {
            //           Width = "800px",
            //           Height = "450px",
            //           Resizable = true,
            //           Draggable = true,
            //           CloseDialogOnOverlayClick = true
            //       });

            //if (res == "Success")
            //{
            //    RefreshDataAsync();
            //}
        }

        async Task AddNewItemAsync()
        {
            _navigation.NavigateTo($"/addRole/create/{_localizer["Detail.Create"]} {_localizer["Role"]}");
            //var res = await _dialogService.OpenAsync<DialogCardPageAddNewRole>($"Create New Role",
            //        new Dictionary<string, object>() { },
            //        new DialogOptions()
            //        {
            //            Width = "800px",
            //            Height = "300px",
            //            Resizable = true,
            //            Draggable = true,
            //            CloseDialogOnOverlayClick = true
            //        });

            //if (res == "Success")
            //{
            //    RefreshDataAsync();
            //}
        }
        async Task ViewItemAsync(string id)
        {

            _navigation.NavigateTo($"/addRole/{_localizer["Detail.View"]} {_localizer["Role"]}|{id}");
        }


        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _authenServices.GetRolesAsync();

                if (res == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = _localizerNotification["Something went wrong."],
                    });
                    return;
                }

                
                _dataGrid = null;
                _dataGrid = new List<GetRoleResponseDTO>();
                _dataGrid = res;

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
                    Detail = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                    Duration = 5000
                });
                return;

                throw;
            }
        }
    }
}
