using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using static System.Runtime.InteropServices.JavaScript.JSType;
using SupplierEntity = FBT.ShareModels.Entities.Supplier;

namespace WebUIFinal.Pages.Supplier
{
    public partial class DetailSupplier
    {
        [Parameter] public string Title { get; set; }
        
        public string? Id { get; set; }

        private bool _visibleBtnSubmit = true;
        private bool isDisabled = false;
        private SupplierEntity _model = new SupplierEntity();
        private EnumStatus selectStatus;

        //List<TenantAuth> _tenants = [];
        //TenantAuth _selectTenant;
        List<CompanyTenant> _tenants = [];
        CompanyTenant _selectTenant;

        bool _disibleBtnDelete = true;

        protected override async Task OnInitializedAsync()
        {
            selectStatus = EnumStatus.Activated;

            if (Title.Contains(_localizerCommon["Detail.Create"])) _visibleBtnSubmit = false;

            await GetTenantsAsync();
            await RefreshDataAsync();
            await base.OnInitializedAsync();
        }
        async Task RefreshDataAsync()
        {
            try
            {
                if (Title.Contains(_localizerCommon["Detail.Edit"]))
                {
                    //var res = await _localStorage.GetItemAsync<SupplierTenantDTO>("SupplierDetailTransfer");
                    var res = _MasterTransferToDetails.SupplierInfo ?? null;
                    if (res == null)
                    {
                        NotificationHelper.ShowNotification(_notificationService
                                , NotificationSeverity.Warning
                                , _localizerNotification["Warning"], _localizerCommon["Detail model is null"]);
                        return;
                    }

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<SupplierEntity, SupplierTenantDTO>();
                    });
                    var mapper = config.CreateMapper();

                    _model = mapper.Map<SupplierTenantDTO>(res);

                    //_model = ConvertData(res);

                    _selectTenant = _tenants.FirstOrDefault(x => x.AuthPTenantId == _model.CompanyId);

                    var checkIsUse = await _productServices.GetBySupplierAsync(_model.Id);

                    if (!checkIsUse.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(checkIsUse.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                           , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                           , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    if (checkIsUse.Data != null) _disibleBtnDelete = true;
                    else _disibleBtnDelete = false;
                }
                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService
                         , NotificationSeverity.Error
                         , _localizerNotification["Error"], $"{ex.Message}|{ex.InnerException}");
                return;
            }
        }
        private async Task GetTenantsAsync()
        {
            try
            {
                var res = await _companiesServices.GetAllAsync();

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _tenants = res.Data;

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService
                       , NotificationSeverity.Error
                       , _localizerNotification["Error"], $"{ex.Message}|{ex.InnerException}");
                return;
            }
        }
        async Task Submit(SupplierEntity arg)
        {
            // Hiển thị hộp thoại xác nhận
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}: {arg.SupplierName}?", _localizerCommon["Save"], new ConfirmOptions()
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm != true) return;

            // Cập nhật trạng thái
            if (selectStatus == null)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = _localizerCommon["Invalid.Status"],
                    Duration = 5000
                });
                return;
            }

            arg.Status = selectStatus;
            arg.CompanyId = _selectTenant.AuthPTenantId;
            arg.DataKey = _selectTenant.DataKey;

            // Kiểm tra chế độ (tạo mới hoặc chỉnh sửa)
            bool isCreating = Title.Contains(_localizerCommon["Detail.Create"]);
            var res = isCreating ? await _suppliersServices.InsertAsync(arg) : await _suppliersServices.UpdateAsync(arg);

            // Xử lý thông báo dựa trên kết quả
            if (!res.Succeeded)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
                   , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                   , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                return;
            }

            NotificationHelper.ShowNotification(_notificationService
                    , NotificationSeverity.Success
                    , _localizerNotification["Success"], _localizerNotification["Success"]);
        }
        async Task DeleteItemAsync()
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}?", $"{_localizer["Supplier"]}: {_model.SupplierName}", new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                // Tạo thực thể Supplier từ model
                var supplierToDelete = new SupplierEntity
                {
                    Id = _model.Id // Sử dụng Id làm khóa chính để xóa
                };

                var res = await _suppliersServices.DeleteAsync(supplierToDelete); // Gọi phương thức xóa với thực thể Supplier

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService
                        , NotificationSeverity.Success
                        , _localizerNotification["Success"], _localizerNotification["Success"]);
            }
            catch (Exception ex)
            {

                NotificationHelper.ShowNotification(_notificationService
                        , NotificationSeverity.Error
                        , _localizerNotification["Error"], $"{ex.Message}|{ex.InnerException}");
            }
        }
        private SupplierEntity ConvertData(SupplierTenantDTO s) =>
                new SupplierEntity { SupplierId = s.SupplierId, SupplierName = s.SupplierName, CompanyId = s.CompanyId, Id = s.Id };
    }
}
