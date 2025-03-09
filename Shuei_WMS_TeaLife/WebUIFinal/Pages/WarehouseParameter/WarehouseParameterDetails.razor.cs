using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using WebUIFinal.Pages.WarehouseTransfer;
using WarehouseParameterModel = FBT.ShareModels.WMS.WarehouseParameter;
namespace WebUIFinal.Pages.WarehouseParameter
{
    public partial class WarehouseParameterDetails
    {
        private WarehouseParameterModel _model = new WarehouseParameterModel();
        private List<LocationDisplayDto> locations = new List<LocationDisplayDto>();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                await GetWarehouseParameterAsync();
                await GetLocationssAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task GetWarehouseParameterAsync()
        {

          
                var data = await _warehouseParametersServices.GetFirstOrDefaultAsync();
                if (!data.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _CLoc["Error"],
                        Detail = "Result warehouseParameters null or not found",
                        Duration = 1000
                    });

                    return;
                }
            _model = data.Data;  
        }
        private async Task GetLocationssAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
        }
        async void Submit(WarehouseParameterModel arg)
        {
            // Update the model with the new values from arg

           
                // Map other necessary properties
            

            var response = await _warehouseParametersServices.UpdateAsync(arg);


            if (response.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                    Summary = _CLoc["Success"],
                    Detail = "Successfully updated warehouseParameters",
                    Duration = 5000
                });

                
            }
            else
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = "Failed to update warehouseParameters",
                    Duration = 5000
                });
            }
        }


    }
}
