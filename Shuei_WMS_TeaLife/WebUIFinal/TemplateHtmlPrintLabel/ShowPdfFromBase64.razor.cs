using Microsoft.AspNetCore.Components;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class ShowPdfFromBase64
    {
        [Parameter]
        public string PdfBase64 { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                PdfBase64 = await _localStorage.GetItemAsync<string>("base64PdfString");
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
