
namespace WebUIFinal.Pages.Reporting
{
    public partial class ReportShipping
    {

        public List<ShipmentReportDTO> ShippingReports { get; set; } = new List<ShipmentReportDTO>();
        public RadzenDataGrid<ShipmentReportDTO> _shipmentReportGrid;

        public RadzenDataGrid<PutawayReportDTO> _PutawayReportGrid;
        public List<PutawayReportDTO> PutawayReports { get; set; } = new List<PutawayReportDTO>();

        public RadzenDataGrid<ReceiptReportDTO> _ReceiptReportGrid;
        public List<ReceiptReportDTO> ReceiptReports { get; set; } = new List<ReceiptReportDTO>();

        public RadzenDataGrid<OrderReportDto> _orderReportGrid;
        public List<OrderReportDto> orderReports = new();

        public RadzenDataGrid<TaskReportDto> _taskModelReportGrid;
        public List<TaskReportDto> taskModelReports = new();

        protected override async Task OnInitializedAsync()
        {

            await LoadDataOrderReportAsync();
            await LoadDataTaskReportAsync();
            await LoadDataShippingReportAsync();
            await LoadDataPutawayReportAsync();
            await LoadDataReceiptReportAsync();

            StateHasChanged();
        }

        private async Task LoadDataShippingReportAsync()
        {
            try
            {
                var result = await _warehouseShipmentServices.GetShippingReportAsync();
                if (result.Succeeded)
                {
                    ShippingReports = result.Data;
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = result.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                }
            }
            catch (RestEase.ApiException apiEx)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "API Error",
                    Detail = $"API request failed: {apiEx.Message}"
                });
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }
        private async Task LoadDataPutawayReportAsync()
        {
            try
            {
                var result = await _warehousePutAwayServices.GetPutawayReportAsync();
                if (!result.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = result.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                }

                PutawayReports = result.Data;
            }
            catch (RestEase.ApiException apiEx)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "API Error",
                    Detail = $"API request failed: {apiEx.Message}",
                    Duration = 5000
                });
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = $"An unexpected error occurred: {ex.Message}",
                    Duration = 5000
                });
            }
        }
        private async Task LoadDataReceiptReportAsync()
        {
            try
            {
                var result = await _warehouseReceiptOrderService.GetReceiptReportAsync();
                if (result.Succeeded)
                {
                    ReceiptReports = result.Data;
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = result.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                }
            }
            catch (RestEase.ApiException apiEx)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "API Error",
                    Detail = $"API request failed: {apiEx.Message}"
                });
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }


        private async Task LoadDataOrderReportAsync()
        {
            try
            {
                var result = await _orderServices.GetOrderReport();
                if (result.Succeeded)
                {
                    orderReports = result.Data;
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = result.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                }
            }
            catch (RestEase.ApiException apiEx)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "API Error",
                    Detail = $"API request failed: {apiEx.Message}"
                });
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }
        #region DASHBOARD 2
        private async Task LoadDataTaskReportAsync()
        {
            try
            {
                var result = await _taskModelServices.GetTaskReport();
                if (result.Succeeded)
                {
                    taskModelReports = result.Data;
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = result.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                }
            }
            catch (RestEase.ApiException apiEx)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "API Error",
                    Detail = $"API request failed: {apiEx.Message}"
                });
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = $"An unexpected error occurred: {ex.Message}"
                });
            }
        }
        #endregion
    }
}
