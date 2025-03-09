using Application.DTOs.Response;
using Application.DTOs.Response.Account;

using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using RestEase;
using WebUIFinal.Pages.Components;

namespace WebUIFinal.Pages.Account
{
    public partial class Permission
    {
        List<PermissionsListResponseDTO> _dataGrid = null;
        RadzenDataGrid<PermissionsListResponseDTO> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizer["DisplayPage"] + " {0} " + _localizer["Of"] + " {1} <b>(" + _localizer["Total"] + " {2} " + _localizer["Records"] + ")</b>";

            await RefreshDataAsync();
        }

        async Task DeleteItemAsync(PermissionsListResponseDTO model)
        {
            try
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Delete"] + _localizer["Permission.Name"] + $": {model.Name}?", _localizer["Delete"] + " " + _localizer["Permission.Name"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _permissionsServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizer["Success"],
                        Detail = $"Delete permission {model.Name} successfully.",
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
                        Detail = res.Messages.ToString(),
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

            _navigation.NavigateTo($"/addpermission/{_localizer["Detail.Edit"]} {_localizer["Permission"]}|{id}");
            //var res = await _dialogService.OpenAsync<DialogCardPageAddNewPermission>($"Edit Permission",
            //       new Dictionary<string, object>() { { "_model", model } },
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
            _navigation.NavigateTo($"/addpermission/{_localizer["Detail.Create"]} {_localizer["Permission"]}");
            //var res = await _dialogService.OpenAsync<DialogCardPageAddNewPermission>($"Create New Permission",
            //        new Dictionary<string, object>() { },
            //        new DialogOptions()
            //        {
            //            Width = "800px",
            //            Height = "450px",
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

            _navigation.NavigateTo($"/addpermission/{_localizer["Detail.View"]} {_localizer["Permission"]}|{id}");
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _permissionsServices.GetAllPermissionWithAssignedRoleAsync();

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _dataGrid = null;
                _dataGrid = new List<PermissionsListResponseDTO>();
                _dataGrid = res.Data.ToList();

                //await _profileGrid.RefreshDataAsync();

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService
                   , NotificationSeverity.Error
                   , _localizerNotification["Error"], ex.Message);

                return;
            }
        }
    }
}
