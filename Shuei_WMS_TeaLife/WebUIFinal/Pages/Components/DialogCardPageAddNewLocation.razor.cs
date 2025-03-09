using Microsoft.AspNetCore.Components;
using WebUIFinal.TemplateHtmlPrintLabel;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using Application.DTOs.Response;
using Newtonsoft.Json;
using RestEase;

namespace WebUIFinal.Pages.Components
{
    public partial class DialogCardPageAddNewLocation
    {
        [Parameter] public string Title { get; set; } = string.Empty;

        [Parameter] public Location _model { get; set; } = new Location();

        EnumStatus _selectStatus;
        bool _visibleBtnSubmit = true, _disable = false;
        string _id = string.Empty;

        List<Bin> _dataGrid = [];
        IList<Bin> _selectedDataBinList = [];
        RadzenDataGrid<Bin>? _profileGrid;
        bool allowRowSelectOnRowClick = true;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 50, 100 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        List<Bin> _listRemoveBin = new List<Bin>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizer["DisplayPage"] + " {0} " + _localizer["Of"] + " {1} <b>(" + _localizer["Total"] + " {2} " + _localizer["Records"] + ")</b>";

            await RefreshDataAsync();

            StateHasChanged();
        }
        async Task RefreshDataAsync()
        {
            try
            {
                _selectStatus = EnumStatus.Activated;

                if (Title.Contains($"{_localizer["Detail.Create"]}"))
                {
                    _visibleBtnSubmit = false;
                }

                if (Title.Contains("|"))
                {
                    var arr = Title.Split('|');
                    Title = arr[0];
                    _id = arr[1];

                    var res = await _locationServices.GetByIdAsync(Guid.Parse(_id));
                    var resMessage = res.Messages.FirstOrDefault();
                    if (!res.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                           , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                           , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    _model = res.Data;

                    //get bin information
                    var resBin = await _binServices.GetByLocationId(_model.Id);
                    resMessage = res.Messages.FirstOrDefault();
                    if (!res.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(resBin.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                           , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                           , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }

                    _dataGrid = resBin.Data;

                    _selectStatus = _model.Status;
                }
                else
                {
                    _model.Id = Guid.NewGuid();
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
        async void Submit(Location arg)
        {
            try
            {
                var l = _localizer["Detail.Create"];

                if (Title.Contains(_localizer["Detail.Create"]))
                {
                    var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Create"]} {_localizer["Location"]}: {arg.LocationName}?", $"{_localizer["Detail.Create"]} {_localizer["Location"]}", new ConfirmOptions()
                    {
                        OkButtonText = _localizer["Yes"],
                        CancelButtonText = _localizer["No"],
                        AutoFocusFirstElement = true,
                    });

                    if (confirm == null || confirm == false) return;
                }
                else
                {
                    var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Update"]} {_localizer["Location"]}: {arg.LocationName}?", $"{_localizer["Update"]} {_localizer["Location"]}", new ConfirmOptions()
                    {
                        OkButtonText = _localizer["Yes"],
                        CancelButtonText = _localizer["No"],
                        AutoFocusFirstElement = true,
                    });

                    if (confirm == null || confirm == false) return;
                }

                _model.Status = _selectStatus;

                string resMess = string.Empty;
                if (Title.Contains(_localizer["Detail.Create"]))
                {
                    //_model.Id = Guid.NewGuid();
                    var response = await _locationServices.InsertAsync(_model);
                    resMess = response.Messages.FirstOrDefault();
                    if (!response.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                           , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                           , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }
                }
                else if (Title.Contains(_localizer["Detail.Edit"]))
                {
                    var response = await _locationServices.UpdateAsync(_model);
                    resMess = response.Messages.FirstOrDefault();

                    if (!response.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                           , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                           , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }
                }

                //BIN
                if (_listRemoveBin.Count > 0)
                {
                    var responseDeleteNin = await _binServices.DeleteRangeAsync(_listRemoveBin);

                    if (!responseDeleteNin.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(responseDeleteNin.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                           , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                           , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }
                }

                if (_dataGrid.Count > 0)
                {
                    foreach (var item in _dataGrid)
                    {
                        item.LocationCD = _model.LocationCD;
                        item.LocationName = _model.LocationName;
                    }

                    var responseBin = await _binServices.AddOrUpdateAsync(_dataGrid);

                    if (!responseBin.Succeeded)
                    {
                        var error = JsonConvert.DeserializeObject<ErrorResponse>(responseBin.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                        NotificationHelper.ShowNotification(_notificationService
                           , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                           , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                        return;
                    }
                }

                NotificationHelper.ShowNotification(_notificationService
                   , NotificationSeverity.Success
                   , _localizerNotification["Success"], _localizerNotification["Success"]);

                _dialogService.Close("Success");
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);

                return;
            }
        }
        async Task PrintLable()
        {
            try
            {
                var dataPrint = await _binServices.GetLabelByLocationIdAsync(_model.Id);

                foreach (var item in dataPrint)
                {
                    item.Title1 = $"{_localizer["Location.Name"]}:";
                    item.Title2 = $"{_localizer["Bin.Code"]}:";
                }

                // Lưu labelsToPrint vào LocalStorage
                await _localStorage.SetItemAsync("labelDataTransfer", dataPrint);

                // Gọi phương thức JavaScript để mở trang trong tab mới
                await _jsRuntime.InvokeVoidAsync("openTab", "/printLocationLabel");
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }
        async Task AddBin()
        {
            try
            {
                BinDto binInfor = new BinDto()
                {
                    LocationId = _model.Id
                };

                var res = await _dialogService.OpenAsync<DialogCardPageAddNewBin>($"{_localizer["Detail.Create"]} {_localizer["Bin"]}",
                        new Dictionary<string, object>() { { "_model", binInfor }, { "VisibleBtnSubmit", true } },
                        new DialogOptions()
                        {
                            Width = "800",
                            Height = "400",
                            Resizable = true,
                            Draggable = true,
                            CloseDialogOnOverlayClick = true
                        });


                if (res != null)
                {
                    var selectResult = (BinDto)res;

                    var returnModel = _dataGrid.FirstOrDefault(x => x.BinCode == selectResult.BinCode);

                    if (returnModel != null)
                    {
                        NotificationHelper.ShowNotification(_notificationService
                           , NotificationSeverity.Warning
                           , _localizerNotification["Warning"], _localizerNotification["The data exists."]);

                        return;
                    }

                    _dataGrid.Add(selectResult);
                    _dataGrid = _dataGrid.OrderBy(x => x.SortOrderNum).ThenBy(x => x.BinCode).ToList();

                    await _profileGrid.RefreshDataAsync();
                }
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);

                return;
            }
        }
        async Task DeleteItemLocationAsync(Location model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Delete"]}?", $"{_localizer["Location"]}: {model.LocationName}", new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                #region delete bin of location
                var responseBins = await _binServices.GetByLocationId(model.Id);
                if (!responseBins.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(responseBins.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }
                var responseDeleteBin = await _binServices.DeleteRangeAsync(responseBins.Data);
                if (!responseDeleteBin.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(responseDeleteBin.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }
                #endregion

                var res = await _locationServices.DeleteAsync(model);

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Success, _localizerNotification["Success"], _localizerNotification["Success"]);

                _navigation.NavigateTo("/LocationList");
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);

                return;
            }
        }
        async Task DeleteItemAsync(Bin model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Delete"]} {_localizer["Bin"]}: {model.BinCode}?", $"{_localizer["Delete"]} {_localizer["Bin"]}", new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                _dataGrid.Remove(model);
                _listRemoveBin.Add(model);

                await _profileGrid.RefreshDataAsync();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);

                return;
            }
        }
        async Task ViewItemAsync(Bin model)
        {
            try
            {
                var res = await _dialogService.OpenAsync<DialogCardPageAddNewBin>($"{_localizer["Detail.View"]} {_localizer["Bin"]}",
                   new Dictionary<string, object>() { { "_model", model }, { "VisibleBtnSubmit", false } },
                   new DialogOptions()
                   {
                       Width = "800",
                       Height = "400",
                       Resizable = true,
                       Draggable = true,
                       CloseDialogOnOverlayClick = true
                   });

                if (res == "Success")
                {
                    await RefreshDataAsync();
                }
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);

                return;
            }
        }
        async Task EditItemAsync(Bin model)
        {
            try
            {
                var modelEdit = new BinDto()
                {
                    Id = model.Id,
                    LocationId = model.LocationId,
                    LocationName = model.LocationName,
                    LocationCD = model.LocationCD,
                    BinCode = model.BinCode,
                    Remarks = model.Remarks,
                    Status = model.Status,
                    SortOrderNum = model.SortOrderNum,
                    CreateAt = model.CreateAt,
                    CreateOperatorId = model.CreateOperatorId,
                    UpdateAt = model.UpdateAt,
                    UpdateOperatorId = model.UpdateOperatorId,
                    IsDelete = false
                };
                var res = await _dialogService.OpenAsync<DialogCardPageAddNewBin>($"{_localizer["Detail.Edit"]} {_localizer["Bin"]}",
                   new Dictionary<string, object>() { { "_model", modelEdit }, { "VisibleBtnSubmit", true } },
                   new DialogOptions()
                   {
                       Width = "1000",
                       Height = "400",
                       Resizable = true,
                       Draggable = true,
                       CloseDialogOnOverlayClick = true
                   });

                if (res != null)
                {
                    var selectResult = (BinDto)res;

                    model.Id = selectResult.Id;
                    model.BinCode = selectResult.BinCode;
                    model.Remarks = selectResult.Remarks;
                    model.SortOrderNum = selectResult.SortOrderNum;

                    if (selectResult.IsDelete == true)
                    {
                        _dataGrid.Remove(model);
                        _listRemoveBin.Add(model);
                    }
                    _dataGrid = _dataGrid.OrderBy(x => x.SortOrderNum).ThenBy(x => x.BinCode).ToList();
                    await _profileGrid.RefreshDataAsync();
                }
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);

                return;
            }
        }
    }
}
