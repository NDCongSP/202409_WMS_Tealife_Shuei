using Application.DTOs.Request.Products;
using Application.DTOs.Transfer;
using Magicodes.ExporterAndImporter.Core.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor.Rendering;

namespace WebUIFinal.Pages.StockTake
{
    public partial class StockTakeDetail
    {
        //params
        [Parameter] public string Title { get; set; }
        [Parameter] public string? StockTakeNo { get; set; }
        [Inject] private DialogService DialogService { get; set; }

        private RadzenDialog _dialog;

        // variables
        private InventStockTakeDto StockTakeDto = new();
        private InventStockTakeDto StockTakeDtoBefore = new();
        private List<LocationDisplayDto> _locations = new();
        private LocationDisplayDto _locationSelected = new();
        private LocationDisplayDto _locationOriginal = new();
        private List<CompanyDisplayDto> _tenants = new();
        private CompanyDisplayDto _tenantSelected = new();
        private CompanyDisplayDto _tenantOriginal = new();
        private List<UserDto> _users = new();
        //private bool _showPagerSummary = true;
        //private bool allowRowSelectOnRowClick = true;

        // line
        private RadzenDataGrid<InventStockTakeLineDto>? _lineProfileGrid;
        private InventStockTakeLineDto _lineSelected = new();
        private string _productSelected = "";
        private bool _isEditRow = false;

        // status
        private bool _disabled, _disabledSave, _disabledPrint, _disabledDetele, _disabledAddRow, _disabledComplete, _disabledGotoRecording, _disabledCreateRecording, _disabledAutoAdd;
        private bool _visibledPrint, _visibledDelete, _visibledAddRow, _visibledEditRow, _visibledComplete, _visibledGotoRecording, _visibledCreateRecording, _visibledAutoAdd;
        private string _createText = "";
        private bool _new = false;
        private bool _isLoading = false;
        private bool _isConfirm = false;
        // event
        //private ElementReference dropDownLocationRef;
        //private ElementReference textBoxNoRef;
        //private ElementReference requiredValidatorTranDateRef;

        protected override async Task OnInitializedAsync()
        {
            Loading();
            await Task.WhenAll(RefreshDataAsync(), GetUserAsync(), GetLocationAsync(), GetTenantAsync());
            StockTakeDtoBefore = Clone(StockTakeDto);
            _isLoading = false;
            CheckStatus();
            CheckPermission();
        }
        private async Task RefreshDataAsync()
        {

            if (Title.Contains("~"))
            {
                StockTakeNo = Title[1..];
                var result = await _inventStockTakeServices.GetByStockTakeNo(StockTakeNo);
                if (!result.Succeeded)
                {
                    NotifyError(_localizer[result.Messages.FirstOrDefault()]);
                    await Task.Delay(1000);
                    _navigation.NavigateTo("/stocktake");
                    return;
                }
                StockTakeDto = result.Data;
                _locationSelected.Id = StockTakeDto.Location;
                _tenantSelected.Id = StockTakeDto.TenantId;
                _new = false;
                StateHasChanged();
            }
            else
            {
                StockTakeDto.Status = EnumInventStockTakeStatus.Create;
                StockTakeDto.TransactionDate = DateTime.Now;
                _new = true;
            }
            StockTakeDtoBefore = Clone(StockTakeDto);
        }
        private async Task GetLocationAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded)
            {
                _locations.AddRange(data.Data.Select(l => new LocationDisplayDto(l)));
                if (string.IsNullOrEmpty(StockTakeDto.Location) && _locations.Count > 0)
                {
                    _locationSelected = CloneLocation(_locations.First());
                    _locationOriginal = CloneLocation(_locationSelected);
                }
                else
                {
                    _locationSelected = CloneLocation(_locations.Where(l => l.Id == StockTakeDto.Location).FirstOrDefault());
                    _locationOriginal = CloneLocation(_locationSelected);
                }
                StockTakeDto.Location = _locationSelected.Id;
                StockTakeDto.LocationName = _locationSelected.LocationName;
            }
            else
            {
                _locations = new List<LocationDisplayDto>();
                _locationSelected = null;
            }
        }
        private async Task GetTenantAsync()
        {
            var data = await _companiesServices.GetAllAsync();
            if (data.Succeeded)
            {
                _tenants.AddRange(data.Data.Select(t => new CompanyDisplayDto(t)));

                if (StockTakeDto.TenantId == 0 && _tenants.Count > 0)
                {
                    _tenantSelected = CloneTenant(_tenants.First());
                    _tenantOriginal = CloneTenant(_tenantSelected);
                }
                else
                {
                    _tenantSelected = CloneTenant(_tenants.Where(t => t.Id == StockTakeDto.TenantId).FirstOrDefault());
                    _tenantOriginal = CloneTenant(_tenantSelected);
                }
                StockTakeDto.TenantId = _tenantSelected.Id;
                StockTakeDto.TenantFullName = _tenantSelected.CompanyName;
            }
            else
            {
                _tenants = new List<CompanyDisplayDto>();
                _tenantSelected = null;
            }
        }
        private async Task GetUserAsync()
        {
            var data = await _userToTenantServices.GetUsersAsync();
            if (data.Count > 0) _users.AddRange(data);
            if (string.IsNullOrEmpty(StockTakeDto.PersonInCharge))
            {
                var currentUser = await _inventStockTakeServices.GetCurrentUserAsync();
                if (currentUser.Succeeded)
                {
                    StockTakeDto.PersonInCharge = currentUser.Data;
                }
            }
        }
        private async Task Submit(InventStockTakeDto arg)
        {
            if (_isEditRow == false)
            {
                var check = AreEqual(StockTakeDtoBefore, arg);
                if (check && !_new)
                {
                    NotifySuccess(_localizer["No changes detected"]);
                    return;
                }
                await SaveAsync(arg);
                return;
            }
            else
            {
                NotifyError(_localizer["You have unfinished lines"]);
                return;
            }
        }
        private async Task SaveAsync(InventStockTakeDto arg)
        {
            if (StockTakeDto.InventStockTakeLines.Any())
            {
                var exists = new List<string>();
                foreach (var item in StockTakeDto.InventStockTakeLines)
                {
                    if (await CheckExistLineinDatabase(item))
                    {
                        exists.Add(item.ProductCode);
                    }
                }
                if (exists.Count > 0)
                {
                    string mess = string.Join(",", exists);
                    NotifyError(mess + _localizer["already exists in another stocktake"]);
                }
            }
            if (Title.Contains("~"))
            {
                await UpdateStockTake(arg);
            }
            else
            {
                await CreateStockTake(arg);
            }
        }
        private async Task CreateStockTake(InventStockTakeDto arg)
        {
            var result = await _inventStockTakeServices.GetNewStocktakeNo();
            if (result.Succeeded)
            {
                arg.StockTakeNo = result.Data;
            }
            else
            {
                NotifyError("");
            }
            var response = await _inventStockTakeServices.Insert(arg);
            if (response.Succeeded)
            {
                var res = response.Data;
                arg.InventStockTakeLines.ForEach(l => l.StockTakeNo = response.Data.StockTakeNo);
                StockTakeNo = response.Data.StockTakeNo;
                StockTakeDto.StockTakeNo = StockTakeNo;
                StockTakeDto.Status = EnumInventStockTakeStatus.Open;
                NotifySuccess("");
                _navigation.NavigateTo($"/stocktakedetail/{"~" + response.Data.StockTakeNo}");
                await OnInitializedAsync();
            }
            else
            {
                NotifyError("");
            }
        }
        private async Task UpdateStockTake(InventStockTakeDto arg)
        {
            var response = await _inventStockTakeServices.Update(arg);

            if (response.Succeeded)
            {
                NotifySuccess("");
                StockTakeDtoBefore = Clone(response.Data);
                await RefreshDataAsync();
            }
            else
            {
                NotifyError("");
            }
        }
        private async Task Delete()
        {
            var ggggggggggggg = _visibledEditRow;
            if (!await ConfirmAction(_localizerCommon["Delete"], $"{_localizerCommon["Confirmation.Delete"]}")) return;
            try
            {
                var response = await _inventStockTakeServices.Delete(StockTakeDto.Id);

                if (response.Succeeded)
                {
                    NotifySuccess("");
                    await Task.Delay(500);
                    _navigation.NavigateTo("/stocktake");
                }
                else
                {
                    NotifyError(_localizer[response.Messages.FirstOrDefault()]);
                }
            }
            catch (Exception ex)
            {
                NotifyError(_localizer[ex.Message]);
            }
        }
        private async Task CompleteStockTake()
        {
            // check unfinished lines
            if (_isEditRow == true)
            {
                NotifyError(_localizer["You have unfinished lines"]);
                return;
            }
            if (!StockTakeDto.InventStockTakeLines.Any())
            {
                NotifyError(_localizer["Stocktake does not have any products to complete"]);
                return;
            }

            // check recording
            var recordings = await _inventStockTakeRecordingServices.GetListByStockTakeNoDTOAsync(StockTakeNo);
            if (recordings.Data == null || !recordings.Data.Any())
            {
                NotifyError(_localizer["You haven't created a recording for this stocktake"]);
                return;
            }
            if (!recordings.Data.Any(i => i.Status == EnumInvenTransferStatus.Completed))
            {
                NotifyError(_localizer["This stocktake does not have any completed recordings"]);
                return;
            }
            try
            {
                var latestRecording = recordings.Data
                    .OrderByDescending(line => line.CreateAt)
                    .FirstOrDefault();
                if (latestRecording != null && latestRecording.Status != EnumInvenTransferStatus.Completed)
                {
                    NotifyError(_localizer["The latest recording you created has not been completed"]);
                    return;
                    //if (!await ConfirmAction(_localizerCommon["Complete"], _localizer["The latest recording you created has not been completed"])) return;
                }
                if (!await ConfirmAction(_localizerCommon["Complete"], StockTakeNo)) return;
                var response = await _inventStockTakeServices.CompleteInventStockTake(StockTakeDto);
                if (response.Succeeded)
                {
                    NotifySuccess(_localizer[response.Messages.First()]);
                    await OnInitializedAsync();
                }
                else
                {
                    NotifyError(_localizer[response.Messages.FirstOrDefault()]);
                }
            }
            catch (Exception ex)
            {
                //NotifyError(_localizer[ex.Message]);
                Console.WriteLine(ex.Message);
            }
        }
        private async Task CreateRecording()
        {
            if (!StockTakeDto.InventStockTakeLines.Any()) { NotifyError(_localizer["You have not selected any product"]); return; }
            if (_isEditRow == true) { NotifyError(_localizer["You have unfinished lines"]); return; }
            var check = AreEqual(StockTakeDtoBefore, StockTakeDto);
            if (!check || _new)
            {
                NotifyError("You must save the changes before creating a record");
            }
            var parameters = new Dictionary<string, object>
            {
                { "Location", _locations.Where(_ => _.Id == StockTakeDto.Location).Select(_ => _.LocationName).FirstOrDefault() },
                { "StockTakeDto", StockTakeDto },
                { "Users", _users },
            };
            await DialogService.OpenAsync<PopupCreateRecording>("棚卸レコーディング開始", parameters);
            await RefreshDataAsync();
            CheckStatus();
        }
        private async Task PrintStockTakeSlip()
        {
            if (_isEditRow == true) { NotifyError(_localizer["You have unfinished lines"]); return; }
            var _dataTrasfers = new List<StockTakeSlipInfos> { new StockTakeSlipInfos(StockTakeDto, StockTakeDto.InventStockTakeLines) };
            await _localStorage.SetItemAsync("StockTakeSlipInfosTransfer", _dataTrasfers);
            await JSRuntime.InvokeVoidAsync("openTab", "/printstocktakeslip");
        }
        private async Task GotoRecording()
        {
            if (_isEditRow)
            {
                NotifyError(_localizer["You have unfinished lines"]);
                return;
            }

            var recordings = await _inventStockTakeRecordingServices.GetListByStockTakeNoDTOAsync(StockTakeNo);

            if (recordings.Data == null || !recordings.Data.Any())
            {
                NotifyError(_localizer["This stocktake does not have any recordings"]);
                return;
            }

            var recording = recordings.Data.OrderByDescending(r => r.CreateAt).FirstOrDefault();
            _navigation.NavigateTo($"/stocktakerecordingdetail/~{recording.Id}");
            return;
        }
        private async Task AutoAddProductLinesAsync()
        {
            if (_isConfirm == false)
            {
                if (StockTakeDto.InventStockTakeLines.Any())
                {
                    if (!await ConfirmAction(_localizerCommon["Refresh"], _localizer["The product list will be refreshed. Continue?"])) { return; }
                }
            }
            else
            {
                _isConfirm = false;
            }
            if (_isEditRow)
            {
                CancelEdit(_lineSelected);
            }
            StockTakeDto.InventStockTakeLines = new List<InventStockTakeLineDto>();
            Loading();
            try
            {
                var products = await _productServices.GetAllDtoAsync(new ProductSearchRequestDTO() { TenantId = _tenantSelected.Id });
                if (products.Succeeded)
                {
                    foreach (var product in products.Data)
                    {
                        var stock = await CheckStockQuantity(product.ProductCode);
                        if (!stock.Succeeded)
                        {
                            continue;
                        }
                        _lineSelected = new InventStockTakeLineDto
                        {
                            ExpectedQty = stock.Succeeded ? stock.Data : 0,
                            ProductCode = product.ProductCode,
                            ProductName = product.ProductName,
                            UnitId = product.UnitId,
                            UnitName = product.UnitName,
                            ActualQty = 0
                        };
                        //bool isLineExist = false;
                        if (_lineSelected.Id == Guid.Empty)
                        {
                            _lineSelected.Id = Guid.NewGuid();
                            StockTakeDto.InventStockTakeLines.Add(_lineSelected);
                        }
                        // Cập nhật lại DataGrid
                        await _lineProfileGrid.Reload();
                    }
                    if (!StockTakeDto.InventStockTakeLines.Any())
                    {
                        NotifySuccess("No inventory found");
                    }
                    else
                    {
                        NotifySuccess("");
                    }
                }
                else
                {
                    NotifyError("");
                }
            }
            catch (Exception ex)
            {
                //NotifyError(_localizer[ex.Message]);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _isLoading = false;
                CheckStatus();
            }
        }
        #region COMMON COMPONENT
        private string GetStatusColor(EnumInventStockTakeStatus status) => status switch
        {
            EnumInventStockTakeStatus.Create => "primary",
            EnumInventStockTakeStatus.Open => "secondary",
            EnumInventStockTakeStatus.Blocking => "error",
            EnumInventStockTakeStatus.Completed => "success",
        };
        private async Task OnLocationChanged()
        {
            if (StockTakeDto.InventStockTakeLines.Any())
            {
                if (await ConfirmAction(_localizerCommon["Refresh"], _localizer["Do you want to continue new location"]))
                {
                    _locationSelected.LocationName = _locations
                        .Where(l => l.Id == _locationSelected.Id)
                        .Select(l => l.LocationName)
                        .FirstOrDefault();
                    StockTakeDto.Location = CloneString(_locationSelected.Id);
                    StockTakeDto.LocationName = CloneString(_locationSelected.LocationName);
                    _locationOriginal = CloneLocation(_locationSelected);
                    _isConfirm = true;
                    await AutoAddProductLinesAsync();
                }
                else
                {
                    _locationSelected = CloneLocation(_locationOriginal);
                }
            }
            else
            {
                _locationSelected.LocationName = _locations
                        .Where(l => l.Id == _locationSelected.Id)
                        .Select(l => l.LocationName)
                        .FirstOrDefault();
                StockTakeDto.Location = CloneString(_locationSelected.Id);
                StockTakeDto.LocationName = CloneString(_locationSelected.LocationName);
                _locationOriginal = CloneLocation(_locationSelected);
            }
            StateHasChanged();
        }
        private async Task OnTenantChanged()
        {
            if (StockTakeDto.InventStockTakeLines.Any())
            {
                if (await ConfirmAction(_localizerCommon["Refresh"], _localizer["Do you want to continue new tenant"]))
                {
                    _tenantSelected.CompanyName = _tenants
                        .Where(l => l.Id == _tenantSelected.Id)
                        .Select(l => l.CompanyName)
                        .FirstOrDefault();
                    StockTakeDto.TenantId = CloneInt(_tenantSelected.Id);
                    StockTakeDto.TenantFullName = CloneString(_tenantSelected.CompanyName);
                    _tenantOriginal = CloneTenant(_tenantSelected);
                    _isConfirm = true;
                    await AutoAddProductLinesAsync();
                }
                else
                {
                    _tenantSelected = CloneTenant(_tenantOriginal);
                }
            }
            else
            {
                _tenantSelected.CompanyName = _tenants
                        .Where(l => l.Id == _tenantSelected.Id)
                        .Select(l => l.CompanyName)
                        .FirstOrDefault();
                StockTakeDto.TenantId = CloneInt(_tenantSelected.Id);
                StockTakeDto.TenantFullName = CloneString(_tenantSelected.CompanyName);
                //StockTakeDtoBefore.TenantId = CloneInt(_tenantSelected.Id);
                //StockTakeDtoBefore.TenantFullName = CloneString(_tenantSelected.CompanyName);
                _tenantOriginal = CloneTenant(_tenantSelected);
            }
            StateHasChanged();
        }
        private static InventStockTakeDto Clone(InventStockTakeDto source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var clone = new InventStockTakeDto
            {
                Id = source.Id,
                StockTakeNo = source.StockTakeNo,
                Description = source.Description,
                Location = source.Location,
                TenantId = source.TenantId,
                TransactionDate = source.TransactionDate,
                Status = source.Status,
                PersonInCharge = source.PersonInCharge,
                CreateOperatorId = source.CreateOperatorId,
                CreateAt = source.CreateAt,
                UpdateOperatorId = source.UpdateOperatorId,
                UpdateAt = source.UpdateAt,
                IsDeleted = source.IsDeleted,
                PersonInChargeName = source.PersonInChargeName,
                LocationName = source.LocationName
            };

            foreach (var line in source.InventStockTakeLines)
            {
                var lineClone = new InventStockTakeLineDto
                {
                    Id = line.Id,
                    StockTakeNo = line.StockTakeNo,
                    ProductCode = line.ProductCode,
                    ExpectedQty = line.ExpectedQty,
                    ActualQty = line.ActualQty,
                    UnitId = line.UnitId,
                    Status = line.Status,
                    CreateOperatorId = line.CreateOperatorId,
                    CreateAt = line.CreateAt,
                    UpdateOperatorId = line.UpdateOperatorId,
                    UpdateAt = line.UpdateAt,
                    IsDeleted = line.IsDeleted,
                    ProductName = line.ProductName,
                    UnitName = line.UnitName
                };
                clone.InventStockTakeLines.Add(lineClone);
            }

            return clone;
        }
        private static LocationDisplayDto CloneLocation(LocationDisplayDto source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var clone = new LocationDisplayDto
            {
                Id = source.Id,
                LocationName = source.LocationName
            };
            return clone;
        }
        private static CompanyDisplayDto CloneTenant(CompanyDisplayDto source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var clone = new CompanyDisplayDto
            {
                Id = source.Id,
                CompanyName = source.CompanyName
            };
            return clone;
        }
        private static string CloneString(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }
            string clone = string.Empty;
            clone = source;
            return clone;
        }
        private static int CloneInt(int source)
        {
            int clone = 0;
            clone = source;
            return clone;
        }
        private static bool AreEqual(InventStockTakeDto dto1, InventStockTakeDto dto2)
        {
            if (dto1 == null || dto2 == null)
            {
                return false;
            }
            bool isEqual = dto1.Description == dto2.Description &&
                           dto1.Location == dto2.Location &&
                           dto1.TenantId == dto2.TenantId &&
                           dto1.TransactionDate == dto2.TransactionDate &&
                           dto1.PersonInCharge == dto2.PersonInCharge;
            if (!isEqual)
            {
                return false;
            }
            if (dto1.InventStockTakeLines.Count != dto2.InventStockTakeLines.Count)
            {
                return false;
            }
            bool areListsEqual = dto1.InventStockTakeLines.Count == dto2.InventStockTakeLines.Count &&
                     dto1.InventStockTakeLines.All(line1 =>
                         dto2.InventStockTakeLines.Any(line2 =>
                             line1.ProductCode == line2.ProductCode &&
                             line1.ExpectedQty == line2.ExpectedQty &&
                             line1.ActualQty == line2.ActualQty &&
                             line1.UnitId == line2.UnitId));

            if (!areListsEqual)
            {
                return false;
            }
            return true;
        }
        #endregion
        #region STOCKTAKE LINE      
        private async Task InsertRow()
        {
            if (_isEditRow == false)
            {
                _lineSelected = new InventStockTakeLineDto();
                await _lineProfileGrid.InsertRow(_lineSelected); // Thêm dòng mới vào grid
                _isEditRow = true;
            }
            else
            {
                NotifyError(_localizer["You have unfinished lines"]);
                return;
            }
        }
        private async Task EditRow(InventStockTakeLineDto line)
        {
            _productSelected = line.ProductCode;
            _lineSelected = line;
            _isEditRow = true;
            await _lineProfileGrid.EditRow(line);
        }
        private async Task DeleteRow(InventStockTakeLineDto line)
        {
            StockTakeDto.InventStockTakeLines.Remove(line);
            _lineProfileGrid.CancelEditRow(line);
            await _lineProfileGrid.Reload();
        }
        private async Task SaveRow(InventStockTakeLineDto line)
        {
            // Kiểm tra nếu ProductCode không có giá trị
            if (string.IsNullOrEmpty(_lineSelected.ProductCode))
            {
                NotifyError(_localizer["Please select a product"]);
                return;
            }
            // Kiểm tra nếu dòng chưa tồn tại trong danh sách
            if (line.ProductCode != _productSelected)
            {
                if (CheckExistLine(_lineSelected)) { return; }
                var exist = await CheckExistLineinDatabase(_lineSelected);
                if (exist)
                {
                    NotifyError(_lineSelected.ProductCode + _localizer["already exists in another stocktake"]);
                    return;
                }
                // Đồng bộ dữ liệu của _lineSelected vào dòng đang chỉnh sửa
                line.Id = _lineSelected.Id;
                line.ProductCode = _lineSelected.ProductCode;
                line.StockTakeNo = StockTakeNo;
                line.UnitId = _lineSelected.UnitId;
                line.ExpectedQty = _lineSelected.ExpectedQty;
                line.ActualQty = 0;
                if (line.Id == Guid.Empty) // Nếu là dòng mới
                {
                    line.Id = Guid.NewGuid();
                    StockTakeDto.InventStockTakeLines.Add(line);
                }
                else // Nếu là dòng đã tồn tại
                {
                    var existingLine = StockTakeDto.InventStockTakeLines.FirstOrDefault(x => x.Id == line.Id);
                    if (existingLine != null)
                    {
                        // Cập nhật lại thông tin của dòng đã tồn tại
                        existingLine.ProductCode = _lineSelected.ProductCode;
                        existingLine.StockTakeNo = _lineSelected.StockTakeNo;
                        existingLine.UnitName = _lineSelected.UnitName;
                        existingLine.ExpectedQty = _lineSelected.ExpectedQty;
                        existingLine.ActualQty = 0;
                    }
                }
                // Cập nhật giao diện
                await _lineProfileGrid.UpdateRow(line);
            }
            else _lineProfileGrid.CancelEditRow(line);

            // Reset trạng thái chỉnh sửa
            _isEditRow = false;
            _lineSelected = new();
        }
        private void CancelEdit(InventStockTakeLineDto line)
        {
            _lineSelected = new();
            _isEditRow = false;
            _lineProfileGrid.CancelEditRow(line);
        }
        private async Task OnProductSelected(ProductDto pro)
        {
            var stock = await CheckStockQuantity(pro.ProductCode);
            _lineSelected.ExpectedQty = stock.Succeeded ? stock.Data : 0;
            if (!stock.Succeeded)
            {
                NotifyWarning($"{pro.ProductCode} {_localizer["does not have a stock history in the warehouse you selected"]}");
            }
            _lineSelected.ProductCode = pro.ProductCode;
            _lineSelected.ProductName = pro.ProductName;
            _lineSelected.UnitId = pro.UnitId;
            _lineSelected.UnitName = pro.UnitName;
            _lineSelected.ActualQty = 0;
            StateHasChanged();
        }
        private bool CheckExistLine(InventStockTakeLineDto line)
        {
            // Kiểm tra nếu dòng với ProductCode và StockTakeNo đã tồn tại trong danh sách
            bool isLineExist = StockTakeDto.InventStockTakeLines
                .Any(existingLine => existingLine.ProductCode == line.ProductCode);
            if (isLineExist)
            {
                NotifyError(_lineSelected.ProductName + _localizer["has already been selected by you previously"]);
            }
            return isLineExist;
        }
        private async Task<bool> CheckExistLineinDatabase(InventStockTakeLineDto line)
        {
            line.Location = StockTakeDto.Location;
            line.TenantId = StockTakeDto.TenantId;
            line.StockTakeNo = StockTakeDto.StockTakeNo;
            // Kiểm tra sự tồn tại trong cơ sở dữ liệu thông qua dịch vụ
            var response = await _inventStockTakeServices.CheckProductExistenceinStocktake(line);
            // Trả về dữ liệu kiểm tra từ dịch vụ
            return response.Data;
        }
        private void CheckStatus()
        {
            switch (StockTakeDto.Status)
            {
                case EnumInventStockTakeStatus.Create:
                    _createText = "Create";
                    _disabled = false;
                    _disabledSave = false;
                    _disabledPrint = true;
                    _disabledDetele = true;
                    _disabledAddRow = false;
                    _disabledComplete = true;
                    _disabledGotoRecording = true;
                    _disabledCreateRecording = true;
                    _disabledAutoAdd = false;

                    _visibledEditRow = true;
                    _visibledPrint = false;
                    _visibledDelete = false;
                    _visibledAddRow = true;
                    _visibledComplete = false;
                    _visibledGotoRecording = false;
                    _visibledCreateRecording = false;
                    _visibledAutoAdd = true;
                    break;
                case EnumInventStockTakeStatus.Open:
                    _createText = "Save";
                    _disabled = false;
                    _disabledSave = false;
                    _disabledPrint = true;
                    _disabledDetele = false;
                    _disabledAddRow = false;
                    _disabledComplete = true;
                    _disabledGotoRecording = true;
                    _disabledCreateRecording = false;
                    _disabledAutoAdd = false;

                    _visibledEditRow = true;
                    _visibledPrint = true;
                    _visibledDelete = true;
                    _visibledAddRow = true;
                    _visibledComplete = true;
                    _visibledGotoRecording = true;
                    _visibledCreateRecording = true;
                    _visibledAutoAdd = true;
                    break;
                case EnumInventStockTakeStatus.Blocking:
                    _createText = "Save";
                    _disabled = true;
                    _disabledSave = true;
                    _disabledPrint = true;
                    _disabledDetele = false;
                    _disabledAddRow = true;
                    _disabledComplete = false;
                    _disabledGotoRecording = false;
                    _disabledCreateRecording = false;
                    _disabledAutoAdd = true;

                    _visibledEditRow = false;
                    _visibledPrint = true;
                    _visibledDelete = true;
                    _visibledAddRow = true;
                    _visibledComplete = true;
                    _visibledGotoRecording = true;
                    _visibledCreateRecording = true;
                    _visibledAutoAdd = true;
                    break;
                case EnumInventStockTakeStatus.Completed:
                    _createText = "Save";
                    _disabled = true;
                    _disabledSave = true;
                    _disabledPrint = false;
                    _disabledDetele = true;
                    _disabledAddRow = true;
                    _disabledComplete = true;
                    _disabledGotoRecording = false;
                    _disabledCreateRecording = true;
                    _disabledAutoAdd = true;

                    _visibledEditRow = false;
                    _visibledPrint = true;
                    _visibledDelete = true;
                    _visibledAddRow = true;
                    _visibledComplete = true;
                    _visibledGotoRecording = true;
                    _visibledCreateRecording = true;
                    _visibledAutoAdd = true;
                    break;
            }
            StateHasChanged();
        }
        private void CheckPermission()
        {
            if (GlobalVariable.AuthenticationStateTask.HasPermission("Edit") || GlobalVariable.AuthenticationStateTask.HasPermission("StocktakeEdit"))
            {
                CheckStatus();
            }
            else if (GlobalVariable.AuthenticationStateTask.HasPermission("View") || GlobalVariable.AuthenticationStateTask.HasPermission("StocktakeView"))
            {
                _visibledEditRow = false;
                _visibledPrint = false;
                _visibledDelete = false;
                _visibledAddRow = false;
                _visibledComplete = false;
                _visibledGotoRecording = false;
                _visibledCreateRecording = false;
                _visibledAutoAdd = false;
            }
            else
            {
                NotifyError("You do not have permission to access this functionality");
                _navigation.NavigateTo("/");
            }
        }
        private async Task<Result<double>> CheckStockQuantity(string code)
        {
            try
            {
                var productInfos = await _warehouseTranServices.GetByProductCodeInventoryInfoFlowBinLotAsync(code);
                if (productInfos.Succeeded && productInfos.Data.Details.Any())
                {
                    double stock = 0;
                    stock = productInfos.Data.Details
                        .Where(d => (string.IsNullOrEmpty(_locationSelected.Id) || d.WarehouseCode == _locationSelected.LocationName) &&
                      (_tenantSelected.Id == 0 || d.CompanyId == _tenantSelected.Id) && d.AvailableStock > 0)
                        .Sum(d => (double?)d.AvailableStock) ?? 0;
                    return stock > 0
                    ? await Result<double>.SuccessAsync(stock)
                    : await Result<double>.FailAsync(code);
                }
                return await Result<double>.FailAsync("");
            }
            catch (Exception ex)
            {
                return await Result<double>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException?.Message}");
            }
        }
        private void Loading()
        {
            _isLoading = true;
            _disabled = true;
            _disabledSave = true;
            _disabledAutoAdd = true;
            _disabledCreateRecording = true;
            _disabledAddRow = true;
            StateHasChanged();
        }
        #endregion
        // Cac ham thong bao
        #region NOTIFY
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
        #endregion
    }
}
