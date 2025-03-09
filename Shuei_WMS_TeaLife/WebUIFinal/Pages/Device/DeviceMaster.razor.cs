using Application.DTOs.Response;
using Newtonsoft.Json;
using DeviceModel = FBT.ShareModels.WMS.Device;

namespace WebUIFinal.Pages.Device
{
    public partial class DeviceMaster
    {
        private List<DeviceModel>? _dataGrid = null;
        private RadzenDataGrid<DeviceModel>? _profileGrid;
        private IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        private bool _showPagerSummary = true;
        private string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        private bool _visibled;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";
            await RefreshDataAsync();
            CheckPermission();
        }
        private void OpenAsync(DeviceModel model)
        {
            _navigation.NavigateTo($"/devicedetail/{"~" + model.Name}");
        }
        private void CreateAsync()
        {
            _navigation.NavigateTo($"/devicedetail/{_localizerCommon["Detail.Create"]}");
        }
        private async Task RefreshDataAsync()
        {
            try
            {
                var res = await _deviceServices.GetAllAsync();

                if (!res.Succeeded)
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(res.Messages.FirstOrDefault())?.Errors.FirstOrDefault();

                    NotificationHelper.ShowNotification(_notificationService
                       , error?.Key == "Warning" ? NotificationSeverity.Warning : NotificationSeverity.Error
                       , _localizerNotification[error?.Key], _localizerNotification[error?.Value]);

                    return;
                }

                _dataGrid = null;
                _dataGrid = new List<DeviceModel>();
                _dataGrid = res.Data.ToList();

                filteredData = _dataGrid;

                StateHasChanged();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowNotification(_notificationService, NotificationSeverity.Error, _localizerNotification["Error"], ex.Message);
                return;
            }
        }
        private void CheckPermission()
        {
            if (GlobalVariable.AuthenticationStateTask.HasPermission("Edit"))
            {
                _visibled = true;
            }
            else if (GlobalVariable.AuthenticationStateTask.HasPermission("View"))
            {
                _visibled = false;
            }
        }
    }
}
