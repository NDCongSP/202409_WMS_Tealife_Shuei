using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class PackingSlip
    {
        [Parameter]
        public string PdfBase64 { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                PdfBase64 = await _localStorage.GetItemAsync<string>("PackingSlip");
            }
            catch (Exception ex)
            {

            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                //await Task.Delay(3000);
                //_ = _jsRuntime.InvokeVoidAsync("printLabel");
            }
        }
    }
}
