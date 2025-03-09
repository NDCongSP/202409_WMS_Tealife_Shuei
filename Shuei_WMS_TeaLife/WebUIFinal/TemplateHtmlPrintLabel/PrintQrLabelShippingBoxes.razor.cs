using Application.DTOs.Response.ShippingBoxs;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class PrintQrLabelShippingBoxes
    {
        [Parameter] public List<ShippingBoxLabelDto> LabelPrintModel { get; set; }
        List<ShippingBoxModel> _dataMaster = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _dataMaster = await _localStorage.GetItemAsync<List<ShippingBoxModel>>("selectedShippingBoxes");
            List<ShippingBoxLabelDto> labelsToPrint = new List<ShippingBoxLabelDto>();

            int index = 0;

            foreach (var item in _dataMaster)
            {
                string qrCodeContent = $"{item.BoxName}:{item.BoxType}";
                labelsToPrint.Add(new ShippingBoxLabelDto()
                {
                    QrValue = GlobalVariable.GenerateQRCode(qrCodeContent),
                    BoxName = item.BoxName,
                    BoxType = item.BoxType,
                });

                index += 1;
            }
            LabelPrintModel = labelsToPrint;

            if (LabelPrintModel == null)
            {
                Console.WriteLine("No label data found in LocalStorage.");
            }

            int totalLabel = LabelPrintModel.Count;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _jsRuntime.InvokeVoidAsync("loadPrintPageCSS");
                await Task.Delay(1000);
                _ = _jsRuntime.InvokeVoidAsync("printLabel");
            }
        }
    }
}

