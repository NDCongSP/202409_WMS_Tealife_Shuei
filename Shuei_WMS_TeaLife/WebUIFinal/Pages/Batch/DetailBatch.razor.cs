using Application.DTOs.Response;
using Application.DTOs.Response.Account;

using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using RestEase;
using WebUIFinal.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BatchEntity = FBT.ShareModels.WMS.Batches;

namespace WebUIFinal.Pages.Batch
{
    public partial class DetailBatch
    {
        [Parameter] public string Title { get; set; }
        public Guid? Id { get; set; }

        private bool isDisabled = false;
        private BatchEntity _model = new BatchEntity();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (Title.Contains(_localizerCommon["Detail.View"])) isDisabled = true;

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
                var arg = await _batchServices.GetByIdAsync(Id.Value);
                if (!arg.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(arg.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                   , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                   , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _model.Id = arg.Data.Id;
                _model.ProductCode = arg.Data.ProductCode;
                _model.TenantId = arg.Data.TenantId;
                _model.LotNo = arg.Data.LotNo;
                _model.ManufacturingDate = arg.Data.ManufacturingDate;
                _model.ExpirationDate = arg.Data.ExpirationDate;
            }
            #endregion
            StateHasChanged();
        }

        async Task Submit(BatchEntity arg)
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}", _localizerCommon["Save"], new ConfirmOptions()
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            if (Title.Contains(_localizerCommon["Detail.Create"])) // Add new number sequence
            {
                var res = await _batchServices.InsertAsync(arg);
                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                   , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                   , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService
                  , NotificationSeverity.Success
                  , _localizerNotification["Success"], _localizerNotification["Success"]);

                _navigation.NavigateTo("/batches", true);
            }
            else if (Title.Contains(_localizerCommon["Detail.Edit"])) // Update existing number sequence
            {
                var res = await _batchServices.UpdateAsync(arg);
                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                   , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                   , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService
                  , NotificationSeverity.Success
                  , _localizerNotification["Success"], _localizerNotification["Success"]);

                _navigation.NavigateTo("/batches", true);
            }
        }
        async Task DeleteItemAsync(BatchEntity _batch)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delate"]}: {_batch.Id}?", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _batchServices.DeleteAsync(_batch);

                if (res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                   , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                   , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService
                , NotificationSeverity.Success
                , _localizerNotification["Success"], _localizerNotification["Success"]);

                _navigation.NavigateTo("/device", true);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService
                , NotificationSeverity.Error
                , _localizerNotification["Error"], ex.Message);
            }
        }
    }
}
