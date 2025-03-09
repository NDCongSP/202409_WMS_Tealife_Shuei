
using Application.DTOs.Response;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using RestEase;
using WebUIFinal.Core;
using NumberSequenceEntity = FBT.ShareModels.WMS.NumberSequences;

namespace WebUIFinal.Pages.NumberSequence
{
    public partial class NumberSequenceDetail
    {
        [Parameter] public string Title { get; set; }
        public Guid? Id { get; set; }

        private bool _isVisible = true;
        private NumberSequenceEntity _model = new NumberSequenceEntity();

        // Enum values for warehouse transaction types
        private List<dynamic> warehouseTransTypes = new List<dynamic>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Populate the dropdown with enum values
            warehouseTransTypes = Enum.GetValues(typeof(EnumWarehouseTransType))
                              .Cast<EnumWarehouseTransType>()
                              .Select(e => new { Text = e.ToString(), Value = e.ToString() }) // Both Text and Value are strings
                              .ToList<dynamic>();

            if (Title.Contains(_localizerCommon["Detail.View"])) _isVisible = false;

            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (Guid.TryParse(sub[1], out Guid guid))
                {
                    Id = guid;
                }
            }

            #region Get info
            if (Id.HasValue && Id != Guid.Empty)
            {
                var arg = await _numberSequenceServices.GetByIdAsync(Id.Value);
                if (!arg.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(arg.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _model.Id = arg.Data.Id;
                _model.JournalType = arg.Data.JournalType;
                _model.Prefix = arg.Data.Prefix;
                _model.SequenceLength = arg.Data.SequenceLength;
                _model.CurrentSequenceNo = arg.Data.CurrentSequenceNo;
            }
            #endregion

            StateHasChanged();
        }

        async Task Submit(NumberSequenceEntity arg)
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}: {arg.JournalType}?", _localizerCommon["Save"], new ConfirmOptions()
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            // arg.Status = selectStatus.ToString();

            if (Title.Contains(_localizerCommon["Detail.Create"]))//Add
            {
                var res = await _numberSequenceServices.InsertAsync(_model);
                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
            }
            else if (Title.Contains(_localizerCommon["Detail.Edit"]))//update
            {
                var res = await _numberSequenceServices.UpdateAsync(_model);
                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
            }
        }

        async Task DeleteItemAsync(NumberSequenceEntity numberSequence)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}: {numberSequence.JournalType}?", $"{_localizerCommon["Delete"]} {_localizer["Number Sequence"]}", new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _numberSequenceServices.DeleteAsync(numberSequence);

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.First();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
                _navigation.NavigateTo("/numbersequencelist", true);
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
