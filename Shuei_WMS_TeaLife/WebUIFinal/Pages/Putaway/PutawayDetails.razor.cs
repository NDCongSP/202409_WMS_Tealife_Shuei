using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QRCoder.Core;
using PutAwayLineModel = FBT.ShareModels.WMS.WarehousePutAwayLine;
namespace WebUIFinal.Pages.Putaway
{
    public partial class PutawayDetails
    {
        [Parameter] public string Title { get; set; } = string.Empty;
        public string PutAwayNo { get; set; } = string.Empty;
        public Guid? WarehousePutAwayLineId { get; set; }
        PutAwayLineModel model = new WarehousePutAwayLine();
        WarehousePutAwayDto putAwayDto = new WarehousePutAwayDto();
        private IEnumerable<WarehousePutAwayLine> childTableData = Enumerable.Empty<WarehousePutAwayLine>();
        public IEnumerable<WarehousePutAwayLine> ChildEntities { get; set; } = Enumerable.Empty<WarehousePutAwayLine>();
        bool allowRowSelectOnRowClick = true;
        IEnumerable<PutAwayLineModel> PutAwayLine;
        IList<WarehousePutAwayLineDto> selectedPutAwayLine;
        private List<WarehousePutAwayLine> copiedStagingData = new List<WarehousePutAwayLine>();
        List<TenantAuth> tenants = new();
        List<TenantAuth> units = new();
        private string inputText = string.Empty;
        private string qrCodeBase64 = string.Empty;
        [Parameter] public Guid PutawayLineId { get; set; }
        RadzenDataGrid<WarehousePutAwayLineDto> grid;
        private IEnumerable<WarehousePutAwayLineDto> stagingData;
        List<Bin> Bins = new List<Bin>();
        public string ReceiptNo { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        private List<LocationDisplayDto> locations = new();
        private ProductDto selectedProduct = new();

        private LocationDisplayDto selectLocationDisplayDto;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                await GetTenantsAsync();
                await GetLocationssAsync();
                await GetWarehousePutAwayDetail();

                await OnLocationChanged(putAwayDto.Location);
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
        private async Task OnLocationChanged(string arg)
        {
            if (Guid.TryParse(arg, out Guid locationId)) // Ensure the argument is parsed correctly
            {
                var data = await _binServices.GetByLocationId(locationId);
                selectLocationDisplayDto = locations.FirstOrDefault(x => x.Id == arg);
                if (data.Succeeded) // Check if the data retrieval was successful
                {
                    Bins.Clear(); // Clear existing bins before adding new ones
                    Bins.AddRange(data.Data); // Add the new bins to the list

                }
                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizer["FailedToLoadBins", data.Messages],
                        Duration = 4000
                    });
                }
            }
            else
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _localizerCommon["Warning"],
                    Detail = _localizer["InvalidLocationId"],
                    Duration = 4000
                });
            }
        }
        private async Task CompletePutawayAsync()
        {
            var confirm = await _dialogService.Confirm($"{_localizer["Are you sure you want to complete putaway for all products"]}: {putAwayDto.PutAwayNo}?", _localizer["CoompletedPutaway"], new ConfirmOptions()
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            if (childTableData == null)
            {
                childTableData = Enumerable.Empty<WarehousePutAwayLine>();
            }


            try
            {
                var response = await _warehousePutAwayServices.AdjustActionPutAway(putAwayDto);

                if (response.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = "Putaway status updated to Completed.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/putaway");
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = _localizer[response.Messages[0]],
                        Duration = 5000
                    });
                }
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
        private async Task SyncHTData()
        {
            try
            {
                var response = await _warehousePutAwayServices.SyncHTData(putAwayDto);
                var result = await _warehousePutAwayStagingServices.GetByMasterCodeAsync(PutAwayNo);
                if (response.Succeeded)
                {
                    // Update the putAwayDto with new data
                    putAwayDto = (await _warehousePutAwayServices.GetPutAwayAsync(putAwayDto.PutAwayNo)).Data;
                    var deleteResult = await _warehousePutAwayStagingServices.DeleteAsync(result.Data.FirstOrDefault());
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = "WarehousePutAwayLines updated successfully.",
                        Duration = 5000
                    });


                }

                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = response.Messages.ToString(),
                        Duration = 5000
                    });

                }
                await grid.Reload();
                StateHasChanged();

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
            }
        }
        private async Task CopyAndAddNewRow(WarehousePutAwayLineDto staging)
        {

            try
            {

                if (staging == null)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Warning,
                        Summary = "Warning",
                        Detail = "Please select a row to copy.",
                        Duration = 4000
                    });
                    return;
                }

                var newRow = new WarehousePutAwayLineDto
                {
                    Id = Guid.NewGuid(), // Generate a new ID for the new row
                    PutAwayNo = staging.PutAwayNo,
                    ProductCode = staging.ProductCode,
                    UnitId = staging.UnitId,
                    UnitName = staging.UnitName,
                    ReceiptLineId = staging.ReceiptLineId,
                    //JournalQty = staging.JournalQty,
                    //TransQty = staging.TransQty,
                    Bin = staging.Bin,
                    LotNo = staging.LotNo,
                    ExpirationDate = staging.ExpirationDate,
                    TenantId = staging.TenantId,
                    IsDeleted = true,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = staging.CreateOperatorId,
                    Status = staging.Status
                }; newRow.TenantId = staging.TenantId;
                var result = await _warehousePutAwayLineServices.InsertAsync(new WarehousePutAwayLine
                {
                    Id = newRow.Id,
                    PutAwayNo = newRow.PutAwayNo,
                    ProductCode = newRow.ProductCode,
                    UnitId = newRow.UnitId,
                    ReceiptLineId = newRow.ReceiptLineId,
                    //JournalQty = newRow.JournalQty,
                    //TransQty = newRow.TransQty,
                    Bin = newRow.Bin,
                    LotNo = newRow.LotNo,
                    ExpirationDate = newRow.ExpirationDate,
                    TenantId = staging.TenantId,
                    IsDeleted = newRow.IsDeleted,
                    Status = newRow.Status,
                    CreateAt = newRow.CreateAt,
                    CreateOperatorId = newRow.CreateOperatorId,
                });
                if (result.Succeeded)
                {
                    if (stagingData == null)
                    {
                        stagingData = new List<WarehousePutAwayLineDto>();
                    }

                    var lstObj = putAwayDto.WarehousePutAwayLines;

                    putAwayDto.WarehousePutAwayLines = lstObj.Concat(new List<WarehousePutAwayLineDto> { newRow }).ToList();


                    await grid.Reload();
                    StateHasChanged();

                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = "Row copied and added successfully.",
                        Duration = 4000
                    });
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = "Failed to add new row.",
                        Duration = 4000
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = "An unexpected error occurred: " + ex.Message,
                    Duration = 4000
                });
            }
        }

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
        // ...

        private async Task PrintSelectedLabels()
        {
            if (selectedPutAwayLine == null || !selectedPutAwayLine.Any())
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = "Putaway Details",
                    Detail = "Please select at least one item to print labels.",
                    Duration = 4000
                });
                return;
            }

            //List<LabelInfoDto> labelsToPrint = new List<LabelInfoDto>();

            //int index = 0;

            //foreach (var item in selectedPutAwayLine)
            //{
            //    for (int i = 0; i < item.TransQty; i++)
            //    {
            //        // Generate QR code content
            //        if (item.ProductJanCodes.Count == 0)
            //        {
            //            string qrCodeContent = $"{item.ProductCode}::{"N/A"}::{item.LotNo}::{DateTime.Now.ToString("yyyyMMddHHmmss")}{index}" +
            //                                    $"::{item.ExpirationDate:yyyy/MM/dd}::{item.TenantId}::{item.ReceiptNo}";
            //            labelsToPrint.Add(new LabelInfoDto()
            //            {
            //                QrValue = GlobalVariable.GenerateQRCode(qrCodeContent),
            //                Title1 = "商品コード:",
            //                Content1 = item.ProductCode,
            //                Title2 = "JANコード:",
            //                Content2 = "N/A",
            //                Title3 = "LOT:",
            //                Content3 = item.LotNo,
            //                Title4 = "賞味期限:",
            //                Content4 = item.ExpirationDate?.ToString("yyyy/MM/dd") ?? "N/A",
            //                QrValue2 = GlobalVariable.GenerateQRCode(item.ProductUrl)
            //            });

            //            index += 1;
            //        }
            //        else
            //        {
            //            item.ProductJanCodes.ForEach(t =>
            //            {
            //                string qrCodeContent = $"{item.ProductCode}::{t}::{item.LotNo}::{DateTime.Now.ToString("yyyyMMddHHmmss")}{index}" +
            //                                        $"::{item.ExpirationDate:yyyy/MM/dd}::{item.TenantId}::{item.ReceiptNo}";
            //                labelsToPrint.Add(new LabelInfoDto()
            //                {
            //                    QrValue = GlobalVariable.GenerateQRCode(qrCodeContent),
            //                    Title1 = "商品コード:",
            //                    Content1 = item.ProductCode,
            //                    Title2 = "JANコード:",
            //                    Content2 = t,
            //                    Title3 = "LOT:",
            //                    Content3 = item.LotNo,
            //                    Title4 = "賞味期限:",
            //                    Content4 = item.ExpirationDate?.ToString("yyyy/MM/dd") ?? "N/A",
            //                    QrValue2 = GlobalVariable.GenerateQRCode(item.ProductUrl)
            //                });
            //                index += 1;
            //            });
            //        }
            //    }
            //}
            //var dataPrint = labelsToPrint;
            //await _localStorage.SetItemAsync("labelDataTransfer", dataPrint);
            await _localStorage.SetItemAsync("selectInputLine", selectedPutAwayLine);
            await JSRuntime.InvokeVoidAsync("openTab", "/PrintProductLabel");
        }
        private async Task GetWarehousePutAwayDetail()
        {
            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (sub.Length > 1)
                {
                    PutAwayNo = sub[1].Trim();
                }
            }

            if (!string.IsNullOrEmpty(PutAwayNo))
            {
                var warehouse = await _warehousePutAwayServices.GetPutAwayAsync(PutAwayNo);
                if (!warehouse.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = "Result warehouse null or not found",
                        Duration = 1000
                    });

                    return;
                }

                putAwayDto = warehouse.Data;

            }
        }

        private async Task GetTenantsAsync()
        {
            var data = await _tenantsServices.GetAllAsync();
            if (data.Succeeded) tenants.AddRange(data.Data);
        }
        async void Submit(WarehousePutAwayDto arg)
        {
            // Update the model with the new values from arg
            var warehousePutAway = new WarehousePutAway
            {
                Id = putAwayDto.Id,
                ReceiptNo = putAwayDto.ReceiptNo,
                PutAwayNo = putAwayDto.PutAwayNo,
                Location = putAwayDto.Location,
                TransDate = putAwayDto.TransDate,
                TenantId = putAwayDto.TenantId,
                Status = putAwayDto.Status,
                UpdateAt = DateTime.Now
                // Map other necessary properties
            };

            var response = await _warehousePutAwayServices.UpdateAsync(warehousePutAway);


            if (response.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                    Summary = _localizerCommon["Success"],
                    Detail = "Successfully updated putaway",
                    Duration = 5000
                });

                // Navigate to the /putaway page
                _navigation.NavigateTo("/putaway");
            }
            else
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = "Failed to update putaway",
                    Duration = 5000
                });
            }
        }

        async Task DeleteItemAsync(WarehousePutAwayDto putAwayDto)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{putAwayDto.PutAwayNo} {_localizer["Are you sure you want to delete the shelving number"]}", _localizer["Delete Warehouse"], new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var result = await _warehousePutAwayServices.GetByIdAsync(putAwayDto.Id);

                if (result.Data != null)
                {
                    var res = await _warehousePutAwayServices.DeleteAsync(result.Data);
                }

                if (result.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = $"Delete putaway {putAwayDto.PutAwayNo} successfully.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/putaway", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = $"Failed to delete warehouse {putAwayDto.PutAwayNo}.",
                        Duration = 5000
                    });
                }
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
            }
        }




        DataGridEditMode editMode = DataGridEditMode.Single;

        List<WarehousePutAwayLineDto> PutAwayLineToInsert = new List<WarehousePutAwayLineDto>();
        List<WarehousePutAwayLineDto> PutAwayLineUpdate = new List<WarehousePutAwayLineDto>();

        private async Task DeleteRow(WarehousePutAwayLineDto staging)
        {
            var confirm = await _dialogService.Confirm(_localizerCommon["Confirmation.Delete"], _localizerCommon["Confirm"], new ConfirmOptions() { OkButtonText = _localizerCommon["Yes"], CancelButtonText = _localizerCommon["No"] });
            if (confirm == true)
            {
                // Remove the item from the local collection
                if (putAwayDto.WarehousePutAwayLines.Contains(staging))
                {
                    putAwayDto.WarehousePutAwayLines.Remove(staging);
                }

                // Remove the item from the database
                var result = await _warehousePutAwayLineServices.DeleteAsync(new WarehousePutAwayLine
                {
                    Id = staging.Id,
                    PutAwayNo = staging.PutAwayNo,
                    ProductCode = staging.ProductCode,
                    UnitId = staging.UnitId,
                    JournalQty = staging.JournalQty,
                    TransQty = staging.TransQty,
                    Bin = staging.Bin,
                    LotNo = staging.LotNo,
                    ExpirationDate = staging.ExpirationDate,
                    TenantId = staging.TenantId,
                    IsDeleted = staging.IsDeleted,
                    Status = staging.Status
                });
                if (result.Succeeded)
                {
                    await grid.Reload();
                    StateHasChanged();
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = "Row deleted successfully.",
                        Duration = 4000
                    });

                }
                else
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizerCommon["Error"],
                        Detail = "Failed to delete row from the database.",
                        Duration = 4000
                    });
                }



            }
        }
        void Reset()
        {
            PutAwayLineToInsert.Clear();
            PutAwayLineUpdate.Clear();
        }

        void Reset(WarehousePutAwayLineDto staging)
        {
            PutAwayLineToInsert.Remove(staging);
            PutAwayLineUpdate.Remove(staging);
        }

        private async Task EditRow(WarehousePutAwayLineDto staging)
        {
            if (editMode == DataGridEditMode.Single && PutAwayLineToInsert.Count > 0)
            {
                Reset();
            }

            // Find the existing item in the list
            var existingItem = putAwayDto.WarehousePutAwayLines.FirstOrDefault(x => x.Id == staging.Id);
            if (existingItem != null)
            {
                // Update the existing item with the new values
                existingItem.ProductCode = staging.ProductCode;
                existingItem.UnitId = staging.UnitId;
                existingItem.JournalQty = staging.JournalQty;
                existingItem.TransQty = staging.TransQty;
                existingItem.Bin = staging.Bin;
                existingItem.LotNo = staging.LotNo;
                existingItem.ExpirationDate = staging.ExpirationDate;
                existingItem.ReceiptLineId = staging.ReceiptLineId;
                existingItem.UpdateAt = DateTime.Now;
                existingItem.UpdateOperatorId = staging.UpdateOperatorId;
                existingItem.CreateAt = staging.CreateAt;
                existingItem.CreateOperatorId = staging.CreateOperatorId;
            }
            else
            {
                // If the item does not exist, add it to the list
                putAwayDto.WarehousePutAwayLines.Add(staging);
            }

            PutAwayLineUpdate.Add(staging);
            await grid.EditRow(staging);
        }

        void OnUpdateRow(WarehousePutAwayLineDto staging)
        {
            // Reset the staging item from the insert and update lists
            Reset(staging);

            // Find the existing item in the list
            var existingItem = putAwayDto.WarehousePutAwayLines.FirstOrDefault(x => x.Id == staging.Id);
            if (existingItem != null)
            {
                // Update the existing item with the new values
                existingItem.ProductCode = staging.ProductCode;
                existingItem.UnitId = staging.UnitId;
                existingItem.JournalQty = staging.JournalQty;
                existingItem.TransQty = staging.TransQty;
                existingItem.Bin = staging.Bin;
                existingItem.LotNo = staging.LotNo;
                existingItem.ExpirationDate = staging.ExpirationDate;
                existingItem.ReceiptLineId = staging.ReceiptLineId;
                existingItem.UpdateAt = DateTime.Now;
                existingItem.UpdateOperatorId = staging.CreateOperatorId;
                existingItem.CreateAt = staging.CreateAt;
                existingItem.CreateOperatorId = staging.CreateOperatorId;
            }
            else
            {
                // If the item does not exist, add it to the list
                putAwayDto.WarehousePutAwayLines.Add(staging);
            }

            // Optionally, save the updated row to the database


            // Notify the user about the success
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = _localizerCommon["Success"],
                Detail = "Row updated successfully.",
                Duration = 4000
            });
        }


        async Task SaveRow(WarehousePutAwayLineDto staging)
        {
            // Ensure the staging object is not null
            if (staging == null)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = "Cannot save a null record",
                    Duration = 4000
                });
                return;
            }
            var warehousePutAwayLine = new WarehousePutAwayLine
            {
                Id = staging.Id,
                PutAwayNo = staging.PutAwayNo,
                ProductCode = staging.ProductCode,
                UnitId = staging.UnitId,
                JournalQty = staging.JournalQty,
                TransQty = staging.TransQty,
                Bin = staging.Bin,
                LotNo = staging.LotNo,
                ExpirationDate = staging.ExpirationDate,
                TenantId = staging.TenantId,
                IsDeleted = staging.IsDeleted,
                Status = staging.Status,
                ReceiptLineId = staging.ReceiptLineId,
                UpdateAt = DateTime.Now,
                UpdateOperatorId = staging.CreateOperatorId
            };

            var updateResult = await _warehousePutAwayLineServices.UpdateAsync(warehousePutAwayLine);
            await grid.UpdateRow(staging);
        }
        private void CancelEdit(WarehousePutAwayLineDto staging)
        {
            grid.CancelEditRow(staging);
        }
    }
}