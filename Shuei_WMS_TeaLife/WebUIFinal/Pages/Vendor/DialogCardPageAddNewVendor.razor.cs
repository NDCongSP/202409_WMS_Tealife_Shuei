
using Microsoft.AspNetCore.Components;
using Radzen;
using WebUIFinal.Core;
using VendorEntity = FBT.ShareModels.Entities.Vendor;

namespace WebUIFinal.Pages.Vendor
{
    public partial class DialogCardPageAddNewVendor
    {
        [Parameter] public string Title { get; set; }
        public int? VendorId { get; set; }

        private bool isDisabled = false;
        private VendorEntity _model = new VendorEntity();
        private List<string> _status = new List<string>();
        private EnumStatus selectStatus;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (Title.Contains("Detail")) isDisabled = true;

            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (Int32.TryParse(sub[1], out int x))
                {
                    VendorId = x;
                }
            }

            #region Get vendor info
            if (VendorId.HasValue && VendorId > 0)
            {
                var resultVendor = await _vendorsServices.GetByIdAsync((int)VendorId);
                if (!resultVendor.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Get vendor error",
                        Detail = resultVendor.Messages.FirstOrDefault(),
                        Duration = 1000
                    });

                    return;
                }

                _model = resultVendor.Data;

                selectStatus = resultVendor.Data.Status;
            }
            #endregion

            StateHasChanged();
        }

        async void Submit(VendorEntity arg)
        {
            var confirm = await _dialogService.Confirm($"Do you want to Save: {arg.VendorName}?", "vendor", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Status = selectStatus;

            if (Title.Contains("Create"))//Add
            {
                var res = await _vendorsServices.InsertAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                         Summary = _localizerCommon["Success"],
                        Detail = "Sucessfully created vendor",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/vendorlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = "Failed to create vendor",
                        Duration = 5000
                    });
                }
            }

            if (Title.Contains("Edit"))//update
            {
                var res = await _vendorsServices.UpdateAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                         Summary = _localizerCommon["Success"],
                        Detail = "Sucessfully edited vendor",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/vendorlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = "Failed to edit vendor",
                        Duration = 5000
                    });
                }
            }
        }

        async Task DeleteItemAsync(VendorEntity vendor)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete vendor: {vendor.VendorName}?", "Delete vendor", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _vendorsServices.DeleteAsync(vendor);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                         Summary = _localizerCommon["Success"],
                        Detail = $"Delete vendor {vendor.VendorName} successfully.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/vendorlist", true);
                    StateHasChanged();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = $"Failed to delete vendor {vendor.VendorName}.",
                        Duration = 5000
                    });
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = $"Failed to delete vendor {vendor.VendorName}.",
                    Duration = 5000
                });

                return;
            }
        }
    }
}
