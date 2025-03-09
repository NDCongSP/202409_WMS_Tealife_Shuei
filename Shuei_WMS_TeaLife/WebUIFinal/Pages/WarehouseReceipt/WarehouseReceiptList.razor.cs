using Application.DTOs.Request;
using Application.Extentions.Pagings;
using SupplierModel = FBT.ShareModels.Entities.Supplier;

namespace WebUIFinal.Pages.WarehouseReceipt
{
    public partial class WarehouseReceiptList
    {
        //private List<WarehouseReceiptOrderDto> receiptOrders = new();
        private PageList<WarehouseReceiptOrderDto> receiptOrders = new();
        private List<SupplierModel> suppliers = new();
        private List<Location> locations = new();
        private List<CompanyTenant> tenants;
        private RadzenDataGrid<WarehouseReceiptOrderDto> _profileGrid;
        private IList<WarehouseReceiptOrderDto> _selectedReceiptOrders = [];
        private int selectedTenantId;
        private Guid selectedLocationId;
        private bool _showPagerSummary = true;
        private bool allowRowSelectOnRowClick = false;
        private string pagingSummaryFormat;
        private int _count, _pageNumber = 1, _pageSize = 20;
        List<WarehouseReceiptOrderDto> _filteredModel = new();
        WarehouseReceiptSearchModel _searchModel = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                pagingSummaryFormat = $"{_CLoc["DisplayPage"]} {{0}} {_CLoc["Of"]} {{1}} <b>({_CLoc["Total"]} {{2}} {_CLoc["Records"]})</b>";

                await GetMasterData();
                await RefreshDataAsync();

                //_filteredModel = new List<WarehouseReceiptOrderDto>(receiptOrders);
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], e.Message);
            }
        }

        async Task LoadData(LoadDataArgs args)
        {
            _pageNumber = (int)((args.Skip / args.Top) + 1);
            _pageSize = (int)args.Top;
            await RefreshDataAsync();
        }

        private async Task GetMasterData()
        {
            suppliers = (await _suppliersServices.GetAllAsync()).Data.ToList();
            locations = (await _locationServices.GetAllAsync()).Data.ToList();
            tenants = (await _companiesServices.GetAllAsync()).Data.ToList();
        }

        private void EditItemAsync(string receiptNo) => _navigation.NavigateTo($"/addreceipt/{_CLoc["Detail.Edit"]} {_localizer["WarehouseReceipt"]}|{receiptNo}");

        private void AddNewItemAsync() => _navigation.NavigateTo($"/addreceipt/{_CLoc["Detail.Create"]} {_localizer["WarehouseReceipt"]}");

        private void NavigateDetailPage(string receiptNo) => _navigation.NavigateTo($"/addreceipt/{_localizer["WarehouseReceipt"]} {_CLoc["Detail.View"]}|{receiptNo}");

        private async Task DeleteItemAsync(WarehouseReceiptOrderDto model)
        {
            //if (model.Status == EnumReceiptOrderStatus.Completed) return;
            if (model.Status != EnumReceiptOrderStatus.Draft) return;

            if (!await ConfirmAction(_CLoc["Confirmation.Delete"], _CLoc["Delete"])) return;

            try
            {
                var res = await _warehouseReceiptOrderService.DeleteAsync(new WarehouseReceiptOrder 
                { 
                    Id = model.Id, 
                    ReceiptNo = model.ReceiptNo, 
                    Location = model.Location, 
                    TenantId = model.TenantId, 
                    SupplierId = model.SupplierId 
                });

                if (res.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], "");
                    //receiptOrders.Remove(model);
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], res.Messages?.FirstOrDefault()?.ToString());
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }

            if (_profileGrid != null)
                await _profileGrid.RefreshDataAsync();
        }

        private async Task RefreshDataAsync()
        {
            try
            {
                if (selectedTenantId > 0)
                {
                    _searchModel.TenantId = selectedTenantId;
                }
                if (selectedLocationId != Guid.Empty)
                {
                    _searchModel.Location = selectedLocationId;
                }
                var model = new QueryModel<WarehouseReceiptSearchModel>
                {
                    Entity = _searchModel,
                    PageNumber = _pageNumber,
                    PageSize = _pageSize
                };
                var res = await _warehouseReceiptOrderService.SearchReceiptOrderListAsync(model);

                if (!res.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], res.Messages.ToString());
                    return;
                }

                receiptOrders = res.Data;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }
        }

        private async Task<bool> ConfirmInsertWarehousePutAwayOrder()
        {
            if (_selectedReceiptOrders.Count < 1)
            {
                await _dialogService.Confirm(_localizer["Validation.CreateShelving"], $"{_localizer["CreateShelving"]}",
                    new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true });
                return false;
            }

            return await ConfirmAction(_localizer["Confirmation.CreateShelving"], $"{_localizer["CreateShelving"]}");
        }

        private async Task InsertWarehousePutAwayOrder()
        {
            if (!await ConfirmInsertWarehousePutAwayOrder()) return;

            try
            {
                foreach (var receiptOrderDto in _selectedReceiptOrders.Where(x => x.Status == EnumReceiptOrderStatus.Received))
                {
                    var payload = CreateWarehousePutAwayPayload(receiptOrderDto); //_selectedReceiptOrders.Select(CreateWarehousePutAwayDto);
                    var res = await _warehousePutAwayServices.InsertWarehousePutAwayOrder(payload);
                    receiptOrderDto.Status = EnumReceiptOrderStatus.OnPutaway;
                    var response = await _warehouseReceiptOrderService.AdjustActionReceiptOrder(receiptOrderDto);
                    if (!res.Succeeded)
                    {
                        ShowNotification(NotificationSeverity.Error, _CLoc["Error"], res.Messages.ToString());
                        return;
                    }

                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], _localizer["CreatedShelvingSuccessfully"]);
                }
               
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }
        }
        private IEnumerable<WarehousePutAwayDto> CreateWarehousePutAwayPayload(WarehouseReceiptOrderDto receiptOrderDto)
        {
            return new List<WarehouseReceiptOrderDto> { receiptOrderDto }
                .Select(_ => new WarehousePutAwayDto
                {
                    Id = _.Id,
                    ReceiptNo = _.ReceiptNo,
                    TenantId = _.TenantId,
                    DocumentNo = _.DocumentNo,
                    Location = _.Location,
                    WarehousePutAwayLines = receiptOrderDto.WarehouseReceiptOrderLines.Select(r => new WarehousePutAwayLineDto
                    {
                        Id = r.Id,
                        ProductCode = r.ProductCode,
                        ProductShortCode = r.ProductCode,
                        UnitId = (int)r.UnitId,
                        JournalQty = r.OrderQty,
                        TransQty = r.TransQty,
                        Bin = r.Bin,
                        LotNo = r.LotNo
                    }).ToList(),
                });
        }
        private WarehousePutAwayDto CreateWarehousePutAwayDto(WarehouseReceiptOrderDto _) => new()
        {
            Id = _.Id,
            ReceiptNo = _.ReceiptNo,
            TenantId = _.TenantId,
            DocumentNo = _.DocumentNo,
            Location = _.Location,
            WarehousePutAwayLines = _.WarehouseReceiptOrderLines.Select(CreateWarehousePutAwayLineDto).ToList()
        };

        private WarehousePutAwayLineDto CreateWarehousePutAwayLineDto(WarehouseReceiptOrderLineDto r) => new()
        {
            Id = r.Id,
            ProductCode = r.ProductCode,
            UnitId = r.UnitId,
            JournalQty = r.OrderQty,
            TransQty = r.TransQty,
            Bin = r.Bin,
            LotNo = r.LotNo
        };

        private void ShowNotification(NotificationSeverity severity, string summary, string detail, int duration = 5000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }

        private async Task<bool> ConfirmAction(string message, string title)
        {
            return await _dialogService.Confirm(message, title, new ConfirmOptions
            {
                OkButtonText = _CLoc["Yes"],
                CancelButtonText = _CLoc["No"],
                AutoFocusFirstElement = true
            }) ?? false;
        }

        private string GetLocalizedStatus(EnumReceiptOrderStatus status)
        {
            return status switch
            {
                EnumReceiptOrderStatus.Draft => _localizer["Draft"],
                EnumReceiptOrderStatus.Open => _localizer["Open"],
                EnumReceiptOrderStatus.Received => _localizer["Received"],
                EnumReceiptOrderStatus.OnPutaway => _localizer["OnPutaway"],
                EnumReceiptOrderStatus.Completed => _localizer["Completed"],
                _ => status.ToString(),
            };
        }

        private List<EnumDisplay<EnumReceiptOrderStatus>> GetDisplayReceiptOrderStatus()
        {
            return Enum.GetValues(typeof(EnumReceiptOrderStatus)).Cast<EnumReceiptOrderStatus>().Select(_ => new EnumDisplay<EnumReceiptOrderStatus>
            {
                Value = _,
                DisplayValue = GetValueReceiptOrderStatus(_)
            }).ToList();
        }

        private string GetValueReceiptOrderStatus(EnumReceiptOrderStatus receiptStatus) => receiptStatus switch
        {
            EnumReceiptOrderStatus.Draft => _localizer["Draft"],
            EnumReceiptOrderStatus.Open => _localizer["Open"],
            EnumReceiptOrderStatus.Received => _localizer["Received"],
            EnumReceiptOrderStatus.OnPutaway => _localizer["OnPutaway"],
            EnumReceiptOrderStatus.Completed => _localizer["Completed"],
            _ => throw new ArgumentException("Invalid value for ReceiptOrderStatus", nameof(receiptStatus))
        };

        private bool DisableCheckBoxReceiptOrder(WarehouseReceiptOrderDto dto)
        {
            if (dto.Status != EnumReceiptOrderStatus.Received)
            {
                return true;
            }

            if (dto.WarehouseReceiptOrderLines.Any())
            {
                if (dto.WarehouseReceiptOrderLines.Where(line => line.TransQty == null || line.TransQty < 0).Select(line => line).Any()) return true;
                return false;
            }

            return true;
        }

        private bool VisibleCheckBoxAllReceiptOrder()
        {
            if (_selectedReceiptOrders.Where(_ => _.Status != EnumReceiptOrderStatus.Received).Any())
            {
                return false;
            }

            if (_selectedReceiptOrders.Select(_ => _.WarehouseReceiptOrderLines).Any())
            {
                if (_selectedReceiptOrders.Select(_ => _.WarehouseReceiptOrderLines.Where(line => line.TransQty == null || line.TransQty < 0).Select(line => line)).Any()) return false;
                return true;
            }

            return false;
        }

        private void ClearFilter()
        {
            _searchModel = new();
            _profileGrid.Reload();
            StateHasChanged();
        }
    }
}
