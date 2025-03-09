using Application.DTOs;
using Application.DTOs.Request.StockTake;
using Application.Extentions;
using Application.Models;
using Application.Services;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryInventStockTakeRecordingService : IInventStockTakeRecording
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IWarehouseTran warehouseTranService;
        private readonly ApplicationDbContext dbContext;
        public RepositoryInventStockTakeRecordingService(ApplicationDbContext _dbContext, IHttpContextAccessor _contextAccessor, IWarehouseTran _warehouseTranService)
        {
            dbContext = _dbContext;
            contextAccessor = _contextAccessor;
            warehouseTranService = _warehouseTranService;
        }
        public async Task<Result<List<InventStockTakeRecording>>> AddRangeAsync([Body] List<InventStockTakeRecording> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;

                    if (await CheckExist(item)) return await Result<List<InventStockTakeRecording>>.FailAsync("Stock take no is existed");
                }

                await dbContext.InventStockTakeRecordings.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<InventStockTakeRecording>>.SuccessAsync(model, "");
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeRecording>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeRecording>> DeleteRangeAsync([Body] List<InventStockTakeRecording> model)
        {
            try
            {
                dbContext.InventStockTakeRecordings.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventStockTakeRecording>.SuccessAsync("");
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecording>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeRecording>> DeleteAsync([Body] InventStockTakeRecording model)
        {
            try
            {
                dbContext.InventStockTakeRecordings.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventStockTakeRecording>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecording>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<List<InventStockTakeRecording>>> GetAllAsync()
        {
            try
            {
                return await Result<List<InventStockTakeRecording>>.SuccessAsync(await dbContext.InventStockTakeRecordings.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeRecording>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeRecording>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<InventStockTakeRecording>.SuccessAsync(await dbContext.InventStockTakeRecordings.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecording>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeRecording>> InsertAsync([Body] InventStockTakeRecording model)
        {
            try
            {
                //check required
                if (await CheckExist(model))
                    return await Result<InventStockTakeRecording>.FailAsync("Stock take no is existed");
                await dbContext.InventStockTakeRecordings.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventStockTakeRecording>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecording>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeRecording>> UpdateAsync([Body] InventStockTakeRecording model)
        {
            try
            {
                if (await dbContext.InventStockTakeRecordings.FindAsync(model.Id) == null)
                    return await Result<InventStockTakeRecording>.FailAsync("ID could not be found");
                dbContext.InventStockTakeRecordings.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventStockTakeRecording>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecording>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        private async Task<bool> CheckExist(InventStockTakeRecording model)
        {
            return await dbContext.InventStockTakeRecordings.AnyAsync(x => x.StockTakeNo.ToLower() == model.StockTakeNo.ToLower());
        }
        public async Task<Result<List<InventStockTakeRecordingDTO>>> GetAllDTOAsync()
        {
            try
            {
                var responseData = new List<InventStockTakeRecordingDTO>();
                var it = await dbContext.InventStockTakeRecordings.ToListAsync();

                foreach (var item in it)
                {
                    var tenantName = await dbContext.Companies
                        .Where(t => t.AuthPTenantId == item.TenantId)
                        .Select(t => t.FullName)
                        .FirstOrDefaultAsync();

                    var locationName = Guid.TryParse(item.Location, out var locationId)
                        ? await dbContext.Locations.Where(l => l.Id == locationId).Select(l => l.LocationName).FirstOrDefaultAsync()
                        : string.Empty;

                    var personName = await dbContext.Users
                        .Where(u => u.Id == item.PersonInCharge)
                        .Select(u => u.FullName)
                        .FirstOrDefaultAsync();

                    var dto = new InventStockTakeRecordingDTO
                    {
                        Id = item.Id,
                        StockTakeNo = item.StockTakeNo,
                        RecordNo = item.RecordNo,
                        Location = item.Location,
                        PersonInCharge = item.PersonInCharge,
                        TransactionDate = item.TransactionDate,
                        Status = item.Status,
                        Remarks = item.Remarks,
                        HHTStatus = item.HHTStatus,
                        HHTInfo = item.HHTInfo,
                        TenantId = item.TenantId,
                        TenantFullName = tenantName,
                        LocationName = locationName,
                        PersonName = personName,
                        InventStockTakeRecordingLineDtos = new List<InventStockTakeRecordingLineDtos>()
                    };

                    responseData.Add(dto);
                }

                return await Result<List<InventStockTakeRecordingDTO>>.SuccessAsync(responseData);
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeRecordingDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException?.Message}");
            }
        }
        public async Task<Result<InventStockTakeRecordingDTO>> GetByIdDTOAsync(Guid id)
        {
            try
            {
                var stockTakeRecordingData = await dbContext.InventStockTakeRecordings.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (stockTakeRecordingData == null)
                {
                    return await Result<InventStockTakeRecordingDTO>.FailAsync("The specified ID could not be found");
                }
                else
                {
                    // get cac field bo xung
                    var LocationName = string.Empty;
                    var TenantName = await dbContext.TenantAuth.Where(t => t.TenantId == stockTakeRecordingData.TenantId).Select(t => t.TenantFullName).FirstOrDefaultAsync();
                    var PersonName = await dbContext.Users.Where(t => t.Id == stockTakeRecordingData.PersonInCharge).Select(t => t.FullName).FirstOrDefaultAsync();
                    if (Guid.TryParse(stockTakeRecordingData.Location, out var locationId))
                    {
                        LocationName = await dbContext.Locations
                            .Where(l => l.Id == locationId)
                            .Select(l => l.LocationName)
                            .FirstOrDefaultAsync() ?? string.Empty;
                    }
                    var dto = new InventStockTakeRecordingDTO
                    (
                        stockTakeRecordingData,
                        LocationName, TenantName ?? "", PersonName ?? "",
                        new List<InventStockTakeRecordingLineDtos>()
                    );
                    // Lấy danh sách InventStockTakeRecordingLines và chuyển đổi sang DTO
                    var stockTakeRecordingLines = await dbContext.InventStockTakeRecordingLines
                        .Where(x => x.StockTakeRecordingId == id)
                        .ToListAsync();

                    foreach (var stockTakeRecordingLine in stockTakeRecordingLines)
                    {
                        var product = await dbContext.Products.Where(p => p.ProductCode == stockTakeRecordingLine.ProductCode).Select(p => p.ProductName).FirstOrDefaultAsync();
                        var unit = await dbContext.Units.Where(u => u.Id == stockTakeRecordingLine.UnitId).Select(u => u.UnitName).FirstOrDefaultAsync();
                        var lineDto = new InventStockTakeRecordingLineDtos
                        {
                            Id = stockTakeRecordingLine.Id,
                            StockTakeRecordingId = stockTakeRecordingLine.StockTakeRecordingId,
                            ProductCode = stockTakeRecordingLine.ProductCode ?? "",
                            ProductName = product ?? "",
                            Unit = unit ?? "",
                            LotNo = stockTakeRecordingLine.LotNo ?? "",
                            Bin = stockTakeRecordingLine.Bin ?? "",
                            ExpectedQty = stockTakeRecordingLine.ExpectedQty,
                            ActualQty = stockTakeRecordingLine.ActualQty,
                            Remaining = stockTakeRecordingLine.ExpectedQty - stockTakeRecordingLine.ActualQty,
                        };
                        dto.InventStockTakeRecordingLineDtos.Add(lineDto);
                    }
                    return await Result<InventStockTakeRecordingDTO>.SuccessAsync(dto);
                }
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecordingDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException?.Message}");
            }
        }
        public async Task<Result<List<InventStockTakeRecordingDTO>>> GetListByStockTakeNoDTOAsync(string stockTakeNo)
        {
            try
            {
                // Lấy danh sách các bản ghi từ cơ sở dữ liệu dựa trên StockTakeNo
                var recordings = await dbContext.InventStockTakeRecordings
                    .Where(x => x.StockTakeNo == stockTakeNo)
                    .ToListAsync();

                // Kiểm tra nếu không có bản ghi nào
                if (!recordings.Any())
                {
                    return await Result<List<InventStockTakeRecordingDTO>>.FailAsync("The transfer No could not be found");
                }

                // Tạo danh sách để lưu trữ các DTO
                var responseDataList = new List<InventStockTakeRecordingDTO>();

                // Lặp qua các bản ghi và ánh xạ chúng sang DTO
                foreach (var item in recordings)
                {
                    var tenantName = await dbContext.TenantAuth
                        .Where(t => t.TenantId == item.TenantId)
                        .Select(t => t.TenantFullName)
                        .FirstOrDefaultAsync();

                    var locationName = Guid.TryParse(item.Location, out var locationId)
                        ? await dbContext.Locations.Where(l => l.Id == locationId).Select(l => l.LocationName).FirstOrDefaultAsync()
                        : string.Empty;

                    var personName = await dbContext.Users
                        .Where(u => u.Id == item.PersonInCharge)
                        .Select(u => u.FullName)
                        .FirstOrDefaultAsync();

                    var dto = new InventStockTakeRecordingDTO
                    {
                        Id = item.Id,
                        StockTakeNo = item.StockTakeNo,
                        RecordNo = item.RecordNo,
                        Location = item.Location,
                        PersonInCharge = item.PersonInCharge,
                        TransactionDate = item.TransactionDate,
                        Status = item.Status,
                        Remarks = item.Remarks,
                        HHTStatus = item.HHTStatus,
                        HHTInfo = item.HHTInfo,
                        TenantId = item.TenantId,
                        TenantFullName = tenantName,
                        LocationName = locationName,
                        PersonName = personName,
                        InventStockTakeRecordingLineDtos = new List<InventStockTakeRecordingLineDtos>() // Bạn có thể thêm các dòng cụ thể nếu cần
                    };
                    responseDataList.Add(dto);
                }

                return await Result<List<InventStockTakeRecordingDTO>>.SuccessAsync(responseDataList);
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeRecordingDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException?.Message}");
            }
        }
        public async Task<Result<List<InventStockTakeRecordingDTO>>> GetStockTakeRecordingAsync(StockTakeRecordingSearchRequestDto model)
        {
            try
            {
                // Lấy danh sách các bản ghi từ cơ sở dữ liệu
                var stockTakeRecordings = await dbContext.InventStockTakeRecordings
                    .Select(item => new
                    {
                        Original = item,
                        Location = item.Location,
                        PersonInCharge = item.PersonInCharge
                    })
                    .ToListAsync();

                // Chuyển đổi bản ghi
                var convertedStockTakeRecordings = stockTakeRecordings.Select(x =>
                {
                    var stockTakeRecording = x.Original;

                    if (Guid.TryParse(x.Location, out Guid locationGuid))
                    {
                        var locationName = dbContext.Locations
                            .Where(l => l.Id == locationGuid)
                            .Select(l => l.LocationName)
                            .FirstOrDefault();
                        stockTakeRecording.Location = locationName;
                    }

                    var userName = dbContext.Users
                        .Where(u => u.Id == x.PersonInCharge)
                        .Select(u => u.FullName)
                        .FirstOrDefault();
                    stockTakeRecording.PersonInCharge = userName;

                    return stockTakeRecording;
                }).ToList();

                // Lọc bản ghi dựa trên các tiêu chí tìm kiếm
                if (!string.IsNullOrEmpty(model.StockTakeNo))
                {
                    convertedStockTakeRecordings = convertedStockTakeRecordings
                        .Where(p => p.StockTakeNo.Contains(model.StockTakeNo))
                        .ToList();
                }

                if (!string.IsNullOrEmpty(model.Location))
                {
                    convertedStockTakeRecordings = convertedStockTakeRecordings
                        .Where(p => p.Location.Contains(model.Location))
                        .ToList();
                }
                if (model.Tenant != 0 && model.Tenant != null)
                {
                    convertedStockTakeRecordings = convertedStockTakeRecordings
                        .Where(p => p.TenantId == model.Tenant)
                        .ToList();
                }

                if (!string.IsNullOrEmpty(model.Bin))
                {
                    convertedStockTakeRecordings = convertedStockTakeRecordings
                        .Where(p => dbContext.InventStockTakeRecordingLines
                            .Any(pl => pl.Bin == model.Bin && pl.StockTakeRecordingId == p.Id))
                        .ToList();
                }

                var filteredStockTakeRecordings = convertedStockTakeRecordings.AsEnumerable();
                if (model.TransactionDateFrom != null)
                {
                    filteredStockTakeRecordings = filteredStockTakeRecordings
                        .Where(p => p.TransactionDate >= model.TransactionDateFrom);
                }

                if (model.TransactionDateTo != null)
                {
                    filteredStockTakeRecordings = filteredStockTakeRecordings
                        .Where(p => p.TransactionDate <= model.TransactionDateTo);
                }

                if (model.Status.HasValue)
                {
                    filteredStockTakeRecordings = filteredStockTakeRecordings
                        .Where(p => p.Status == model.Status.Value);
                }

                // Tạo danh sách DTO
                var dtos = new List<InventStockTakeRecordingDTO>();

                foreach (var item in filteredStockTakeRecordings)
                {
                    var tenant = await dbContext.TenantAuth.FirstOrDefaultAsync(x => x.TenantId == item.TenantId);
                    Location location = new Location();
                    if (Guid.TryParse(item.Location, out var locationId))
                    {
                        location = await dbContext.Locations.FirstOrDefaultAsync(l => l.Id == locationId);
                    }
                    var person = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == item.PersonInCharge);
                    var dto = new InventStockTakeRecordingDTO
                    {
                        Id = item.Id,
                        StockTakeNo = item.StockTakeNo,
                        RecordNo = item.RecordNo,
                        Location = item.Location,
                        PersonInCharge = item.PersonInCharge,
                        TransactionDate = item.TransactionDate,
                        Status = item.Status,
                        Remarks = item.Remarks,
                        HHTStatus = item.HHTStatus,
                        HHTInfo = item.HHTInfo,
                        TenantId = item.TenantId,
                        TenantFullName = tenant?.TenantFullName ?? string.Empty,
                        LocationName = location?.LocationName ?? string.Empty,
                        PersonName = person?.FullName ?? string.Empty,
                        InventStockTakeRecordingLineDtos = new List<InventStockTakeRecordingLineDtos>() // Bạn có thể thêm các dòng cụ thể nếu cần
                    };
                    dtos.Add(dto);
                }

                return await Result<List<InventStockTakeRecordingDTO>>.SuccessAsync(dtos);
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeRecordingDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException?.Message}");
            }
        }
        public async Task<Result> CreateStockTakeRecordingAsync([FromBody] InventStockTakeWithDetailsDTO model)
        {
            try
            {
                if (model == null || model.InventStockTakeDto == null || model.InventStockTakeLineDto == null)
                {
                    return await Result.FailAsync("Invalid data");
                }

                Guid newId = Guid.NewGuid();
                // Kiểm tra xem đã có bản ghi nào với số tồn kho này chưa
                var StockTakeAlready = dbContext.InventStockTakeRecordings
                    .Where(p => p.StockTakeNo == model.InventStockTakeDto.StockTakeNo)
                    .ToList();

                // Kiểm tra xem có bản ghi nào với trạng thái khác `InProcess` không
                var invalidStatusRecord = StockTakeAlready
                    .FirstOrDefault(p => p.Status == EnumInvenTransferStatus.InProcess);

                if (invalidStatusRecord != null)
                {
                    return await Result.FailAsync("Stock take already has another record in progress");
                }

                int recordNo = StockTakeAlready.Count > 0 ? StockTakeAlready.Count + 1 : 1;
                // Fetch the user information from the context
                var userInfo = contextAccessor?.HttpContext?.User.FindFirst("UserId");

                var StockTake = new InventStockTakeRecording
                {
                    Id = newId,
                    StockTakeNo = model.InventStockTakeDto.StockTakeNo,
                    RecordNo = recordNo,
                    Remarks = string.IsNullOrEmpty(model.InventStockTakeDto.Description) ? "..." : model.InventStockTakeDto.Description,
                    Location = model.InventStockTakeDto.Location,
                    TenantId = model.InventStockTakeDto.TenantId,
                    TransactionDate = model.InventStockTakeDto.TransactionDate.HasValue
                                ? DateOnly.FromDateTime(model.InventStockTakeDto.TransactionDate.Value)
                                : DateOnly.MinValue,
                    PersonInCharge = model.InventStockTakeDto.PersonInCharge,
                    Status = EnumInvenTransferStatus.InProcess,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value
                };

                dbContext.InventStockTakeRecordings.Add(StockTake);

                foreach (var line in model.InventStockTakeLineDto)
                {
                    string productCode = line.ProductCode ?? string.Empty;
                    InventoryInfoBinLotResponseDTO? inventoryInfo = null;

                    // Nếu có mã sản phẩm, gọi dịch vụ lấy thông tin tồn kho
                    if (!string.IsNullOrWhiteSpace(productCode))
                    {
                        var response = await warehouseTranService.GetByProductCodeInventoryInfoFlowBinLotAsync(productCode);

                        if (!response.Succeeded || response.Data == null)
                        {
                            return await Result.FailAsync("No inventory data found for product code");
                        }
                        else
                        {
                            inventoryInfo = response.Data;
                            var location = dbContext.Locations.FirstOrDefault(l => l.Id == Guid.Parse(model.InventStockTakeDto.Location ?? ""));

                            var filteredDetails = new List<InventoryInfoBinLotDetails>();

                            if (location != null)
                            {
                                filteredDetails = inventoryInfo.Details.Where(detail => detail.WarehouseCode == location.LocationName && detail.CompanyId == model.InventStockTakeDto.TenantId).ToList();
                            }

                            // Duyệt qua từng chi tiết tồn kho và tạo bản ghi cho từng dòng
                            foreach (var detailLine in filteredDetails)
                            {
                                if (detailLine.AvailableStock > 0)
                                {
                                    var lineEntity = new InventStockTakeRecordingLine
                                    {
                                        StockTakeRecordingId = newId,
                                        StockTakeNo = model.InventStockTakeDto.StockTakeNo,
                                        RecordNo = recordNo,
                                        ProductCode = productCode,
                                        TenantId = detailLine.CompanyId ?? 0,
                                        Bin = detailLine.BinCode ?? "",
                                        LotNo = detailLine.LotNo ?? "",
                                        ExpectedQty = detailLine.AvailableStock,
                                        ActualQty = 0,
                                        UnitId = line.UnitId,
                                        Status = EnumInvenTransferStatus.InProcess,
                                        CreateOperatorId = line.CreateOperatorId,
                                        CreateAt = DateTime.Now
                                    };
                                    dbContext.InventStockTakeRecordingLines.Add(lineEntity);
                                }
                            }
                        }
                    }
                }
                var _stockTake = dbContext.InventStockTakes.Where(i => i.StockTakeNo == model.InventStockTakeDto.StockTakeNo).FirstOrDefault();
                if (_stockTake != null)
                {
                    _stockTake.Status = EnumInventStockTakeStatus.Blocking;
                }
                await dbContext.SaveChangesAsync();

                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ?
                    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}" : ex.Message;
                return await Result.FailAsync(errorMessage);
            }
        }
        public async Task<Result> DeleteStockTakeRecordingAsync([FromRoute] Guid StockTakeRecordingId)
        {
            try
            {
                var errorMessages = new List<string>();
                var StocktakeNo = string.Empty;
                // remove picking
                var StockTakeRecording = await dbContext.InventStockTakeRecordings
                    .FirstOrDefaultAsync(x => x.Id == StockTakeRecordingId);
                if (StockTakeRecording == null)
                {
                    return await Result.FailAsync("No found with the specified");
                }
                StocktakeNo = StockTakeRecording.StockTakeNo;
                dbContext.InventStockTakeRecordings.Remove(StockTakeRecording);

                //remove picking line
                var StockTakeRecordingLines = dbContext.InventStockTakeRecordingLines
                    .Where(p => p.StockTakeRecordingId == StockTakeRecordingId).ToList();
                if (!StockTakeRecordingLines.Any())
                {
                    errorMessages.Add("No picking line found with the specified pick no");
                }
                dbContext.InventStockTakeRecordingLines.RemoveRange(StockTakeRecordingLines);

                if (errorMessages.Any())
                {
                    foreach (var error in errorMessages)
                    {
                        Console.WriteLine(error);
                    }
                }
                // check recording count of stock take
                var recordings = await dbContext.InventStockTakeRecordings.Where(x => x.StockTakeNo == StocktakeNo && x.Id != StockTakeRecordingId).ToListAsync();
                if (!recordings.Any())
                {
                    var stocktake = dbContext.InventStockTakes.Where(x => x.StockTakeNo == StocktakeNo).FirstOrDefault();
                    if (stocktake != null)
                    {
                        stocktake.Status = EnumInventStockTakeStatus.Open;
                    }
                }
                // save changes
                await dbContext.SaveChangesAsync();
                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ?
                    $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}" : ex.Message;
                return await Result.FailAsync(errorMessage);
            }
        }
        public async Task<Result> CompleteStockTakeRecordingAsync([FromRoute] Guid StockTakeRecordingId)
        {
            try
            {
                var messages = new List<string>();
                // Tìm đối tượng StockTakeRecording
                var StockTakeRecording = await dbContext.InventStockTakeRecordings
                    .FirstOrDefaultAsync(x => x.Id == StockTakeRecordingId);

                if (StockTakeRecording == null)
                {
                    return await Result.FailAsync("Stock take recording not found");
                }

                // Lấy danh sách StockTakeLines dựa vào StockTakeNo
                var StockTakeLines = await dbContext.InventStockTakeLines
                    .Where(x => x.StockTakeNo == StockTakeRecording.StockTakeNo)
                    .ToListAsync();

                if (!StockTakeLines.Any())
                {
                    return await Result.FailAsync("No stock take lines found");
                }
                else
                {
                    // Lấy RecordNo va StockTakeNo từ StockTakeRecording
                    var targetRecordNo = StockTakeRecording.RecordNo;
                    var StockTakeNo = StockTakeRecording.StockTakeNo;

                    // Lấy danh sách StockTakeRecordingLines theo ProductCode và RecordNo khớp với StockTakeRecording
                    var productCodes = StockTakeLines.Select(line => line.ProductCode).ToList();
                    var allStockTakeRecordingLines = await dbContext.InventStockTakeRecordingLines
                        .Where(x => productCodes.Contains(x.ProductCode) && x.RecordNo == targetRecordNo && x.StockTakeNo == StockTakeNo)
                        .ToListAsync();

                    if (!allStockTakeRecordingLines.Any())
                    {
                        messages.Add("No stock take recording lines found");
                    }
                    else
                    {
                        // Nhóm và tính tổng ActualQty theo ProductCode
                        var groupedLines = allStockTakeRecordingLines
                            .GroupBy(line => line.ProductCode)
                            .ToDictionary(
                                g => g.Key,                          // Khóa là ProductCode
                                g => g.Sum(line => line.ActualQty ?? 0) // Giá trị là tổng ActualQty
                            );

                        // Cập nhật ActualQty trong StockTakeLines
                        foreach (var StockTakeLine in StockTakeLines)
                        {
                            if (groupedLines.TryGetValue(StockTakeLine.ProductCode, out var sum))
                            {
                                StockTakeLine.ActualQty = sum;               // Gán giá trị tổng ActualQty
                                dbContext.Entry(StockTakeLine).State = EntityState.Modified; // Đánh dấu thực thể đã thay đổi
                            }
                        }
                    }
                }
                // Cập nhật trạng thái
                StockTakeRecording.Status = EnumInvenTransferStatus.Completed;
                // Lưu thay đổi vào cơ sở dữ liệu
                await dbContext.SaveChangesAsync();
                messages.Add("Stockake recording completed successfully");
                return await Result.SuccessAsync(messages.First());
            }
            catch (Exception ex)
            {
                return await Result.FailAsync(ex.InnerException?.Message ?? "");
            }
        }
        // Lines
        public async Task<Result<List<InventStockTakeRecordingLine>>> AddRangeLineAsync([Body] List<InventStockTakeRecordingLine> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;

                    if (await CheckExistLine(item)) return await Result<List<InventStockTakeRecordingLine>>.FailAsync($"{item.StockTakeNo}|{item.ProductCode}|{item.Bin}|{item.LotNo}|{item.ExpectedQty} is existed");
                }

                await dbContext.InventStockTakeRecordingLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<InventStockTakeRecordingLine>>.SuccessAsync(model, "");
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeRecordingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeRecordingLine>> DeleteRangeLineAsync([Body] List<InventStockTakeRecordingLine> model)
        {
            try
            {
                dbContext.InventStockTakeRecordingLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventStockTakeRecordingLine>.SuccessAsync("");
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecordingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeRecordingLine>> DeleteLineAsync([Body] InventStockTakeRecordingLine model)
        {
            try
            {
                dbContext.InventStockTakeRecordingLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventStockTakeRecordingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecordingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<List<InventStockTakeRecordingLine>>> GetAllLineAsync()
        {
            try
            {
                return await Result<List<InventStockTakeRecordingLine>>.SuccessAsync(await dbContext.InventStockTakeRecordingLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeRecordingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeRecordingLine>> GetLineByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<InventStockTakeRecordingLine>.SuccessAsync(await dbContext.InventStockTakeRecordingLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecordingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeRecordingLine>> InsertLineAsync([Body] InventStockTakeRecordingLine model)
        {
            try
            {
                //check required
                if (await CheckExistLine(model))
                    if (await CheckExistLine(model))
                        return await Result<InventStockTakeRecordingLine>.FailAsync($"{model.StockTakeNo}|{model.ProductCode}|{model.Bin}|{model.LotNo}|{model.ExpectedQty} is existed");
                await dbContext.InventStockTakeRecordingLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventStockTakeRecordingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecordingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeRecordingLine>> UpdateLineAsync([Body] InventStockTakeRecordingLine model)
        {
            try
            {
                //check required
                if (await dbContext.InventStockTakeRecordingLines.FindAsync(model.Id) == null)
                    return await Result<InventStockTakeRecordingLine>.FailAsync("ID could not be found");
                dbContext.InventStockTakeRecordingLines.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<InventStockTakeRecordingLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<InventStockTakeRecordingLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        private async Task<bool> CheckExistLine(InventStockTakeRecordingLine model)
        {
            return await dbContext.InventStockTakeRecordingLines.AnyAsync(x => x.StockTakeNo.ToLower() == model.StockTakeNo.ToLower()
                                                                            && x.ProductCode == model.ProductCode
                                                                            && x.Bin == model.Bin
                                                                            && x.LotNo == model.LotNo
                                                                            && x.ExpectedQty == model.ExpectedQty);
        }
        public async Task<Result<List<InventStockTakeRecordingLine>>> GetLineByStockTakeNoDTOAsync([Path] string StockTakeNo)
        {
            try
            {
                var response = await dbContext.InventStockTakeRecordingLines.Where(x => x.StockTakeNo == StockTakeNo).ToListAsync();

                return await Result<List<InventStockTakeRecordingLine>>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeRecordingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<List<InventStockTakeRecordingLine>>> GetLineByStockTakeRecordingIdAsync([Path] Guid StockTakeRecordingId)
        {
            try
            {
                var response = await dbContext.InventStockTakeRecordingLines.Where(x => x.StockTakeRecordingId == StockTakeRecordingId).ToListAsync();

                return await Result<List<InventStockTakeRecordingLine>>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeRecordingLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result> UpdateStockTakeRecordingLinesAsync(List<InventStockTakeRecordingLineDtos> models)
        {
            try
            {
                foreach (var model in models)
                {
                    var result = await dbContext.InventStockTakeRecordingLines
                        .FirstOrDefaultAsync(x => x.Id == model.Id);

                    if (result != null)
                    {
                        result.ActualQty = model.ActualQty;
                        dbContext.InventStockTakeRecordingLines.Update(result);
                    }
                }

                await dbContext.SaveChangesAsync();

                return await Result.SuccessAsync();
            }
            catch (Exception ex)
            {
                return await Result.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
