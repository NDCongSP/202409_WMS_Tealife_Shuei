

using Microsoft.AspNetCore.Components;
using Radzen;
using WebUIFinal.Core;

namespace WebUIFinal.Pages.Product
{
    public partial class DialogCardPageAddNewProductJanCode
    {
        [Parameter] public ProductJanCodeDto productJanCode { get; set; } = new ProductJanCodeDto();
        [Parameter] public bool VisibleBtnSubmit { get; set; } = true;

        private EnumStatus selectedStatus;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await RefreshDataAsync();

            StateHasChanged();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                selectedStatus = EnumStatus.Activated;

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizeNotification["Error"],$"{ex.Message}|{ex.InnerException}");
               
                return;
            }
        }

        async void Submit(ProductJanCode arg)
        {
            arg.Status = selectedStatus;

            if (productJanCode.Id == 0)
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Create"] + _localizer["Product.JanCode"] + $"{arg.JanCode}?", _localizer["Create"] + _localizer["Product.JanCode"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;
            }
            else
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Update"] + _localizer["Product.JanCode"] + $"{arg.JanCode}?", _localizer["Update"] + _localizer["Product.JanCode"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;
            }

            _dialogService.Close(productJanCode);
        }

        async Task DeleteItemAsync()
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Delete"]} {_localizer["Product.JanCode"]}: {productJanCode.JanCode}?", $"{_localizer["Delete"]} {_localizer["Product.JanCode"]}", new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                productJanCode.IsDelete = true;
                _dialogService.Close(productJanCode);
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizeNotification["Error"], $"{ex.Message}|{ex.InnerException}");

                return;
            }
        }
    }
}
