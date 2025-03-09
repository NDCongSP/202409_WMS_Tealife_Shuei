using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Blazored.LocalStorage;
using System.Text.Json;
using WebUIFinal.Core.Dto;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class PrintViewer
    {
        [Parameter] public List<LabelInfoDto> LabelPrintModel { get; set; }

        

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                LabelPrintModel = await _localStorage.GetItemAsync<List<LabelInfoDto>>("labelDataTransfer");

                if (LabelPrintModel == null)
                {
                    Console.WriteLine("No label data found in LocalStorage.");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing label data from LocalStorage: {ex.Message}");
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
