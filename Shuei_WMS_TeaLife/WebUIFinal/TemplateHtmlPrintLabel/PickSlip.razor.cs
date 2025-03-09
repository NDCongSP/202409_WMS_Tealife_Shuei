using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Blazored.LocalStorage;
using Application.DTOs.Transfer;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class PickSlip
    {
        [Parameter] public List<PickingSlipInfos> DataTransfer { get; set; } = new();
        [Parameter] public bool IsCallPrint { get; set; } = true;

        string pickNo = null;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                if(DataTransfer.Count == 0)
                    DataTransfer = await _localStorage.GetItemAsync<List<PickingSlipInfos>>("PickingSlipInfosTransfer");
                if (DataTransfer == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizerCommon["Detail model is null"],
                        Duration = 5000
                    });
                    _navigation.NavigateTo("/pickinglist");
                    return;
                }
                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }
        }

        private List<EnumDisplay<EnumInvenTransferStatus>> GetDisplayStatus()
        {
            return Enum.GetValues(typeof(EnumInvenTransferStatus)).Cast<EnumInvenTransferStatus>().Select(_ => new EnumDisplay<EnumInvenTransferStatus>
            {
                Value = _,
                DisplayValue = GetValueLocalizedStatus(_)
            }).ToList();
        }

        private string GetValueLocalizedStatus(EnumInvenTransferStatus enumStatus)
        {
            return _localizerEnum[enumStatus.ToString()];
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && IsCallPrint)
            {
                await Task.Delay(1000);
                _ = _jsRuntime.InvokeVoidAsync("printLabel");
                await _localStorage.RemoveItemAsync("PickingSlipInfosTransfer");
            }
        }
    }
}
