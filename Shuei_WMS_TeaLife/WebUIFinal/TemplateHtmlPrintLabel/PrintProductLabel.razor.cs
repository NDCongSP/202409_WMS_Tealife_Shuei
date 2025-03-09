using Blazored.LocalStorage;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.Eventing.Reader;
using System.Text.Json;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class PrintProductLabel
    {
        [Parameter] public List<LabelInfoDto> LabelPrintModel { get; set; }

        //

        List<ProductLabelPrint> _dataPrint = new List<ProductLabelPrint>();
        List<WarehousePutAwayLineDto> _dataMaster = new List<WarehousePutAwayLineDto>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            try
            {
                _dataMaster = await _localStorage.GetItemAsync<List<WarehousePutAwayLineDto>>("selectInputLine");

                List<LabelInfoDto> labelsToPrint = new List<LabelInfoDto>();

                foreach (var item in _dataMaster)
                {
                    for (int i = 0; i < item.TransQty; i++)
                    {
                        // Generate QR code content
                        if (item.ProductJanCodes.Count == 0)
                        {
                            var lot = !string.IsNullOrEmpty(item.LotNo) ? item.LotNo : "N/A";
                            //string qrCodeContent = $"{item.ProductCode}:{"N/A"}:{lot}:{item.ExpirationDate:yyyy/MM/dd}:{item.ReceiptNo}:{Guid.NewGuid()}";
                            string qrCodeContent = $"{item.ProductCode}:{"N/A"}:{lot}:{item.ExpirationDate:yyyy/MM/dd}";
                            labelsToPrint.Add(new LabelInfoDto()
                            {
                                QrValue = GlobalVariable.GenerateQRCode(qrCodeContent),
                                Title1 = "商品コード:",
                                Content1 = item.ProductCode,
                                Title2 = "JANコード:",
                                Content2 = "N/A",
                                Title3 = "賞味期限:",
                                Content3 = item.ExpirationDate?.ToString("yyyy/MM/dd") ?? "N/A",
                                Title4 = "LOT:",
                                Content4 = lot,
                                QrValue2 = GlobalVariable.GenerateQRCode(item.ProductUrl)
                            });
                        }
                        else
                        {
                            item.ProductJanCodes.ForEach(t =>
                            {
                                var lot = !string.IsNullOrEmpty(item.LotNo) ? item.LotNo : "N/A";
                                //string qrCodeContent = $"{item.ProductCode}:{"N/A"}:{lot}:{item.ExpirationDate:yyyy/MM/dd}:{item.ReceiptNo}:{Guid.NewGuid()}";
                                string qrCodeContent = $"{item.ProductCode}:{t}:{lot}:{item.ExpirationDate:yyyy/MM/dd}";

                                labelsToPrint.Add(new LabelInfoDto()
                                {
                                    QrValue = GlobalVariable.GenerateQRCode(qrCodeContent),
                                    Title1 = "商品コード:",
                                    Content1 = item.ProductCode,
                                    Title2 = "JANコード:",
                                    Content2 = t,
                                    Title3 = "賞味期限:",
                                    Content3 = item.ExpirationDate?.ToString("yyyy/MM/dd") ?? "N/A",
                                    Title4 = "LOT:",
                                    Content4 = lot,
                                    QrValue2 = GlobalVariable.GenerateQRCode(item.ProductUrl)
                                });
                            });
                        }
                    }

                    #region Add them 1 trang trang de phan tach giua 2 product
                    if (item.TransQty % 2 != 0)
                    {
                        labelsToPrint.Add(new LabelInfoDto());
                        labelsToPrint.Add(new LabelInfoDto() { SplitLabel = true });
                        labelsToPrint.Add(new LabelInfoDto() { SplitLabel = true });
                    }
                    else
                    {
                        labelsToPrint.Add(new LabelInfoDto() { SplitLabel = true });
                        labelsToPrint.Add(new LabelInfoDto() { SplitLabel = true });
                    }
                    #endregion
                }
                LabelPrintModel = labelsToPrint;

                //LabelPrintModel = await _localStorage.GetItemAsync<List<LabelInfoDto>>("labelDataTransfer");

                if (LabelPrintModel == null)
                {
                    Console.WriteLine("No label data found in LocalStorage.");
                }

                #region Tao lit model de in 2 tem tren 1 trang, theo page size cua may in va kieu tem
                int totalLabel = LabelPrintModel.Count;
                int indexRow = 1;

                foreach (var item in LabelPrintModel)
                {
                    if (indexRow % 2 != 0)
                    {
                        List<LabelInfoDto> pp = new List<LabelInfoDto>();
                        pp.Add(item);

                        _dataPrint.Add(new ProductLabelPrint()
                        {
                            TotalLabel = totalLabel,
                            RowIndex = indexRow,
                            DataPrint = pp,
                        });
                    }
                    else
                    {
                        var existItem = _dataPrint.FirstOrDefault(x => x.RowIndex == indexRow - 1);

                        if (existItem != null)
                        {
                            existItem.RowIndex = indexRow;
                            existItem.DataPrint.Add(item);
                        }
                    }
                    indexRow += 1;
                }
                #endregion
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
                _jsRuntime.InvokeVoidAsync("loadPrintPageCSS");
                await Task.Delay(1000);
                _ = _jsRuntime.InvokeVoidAsync("printLabel");
            }
        }
    }
}

