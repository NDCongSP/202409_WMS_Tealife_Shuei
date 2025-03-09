using Application.DTOs.Request.StockTake;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using static Application.Extentions.ApiRoutes;

namespace WebUIFinal.Pages.StockTake
{
    public partial class PopupCreateRecording
    {
        [Parameter] public string Location { get; set; }
        [Parameter] public InventStockTakeDto StockTakeDto { get; set; }
        [Parameter] public List<UserDto> Users { get; set; }
        private StockTakeRecordingModel model = new();
        public class StockTakeRecordingModel
        {
            public string Location { get; set; }
            public string PersonInCharge { get; set; }
            public string Remarks { get; set; }
        }
        protected override async Task OnInitializedAsync()
        {
            var data = await _userToTenantServices.GetUsersAsync();
            if (data.Count > 0)
            {
                Users.AddRange(data);
                var currentUser = await _inventStockTakeServices.GetCurrentUserAsync();
                if (currentUser.Succeeded)
                {
                    model.PersonInCharge = currentUser.Data;
                }
            }
        }
        private void Cancel()
        {
            dialogService.Close(false);
        }
        private async void Start()
        {
            StockTakeDto.PersonInCharge = model.PersonInCharge;
            StockTakeDto.Description = model.Remarks;
            var result = await _inventStockTakeRecordingServices.CreateStockTakeRecordingAsync(new InventStockTakeWithDetailsDTO() { InventStockTakeDto = StockTakeDto, InventStockTakeLineDto = StockTakeDto.InventStockTakeLines });
            if (result.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, _localizerCommon["Success"], "");
                dialogService.Close(false);
            }
            else
            {
                ShowNotification(NotificationSeverity.Error,
                 _localizerCommon["Error"],
                 _localizer[result.Messages.FirstOrDefault() ?? "DefaultMessage"]);
            }
        }
        private void ShowNotification(NotificationSeverity severity, string summary, string detail, int duration = 5000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }
    }
}
