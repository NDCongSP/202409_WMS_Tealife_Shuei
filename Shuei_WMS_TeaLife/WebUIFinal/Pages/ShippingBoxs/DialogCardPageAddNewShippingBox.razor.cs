using Application.DTOs.Response;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RestEase;
using ShippingBoxModel = FBT.ShareModels.WMS.ShippingBox;

namespace WebUIFinal.Pages.ShippingBoxs
{
    public partial class DialogCardPageAddNewShippingBox
    {
        [Parameter] public string Mode { get; set; }
        public string Title { get; set; }
        public Guid? ShippingBoxId { get; set; }

        private EnumStatus selectedStatus;
        private bool isDisabled = false;
        private string? imageBase64String;
        bool _showPagerSummary = true;
        bool allowRowSelectOnRowClick = true;
        bool _visibleBtnSubmit = true;
        private List<ShippingCarrierDTO> _shippingCarriers;

        ShippingBoxModel model = new ShippingBoxModel();
        List<TenantAuth> tenants = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();

                selectedStatus = EnumStatus.Activated;
                await GetShippingBoxDetail();
                await GetTenantsAsync();

                await GetMasterDataAsync();

            }
            catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                StateHasChanged();
            }
        }

        private async Task GetMasterDataAsync()
        {
            try
            {
                var shippingCarrierResult = await _shippingBoxServices.GetAllShippingCarrierAsync();
                _shippingCarriers = shippingCarrierResult.Data;

            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
            }
        }

        private async Task GetTenantsAsync()
        {
            var data = await _tenantsServices.GetAllAsync();
            tenants.AddRange(data.Data);
        }

        private async Task GetShippingBoxDetail()
        {
            if (Mode.StartsWith("Edit"))
            {
                Title = _localizer["EditShippingBox"];
                var sub = Mode.Split('|');
                Mode = sub[0];

                if (Guid.TryParse(sub[1], out Guid x))
                {
                    ShippingBoxId = x;
                }
                if (ShippingBoxId.HasValue && ShippingBoxId != Guid.Empty)
                {
                    var shippingBox = await _shippingBoxServices.GetByIdAsync((Guid)ShippingBoxId);
                    if (!shippingBox.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(shippingBox.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                        , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                        , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    model = shippingBox.Data;
                    selectedStatus = shippingBox.Data.Status;
                }
            }
            else
            {
                Title = _localizer["CreateShippingBox"];
            }
        }

        async void Submit(ShippingBoxModel arg)
        {
            if (!string.IsNullOrWhiteSpace(arg.ShippingCarrierCode))
            {
                var scs = await _shippingCarrierServices.GetAllAsync();
                var sc = scs.Data.FirstOrDefault(x => x.ShippingCarrierCode == arg.ShippingCarrierCode);
                if (sc != null)
                {
                    arg.ShippingCarrierId = sc.Id;
                }
            }
            if (Mode.StartsWith("Create"))
            {
                var confirm = await _dialogService.Confirm(_localizer["DoYouWantToCreateANewShippingBox"], _localizer["CreateShippingBox"], new ConfirmOptions()
                {
                    OkButtonText = _CLoc["Yes"],
                    CancelButtonText = _CLoc["No"],
                    AutoFocusFirstElement = true,
                });
                if (confirm == null || confirm == false) return;

                model.Status = selectedStatus;

                var response = await _shippingBoxServices.InsertAsync(arg);

                if (!response.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
            }

            if (Mode.Contains("Edit"))
            {
                var confirm = await _dialogService.Confirm(_localizer["DoYouWantToUpdateShippingBox"], _localizer["UpdateShippingBox"], new ConfirmOptions()
                {
                    OkButtonText = _CLoc["Yes"],
                    CancelButtonText = _CLoc["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                model.Status = selectedStatus;

                var response = await _shippingBoxServices.UpdateAsync(model);

                if (!response.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
            }
            _dialogService.Close(_CLoc["Success"]);
        }

        async Task DeleteItemAsync(ShippingBoxModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{model.BoxName}" + _localizer["AreYouSureYouWantToDeleteShippingBox"], _localizer["DeleteShippingBox"], new ConfirmOptions()
                {
                    OkButtonText = _CLoc["Yes"],
                    CancelButtonText = _CLoc["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _shippingBoxServices.DeleteAsync(model);

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
                _navigation.NavigateTo("/shippingboxlist");
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
            }
        }
    }
}
