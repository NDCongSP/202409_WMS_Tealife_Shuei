using Application.DTOs.Response;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using RestEase;
using NumberSequenceModel = FBT.ShareModels.WMS.NumberSequences;

namespace WebUIFinal.Pages.NumberSequence
{
    public partial class NumberSequenceMaster
    {
        List<NumberSequenceModel>? _dataGrid = null;
        RadzenDataGrid<NumberSequenceModel>? _profileGrid;

        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            RefreshDataAsync();
        }

        async Task DeleteItemAsync(NumberSequenceModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}: {model.JournalType}?", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _numberSequenceServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                         Summary = _localizerCommon["Success"],
                        Detail = $"Delete {model.JournalType} successfully.",
                        Duration = 5000
                    });

                    RefreshDataAsync();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
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
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }
        }

        async Task ViewItemAsync(NumberSequenceModel model)
        {
            _navigation.NavigateTo($"/detailnumbersequence/{_localizerCommon["Detail.View"]} {_localizer["Number Sequence"]}|{model.Id}");
        }

        async Task EditItemAsync(NumberSequenceModel model)
        {
            _navigation.NavigateTo($"/detailnumbersequence/{_localizerCommon["Detail.Edit"]} {_localizer["Number Sequence"]}|{model.Id}");
        }

        async Task AddNewItemAsync()
        {
            _navigation.NavigateTo($"/detailnumbersequence/{_localizerCommon["Detail.Create"]} {_localizer["Number Sequence"]}");
        }

        async void RefreshDataAsync()
        {
            try
            {
                var res = await _numberSequenceServices.GetAllAsync();

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _dataGrid = null;
                _dataGrid = new List<NumberSequenceModel>();
                _dataGrid = res.Data.ToList();

                filteredData = _dataGrid;

                StateHasChanged();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }
    }
}
