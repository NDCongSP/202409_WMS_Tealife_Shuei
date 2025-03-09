using Radzen;

using Application.DTOs;
using Microsoft.AspNetCore.Components;

namespace WebUIFinal.Pages.Movements;

public partial class DialogCreatePickingMovement
{

    [Parameter] public SubmitCompletedShipmentDto _model { get; set; } = new SubmitCompletedShipmentDto();
    [Parameter] public bool VisibleBtnSubmit { get; set; } = true;

    private List<SelectListItem> _personInChargeList;

    private async Task GetMasterDataAsync()
    {
        try
        {
            var personInChargeTask = _categoriesService.GetUserDropdown();

            await Task.WhenAll(personInChargeTask);

            _personInChargeList = personInChargeTask.Result.Data;
        }
        catch (Exception ex)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _movementLocalizer["FailedToLoadMasterData", ex.Message],
                Duration = 5000
            });
        }
    }
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await GetMasterDataAsync();
        //StateHasChanged();
    }
    void Cancel()
    {
        _dialogService.Close(false);
    }
    async Task Submit(SubmitCompletedShipmentDto arg)
    {
        var confirm = await _dialogService.Confirm($"{_movementLocalizer["Create.Picking"]}?", $"{_commonLocalizer["Create"]} {_movementLocalizer["Picking"]}", new ConfirmOptions()
        {
            OkButtonText = _commonLocalizer["Yes"],
            CancelButtonText = _commonLocalizer["No"],
            AutoFocusFirstElement = true,
        });

        if (confirm == null || confirm == false) return;

        try
        {
            var response = await _warehouseShipmentServices.CreatePickingAsync(arg);
            if (response.Item1)
            {
                _dialogService.Close(true);
            }
            else
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = _movementLocalizer["FailedToCreatePicking"],
                    Duration = 5000
                });
                _dialogService.Close(false);
            }
        }
        catch (Exception ex)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _movementLocalizer["ErrorOccurredWhileCreatingPicking", ex.Message],
                Duration = 5000
            });
        }
    }
}
