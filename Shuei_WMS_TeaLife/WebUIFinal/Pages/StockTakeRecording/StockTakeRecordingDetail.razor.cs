using Application.DTOs.Transfer;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.JSInterop;
using System.Reflection;

namespace WebUIFinal.Pages.StockTakeRecording
{
    public partial class StockTakeRecordingDetail
    {
        [Parameter] public string Title { get; set; }
        private InventStockTakeRecordingDTO _model { get; set; }
        private RadzenDataGrid<InventStockTakeRecordingLineDtos> _profileGrid;
        private List<InventStockTakeRecordingLineDtos> _dataGrid = null;

        private Guid _id = Guid.Empty;
        private string StockTakeNo = string.Empty;
        private IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        private bool _showPagerSummary = true;
        private string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        private bool _allowRowSelectOnRowClick = false;
        private bool _manualEntry = false;
        private bool _visibled;
        private bool _disableEdit = false;

        private InventStockTakeRecordingLineDtos selectedLine = new InventStockTakeRecordingLineDtos() { ProductCode = "default" };
        private InventStockTakeRecordingDTO dataBefore = new InventStockTakeRecordingDTO();
        private List<InventStockTakeRecordingLineDtos> detailsToUpdate = new List<InventStockTakeRecordingLineDtos>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";
            await RefreshDataAsync();
            CheckPermission();
        }
        private async Task RefreshDataAsync()
        {
            try
            {
                _id = Guid.Parse(Title[1..]);
                var result = await _inventStockTakeRecordingServices.GetByIdDTOAsync(_id);
                if (!result.Succeeded)
                {
                    NotifyError(_localizer["Detail model is null"]);
                    _navigation.NavigateTo("/stocktakerecording");
                    return;
                }
                _model = result.Data;
                dataBefore = CloneData(result.Data);
                StockTakeNo = _model.StockTakeNo;
                if (_model.Status != EnumInvenTransferStatus.InProcess) _disableEdit = true;
                // line
                _dataGrid = new List<InventStockTakeRecordingLineDtos>();
                _dataGrid = _model.InventStockTakeRecordingLineDtos;
                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                NotifyError(_localizer[ex.Message]);
                return;
            }
        }
        private InventStockTakeRecordingDTO CloneData(InventStockTakeRecordingDTO data)
        {
            return new InventStockTakeRecordingDTO
            {
                Id = data.Id,
                StockTakeNo = data.StockTakeNo,
                RecordNo = data.RecordNo,
                Location = data.Location,
                PersonInCharge = data.PersonInCharge,
                TransactionDate = data.TransactionDate,
                Status = data.Status,
                Remarks = data.Remarks,
                HHTStatus = data.HHTStatus,
                HHTInfo = data.HHTInfo,
                TenantId = data.TenantId,
                TenantFullName = data.TenantFullName,
                LocationName = data.LocationName,
                PersonName = data.PersonName,
                InventStockTakeRecordingLineDtos = data.InventStockTakeRecordingLineDtos.Select(line => new InventStockTakeRecordingLineDtos
                {
                    Id = line.Id,
                    StockTakeRecordingId = line.StockTakeRecordingId,
                    ProductCode = line.ProductCode,
                    ProductName = line.ProductName,
                    Unit = line.Unit,
                    LotNo = line.LotNo,
                    Bin = line.Bin,
                    ExpectedQty = line.ExpectedQty,
                    ActualQty = line.ActualQty,
                    Remaining = line.Remaining
                }).ToList()
            };
        }
        private async Task CompleteAsync()
        {
            try
            {
                if (detailsToUpdate.Any())
                {
                    NotifyWarning(_localizer["MustKeep"]);
                    return;
                }
                string messengerconfirm = $"{_localizerCommon["Complete"]}: {StockTakeNo}";
                int remainingCount = _dataGrid.Count(pickDetail => pickDetail.Remaining != 0);
                if (remainingCount > 0) messengerconfirm += _localizer["InsufficientQuantity"];
                if (!await ConfirmAction(messengerconfirm + "?", _localizerCommon["Complete"])) return;
                var res = await _inventStockTakeRecordingServices.CompleteStockTakeRecordingAsync(_id);
                 if (res.Succeeded)
                {
                    NotifySuccess(_localizer["completed"] + " " + StockTakeNo);
                    await Task.Delay(500);
                    _navigation.NavigateTo($"/stocktakedetail/{"~" + StockTakeNo}");
                    StateHasChanged();
                }
                else NotifyError(_localizer[res.Messages.First()]);
            }
            catch (Exception ex) { NotifyError(StockTakeNo + " " + _localizer["completion failed"] + $" (Exception: {ex.Message})"); }
        }
        private async Task DeleteAsync()
        {
            try
            {
                var confirm = await _dialogService.Confirm(
                                    $"{_localizer["StockTakeNo"]}: {StockTakeNo}?<br/>{_localizer["DeleteNoti"]}<br/>{_localizer["DeleteWarning"]}",
                                    _localizerCommon["Delete"],
                                    new ConfirmOptions()
                                    {
                                        OkButtonText = _localizerCommon["Yes"],
                                        CancelButtonText = _localizerCommon["No"],
                                        AutoFocusFirstElement = true
                                    });

                if (confirm == null || confirm == false) return;

                var res = await _inventStockTakeRecordingServices.DeleteStockTakeRecordingAsync(_id);

                if (res.Succeeded)
                {
                    NotifySuccess("");
                    await Task.Delay(1000);
                    _navigation.NavigateTo($"/stocktakedetail/{"~" + StockTakeNo}");
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                NotifyError(_localizer["Failed to delete"] + StockTakeNo + _localizer[ex.Message]);
            }
        }
        private void Cancel()
        {
            dataBefore = new InventStockTakeRecordingDTO();
            detailsToUpdate.Clear();
            _navigation.NavigateTo("/stocktakerecording");
        }
        private async Task KeepAsync()
        {
            if (!CheckSelectedProduct())
            {
                var changedDetails = detailsToUpdate.Where(updatedDetail =>
                {
                    var originalDetail = dataBefore.InventStockTakeRecordingLineDtos
                        .FirstOrDefault(beforeDetail => beforeDetail.ProductCode == updatedDetail.ProductCode);
                    return originalDetail == null || !AreDetailsEqual(originalDetail, updatedDetail);
                }).ToList();

                // Check the count of changedDetails
                if (changedDetails.Count <= 0)
                {
                    NotifySuccess(_localizer["No changes detected"]);
                    return;
                }

                // Call UpdateWarehousePickingLinesAsync and check the result
                var updateResult = await _inventStockTakeRecordingServices.UpdateStockTakeRecordingLinesAsync(changedDetails);

                if (updateResult.Succeeded)
                {
                    detailsToUpdate.Clear();
                    await RefreshDataAsync();
                    NotifySuccess("");
                }
                else
                {
                    NotifyError(_localizer[updateResult.Messages.FirstOrDefault()] ?? _localizer["ErrorUpdate"]);
                }
            }
        }
        private bool AreDetailsEqual(InventStockTakeRecordingLineDtos detail1, InventStockTakeRecordingLineDtos detail2)
        {
            return detail1.ActualQty == detail2.ActualQty;
        }
        private async Task EditRow(InventStockTakeRecordingLineDtos detail)
        {
            if (!CheckSelectedProduct())
            {
                await _profileGrid.EditRow(detail);
                selectedLine = detail;
            }
        }
        private async Task SaveRow(InventStockTakeRecordingLineDtos detail)
        {
            if (string.IsNullOrEmpty(detail.ProductCode))
            {
                NotifyError(_localizer["ProductIsRequired"]);
                return;
            }
            var existingDetail = detailsToUpdate.FirstOrDefault(d => d.Id == detail.Id);

            if (existingDetail != null)
            {
                int index = detailsToUpdate.IndexOf(existingDetail);
                detailsToUpdate[index] = detail;
            }
            else
            {
                detailsToUpdate.Add(detail);
            }
            selectedLine = null;
            detail.Remaining = detail.ExpectedQty - detail.ActualQty;
            await _profileGrid.UpdateRow(detail);
        }
        private void CancelEdit(InventStockTakeRecordingLineDtos detail)
        {
            selectedLine = null;
            _profileGrid.CancelEditRow(detail);
        }
        private List<EnumDisplay<EnumInvenTransferStatus>> GetDisplayStatus()
        {
            return Enum.GetValues(typeof(EnumInvenTransferStatus)).Cast<EnumInvenTransferStatus>().Select(_ => new EnumDisplay<EnumInvenTransferStatus>
            {
                Value = _,
                DisplayValue = GetValueLocalizedStatus(_)
            }).ToList();
        }
        private string GetValueLocalizedStatus(EnumInvenTransferStatus enumStatus)
        {
            return _localizerEnum[enumStatus.ToString()];
        }
        private bool CheckSelectedProduct()
        {
            if (selectedLine != null && selectedLine.ProductCode != "default")
            {
                NotifyWarning(_localizer["MustComplete"]);
                return true;
            }
            return false;
        }
        async Task PrintStockTakeSlip()
        {
            var inventStockTakeRecordingDetailsDTOs = new List<StockTakeRecordingSlipInfos>
            {
                new StockTakeRecordingSlipInfos (_model, _dataGrid)
            };
            await _localStorage.SetItemAsync("StockTakeRecordingSlipInfosTransfer", inventStockTakeRecordingDetailsDTOs);
            await JSRuntime.InvokeVoidAsync("openTab", "/printstocktakerecordingslip");
        }
        private void CheckPermission()
        {
            if (GlobalVariable.AuthenticationStateTask.HasPermission("Edit") || GlobalVariable.AuthenticationStateTask.HasPermission("StocktakeEdit"))
            {
                _visibled = true;
            }
            else if (GlobalVariable.AuthenticationStateTask.HasPermission("View") || GlobalVariable.AuthenticationStateTask.HasPermission("StocktakeView"))
            {
                _visibled = false;
            }
            else
            {
                NotifyError("You do not have permission to access this functionality");
                _navigation.NavigateTo("/");
            }
        }
        // Cac ham thong bao
        private async Task<bool> ConfirmAction(string message, string title)
        {
            return await _dialogService.Confirm(title, message, new ConfirmOptions
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true
            }) ?? false;
        }
        private void NotifyError(string message)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _localizerCommon["Error"],
                Detail = message,
                Duration = 5000
            });
        }
        private void NotifySuccess(string message)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = _localizerCommon["Success"],
                Detail = message,
                Duration = 5000
            });
        }
        private void NotifyWarning(string message)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = _localizerCommon["Warning"],
                Detail = message,
                Duration = 10000
            });
        }
    }
}
