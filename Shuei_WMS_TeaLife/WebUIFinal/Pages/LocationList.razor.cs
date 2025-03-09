using Application.DTOs.Response.Account;
using Radzen.Blazor;
using Radzen;
using WebUIFinal.Pages.Components;
using Application.DTOs.Response;
using Newtonsoft.Json;
using RestEase;


namespace WebUIFinal.Pages
{
    public partial class LocationList
    {
        List<Location> _dataGrid = null;
        RadzenDataGrid<Location> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizer["DisplayPage"] + " {0} " + _localizer["Of"] + " {1} <b>(" + _localizer["Total"] + " {2} " + _localizer["Records"] + ")</b>";

            RefreshDataAsync();
        }

        async Task DeleteItemAsync(Location model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Delete"]} {_localizer["Location"]}: {model.LocationName}?", $"{_localizer["Delete"]} {_localizer["Location"]}", new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                #region delete bin of location
                var responseBins = await _binServices.GetByLocationId(model.Id);
                if (!responseBins.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = responseBins.Messages.ToString(),
                        Duration = 5000
                    });

                    return;
                }
                var responseDeleteBin = await _binServices.DeleteRangeAsync(responseBins.Data);
                if (!responseDeleteBin.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = responseBins.Messages.ToString(),
                        Duration = 5000
                    });

                    return;
                }
                #endregion

                var res = await _locationServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                         Summary = _localizer["Success"],
                        Detail = $"Delete location {model.LocationName} successfully.",
                        Duration = 5000
                    });

                    await RefreshDataAsync();
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
            _navigation.NavigateTo($"/addlocation/{_localizer["Detail.Edit"]} {_localizer["Location"]}|{id}");
        }

        async Task AddNewItemAsync()
        {
            _navigation.NavigateTo($"/addlocation/{_localizer["Detail.Create"]} {_localizer["Location"]}");
        }

        async Task ViewItemAsync(string id)
        {

            _navigation.NavigateTo($"/addlocation/{_localizer["Detail.View"]} {_localizer["Location"]}|{id}");
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _locationServices.GetAllAsync();

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                 , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                 , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _dataGrid = null;
                _dataGrid = new List<Location>();
                _dataGrid = res.Data.ToList();

                //await _profileGrid.RefreshDataAsync();

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }
    }
}
