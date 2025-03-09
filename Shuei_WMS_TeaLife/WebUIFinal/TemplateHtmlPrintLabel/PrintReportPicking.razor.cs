using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class PrintReportPicking
    {
        [Parameter] public InventBundleDTO _warehouseBundle { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await RefreshDataAsync();
            if (!string.IsNullOrEmpty(_warehouseBundle.TransNo))
            {
                await JSRuntime.InvokeVoidAsync("JsBarcode", "#barcode", _warehouseBundle.TransNo, new
                {
                    format = "CODE128",
                    width = 2,
                    height = 100,
                    displayValue = true
                });
            }
        }

        async Task RefreshDataAsync()
        {
            try
            {
                _warehouseBundle = await _localStorage.GetItemAsync<InventBundleDTO>("WarehouseBundle");
                if (_warehouseBundle == null)
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
