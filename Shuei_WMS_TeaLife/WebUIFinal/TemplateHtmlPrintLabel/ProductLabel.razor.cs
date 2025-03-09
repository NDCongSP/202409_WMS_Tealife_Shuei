using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class ProductLabel
    {
        [Parameter] public List<LabelInfoDto> LabelPrintModel { get; set; }

        

        List<ProductLabelPrint> _dataPrint = new List<ProductLabelPrint>();

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

                int totalLabel = LabelPrintModel.Count;
                int index = 1;


                foreach (var item in LabelPrintModel)
                {
                    if (index % 2 != 0)
                    {
                        List<LabelInfoDto> pp = new List<LabelInfoDto>();
                        pp.Add(item);

                        _dataPrint.Add(new ProductLabelPrint()
                        {
                            TotalLabel = totalLabel,
                            RowIndex = index,
                            DataPrint = pp,
                        });
                    }
                    index += 1;
                }

                index = 1;
                foreach (var item in LabelPrintModel)
                {
                    if (index % 2 == 0)
                    {
                        var existItem = _dataPrint.FirstOrDefault(x => x.RowIndex == index - 1);

                        if (existItem != null)
                        {
                            existItem.DataPrint.Add(item);
                        }
                    }
                    index += 1;
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
