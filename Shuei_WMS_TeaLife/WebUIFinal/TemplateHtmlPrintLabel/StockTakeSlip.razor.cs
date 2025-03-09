using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Blazored.LocalStorage;
using Application.DTOs.Transfer;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class StockTakeSlip
    {
        [Parameter] public List<StockTakeSlipInfos> _dataTransfer { get; set; }
        

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
                _dataTransfer = await _localStorage.GetItemAsync<List<StockTakeSlipInfos>>("StockTakeSlipInfosTransfer");
                if (_dataTransfer == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizerCommon["Detail model is null"],
                        Duration = 5000
                    });
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
        private List<EnumDisplay<EnumInventStockTakeStatus>> GetDisplayStatus()
        {
            return Enum.GetValues(typeof(EnumInventStockTakeStatus)).Cast<EnumInventStockTakeStatus>().Select(_ => new EnumDisplay<EnumInventStockTakeStatus>
            {
                Value = _,
                DisplayValue = GetValueLocalizedStatus(_)
            }).ToList();
        }
        private string GetValueLocalizedStatus(EnumInventStockTakeStatus enumStatus)
        {
            return _localizerEnum[enumStatus.ToString()];
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Task.Delay(1000);
                _ = _jsRuntime.InvokeVoidAsync("printLabel");
            }
        }
    }
}
