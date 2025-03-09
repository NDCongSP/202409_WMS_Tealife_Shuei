using Microsoft.JSInterop;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class DeliveryNote
    {
        [Parameter] public List<Guid> _dataTransfer { get; set; } = new();
        [Parameter] public List<DeliveryNoteModel> DeliveryNoteData { get; set; } = new();
        [Parameter] public bool IsCallPrint { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (DeliveryNoteData.Count == 0)
                {
                    _dataTransfer = await _localStorage.GetItemAsync<List<Guid>>("DeliveryNoteTransfer");
                    if (_dataTransfer != null)
                    {
                        DeliveryNoteData = await _warehouseShipmentServices.GetDeliveryNotes(_dataTransfer);
                    }
                }
                StateHasChanged();
                if (IsCallPrint)
                {
                    _jsRuntime.InvokeVoidAsync("loadPrintPageCSS");
                    await Task.Delay(1000);
                    _ = _jsRuntime.InvokeVoidAsync("printLabel");
                    await _localStorage.RemoveItemAsync("DeliveryNoteTransfer");
                }
            }
            catch (Exception ex) { 
            
            }
        }
    }
}
