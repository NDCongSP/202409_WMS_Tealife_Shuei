using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using RestEase;

namespace WebUIFinal.Pages.Components
{
    public partial class DialogCardPageAddNewRole
    {
        [Parameter] public string Title { get; set; } = string.Empty;

        [Parameter] public CreateRoleRequestDTO _model { get; set; } = new CreateRoleRequestDTO();
        private GetRoleResponseDTO _roleInfo;

        List<PermissionsInRoleModel> _dataGrid = null;
        RadzenDataGrid<PermissionsInRoleModel> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        bool allowRowSelectOnRowClick = false;
        IList<PermissionsInRoleModel> _gridSelected = [];

        bool _visibleBtnSubmit = true;
        bool _disable = false;
        string _id = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizer["DisplayPage"] + " {0} " + _localizer["Of"] + " {1} <b>(" + _localizer["Total"] + " {2} " + _localizer["Records"] + ")</b>";

            await RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                if (!Title.Contains($"{_localizer["Detail.Create"]}")) _disable = true;

                #region Loading all permissions
                var p = await _permissionsServices.GetAllAsync();
                if (!p.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(p.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                        , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                        , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                // Cấu hình AutoMapper
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Permissions, PermissionsInRoleModel>();
                    //cfg.CreateMap<List<Permissions>, List<PermissionsInRoleModel>>();
                });
                var mapper = config.CreateMapper();

                _dataGrid = mapper.Map<List<PermissionsInRoleModel>>(p.Data);
                #endregion

                if (Title.Contains("|"))
                {
                    var arr = Title.Split('|');
                    Title = arr[0];
                    _id = arr[1];

                    var roleResult = await _authenServices.RoleGetById(_id);

                    if (roleResult == null)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(p.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                            , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                            , _localizerNotification[error?.Key], _localizerNotification["Could not load data."]);

                        return;
                    }

                    _model.Id = roleResult.Id;
                    _model.Name = roleResult.Name;
                    _model.Permissions = roleResult.Permissions;

                    foreach (var permission in _model.Permissions)
                    {
                        var existP = _dataGrid.FirstOrDefault(_ => _.Id == permission.Id);
                        if (existP != null) existP.IsSelected = true;
                    }
                }

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
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

        async void Submit(CreateRoleRequestDTO arg)
        {
            var response = new GeneralResponse();

            #region Get permission
            // Cấu hình AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Permissions, PermissionsInRoleModel>();
            });
            var mapper = config.CreateMapper();

            foreach (var item in _dataGrid)
            {
                var p = _model.Permissions.FirstOrDefault(_ => _.Id == item.Id);

                if (item.IsSelected)
                {
                    if (p is null)
                    {
                        _model.Permissions.Add(mapper.Map<PermissionsInRoleModel>(item));
                    }
                }
                else
                {
                    if (p is not null)
                        _model.Permissions.Remove(p);
                }
                   
            }
            #endregion

            if (Title.Contains(_localizer["Detail.Edit"]))
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Update"] + _localizer["Role"] + $": {arg.Name} ", _localizer["Update"] + " " + _localizer["Role"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                _model.Name = arg.Name;

                //response = await _authenServices.UpdateRoleAsync(new UpdateDeleteRequestDTO() { Id = _model.Id, Name = _model.Name });
                response = await _authenServices.UpdateRoleDTOAsync(_model);
            }
            else if (Title.Contains(_localizer["Detail.Create"]))
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Create"] + _localizer["Role"] + $": {arg.Name}?", _localizer["Create"] + " " + _localizer["Role"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                //_model.Id = Guid.NewGuid().ToString();
                _model.Name = arg.Name;

                response = await _authenServices.CreateRoleAsysnc(_model);
            }

            if (!response.Flag)
            {
                var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Message)?.Errors.FirstOrDefault();

                NotificationHelper.ShowNotification(_notificationService
                    , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                    , _localizerNotification[error?.Key], _localizerNotification["Could not load data."]);

                return;
            }

            NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);

            _dialogService.Close("Success");

        }

        async Task DeleteItemAsync(CreateRoleRequestDTO model)
        {
            try
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Delete"] + _localizer["Role"] + $": {model.Name}?", _localizer["Delete"] + " " + _localizer["Role"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _authenServices.DeleteRoleAsync(new UpdateDeleteRequestDTO() { Id = model.Id, Name = model.Name });

                if (!res.Flag)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Message)?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                        , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                        , _localizerNotification[error?.Key], _localizerNotification["Could not load data."]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);

                RefreshDataAsync();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);

                return;
            }
        }
    }
}
