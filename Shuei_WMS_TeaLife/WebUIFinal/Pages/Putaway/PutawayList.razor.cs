using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QRCoder.Core;
using System;
using System.Text.Json;
using WebUIFinal.TemplateHtmlPrintLabel;
using PutAwayModel = FBT.ShareModels.WMS.WarehousePutAway;

using PutAwayLineModel = FBT.ShareModels.WMS.WarehousePutAwayLine;
using Application.DTOs;


namespace WebUIFinal.Pages.Putaway
{
    public partial class PutawayList
    {
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        DateTime? value;
        List<PutAwayModel> _dataGrid = new();
        RadzenDataGrid<PutAwayModel>? _putAwayGrid;
        private List<PutAwayModel> _filteredModel = new List<PutAwayModel>(); // Added this line
        public bool IsDeleted { get; set; }
        bool allowRowSelectOnRowClick = true;
        IEnumerable<PutAwayModel> PutAway = [];
        IList<PutAwayModel> selectedPutAway = [];

        private string inputText = string.Empty;
        private string qrCodeBase64 = string.Empty;
        List<LocationDisplayDto> locations = new();
        string _id = string.Empty;
        WarehousePutAwayDto putAwayDto = new WarehousePutAwayDto();
        bool _visibaleProgressBar = false;
        private IEnumerable<PutAwayModel> childTableData = Enumerable.Empty<PutAwayModel>();

        bool isFirstLoading=true;
        protected override async Task OnInitializedAsync ()
        {
          
          await base.OnInitializedAsync();

        _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            await RefreshDataAsync();
            await GetLocationssAsync();
           

            _filteredPutAwayItems = new List<PutAwayModel>(_dataGrid);
        }
        private WarehousePutAwayDto CreateWarehousePutAwayDto(PutAwayModel _) => new()
        {
            Id = _.Id,
            ReceiptNo = _.ReceiptNo,
            TenantId = _.TenantId,
            PutAwayNo = _.PutAwayNo,
            Location = _.Location,
            Status = _.Status,
           
        };
        private WarehousePutAwayLineDto CreateWarehousePutAwayLineDto(PutAwayLineModel r) => new()
        {
            Id = r.Id,
            ProductCode = r.ProductCode,
            UnitId = r.UnitId,
            JournalQty = r.JournalQty,
            TransQty = r.TransQty,
            Bin = r.Bin,
            LotNo = r.LotNo
        };
        private async Task CompleteSelectedPutawaysAsync()
        {
            var confirm = await _dialogService.Confirm($"{_localizer["Are you sure you want to complete putaway for all selected products"]}?", _localizer["CompletedPutaway"], new ConfirmOptions()
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            try
            {
                foreach (var putAway in selectedPutAway)
                {
                    var putAwayNo = putAway.PutAwayNo;
                    
                    if (string.IsNullOrEmpty(putAwayNo))
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Warning,
                            Summary = _localizerCommon["Warning"],
                            Detail = "No PutAwayNo selected.",
                            Duration = 5000
                        });
                        continue;
                    }

                    // Log the request data for debugging
                    Console.WriteLine($"Sending request to adjust putaway with PutAwayNo: {putAwayNo}");

                    var response = await _warehousePutAwayServices.UpdateWarehousePutAwaysStatus(putAway);

                    if (response.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = _localizerCommon["Success"],
                            Detail = _localizer["Putaway { putAwayNo } status updated to Completed."].Value.Replace("{ putAwayNo }", putAwayNo),
                            Duration = 5000
                        });
                    }
                    else
                    {
                        // Log the error response for debugging
                        Console.WriteLine($"Error response: {string.Join(", ", response.Messages)}");
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = _localizerCommon["Error"],
                            Detail = _localizer[response.Messages.FirstOrDefault()],
                            Duration = 5000
                        });
                    }
                }

                // Refresh the data grid after completing the putaways
               await RefreshDataAsync();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }

        private async Task GetLocationssAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
        }

        // void NavigateDetailPage(string PutAwayNo) => _navigation.NavigateTo($"/putawaydetails/{"Detail.View"}|"+PutAwayNo);
        void EditItemAsync(string PutAwayNo) => _navigation.NavigateTo($"/putawaydetails/|" + PutAwayNo);
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

        private async Task PrintSelectedLabels()
        {
            if (selectedPutAway == null || !selectedPutAway.Any())
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = "Shelvingdetails",
                    Detail = "Please select at least one item to print labels.",
                    Duration = 4000
                });
                return;
            }
            _visibaleProgressBar = true;

            List<LabelInfoDto> labelsToPrint = new List<LabelInfoDto>();
            List<WarehousePutAwayLineDto> _dataMaster = new List<WarehousePutAwayLineDto>();

            foreach (var item in selectedPutAway)
            {
                var putawayPrint = await _warehousePutAwayServices.GetPutAwayAsync(item.PutAwayNo);
                if (putawayPrint.Succeeded)
                {
                    int index = 0;
                    foreach (var line in putawayPrint.Data.WarehousePutAwayLines)
                    {
                        _dataMaster.Add(line);
                    }
                }
            }

            _visibaleProgressBar = false;

            // Lưu labelsToPrint vào LocalStorage
            //await _localStorage.SetItemAsync("labelDataTransfer", labelsToPrint);
            await _localStorage.SetItemAsync("selectInputLine", _dataMaster);

            // Điều hướng mà không cần truyền dữ liệu qua URL
            //await JSRuntime.InvokeVoidAsync("openTab", "/printlabelproduct");
            await _jsRuntime.InvokeVoidAsync("openTab", "/PrintProductLabel");
        }
        async Task DeleteItemAsync(PutAwayModel putAway)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete put away: {putAway.PutAwayNo}?", "Delete put away", new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _warehousePutAwayServices.DeleteAsync(putAway);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                         Summary = _localizerCommon["Success"],
                        Detail = res.Messages.ToString(),
                        Duration = 5000
                    });

                    StateHasChanged();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = res.Messages.ToString(),
                        Duration = 5000
                    });
                }

                await RefreshDataAsync();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });

                return;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _searchModel.Status = EnumPutAwayStatus.PutAway;
                var res = await _warehousePutAwayServices.GetAllAsync();

                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = res.Messages.ToString(),
                    });
                    return;
                }
                _dataGrid = null;
                _dataGrid = new List<PutAwayModel>();
                _dataGrid = res.Data.ToList();
                // Join the location data with the PutAwayModel items
                _filteredPutAwayItems = _dataGrid.Where(_ => _.Status == _searchModel?.Status).ToList();

                StateHasChanged();
            }
        }
        async Task RefreshDataAsync()
        {
            try
            {
                if (isFirstLoading)
                {
                    isFirstLoading = false;
                    return;
                }

                var res = await _warehousePutAwayServices.GetAllAsync();

                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = res.Messages.ToString(),
                    });
                    return;
                }
                _dataGrid = null;
                _dataGrid = new List<PutAwayModel>();
                _dataGrid = res.Data;
                // Join the location data with the PutAwayModel items

                _filteredPutAwayItems = _dataGrid;

                StateHasChanged();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }
        }
        private string GetLocalizedStatus(EnumPutAwayStatus status)
        {
            return status switch
            {
                EnumPutAwayStatus.PutAway => _localizer["PutAway"],
                EnumPutAwayStatus.Completed => _localizer["Completed"],
                _ => status.ToString(),
            };
        }
        private List<EnumDisplay<EnumPutAwayStatus>> GetDisplayPutAwayStatus()
        {
            return Enum.GetValues(typeof(EnumPutAwayStatus)).Cast<EnumPutAwayStatus>().Select(_ => new EnumDisplay<EnumPutAwayStatus>
            {
                Value = _,
                DisplayValue = GetValuePutAwayStatus(_)
            }).ToList();
        }

        private string GetValuePutAwayStatus(EnumPutAwayStatus PutAwayStatus) => PutAwayStatus switch
        {

            EnumPutAwayStatus.PutAway => _localizer["PutAway"],
            EnumPutAwayStatus.Completed => _localizer["Completed"],
            _ => throw new ArgumentException("Invalid value for PutAwayStatus", nameof(PutAwayStatus))
        };

    }
}
