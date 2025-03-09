using Application.DTOs.Request.Account;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QRCoder.Core;
using WebUIFinal.TemplateHtmlPrintLabel;
using WebUIFinal.Pages.Account;
using System.Text.Json;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using Application.DTOs.Response;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Application.Extentions;

namespace WebUIFinal.Pages.Components
{
    public partial class DialogCardPageAddNewUser
    {
        [Parameter] public string Title { get; set; }

        string _id = string.Empty;
        CreateAccountRequestDTO _model = new CreateAccountRequestDTO();

        List<CreateRoleRequestDTO> _roles = new List<CreateRoleRequestDTO>();
        IList<string> _selectedRoles = [];

        List<string> _status = new List<string>();
        EnumStatus? _selectStatus;

        List<CompanyTenant> _tenantList = [];
        IList<CompanyTenant> _selectedTenantList = [];
        IList<string> _selectedTenant = [];

        RadzenDataGrid<TenantAuth> _profileGrid;
        bool allowRowSelectOnRowClick = true;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 50, 100 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        bool password = true;
        void TogglePassword()
        {
            password = !password;
        }
        bool _visible = true, _disable = false;

        List<UserToTenant> tenantCurrent = new List<UserToTenant>();//danh sach cac tenant duoc dang ky cho user
        List<UserToTenant> tenantNew = new List<UserToTenant>();//danh sach cac tenant duoc dang ky cho user

        private string inputText = string.Empty;
        private string qrCodeBase64 = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                //get tenant
                var resultTenant = await _companiesServices.GetAllAsync();
                if (resultTenant.Succeeded)
                    _tenantList = resultTenant.Data;

                var result = await _authenServices.GetRolesAsync();

                foreach (var role in result)
                {
                    _roles.Add(new CreateRoleRequestDTO() { Name = role.Name, Id = role.Id });
                }

                _selectStatus = EnumStatus.Activated;

                if (Title.Contains("|"))
                {
                    var sub = Title.Split('|');
                    Title = sub[0];
                    _id = sub[1];
                    _visible = sub[2] == "True" ? true : false;

                    if (!_visible) _disable = true;//disable fielfSet user information when page opening from userInfo menu

                    if (string.IsNullOrEmpty(_id))
                    {
                        var user = await _authenServices.UserGetByEmailAsync(GlobalVariable.UserAuthorizationInfo.EmailName);
                        if (user != null) { _id = user.Id; }
                    }

                    #region Get user info
                    var resultUser = await _authenServices.UserGetById(_id);
                    if (resultUser == null)
                    {
                        NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Warning, _localizerNotification["Warning"], _localizerNotification["Result user null."]);

                        return;
                    }

                    _model.Email = resultUser.Email;
                    _model.UserName = resultUser.UserName;
                    _model.FullName = resultUser.FullName;
                    _model.Status = resultUser.Status;

                    _selectStatus = _model.Status;

                    foreach (var role in resultUser.Roles)
                    {
                        _model.Roles.Add(new CreateRoleRequestDTO()
                        {
                            Id = role.Id,
                            Name = role.Name,
                        });

                        _selectedRoles.Add(role.Id);
                    }

                    var resultU2T = await _userToTenantServices.GetByUserIdAsync(resultUser.Id);

                    if (!resultU2T.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(resultU2T.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                            , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                            , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    tenantCurrent = resultU2T.Data;

                    foreach (var item in tenantCurrent)
                    {
                        var r = _tenantList.FirstOrDefault(x => x.AuthPTenantId == item.TenantId);
                        _selectedTenantList.Add(r);
                    }
                    #endregion
                }
                //Title = Title.Contains("View") ? $"{_localizer["Detail.View"]} User" : Title.Contains("Edit") ? $"{_localizer["Detail.Edit"]} User" : $"{_localizer["Detail.Create"]} User";

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }

        async void Submit(CreateAccountRequestDTO arg)
        {
            try
            {
                if (Title.Contains(_localizer["Detail.Create"]))//Add
                {
                    var confirm = await _dialogService.Confirm($"{_localizer["User"]}: {arg.UserName} {_localizer["Confirmation.Create"]}?", $"{_localizer["Create"]} {_localizer["User"]}", new ConfirmOptions()
                    {
                        OkButtonText = _localizer["Yes"],
                        CancelButtonText = _localizer["No"],
                        AutoFocusFirstElement = true,
                    });
                    if (confirm == null || confirm == false) return;
                }
                else
                {
                    var confirm = await _dialogService.Confirm($"{_localizer["User"]}: {arg.UserName} {_localizer["Confirmation.Update"]}?", $"{_localizer["UpdateUsetTitle"]}", new ConfirmOptions()
                    {
                        OkButtonText = _localizer["Yes"],
                        CancelButtonText = _localizer["No"],
                        AutoFocusFirstElement = true,
                    });
                    if (confirm == null || confirm == false) return;
                }


                //If edit user then clear roles for adding new roles
                if (!string.IsNullOrEmpty(_id))
                {
                    arg.Roles = null;
                    arg.Roles = new List<CreateRoleRequestDTO>();

                    //get danh sach tenant duoc dang ky cho user
                    var resU2T = await _userToTenantServices.GetByUserIdAsync(_id);
                }

                //lay dang sachs role moi
                foreach (var role in _selectedRoles)
                {
                    var r = _roles.FirstOrDefault(x => x.Id == role);
                    arg.Roles.Add(new CreateRoleRequestDTO() { Id = r.Id, Name = r.Name });
                }

                arg.Status = _selectStatus;
                arg.ConfirmPassword = arg.Password;

                tenantNew = new List<UserToTenant>();
                //lay danh sach tenant moi dc chon
                foreach (var item in _selectedTenantList)
                {
                    var r = _tenantList.FirstOrDefault(x => x.AuthPTenantId == item.AuthPTenantId);
                    tenantNew.Add(new UserToTenant()
                    {
                        Id = Guid.NewGuid(),
                        UserId = _id,
                        TenantId = item.AuthPTenantId
                    });
                }

                if (Title.Contains(_localizer["Detail.Create"]))//Add
                {
                    var res = await _authenServices.CreateAccountAsync(arg);

                    if (!res.Flag)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Message)?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    //var user = await _authenServices.UserGetByEmailAsync(arg.Email);

                    foreach (var item in tenantNew)
                    {
                        item.UserId = res.Message;
                    }

                    //var resU2TD = await _userToTenantServices.DeleteRangeAsync(tenantCurrent);
                    var resU2T = await _userToTenantServices.AddRangeAsync(tenantNew);
                    if (!resU2T.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(resU2T.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                            , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                            , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
                }
                else if (Title.Contains(_localizer["Detail.Edit"]))//update
                {
                    var userInfoUpdate = new UpdateUserInfoRequestDTO();
                    userInfoUpdate.Id = _id;
                    userInfoUpdate.UserName = arg.UserName;
                    userInfoUpdate.Email = arg.Email;
                    userInfoUpdate.FullName = arg.FullName;
                    userInfoUpdate.Status = _selectStatus;
                    userInfoUpdate.Roles = arg.Roles;

                    var res = await _authenServices.UpdateUserInfoAsync(userInfoUpdate);

                    if (!res.Flag)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Message)?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                            , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                            , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    //var resU2TD = await _userToTenantServices.DeleteRangeAsync(tenantCurrent);
                    var resU2T = await _userToTenantServices.AddRangeAsync(tenantNew);

                    if (!resU2T.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(resU2T.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                            , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                            , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);
                }
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }

        async void RefreshData()
        {
            try
            {
                //_ovenId = int.TryParse(OvenId, out int value) ? value : 0;

                //var res = await _ft01Client.GetAllAsync();

                //if (res == null)
                //    return;
                //_ft01 = res.Data.ToList();

                //_ovenInfo = JsonConvert.DeserializeObject<OvensInfo>(_ft01.FirstOrDefault().C001).FirstOrDefault(x => x.Id == _ovenId);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }

            StateHasChanged();
        }
        async Task PrintLable()
        {
            try
            {
                var dataPrint = await _authenServices.GetLabelByIdAsync(_id);

                // Lưu labelsToPrint vào LocalStorage
                await _localStorage.SetItemAsync("labelDataTransfer", dataPrint);

                // Gọi phương thức JavaScript để mở trang trong tab mới
                await _jsRuntime.InvokeVoidAsync("openTab", "/PrintUserLabel");
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }
        }

        async Task PrintLable1()
        {
            //var dataPrint = await _authenServices.GetReportBase64(_id);
            var response = await _packingListServices.GetPdfAsBase64Async("C:\\20240904_ShueiCongData\\sdsdsds.pdf");

            if (!response.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = response.Messages.FirstOrDefault(),
                    Duration = 5000
                });

                return;
            }

            var dataPrint = response.Data;

            //var res = await _dialogService.OpenAsync<ReportViewer>($"Print label for use",
            //      new Dictionary<string, object>() { { "_pdfBase64", dataPrint } },
            //      new DialogOptions()
            //      {
            //          Width = "1000px",
            //          Height = "1000px",
            //          Resizable = true,
            //          Draggable = true,
            //          ShowClose = false,
            //          CloseDialogOnOverlayClick = true
            //      });
            await _localStorage.SetItemAsync("PackingSlip", dataPrint);
            await _jsRuntime.InvokeVoidAsync("openTab", "/PackingSlip");
        }

        async Task DisableUser()
        {
            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Info,
                Summary = "Info",
                Detail = "Disable click",
                Duration = 1000
            });
        }

        void ShowTooltip(ElementReference elementReference, TooltipOptions options = null)
        {
            _tooltipService.Open(elementReference, "Full name", options);
        }

        // Method to generate QR code
        private void GenerateQRCode()
        {
            inputText = "NGUYEN DINH CONG|COng123@456";
            if (string.IsNullOrEmpty(inputText))
                return;

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
            }
        }

        // Method to print the QR code
        private async Task PrintQRCode()
        {
            await _jsRuntime.InvokeVoidAsync("printQRCode");
        }

        async Task DeleteItemAsync(CreateAccountRequestDTO model)
        {
            try
            {
                var d = new UpdateDeleteRequestDTO()
                {
                    Id = _id,
                    Name = model.UserName
                };

                var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Delete"]}?", $"{_localizer["User"]}: {d.Name}", new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _authenServices.DeleteUserAsync(d);

                if (!res.Flag)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Message)?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                        , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                        , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);

                _navigation.NavigateTo("/UserList");
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);

                return;
            }
        }
    }
}
