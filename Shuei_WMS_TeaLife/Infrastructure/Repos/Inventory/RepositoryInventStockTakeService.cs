using Application.DTOs;
using Application.DTOs.Request;
using Application.DTOs.Request.StockTake;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services.Base;
using Application.Services.Inventory;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Infrastructure.Comparer;
using Infrastructure.Data;
using Infrastructure.Validators;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System.Security.Policy;

namespace Infrastructure.Repos.Inventory
{
    public class RepositoryInventStockTakeService : IInventStockTake
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHttpContextAccessor contextAccessor;

        public RepositoryInventStockTakeService(ApplicationDbContext _dbContext, IHttpContextAccessor _contextAccessor)
        {
            dbContext = _dbContext;
            contextAccessor = _contextAccessor;
        }
        #region unnecessary block of code
        public Task<Result<List<InventStockTakeDto>>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        Task<Result<List<InventStockTake>>> IRepository<Guid, InventStockTake>.GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<Result<InventStockTake>> GetByIdAsync([Path] Guid id)
        {
            throw new NotImplementedException();
        }
        public Task<Result<InventStockTake>> InsertAsync([Body] InventStockTake model)
        {
            throw new NotImplementedException();
        }
        public Task<Result<InventStockTake>> UpdateAsync([Body] InventStockTake model)
        {
            throw new NotImplementedException();
        }
        public Task<Result<InventStockTake>> DeleteAsync([Body] InventStockTake model)
        {
            throw new NotImplementedException();
        }
        public Task<Result<List<InventStockTake>>> AddRangeAsync([Body] List<InventStockTake> model)
        {
            throw new NotImplementedException();
        }
        public Task<Result<InventStockTake>> DeleteRangeAsync([Body] List<InventStockTake> model)
        {
            throw new NotImplementedException();
        }
        #endregion
        public async Task<Result<List<InventStockTakeDto>>> GetAll()
        {
            try
            {
                return await Result<List<InventStockTakeDto>>.SuccessAsync(await dbContext.InventStockTakes.Select(_ => new InventStockTakeDto(_)).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeDto>> GetByStockTakeNo([Path] string StockTakeNo)
        {
            try
            {
                var stockTakeQuery = from StockTake in dbContext.InventStockTakes.Where(_ => _.StockTakeNo == StockTakeNo)
                                     join pic in dbContext.Users on StockTake.PersonInCharge equals pic.Id into pics
                                     from pic in pics.DefaultIfEmpty()
                                     join location in dbContext.Locations on StockTake.Location equals location.Id.ToString() into locations
                                     from location in locations.DefaultIfEmpty()
                                     join tenant in dbContext.TenantAuth on StockTake.TenantId equals tenant.TenantId into tenants
                                     from tenant in tenants.DefaultIfEmpty()
                                     select new
                                     {
                                         StockTake,
                                         PicFullName = pic != null ? pic.FullName : "N/A",
                                         LocationName = location != null ? location.LocationName : "N/A",
                                         TenantFullName = tenant != null ? tenant.TenantFullName : "N/A"
                                     };
                var stockTakeResult = await stockTakeQuery.FirstOrDefaultAsync();
                if (stockTakeResult == null)
                {
                    return await Result<InventStockTakeDto>.FailAsync("Stock take not found");
                }
                var lines = await (from line in dbContext.InventStockTakeLines.Where(s => s.IsDeleted == false && s.StockTakeNo == StockTakeNo)
                                   join product in dbContext.Products.Where(p => p.IsDeleted == false && p.CompanyId == stockTakeResult.StockTake.TenantId) on line.ProductCode equals product.ProductCode into productLines
                                   from product in productLines.DefaultIfEmpty()
                                   join unit in dbContext.Units.Where(p => p.IsDeleted == false) on line.UnitId equals unit.Id into unitLines
                                   from unit in unitLines.DefaultIfEmpty()
                                   select new InventStockTakeLineDto()
                                   {
                                       Id = line.Id,
                                       StockTakeNo = StockTakeNo,
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
                                       ProductName = product.ProductName,
                                       UnitName = unit != null ? unit.UnitName : null, // Return null if unit not found
                                   }).ToListAsync();
                if (lines == null)
                {
                    return await Result<InventStockTakeDto>.FailAsync("No lines found");
                }
                var stockTakeDto = new InventStockTakeDto(stockTakeResult.StockTake, stockTakeResult.PicFullName ?? string.Empty,
                    stockTakeResult.LocationName ?? string.Empty, stockTakeResult.TenantFullName, lines.OrderBy(st => st.UpdateAt).ToList());
                return await Result<InventStockTakeDto>.SuccessAsync(stockTakeDto);
            }
            catch (Exception ex)
            {
                // Return error result in case of an exception
                return await Result<InventStockTakeDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeDto>> Insert([Body] InventStockTakeDto dto)
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                // Validate the DTO using a custom validator
                var validator = new StockTakeValidator();
                var result = validator.Validate(dto);
                if (!result.IsValid)
                {
                    return Result<InventStockTakeDto>.Fail(string.Join(',', result.Errors));
                }

                // Check for duplicate StockTakeNo
                bool isStockTakeExisted = await dbContext.InventStockTakes.AnyAsync(_ => _.StockTakeNo == dto.StockTakeNo);
                if (isStockTakeExisted)
                {
                    return Result<InventStockTakeDto>.Fail("This stock take already exists");
                }

                // Fetch the user information from the context
                var userInfo = contextAccessor?.HttpContext?.User.FindFirst("UserId");

                // Map DTO to entity and populate metadata
                var StockTake = dto.Adapt<InventStockTake>();
                StockTake.CreateAt = DateTime.Now;
                StockTake.CreateOperatorId = userInfo?.Value;
                StockTake.UpdateAt = DateTime.Now;
                StockTake.UpdateOperatorId = userInfo?.Value;
                StockTake.Status = EnumInventStockTakeStatus.Open;
                await dbContext.InventStockTakes.AddAsync(StockTake);

                // Insert stock take lines if any
                if (dto.InventStockTakeLines.Any())
                {
                    var StockTakeLines = dto.InventStockTakeLines.Adapt<List<InventStockTakeLine>>();
                    foreach (var StockTakeLine in StockTakeLines)
                    {
                        StockTakeLine.StockTakeNo = dto.StockTakeNo;
                        StockTakeLine.CreateAt = DateTime.Now;
                        StockTakeLine.CreateOperatorId = userInfo?.Value;
                        StockTakeLine.UpdateAt = DateTime.Now;
                        StockTakeLine.UpdateOperatorId = userInfo?.Value;
                    }
                    await dbContext.InventStockTakeLines.AddRangeAsync(StockTakeLines);
                }

                // Update sequence for the next StockTakeNo
                var sequenceIndex = await dbContext.SequencesNumber.Where(_ => _.JournalType == "Counting").FirstOrDefaultAsync();
                if (sequenceIndex != null)
                {
                    sequenceIndex.CurrentSequenceNo += 1;
                    dbContext.SequencesNumber.Update(sequenceIndex);
                }

                // Save changes and commit transaction
                await dbContext.SaveChangesAsync();
                transaction.Commit();

                return await Result<InventStockTakeDto>.SuccessAsync(dto);
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of an exception
                transaction.Rollback();
                return await Result<InventStockTakeDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<InventStockTakeDto>> Update([Body] InventStockTakeDto dto)
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                // Validate the DTO
                var validator = new StockTakeValidator();
                var result = validator.Validate(dto);
                if (!result.IsValid)
                {
                    return Result<InventStockTakeDto>.Fail(string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));
                }

                // Fetch the user information from the context
                var userInfo = contextAccessor?.HttpContext?.User.FindFirst("UserId");

                // Map DTO to entity and populate metadata
                var stockTake = dto.Adapt<InventStockTake>();
                stockTake.UpdateAt = DateTime.Now;
                stockTake.UpdateOperatorId = userInfo?.Value;
                dbContext.InventStockTakes.Update(stockTake);

                if (dto.InventStockTakeLines.Any())
                {
                    var stockTakeLines = dto.InventStockTakeLines.Adapt<List<InventStockTakeLine>>();
                    var existingLines = await dbContext.InventStockTakeLines
                                                        .Where(x => x.StockTakeNo == dto.StockTakeNo)
                                                        .ToListAsync();

                    var newLines = stockTakeLines.Where(x => !existingLines.Any(e => e.Id == x.Id)).ToList();
                    var updatedLines = stockTakeLines.Where(x => existingLines.Any(e => e.Id == x.Id &&
                                        (e.ProductCode != x.ProductCode || e.ExpectedQty != x.ExpectedQty || e.ActualQty != x.ActualQty)
                                    )).ToList();

                    foreach (var line in updatedLines)
                    {
                        var existingLine = existingLines.FirstOrDefault(x => x.Id == line.Id);
                        if (existingLine != null)
                        {
                            existingLine.ProductCode = line.ProductCode;
                            existingLine.ExpectedQty = line.ExpectedQty;
                            existingLine.ActualQty = line.ActualQty;
                            existingLine.UnitId = line.UnitId;
                            existingLine.UpdateAt = DateTime.Now;
                            existingLine.UpdateOperatorId = userInfo?.Value;
                            dbContext.Entry(existingLine).State = EntityState.Modified;
                        }
                    }

                    if (newLines.Any())
                    {
                        foreach (var newLine in newLines)
                        {
                            newLine.StockTakeNo = dto.StockTakeNo;
                            newLine.CreateAt = DateTime.Now;
                            newLine.CreateOperatorId = userInfo?.Value;
                            newLine.UpdateAt = DateTime.Now;
                            newLine.UpdateOperatorId = userInfo?.Value;
                        }
                        dbContext.InventStockTakeLines.AddRange(newLines);
                    }

                    var deletedStockTakeLines = existingLines
                        .Where(x => !stockTakeLines.Any(e => e.Id == x.Id))
                        .ToList();
                    if (deletedStockTakeLines.Any())
                    {
                        dbContext.InventStockTakeLines.RemoveRange(deletedStockTakeLines);
                    }
                }
                else
                {
                    var allline = await dbContext.InventStockTakeLines.Where(x => x.StockTakeNo == dto.StockTakeNo).ToListAsync();
                    if (allline.Any())
                    {
                        dbContext.InventStockTakeLines.RemoveRange(allline);
                    }
                }
                await dbContext.SaveChangesAsync();
                transaction.Commit();

                return await Result<InventStockTakeDto>.SuccessAsync(dto);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<InventStockTakeDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException?.Message}");
            }
        }
        public async Task<Result<bool>> Delete([Path] Guid id)
        {
            using var transaction = dbContext.Database.BeginTransaction();

            try
            {
                // Find the stock take by ID
                var stockTake = await dbContext.InventStockTakes.FirstOrDefaultAsync(_ => _.Id == id);
                if (stockTake == null) return Result<bool>.Fail("This stock take does not exist");

                // Check if there are any completed StocktakeRecordings
                var completedRecordingExists = await dbContext.InventStockTakeRecordings
                                                              .AnyAsync(r => r.StockTakeNo == stockTake.StockTakeNo && r.Status == EnumInvenTransferStatus.Completed);
                if (completedRecordingExists)
                {
                    return Result<bool>.Fail("Cannot delete because there are completed recordings");
                }

                // Remove stock take and its associated lines
                dbContext.InventStockTakes.Remove(stockTake);
                await dbContext.InventStockTakeLines.Where(_ => _.StockTakeNo == stockTake.StockTakeNo).ExecuteDeleteAsync();

                // Remove related records from StocktakeRecording and StocktakeRecordingLine
                var relatedRecordings = await dbContext.InventStockTakeRecordings
                                                       .Where(r => r.StockTakeNo == stockTake.StockTakeNo).ToListAsync();
                dbContext.InventStockTakeRecordings.RemoveRange(relatedRecordings);

                var relatedRecordingLines = await dbContext.InventStockTakeRecordingLines
                                                       .Where(r => r.StockTakeNo == stockTake.StockTakeNo).ToListAsync();
                dbContext.InventStockTakeRecordingLines.RemoveRange(relatedRecordingLines);

                await dbContext.SaveChangesAsync();

                // Commit transaction
                transaction.Commit();
                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of an exception
                transaction.Rollback();
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException?.Message}");
            }
        }
        public async Task<Result<List<InventStockTakeDto>>> GetStockTakeAsync([Body] InventStockTakeSearchModel model)
        {
            try
            {
                // Truy vấn cơ bản với AsNoTracking để cải thiện hiệu suất
                var stockTakesQuery = dbContext.InventStockTakes
                    .AsNoTracking()
                    .AsQueryable();

                // Lọc StockTakeLines theo ProductCode nếu có
                if (!string.IsNullOrEmpty(model.ProductCode))
                {
                    var productStockTakeIds = await dbContext.InventStockTakeLines
                        .Where(line => line.ProductCode.Contains(model.ProductCode))
                        .Select(line => line.StockTakeNo)
                        .Distinct()
                        .ToListAsync();

                    stockTakesQuery = stockTakesQuery.Where(stockTake => productStockTakeIds.Contains(stockTake.StockTakeNo));
                }

                // Áp dụng các bộ lọc khác trong model
                stockTakesQuery = stockTakesQuery
                    .Where(stockTake =>
                        (string.IsNullOrEmpty(model.StockTakeNo) || stockTake.StockTakeNo.Contains(model.StockTakeNo)) &&
                        (string.IsNullOrEmpty(model.Location) || stockTake.Location.Contains(model.Location)) &&
                        (model.Tenant == null || stockTake.TenantId == model.Tenant) &&
                        (!model.StockTakeFrom.HasValue || (stockTake.TransactionDate.HasValue && DateOnly.FromDateTime(stockTake.TransactionDate.Value) >= model.StockTakeFrom)) &&
                        (!model.StockTakeTo.HasValue || (stockTake.TransactionDate.HasValue && DateOnly.FromDateTime(stockTake.TransactionDate.Value) <= model.StockTakeTo)) &&
                        (!model.Status.HasValue || stockTake.Status == model.Status));

                // Lấy dữ liệu và chuyển thành DTO
                var stockTakes = await stockTakesQuery.ToListAsync();
                var result = stockTakes.Select(stockTake => new InventStockTakeDto(stockTake, null, null, null, null)).ToList();

                // Lọc thông tin bổ sung Location, Tenant và Person
                var locationNames = await dbContext.Locations.ToDictionaryAsync(l => l.Id, l => l.LocationName);
                var tenantNames = await dbContext.Companies.ToDictionaryAsync(t => t.AuthPTenantId, t => t.FullName);
                var userNames = await dbContext.Users.ToDictionaryAsync(u => u.Id, u => u.FullName);

                // Lọc thông tin bổ sung
                result.ForEach(item =>
                {
                    if (Guid.TryParse(item.Location, out var locationGuid) && locationNames.TryGetValue(locationGuid, out var locationName))
                        item.LocationName = locationName;
                    if (tenantNames.TryGetValue(item.TenantId, out var tenantName))
                        item.TenantFullName = tenantName;
                    if (Guid.TryParse(item.PersonInCharge, out var userGuid) && userNames.TryGetValue(userGuid.ToString(), out var userName))
                        item.PersonInChargeName = userName;
                });

                return await Result<List<InventStockTakeDto>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<List<InventStockTakeDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<bool>> CompleteInventStockTake([Body] InventStockTakeDto dto)
        {
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                var warehouseTran = new List<WarehouseTran>();
                var messages = new List<string>();

                // Lấy recording được tạo cuối cùng theo ngày tạo (CreateAt)
                var latestRecording = dbContext.InventStockTakeRecordings
                    .Where(line => line.StockTakeNo == dto.StockTakeNo && line.Status == EnumInvenTransferStatus.Completed)
                    .OrderByDescending(line => line.CreateAt)
                    .FirstOrDefault();

                if (latestRecording == null)
                {
                    return await Result<bool>.FailAsync("Stock take recording not found");
                }

                // Lấy tất cả các dòng thuộc recording cuối cùng
                var latestRecordingLines = await dbContext.InventStockTakeRecordingLines
                    .Where(line => line.StockTakeNo == latestRecording.StockTakeNo && line.RecordNo == latestRecording.RecordNo)
                    .Select(line => new StockTakeRecordingLineDto(line, line.TenantId)) // Map về DTO
                    .ToListAsync();

                if (!latestRecordingLines.Any())
                {
                    return await Result<bool>.FailAsync("No recording lines found for the latest recording");
                }

                var userInfo = contextAccessor.HttpContext?.User?.Identity?.Name != null
                    ? await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name)
                    : null;

                bool isValidPayload = !dto.InventStockTakeLines
                    .Any(line => !line.ExpectedQty.HasValue || !line.ActualQty.HasValue);

                if (!isValidPayload)
                {
                    return await Result<bool>.FailAsync("Payload of stock take is invalid");
                }

                foreach (var line in latestRecordingLines)
                {
                    if (line.ExpectedQty.HasValue && line.ActualQty.HasValue)
                    {
                        double? difference = line.ActualQty - line.ExpectedQty;
                        if (difference.HasValue && difference != 0)
                        {
                            bool isOnHandGreaterThanCounting = difference > 0;
                            var tran = new WarehouseTran
                            {
                                TransType = EnumWarehouseTransType.Counting,
                                TransNumber = dto.StockTakeNo,
                                StatusReceipt = isOnHandGreaterThanCounting ? EnumStatusReceipt.Received : null,
                                StatusIssue = isOnHandGreaterThanCounting ? null : EnumStatusIssue.Delivered,
                                DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                                ProductCode = line.ProductCode,
                                Qty = difference.Value,
                                TenantId = dto.TenantId,
                                Location = dto.Location,
                                Bin = String.IsNullOrWhiteSpace(line.Bin) ? "N/A" : line.Bin,
                                LotNo = String.IsNullOrWhiteSpace(line.LotNo) ? "N/A" : line.LotNo,
                                CreateAt = DateTime.Now,
                                CreateOperatorId = userInfo?.ToString()
                            };
                            warehouseTran.Add(tran);
                        }
                    }
                }
                dto.Status = EnumInventStockTakeStatus.Completed;

                if (!dto.InventStockTakeLines.Any()) {
                    return await Result<bool>.FailAsync("Payload of stock take is invalid");
                }
                // Update all stock take lines to "Complete" status.
                foreach (var stockTakeLine in dto.InventStockTakeLines)
                {
                    var line = dbContext.InventStockTakeLines.FirstOrDefault(x => x.Id == stockTakeLine.Id);
                    if (line != null) { line.Status = EnumInventStockTakeStatus.Completed; }
                }

                if (warehouseTran.Any())
                {
                    dbContext.WarehouseTrans.AddRange(warehouseTran);
                    messages.Add("");
                }
                else
                {
                    messages.Add("Nothing has been added to the warehouse transaction");
                }

                dbContext.InventStockTakes.Update(dto.Adapt<InventStockTake>());

                await dbContext.SaveChangesAsync();
                transaction.Commit();
                return await Result<bool>.SuccessAsync(messages.First());
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<List<InventStockTakeLineDto>>> GetStockTakeLineByIdAsync([Path] string StockTakeNo)
        {
            try
            {
                // Retrieve StockTakeLine records from the database based on the StockTakeNo.
                var response = await dbContext.InventStockTakeLines
                    .Where(x => x.StockTakeNo == StockTakeNo)
                    .ToListAsync();

                // Check if no records were found.
                if (response == null)
                {
                    return await Result<List<InventStockTakeLineDto>>.FailAsync("Stock take line not found");
                }

                var results = new List<InventStockTakeLineDto>();

                // Iterate over each StockTakeLine record to map additional data.
                foreach (var item in response)
                {
                    // Fetch the product name based on ProductCode.
                    var productName = dbContext.Products
                        .Where(p => p.ProductCode == item.ProductCode)
                        .Select(p => p.ProductName)
                        .FirstOrDefault();

                    // Fetch the unit name based on UnitId.
                    var unitName = dbContext.Units
                        .Where(u => u.Id == item.UnitId)
                        .Select(u => u.UnitName)
                        .FirstOrDefault();

                    // Map data into a DTO.
                    var result = new InventStockTakeLineDto(
                        item,
                        productName ?? "", // Default to empty string if product name is null.
                        unitName ?? "" // Default to empty string if unit name is null.
                    );

                    // Add the DTO to the result list.
                    results.Add(result);
                }

                // Return the successful result with the mapped data.
                return await Result<List<InventStockTakeLineDto>>.SuccessAsync(results);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return a failure result with the error details.
                return await Result<List<InventStockTakeLineDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<bool>> CheckProductExistenceinStocktake([FromBody] InventStockTakeLineDto inventStockTakeLineDto)
        {
            try
            {
                // Get stocktakes based on location.
                var stocktakes = await dbContext.InventStockTakes
                    .Where(x => x.Location == inventStockTakeLineDto.Location && x.TenantId == inventStockTakeLineDto.TenantId && x.StockTakeNo != inventStockTakeLineDto.StockTakeNo)
                    .ToListAsync();

                // Extract the stocktake IDs.
                var stocktakeNos = stocktakes.Select(x => x.StockTakeNo).ToList();

                // Check if a product with the given ProductCode already exists in the filtered InventStockTakeLines.
                var exists = await dbContext.InventStockTakeLines
                    .AnyAsync(x => stocktakeNos.Contains(x.StockTakeNo) &&
                                   x.ProductCode == inventStockTakeLineDto.ProductCode &&
                                   x.Status != EnumInventStockTakeStatus.Completed);

                // Return the result indicating whether the product exists.
                return await Result<bool>.SuccessAsync(exists);
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return a failure result with the error details.
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        public async Task<Result<string>> GetNewStocktakeNo()
        {
            try
            {
                // Tìm sequence theo JournalType
                var sequenceIndex = await dbContext.SequencesNumber
                    .FirstOrDefaultAsync(seq => seq.JournalType == "Counting");

                if (sequenceIndex == null)
                {
                    return await Result<string>.FailAsync("Sequence not found");
                }
                // Tạo StockTakeNo mới
                var nextOrderNo = "ST" + ((sequenceIndex.CurrentSequenceNo ?? 0) + 1).ToString("D6");
                return await Result<string>.SuccessAsync(nextOrderNo, "Generated successfully");
            }
            catch (Exception ex)
            {
                // Trả về lỗi với thông tin chi tiết
                var errorMessage = $"{ex.Message}{Environment.NewLine}{ex.InnerException?.Message ?? "No inner exception"}";
                return await Result<string>.FailAsync(errorMessage);
            }
        }
        public async Task<Result<string>> GetProductByLocation()
        {
            try
            {
                // Tìm sequence theo JournalType
                var sequenceIndex = await dbContext.SequencesNumber
                    .FirstOrDefaultAsync(seq => seq.JournalType == "Counting");

                if (sequenceIndex == null)
                {
                    return await Result<string>.FailAsync("Sequence not found");
                }
                // Tạo StockTakeNo mới
                var nextOrderNo = "ST" + ((sequenceIndex.CurrentSequenceNo ?? 0) + 1).ToString("D6");
                return await Result<string>.SuccessAsync(nextOrderNo, "Generated successfully");
            }
            catch (Exception ex)
            {
                // Trả về lỗi với thông tin chi tiết
                var errorMessage = $"{ex.Message}{Environment.NewLine}{ex.InnerException?.Message ?? "No inner exception"}";
                return await Result<string>.FailAsync(errorMessage);
            }
        }
        public async Task<Result<string>> GetCurrentUserAsync()
        {
            try
            {
                var userInfo = contextAccessor?.HttpContext?.User.FindFirst("UserId");
                return await Result<string>.SuccessAsync(userInfo?.Value ?? "", "successfully");
            }
            catch (Exception ex)
            {
                // Trả về lỗi với thông tin chi tiết
                var errorMessage = $"{ex.Message}{Environment.NewLine}{ex.InnerException?.Message ?? "No inner exception"}";
                return await Result<string>.FailAsync(errorMessage);
            }
        }
    }
}
