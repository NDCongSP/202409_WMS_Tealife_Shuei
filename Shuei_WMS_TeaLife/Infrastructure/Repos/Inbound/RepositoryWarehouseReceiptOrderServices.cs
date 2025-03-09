using Application.DTOs;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services;
using Application.Services.Inbound;
using Azure.Core;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.ExtendedProperties;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Radzen;
using RestEase;
using System.Linq;
using System.Linq.Dynamic.Core;
using WebUIFinal.Core;
using WarehouseReceiptOrder = FBT.ShareModels.WMS.WarehouseReceiptOrder;
using WarehouseReceiptOrderLine = FBT.ShareModels.WMS.WarehouseReceiptOrderLine;
using WarehouseTran = FBT.ShareModels.WMS.WarehouseTran;

namespace Infrastructure.Repos
{
    public class RepositoryWarehouseReceiptOrderServices : IWarehouseReceiptOrder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INumberSequences _numberSequences;
        private readonly IWarehouseTran _warehouseTranService;
        private readonly IWarehousePutAway _warehousePutAwayServices;
        private readonly DateTime now = DateTime.Now;

        public RepositoryWarehouseReceiptOrderServices(
            ApplicationDbContext dbContext,
            IHttpContextAccessor contextAccessor,
            INumberSequences numberSequences,
            IWarehouseTran warehouseTranService,
            IWarehousePutAway warehousePutAwayServices)
        {
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
            _numberSequences = numberSequences;
            _warehouseTranService = warehouseTranService;
            _warehousePutAwayServices = warehousePutAwayServices;
        }

        public async Task<Result<List<WarehouseReceiptOrder>>> AddRangeAsync([Body] List<WarehouseReceiptOrder> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == _contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;
                }

                await _dbContext.WarehouseReceiptOrders.AddRangeAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<List<WarehouseReceiptOrder>>.SuccessAsync(model, "Add range WarehouseReceiptOrder successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrder>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> DeleteRangeAsync([Body] List<WarehouseReceiptOrder> model)
        {
            try
            {
                _dbContext.WarehouseReceiptOrders.RemoveRange(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrder>.SuccessAsync("Delete range WarehouseReceiptOrder successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> DeleteAsync([Body] WarehouseReceiptOrder model)
        {
            if (model.Status != EnumReceiptOrderStatus.Completed)
            {
                try
                {
                    _dbContext.WarehouseReceiptOrders.Remove(model);
                    await _dbContext.SaveChangesAsync();
                    return await Result<WarehouseReceiptOrder>.SuccessAsync(model);
                }
                catch (Exception ex)
                {
                    return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                }
            }
            else
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"This receipt cannot be deleted as its closed");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrder>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseReceiptOrder>>.SuccessAsync(await _dbContext.WarehouseReceiptOrders.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrder>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseReceiptOrder>.SuccessAsync(await _dbContext.WarehouseReceiptOrders.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> InsertAsync([Body] WarehouseReceiptOrder model)
        {
            try
            {
                await _dbContext.WarehouseReceiptOrders.AddAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrder>> UpdateAsync([Body] WarehouseReceiptOrder model)
        {
            try
            {
                _dbContext.WarehouseReceiptOrders.Update(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrder>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            try
            {
                return await Result<List<WarehouseReceiptOrder>>.SuccessAsync(await _dbContext.WarehouseReceiptOrders.Where(x => x.ReceiptNo == receiptNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrder>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> InsertWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request)
        {
            try
            {
                await AdjustInsertStatusByReferenceType(request);

                var sequenceIndex = _dbContext.SequencesNumber.Where(_ => _.JournalType == "Receipt").FirstOrDefaultAsync();

                if (sequenceIndex == null)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync($"ReceiptNo's prefix does not exist");
                }

                var seqResult = sequenceIndex.Result;
                var receiptIndex = $"{seqResult?.Prefix}{seqResult?.CurrentSequenceNo?.ToString().PadLeft((int)seqResult.SequenceLength, '0')}";

                if (string.IsNullOrEmpty(receiptIndex))
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync($"ReceiptNo's prefix is not valid");
                }

                var model = new WarehouseReceiptOrder
                {
                    Id = request.Id,
                    ReceiptNo = receiptIndex,
                    Location = request.Location,
                    ExpectedDate = request.ExpectedDate,
                    TenantId = request.TenantId,
                    ScheduledArrivalNumber = request.ScheduledArrivalNumber,
                    DocumentNo = request.DocumentNo,
                    SupplierId = request.SupplierId,
                    PersonInCharge = request.PersonInCharge,
                    ConfirmedBy = request.ConfirmedBy,
                    ConfirmedDate = request.ConfirmedDate,
                    Status = EnumReceiptOrderStatus.Draft,
                    CreateAt = now,
                    ReferenceNo = request.ReferenceNo,
                    ReferenceType = request.ReferenceType,
                };

                await _dbContext.WarehouseReceiptOrders.AddAsync(model);
                seqResult.CurrentSequenceNo += 1;
                await _numberSequences.UpdateAsync(seqResult);

                var receiptOrderLines = request.WarehouseReceiptOrderLines.Select(_ => new WarehouseReceiptOrderLine
                {
                    Id = _.Id,
                    ReceiptNo = receiptIndex,
                    ProductCode = _.ProductCode,
                    UnitName = _.UnitName,
                    OrderQty = _.OrderQty,
                    TransQty = _.TransQty,
                    Bin = _.Bin,
                    LotNo = _.LotNo,
                    ExpirationDate = _.ExpirationDate,
                    Putaway = _.Putaway,
                    UnitId = _.UnitId,
                    CreateAt = now,
                });
                await _dbContext.WarehouseReceiptOrderLines.AddRangeAsync(receiptOrderLines);

                await _dbContext.SaveChangesAsync();
                request.ReceiptNo = receiptIndex;

                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> UpdateWarehouseReceiptOrder([Body] WarehouseReceiptOrderDto request)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var receipt = await _dbContext.WarehouseReceiptOrders.Select(_ => _).AsNoTracking().FirstOrDefaultAsync(_ => _.ReceiptNo == request.ReceiptNo);
                if (receipt == null)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("This receipt does not exist");
                }

                if (receipt.Status == EnumReceiptOrderStatus.Draft)
                {
                    receipt.Id = request.Id;
                    receipt.ReceiptNo = request.ReceiptNo;
                    receipt.Location = request.Location;
                    receipt.ExpectedDate = request.ExpectedDate;
                    receipt.TenantId = request.TenantId;
                    receipt.ScheduledArrivalNumber = request.ScheduledArrivalNumber;
                    receipt.DocumentNo = request.DocumentNo;
                    receipt.SupplierId = request.SupplierId;
                    receipt.PersonInCharge = request.PersonInCharge;
                    receipt.ConfirmedBy = request.ConfirmedBy;
                    receipt.ConfirmedDate = request.ConfirmedDate;
                    receipt.Status = EnumReceiptOrderStatus.Draft;
                    receipt.UpdateAt = now;

                    var receiptOrderLines = request.WarehouseReceiptOrderLines.Select(_ => new WarehouseReceiptOrderLine
                    {
                        Id = _.Id,
                        ReceiptNo = receipt.ReceiptNo,
                        ProductCode = _.ProductCode,
                        UnitName = _.UnitName,
                        OrderQty = _.OrderQty,
                        TransQty = _.TransQty,
                        Bin = _.Bin,
                        LotNo = _.LotNo,
                        ExpirationDate = _.ExpirationDate,
                        Putaway = _.Putaway,
                        UnitId = _.UnitId,
                        UpdateAt = now
                    }).ToList();

                    var receiptLines = await _dbContext.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == request.ReceiptNo).ToListAsync();
                    var deletedReceiptLines = receiptLines.Except(receiptOrderLines, new ReceiptLineComparer());
                    var createR = receiptOrderLines.Except(receiptLines, new ReceiptLineComparer());
                    var updateR = receiptLines.Where(x => receiptOrderLines.Select(xx => xx.Id).Contains(x.Id));

                    _dbContext.WarehouseReceiptOrderLines.RemoveRange(deletedReceiptLines);
                    _dbContext.WarehouseReceiptOrders.Update(receipt);
                    _dbContext.WarehouseReceiptOrderLines.UpdateRange(updateR);
                    if (createR.Count() > 0) _dbContext.WarehouseReceiptOrderLines.AddRange(createR);

                    await _dbContext.SaveChangesAsync();

                    transaction.Commit();

                    return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
                }
                else
                {
                    transaction.Rollback();
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("This receipt cannot be updated as it's Draft Status");
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> GetReceiptOrderAsync(string receiptNo)
        {
            try
            {
                var tenantId = _dbContext.WarehouseReceiptOrders.Where(x => x.ReceiptNo == receiptNo).FirstOrDefault().TenantId;
                var receipt = await _dbContext.WarehouseReceiptOrders.FirstOrDefaultAsync(_ => _.ReceiptNo == receiptNo && _.IsDeleted != true);
                var lines = await (from receiptLines in _dbContext.WarehouseReceiptOrderLines.Where(r => r.ReceiptNo == receiptNo && r.IsDeleted != true)
                                   join products in _dbContext.Products.Where(p => p.IsDeleted != true && p.CompanyId == tenantId) on receiptLines.ProductCode equals products.ProductCode into productReceiptLines
                                   from products in productReceiptLines
                                   select new WarehouseReceiptOrderLineDto
                                   {
                                       Id = receiptLines.Id,
                                       ReceiptNo = receiptLines.ReceiptNo,
                                       ProductCode = receiptLines.ProductCode,
                                       UnitName = receiptLines.UnitName,
                                       OrderQty = receiptLines.OrderQty,
                                       TransQty = receiptLines.TransQty,
                                       Bin = receiptLines.Bin,
                                       LotNo = receiptLines.LotNo,
                                       ExpirationDate = receiptLines.ExpirationDate,
                                       Putaway = receiptLines.Putaway,
                                       UnitId = products.UnitId,
                                       ProductName = products.ProductName,
                                       StockAvailableQuantity = products.StockAvailableQuanitty,
                                       ArrivalNo = receiptLines.ArrivalNo == default ? 0 : (int)receiptLines.ArrivalNo,
                                   }).ToListAsync();

                if (receipt != null)
                {
                    var result = new WarehouseReceiptOrderDto(receipt, lines);
                    return await Result<WarehouseReceiptOrderDto>.SuccessAsync(result);
                }
                else
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync();
                }
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrderDto>>> GetReceiptOrderListAsync()
        {
            try
            {
                var receipt = await (
                from receipts in _dbContext.WarehouseReceiptOrders.Where(_ => _.IsDeleted != true)
                join location in _dbContext.Locations.Where(_ => _.IsDeleted != true) on receipts.Location equals location.Id.ToString() into locationReceipts
                from location in locationReceipts.DefaultIfEmpty()
                join tenant in _dbContext.TenantAuth on receipts.TenantId equals tenant.TenantId into tenantReceipts
                from tenant in tenantReceipts.DefaultIfEmpty()
                join supplier in _dbContext.Suppliers on receipts.SupplierId equals supplier.Id into supplierReceipts
                from supplier in supplierReceipts.DefaultIfEmpty()
                join person in _dbContext.Users on receipts.PersonInCharge equals person.Id into personReceipts
                from person in personReceipts.DefaultIfEmpty()
                join receiptLines in (from receiptLines in _dbContext.WarehouseReceiptOrderLines.Where(r => r.IsDeleted != true)
                                      join products in _dbContext.Products.Where(p => p.IsDeleted != true) on receiptLines.ProductCode equals products.ProductCode into productReceiptLines
                                      from products in productReceiptLines
                                      select new WarehouseReceiptOrderLineDto
                                      {
                                          Id = receiptLines.Id,
                                          ReceiptNo = receiptLines.ReceiptNo,
                                          ProductCode = receiptLines.ProductCode,
                                          UnitName = receiptLines.UnitName,
                                          OrderQty = receiptLines.OrderQty,
                                          TransQty = receiptLines.TransQty,
                                          Bin = receiptLines.Bin,
                                          LotNo = receiptLines.LotNo,
                                          ExpirationDate = receiptLines.ExpirationDate,
                                          Putaway = receiptLines.Putaway,
                                          UnitId = products.UnitId,
                                          ProductName = products.ProductName,
                                          StockAvailableQuantity = products.StockAvailableQuanitty,
                                      })
                on receipts.ReceiptNo equals receiptLines.ReceiptNo into receiptOrderLines
                select new WarehouseReceiptOrderDto
                {
                    Id = receipts.Id,
                    ReceiptNo = receipts.ReceiptNo,
                    Location = receipts.Location,
                    ExpectedDate = receipts.ExpectedDate,
                    TenantId = receipts.TenantId,
                    ScheduledArrivalNumber = receipts.ScheduledArrivalNumber == default ? 0 : (int)receipts.ScheduledArrivalNumber,
                    DocumentNo = receipts.DocumentNo,
                    SupplierId = receipts.SupplierId,
                    PersonInCharge = receipts.PersonInCharge,
                    ConfirmedBy = receipts.ConfirmedBy,
                    ConfirmedDate = receipts.ConfirmedDate,
                    Status = receipts.Status,
                    LocationName = location.LocationName,
                    TenantFullName = tenant.TenantFullName,
                    WarehouseReceiptOrderLines = receiptOrderLines.ToList(),
                    SupplierName = supplier.SupplierName,
                    PersonInChargeName = person.FullName,
                    ReferenceNo = receipts.ReferenceNo,
                }).ToListAsync();

                if (receipt != null)
                {
                    return await Result<List<WarehouseReceiptOrderDto>>.SuccessAsync(receipt);
                }
                else
                {
                    return await Result<List<WarehouseReceiptOrderDto>>.FailAsync();
                }
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrderDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        /*
        public async Task<Result<WarehouseReceiptOrderDto>> SyncHTData(WarehouseReceiptOrderDto receiptDto)
        {
            try
            {
                var receiptStagings = await _dbContext.WarehouseReceiptStagings.Where(_ => _.IsDeleted != true && _.ReceiptNo == receiptDto.ReceiptNo).ToListAsync();
                var receiptLineIds = receiptDto.WarehouseReceiptOrderLines.Select(line => line.Id).ToList();
                var hasValidReceiptStaging = receiptStagings
                    .Any(staging => receiptLineIds.Contains(staging.ReceiptLineId));

                if (!hasValidReceiptStaging)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("No data found for this Receipt No");
                }

                var receiptLinesDto = (
                    from receiptLine in receiptDto.WarehouseReceiptOrderLines
                    join receiptStaging in receiptStagings on receiptLine.Id equals receiptStaging.ReceiptLineId into receiptLineStaging
                    from receiptStaging in receiptLineStaging.DefaultIfEmpty()
                        select new WarehouseReceiptOrderLineDto
                        {
                            Id = receiptLine.Id,
                            ReceiptNo = receiptLine.ReceiptNo,
                            ProductCode = receiptLine.ProductCode,
                            UnitName = receiptLine.UnitName,
                            OrderQty = receiptLine.OrderQty,
                            TransQty = receiptStaging.TransQty,
                            Bin = receiptStaging.Bin,
                            LotNo = receiptStaging.LotNo,
                            ExpirationDate = receiptStaging.ExpirationDate,
                            Putaway = receiptLine.Putaway,
                            UnitId = receiptLine.UnitId,
                            ProductName = receiptLine.ProductName,
                            StockAvailableQuantity = receiptLine.StockAvailableQuantity,
                        }
                    );

                var receiptOrderLines = receiptLinesDto.Select(_ => new WarehouseReceiptOrderLine
                {
                    Id = _.Id,
                    ReceiptNo = _.ReceiptNo,
                    ProductCode = _.ProductCode,
                    UnitName = _.UnitName,
                    OrderQty = _.OrderQty,
                    TransQty = _.TransQty,
                    Bin = _.Bin,
                    LotNo = _.LotNo,
                    ExpirationDate = _.ExpirationDate,
                    Putaway = _.Putaway,
                    UnitId = _.UnitId,
                    UpdateAt = now
                });
                receiptDto.WarehouseReceiptOrderLines = receiptLinesDto.ToList();
                _dbContext.WarehouseReceiptOrderLines.UpdateRange(receiptOrderLines);
                await _dbContext.SaveChangesAsync();

                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(receiptDto);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
        */
        /*
        public async Task<Result<WarehouseReceiptOrderDto>> SyncHTData(WarehouseReceiptOrderDto receiptDto)
        {
            try
            {
                var receiptStagings = await _dbContext.WarehouseReceiptStagings.Where(_ => _.IsDeleted != true && _.ReceiptNo == receiptDto.ReceiptNo).ToListAsync();
                if (receiptStagings.Count == 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("No data found in HT");
                }

                var receiptLinesDto = (
                    from staging in _dbContext.WarehouseReceiptStagings.Where(_ => _.ReceiptNo == receiptDto.ReceiptNo)
                    join product in _dbContext.Products on staging.ProductCode equals product.ProductCode into products
                    from product in products.DefaultIfEmpty()
                    join unit in _dbContext.Units on product.UnitId equals unit.Id into units
                    from unit in units.DefaultIfEmpty()
                    select new WarehouseReceiptOrderLineDto
                    {
                        Id = Guid.NewGuid(),
                        ReceiptNo = staging.ReceiptNo,
                        ProductCode = staging.ProductCode,
                        UnitName = unit.UnitName,
                        OrderQty = staging.OrderQty,
                        TransQty = staging.TransQty,
                        Bin = staging.Bin,
                        LotNo = staging.LotNo,
                        ExpirationDate = staging.ExpirationDate,
                        UnitId = product.UnitId,
                        ProductName = product.ProductName,
                        StockAvailableQuantity = product.StockAvailableQuanitty,
                    });
                var productHHTSet = receiptLinesDto.Select(_ => _.ProductCode).ToHashSet();
                var receiptLines = receiptLinesDto.Select(_ => new WarehouseReceiptOrderLine
                {
                    Id = _.Id,
                    ReceiptNo = _.ReceiptNo,
                    ProductCode = _.ProductCode,
                    UnitName = _.UnitName,
                    OrderQty = _.OrderQty,
                    TransQty = _.TransQty,
                    Bin = _.Bin,
                    LotNo = _.LotNo,
                    ExpirationDate = _.ExpirationDate,
                    UnitId = _.UnitId,
                    CreateAt = now,
                    UpdateAt = now
                });
                var missingProducts = receiptDto.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == receiptDto.ReceiptNo && !productHHTSet.Contains(_.ProductCode));
                receiptDto.WarehouseReceiptOrderLines = receiptLinesDto.ToList();
                receiptDto.WarehouseReceiptOrderLines.AddRange(missingProducts);

                _dbContext.WarehouseReceiptOrderLines.RemoveRange(_dbContext.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == receiptDto.ReceiptNo && productHHTSet.Contains(_.ProductCode)));
                _dbContext.WarehouseReceiptOrderLines.AddRange(receiptLines);
                await _dbContext.SaveChangesAsync();
                return Result<WarehouseReceiptOrderDto>.Success(receiptDto);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }*/
        public async Task<Result<WarehouseReceiptOrderDto>> SyncHTData(WarehouseReceiptOrderDto receiptDto)
        {
            try
            {
                var receiptStagings = await _dbContext.WarehouseReceiptStagings.Where(_ => _.IsDeleted != true && _.ReceiptNo == receiptDto.ReceiptNo).ToListAsync();
                if (receiptStagings.Count == 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("No data found in HT");
                }

                var receiptLinesDto = (
                    from staging in _dbContext.WarehouseReceiptStagings.Where(_ => _.ReceiptNo == receiptDto.ReceiptNo)
                    join product in _dbContext.Products on staging.ProductCode equals product.ProductCode into products
                    from product in products.DefaultIfEmpty()
                    join unit in _dbContext.Units on product.UnitId equals unit.Id into units
                    from unit in units.DefaultIfEmpty()
                    select new WarehouseReceiptOrderLineDto
                    {
                        Id = staging.OrderQty == 0 ? Guid.NewGuid() : staging.ReceiptLineId,
                        ReceiptNo = staging.ReceiptNo,
                        ProductCode = staging.ProductCode,
                        UnitName = unit.UnitName,
                        OrderQty = staging.OrderQty,
                        TransQty = staging.TransQty,
                        Bin = staging.Bin,
                        LotNo = staging.LotNo,
                        ExpirationDate = staging.ExpirationDate,
                        UnitId = product.UnitId,
                        ProductName = product.ProductName,
                        StockAvailableQuantity = product.StockAvailableQuanitty,
                        ReceiptLineIdParent = staging.OrderQty == 0 ? staging.ReceiptLineId : Guid.Empty,
                    });
                // var productHHTSet = receiptLinesDto.Select(_ => _.ProductCode).ToHashSet();
                var newReceiptLines = receiptLinesDto.Where(x => x.OrderQty == 0).Select(_ => new WarehouseReceiptOrderLine
                {
                    Id = _.Id,
                    ReceiptNo = _.ReceiptNo,
                    ProductCode = _.ProductCode,
                    UnitName = _.UnitName,
                    OrderQty = _.OrderQty,
                    TransQty = _.TransQty,
                    Bin = _.Bin,
                    LotNo = _.LotNo,
                    ExpirationDate = _.ExpirationDate,
                    UnitId = _.UnitId,
                    ReceiptLineIdParent = _.ReceiptLineIdParent,
                    CreateAt = now,
                    UpdateAt = now
                });
                var receiptLines = _dbContext.WarehouseReceiptOrderLines.Where(x => x.ReceiptNo == receiptDto.ReceiptNo);
                foreach (var receiptLine in receiptLines)
                {
                    var line = receiptLinesDto.FirstOrDefault(x => x.Id == receiptLine.Id);
                    if (line != null)
                    {
                        receiptLine.TransQty = line.TransQty;
                        receiptLine.Bin = line.Bin;
                        receiptLine.LotNo = line.LotNo;
                        receiptLine.ExpirationDate = line.ExpirationDate;
                        receiptLine.UpdateAt = now;

                    }
                }
                //var missingProducts = receiptDto.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == receiptDto.ReceiptNo && !productHHTSet.Contains(_.ProductCode));
                receiptDto.WarehouseReceiptOrderLines = receiptLinesDto.ToList();
                //receiptDto.WarehouseReceiptOrderLines.AddRange(missingProducts);

                // _dbContext.WarehouseReceiptOrderLines.RemoveRange(_dbContext.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == receiptDto.ReceiptNo && productHHTSet.Contains(_.ProductCode)));
                _dbContext.WarehouseReceiptOrderLines.UpdateRange(receiptLines);
                _dbContext.WarehouseReceiptOrderLines.AddRange(newReceiptLines);
                await _dbContext.SaveChangesAsync();
                return Result<WarehouseReceiptOrderDto>.Success(receiptDto);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> CreateLineFromArrivalNo(WarehouseReceiptOrderDto receiptDto)
        {
            try
            {
                var arrivalNo = receiptDto.ScheduledArrivalNumber;

                //check exist receipt
                var existReceipt = await _dbContext.WarehouseReceiptOrders
                    .FirstOrDefaultAsync(x => x.ScheduledArrivalNumber == arrivalNo && x.ReceiptNo != receiptDto.ReceiptNo);
                if (existReceipt != null)
                {
                    var existReceiptLines = await _dbContext.WarehouseReceiptOrderLines.Where(x => x.ReceiptNo == existReceipt.ReceiptNo).ToListAsync();
                    var result = new WarehouseReceiptOrderDto(existReceipt, existReceiptLines);
                    return await Result<WarehouseReceiptOrderDto>.SuccessAsync(result);
                }

                var arrivalInstructions = await (from ai in _dbContext.ArrivalInstructions
                                                 join p in _dbContext.Products
                                                 on ai.ProductCode equals p.ProductCode into pGroup
                                                 from p in pGroup.DefaultIfEmpty()
                                                 join s in _dbContext.Suppliers
                                                 on p.SupplierId equals s.Id into sGroup
                                                 from s in sGroup.DefaultIfEmpty()
                                                 join u in _dbContext.Units
                                                 on p.UnitId equals u.Id into uGroup
                                                 from u in uGroup.DefaultIfEmpty()
                                                 where ai.Id == arrivalNo
                                                 select new
                                                 {
                                                     ArrivalInstruction = ai,
                                                     ProductName = p.ProductName,
                                                     StockAvailableQuanitty = p.StockAvailableQuanitty,
                                                     SupplierId = s.Id,
                                                     SupplierName = s.SupplierName,
                                                     UnitId = u.Id,
                                                     UnitName = u.UnitName
                                                 }).ToListAsync();
                if (arrivalInstructions.Count == 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("NoArrivalData");
                }

                var receiptLines = receiptDto.WarehouseReceiptOrderLines.Where(_ => _.ArrivalNo == arrivalNo);
                if (receiptLines.Count() > 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("ArrivalDataExist");
                }

                var userInfo = _contextAccessor.HttpContext?.User.FindFirst("UserId");

                var firstArrival = arrivalInstructions.First();
                receiptDto.TenantId = firstArrival.ArrivalInstruction.CompanyId;
                receiptDto.SupplierId = (int)firstArrival.SupplierId;
                receiptDto.ScheduledArrivalNumber = firstArrival.ArrivalInstruction.Id;

                var receiptLinesDto = arrivalInstructions.Select(ai => new WarehouseReceiptOrderLineDto
                {
                    Id = Guid.NewGuid(),
                    ReceiptNo = receiptDto.ReceiptNo,
                    ProductCode = ai.ArrivalInstruction.ProductCode,
                    ProductName = ai.ProductName,
                    UnitId = ai.UnitId,
                    UnitName = ai.UnitName,
                    OrderQty = ai.ArrivalInstruction.Quantity,
                    StockAvailableQuantity = ai.StockAvailableQuanitty,
                    ArrivalNo = arrivalNo
                }).ToList();

                var receiptOrderLines = receiptLinesDto.Select(dto => new WarehouseReceiptOrderLine
                {
                    Id = dto.Id,
                    ReceiptNo = dto.ReceiptNo,
                    ProductCode = dto.ProductCode,
                    UnitName = dto.UnitName,
                    OrderQty = dto.OrderQty,
                    UnitId = dto.UnitId,
                    ArrivalNo = dto.ArrivalNo,
                    CreateOperatorId = userInfo?.Value,
                    CreateAt = now
                });

                receiptDto.WarehouseReceiptOrderLines.AddRange(receiptLinesDto);

                if (!String.IsNullOrEmpty(receiptDto.ReceiptNo))
                {
                    await _dbContext.WarehouseReceiptOrders.Where(x => x.ReceiptNo == receiptDto.ReceiptNo)
                        .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.ScheduledArrivalNumber, arrivalNo)
                                                .SetProperty(xx => xx.TenantId, receiptDto.TenantId)
                                                .SetProperty(xx => xx.SupplierId, receiptDto.SupplierId)
                                                .SetProperty(xx => xx.UpdateAt, now)
                                                .SetProperty(xx => xx.UpdateOperatorId, userInfo == null ? "" : userInfo.Value)
                                                );

                    await _dbContext.WarehouseReceiptOrderLines
                        .Where(x => x.ArrivalNo != default && x.ArrivalNo != arrivalNo)
                        .ExecuteDeleteAsync();

                    _dbContext.WarehouseReceiptOrderLines.AddRange(receiptOrderLines);
                    await _dbContext.SaveChangesAsync();
                }
                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(receiptDto);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> AdjustActionReceiptOrder([Body] WarehouseReceiptOrderDto request)
        {
            switch (request.ReferenceType)
            {
                case EnumWarehouseTransType.Receipt:
                    return await ProcessBusinessLogicReceipt(request);
                case EnumWarehouseTransType.Return:
                    return await ProcessBusinessLogicReturn(request);
                case EnumWarehouseTransType.Movement:
                    return await ProcessBusinessLogicMovement(request);
                default:
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("ReferenceType is invalid"); ;
            }
        }

        public async Task AdjustInsertStatusByReferenceType(WarehouseReceiptOrderDto request)
        {
            switch (request.ReferenceType)
            {
                case EnumWarehouseTransType.Return:
                    _dbContext.ReturnOrders.Where(_ => _.ReturnOrderNo == request.ReferenceNo).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReturnOrderStatus.Receiving));
                    break;
                default:
                    return;
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> ProcessBusinessLogicReceipt(WarehouseReceiptOrderDto request)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var receipt = await _dbContext.WarehouseReceiptOrders.Select(_ => new { _.Status, _.ReceiptNo }).FirstOrDefaultAsync(_ => _.ReceiptNo == request.ReceiptNo);
                if (receipt == null)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoReceipt");
                }
                var receiptLines = _dbContext.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == request.ReceiptNo).ToList();
                if (receiptLines.Count == 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoProduct");
                }
                if (request.Status == EnumReceiptOrderStatus.Open)
                {
                    var warehouseTranPayload = request.WarehouseReceiptOrderLines.Select(_ => new WarehouseTran
                    {
                        TransType = EnumWarehouseTransType.Receipt,
                        TransNumber = request.ReceiptNo,
                        StatusReceipt = EnumStatusReceipt.Ordered,
                        TransId = request.Id,
                        TransLineId = _.Id,
                        DatePhysical = null,
                        ProductCode = _.ProductCode,
                        Qty = _.OrderQty ?? 0,
                        TenantId = request.TenantId,
                        Location = request.Location,
                        Bin = String.IsNullOrWhiteSpace(_.Bin) ? "N/A" : _.Bin,
                        LotNo = String.IsNullOrWhiteSpace(_.LotNo) ? CommonHelpers.ParseLotno(_.ExpirationDate, request.ReceiptNo) : _.LotNo,
                        CreateAt = DateTime.Now,
                    }).ToList();

                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.Open));
                    _dbContext.WarehouseTrans.AddRange(warehouseTranPayload);
                }

                if (request.Status == EnumReceiptOrderStatus.Received)
                {
                    var orderLines = request.WarehouseReceiptOrderLines.Where(x => x.TransQty > 0);
                    if (orderLines.Count() == 0)
                    {
                        return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoProductQty");
                    }
                    /*
                    orderLines = request.WarehouseReceiptOrderLines.Where(x => x.TransQty > 0 && (String.IsNullOrEmpty(x.LotNo)));
                    if (orderLines.Count() > 0)
                    {
                        return await Result<WarehouseReceiptOrderDto>.FailAsync("Require.LotNo");
                    }
                    */
                    await _dbContext.WarehouseTrans.Where(_ => _.TransNumber == request.ReceiptNo).ExecuteDeleteAsync();
                    var warehouseTrans = await _dbContext.WarehouseTrans.Where(_ => _.TransNumber == request.ReceiptNo).AsNoTracking().ToListAsync();
                    var warehouseTranPayload = request.WarehouseReceiptOrderLines.Where(x => x.TransQty > 0).Select(x => new WarehouseTran
                    {
                        TransType = EnumWarehouseTransType.Receipt,
                        TransNumber = request.ReceiptNo,
                        StatusReceipt = EnumStatusReceipt.Received,
                        TransId = request.Id,
                        TransLineId = x.Id,
                        DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                        ProductCode = x.ProductCode,
                        Qty = x.TransQty ?? 0,
                        TenantId = request.TenantId,
                        Location = request.Location,
                        Bin = String.IsNullOrWhiteSpace(x.Bin) ? "N/A" : x.Bin,
                        LotNo = String.IsNullOrWhiteSpace(x.LotNo) ? CommonHelpers.ParseLotno(x.ExpirationDate, request.ReceiptNo) : x.LotNo,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now
                    }).ToList();

                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.Received));
                    _dbContext.WarehouseTrans.AddRange(warehouseTranPayload);
                }

                if (request.Status == EnumReceiptOrderStatus.OnPutaway)
                {
                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.OnPutaway));
                }

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<WarehouseReceiptOrderDto>.FailAsync(ex.ToString());
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> ProcessBusinessLogicReturn(WarehouseReceiptOrderDto request)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var receipt = await _dbContext.WarehouseReceiptOrders.Select(_ => new { _.Status, _.ReceiptNo }).FirstOrDefaultAsync(_ => _.ReceiptNo == request.ReceiptNo);
                if (receipt == null)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoReceipt");
                }
                var receiptLines = _dbContext.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == request.ReceiptNo).ToList();
                if (receiptLines.Count == 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoProduct");
                }
                /*
                receiptLines = _dbContext.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == request.ReceiptNo && _.TransQty > 0 && (String.IsNullOrEmpty(_.LotNo))).ToList();
                if (receiptLines.Count() > 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("Require.LotNo");
                }
                */
                if (request.Status == EnumReceiptOrderStatus.Open)
                {
                    var warehouseTranPayload = request.WarehouseReceiptOrderLines.Select(_ => new WarehouseTran
                    {
                        TransType = EnumWarehouseTransType.Receipt,
                        TransNumber = request.ReceiptNo,
                        StatusReceipt = EnumStatusReceipt.Ordered,
                        TransId = request.Id,
                        TransLineId = _.Id,
                        DatePhysical = null,
                        ProductCode = _.ProductCode,
                        Qty = _.OrderQty ?? 0,
                        TenantId = request.TenantId,
                        Location = request.Location,
                        Bin = String.IsNullOrWhiteSpace(_.Bin) ? "N/A" : _.Bin,
                        LotNo = String.IsNullOrWhiteSpace(_.LotNo) ? CommonHelpers.ParseLotno(_.ExpirationDate, request.ReceiptNo) : _.LotNo,
                        CreateAt = DateTime.Now,
                    }).ToList();

                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.Open));
                    _dbContext.WarehouseTrans.AddRange(warehouseTranPayload);
                }

                if (request.Status == EnumReceiptOrderStatus.Received)
                {
                    var orderLines = request.WarehouseReceiptOrderLines.Where(x => x.TransQty > 0);
                    if (orderLines.Count() == 0)
                    {
                        return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoProductQty");
                    }
                    await _dbContext.WarehouseTrans.Where(_ => _.TransNumber == request.ReceiptNo).ExecuteDeleteAsync();
                    var warehouseTrans = await _dbContext.WarehouseTrans.Where(_ => _.TransNumber == request.ReceiptNo).AsNoTracking().ToListAsync();
                    var warehouseTranPayload = request.WarehouseReceiptOrderLines.Where(x => x.TransQty > 0).Select(_ => new WarehouseTran
                    {
                        TransType = EnumWarehouseTransType.Receipt,
                        TransNumber = request.ReceiptNo,
                        StatusReceipt = EnumStatusReceipt.Received,
                        TransId = request.Id,
                        TransLineId = _.Id,
                        DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                        ProductCode = _.ProductCode,
                        Qty = _.TransQty ?? 0,
                        TenantId = request.TenantId,
                        Location = request.Location,
                        Bin = String.IsNullOrWhiteSpace(_.Bin) ? "N/A" : _.Bin,
                        LotNo = String.IsNullOrWhiteSpace(_.LotNo) ? CommonHelpers.ParseLotno(_.ExpirationDate, request.ReceiptNo) : _.LotNo,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now
                    }).ToList();

                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.Received));
                    _dbContext.ReturnOrders.Where(_ => _.ReturnOrderNo == request.ReferenceNo).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReturnOrderStatus.Received));
                    _dbContext.WarehouseTrans.AddRange(warehouseTranPayload);
                }

                if (request.Status == EnumReceiptOrderStatus.OnPutaway)
                {
                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.OnPutaway));
                    _dbContext.ReturnOrders.Where(_ => _.ReturnOrderNo == request.ReferenceNo).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReturnOrderStatus.Putaway));
                }

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<WarehouseReceiptOrderDto>.FailAsync(ex.ToString());
            }
        }

        public async Task<Result<WarehouseReceiptOrderDto>> ProcessBusinessLogicMovement(WarehouseReceiptOrderDto request)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var receipt = await _dbContext.WarehouseReceiptOrders.Select(_ => new { _.Status, _.ReceiptNo }).FirstOrDefaultAsync(_ => _.ReceiptNo == request.ReceiptNo);
                if (receipt == null)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoReceipt");
                }
                var receiptLines = _dbContext.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == request.ReceiptNo).ToList();
                if (receiptLines.Count == 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoProduct");
                }
                /*
                receiptLines = _dbContext.WarehouseReceiptOrderLines.Where(_ => _.ReceiptNo == request.ReceiptNo && _.TransQty > 0 && (String.IsNullOrEmpty(_.LotNo))).ToList();
                if (receiptLines.Count() > 0)
                {
                    return await Result<WarehouseReceiptOrderDto>.FailAsync("Require.LotNo");
                }
                */
                if (request.Status == EnumReceiptOrderStatus.Open)
                {
                    var warehouseTranPayload = request.WarehouseReceiptOrderLines.Select(_ => new WarehouseTran
                    {
                        TransType = EnumWarehouseTransType.Receipt,
                        TransNumber = request.ReceiptNo,
                        StatusReceipt = EnumStatusReceipt.Ordered,
                        TransId = request.Id,
                        TransLineId = _.Id,
                        DatePhysical = null,
                        ProductCode = _.ProductCode,
                        Qty = _.OrderQty ?? 0,
                        TenantId = request.TenantId,
                        Location = request.Location,
                        Bin = String.IsNullOrWhiteSpace(_.Bin) ? "N/A" : _.Bin,
                        LotNo = String.IsNullOrWhiteSpace(_.LotNo) ? CommonHelpers.ParseLotno(_.ExpirationDate, request.ReceiptNo) : _.LotNo,
                        CreateAt = DateTime.Now,
                    }).ToList();

                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.Open));
                    _dbContext.WarehouseTrans.AddRange(warehouseTranPayload);
                }

                if (request.Status == EnumReceiptOrderStatus.Received)
                {
                    var orderLines = request.WarehouseReceiptOrderLines.Where(x => x.TransQty > 0);
                    if (orderLines.Count() == 0)
                    {
                        return await Result<WarehouseReceiptOrderDto>.FailAsync("Confirmation.NoProductQty");
                    }
                    await _dbContext.WarehouseTrans.Where(_ => _.TransNumber == request.ReceiptNo).ExecuteDeleteAsync();
                    var warehouseTrans = await _dbContext.WarehouseTrans.Where(_ => _.TransNumber == request.ReceiptNo).AsNoTracking().ToListAsync();
                    var warehouseTranPayload = request.WarehouseReceiptOrderLines.Where(x => x.TransQty > 0).Select(_ => new WarehouseTran
                    {
                        TransType = EnumWarehouseTransType.Receipt,
                        TransNumber = request.ReceiptNo,
                        StatusReceipt = EnumStatusReceipt.Received,
                        TransId = request.Id,
                        TransLineId = _.Id,
                        DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                        ProductCode = _.ProductCode,
                        Qty = _.TransQty ?? 0,
                        TenantId = request.TenantId,
                        Location = request.Location,
                        Bin = String.IsNullOrWhiteSpace(_.Bin) ? "N/A" : _.Bin,
                        LotNo = String.IsNullOrWhiteSpace(_.LotNo) ? CommonHelpers.ParseLotno(_.ExpirationDate, request.ReceiptNo) : _.LotNo,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now
                    }).ToList();

                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.Received));
                    _dbContext.WarehouseShipments.Where(_ => _.ShipmentNo == request.ReferenceNo).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumShipmentOrderStatus.Received));
                    _dbContext.WarehouseTrans.AddRange(warehouseTranPayload);
                }

                if (request.Status == EnumReceiptOrderStatus.OnPutaway)
                {
                    _dbContext.WarehouseReceiptOrders.Where(_ => _.Id == request.Id).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.OnPutaway));
                    _dbContext.WarehouseShipments.Where(_ => _.ShipmentNo == request.ReferenceNo).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumShipmentOrderStatus.Putaway));
                }

                await _dbContext.SaveChangesAsync();
                transaction.Commit();
                return await Result<WarehouseReceiptOrderDto>.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<WarehouseReceiptOrderDto>.FailAsync(ex.ToString());
            }
        }

        public async Task<Result<string>> CreateReceiptFromReceiptPlan(int arrivalNo)
        {
            try
            {
                var arrivalInstructions = await (from ai in _dbContext.ArrivalInstructions
                                                 join p in _dbContext.Products
                                                 on ai.ProductCode equals p.ProductCode into pGroup
                                                 from p in pGroup.DefaultIfEmpty()
                                                 join s in _dbContext.Suppliers
                                                 on p.SupplierId equals s.Id into sGroup
                                                 from s in sGroup.DefaultIfEmpty()
                                                 join u in _dbContext.Units
                                                 on p.UnitId equals u.Id into uGroup
                                                 from u in uGroup.DefaultIfEmpty()
                                                 where ai.Id == arrivalNo
                                                 select new
                                                 {
                                                     ArrivalInstruction = ai,
                                                     ProductName = p.ProductName,
                                                     StockAvailableQuanitty = p.StockAvailableQuanitty,
                                                     SupplierId = s.Id,
                                                     SupplierName = s.SupplierName,
                                                     UnitId = u.Id,
                                                     UnitName = u.UnitName
                                                 }).ToListAsync();

                if (arrivalInstructions.Count == 0)
                {
                    return await Result<string>.FailAsync("NoArrivalData");
                }

                var userInfo = _contextAccessor.HttpContext?.User.FindFirst("UserId");
                string receiptNo = await _numberSequences.GetNoByType("Receipt");

                var firstArrival = arrivalInstructions.First();
                var newReceipt = new WarehouseReceiptOrder
                {
                    ReceiptNo = receiptNo,
                    TenantId = firstArrival.ArrivalInstruction.CompanyId,
                    SupplierId = (int)firstArrival.SupplierId,
                    ScheduledArrivalNumber = firstArrival.ArrivalInstruction.Id,
                    ExpectedDate = DateOnly.FromDateTime(firstArrival.ArrivalInstruction.ScheduledArrivalDate),
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value,
                    Status = EnumReceiptOrderStatus.Draft,
                    IsDeleted = false
                };

                await _dbContext.AddAsync(newReceipt);

                var receiptLines = arrivalInstructions.Select(ai => new WarehouseReceiptOrderLine
                {
                    Id = Guid.NewGuid(),
                    ReceiptNo = receiptNo,
                    ProductCode = ai.ArrivalInstruction.ProductCode,
                    UnitName = ai.UnitName,
                    OrderQty = ai.ArrivalInstruction.Quantity,
                    UnitId = ai.UnitId,
                    Status = EnumStatus.Activated,
                    ArrivalNo = ai.ArrivalInstruction.Id,
                    CreateAt = DateTime.Now,
                    CreateOperatorId = userInfo?.Value,
                    IsDeleted = false
                });

                await _dbContext.AddRangeAsync(receiptLines);
                await _dbContext.SaveChangesAsync();

                await _numberSequences.IncreaseNumberSequenceByType("Receipt");
                return await Result<string>.SuccessAsync(receiptNo);
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<PageList<WarehouseReceiptOrderDto>>> SearchReceiptOrderListAsync([Body] QueryModel<WarehouseReceiptSearchModel> model)
        {
            try
            {
                var data = model.Entity ?? new WarehouseReceiptSearchModel();

                var receipt = (
                from receipts in _dbContext.WarehouseReceiptOrders
                .Where(_ => _.IsDeleted != true
                && (_.ReceiptNo.Contains(data.ReceiptNo ?? string.Empty))
                && (string.IsNullOrEmpty(data.ReferenceNo) ? true : (string.IsNullOrEmpty(_.ReferenceNo) ? false : _.ReferenceNo.Contains(data.ReferenceNo)))
                && (data.TenantId.HasValue ? (_.TenantId == data.TenantId) : true)
                && (data.SupplierId.HasValue ? (_.SupplierId == data.SupplierId) : true)
                && (data.ScheduledArrivalNumber == default || _.ScheduledArrivalNumber == data.ScheduledArrivalNumber)
                && (data.ExpectedDate.HasValue ? (_.ExpectedDate == data.ExpectedDate) : true)
                && (data.Location == default ? true : _.Location == data.Location.ToString())
                && (data.Status.HasValue ? (_.Status == data.Status) : (_.Status != EnumReceiptOrderStatus.Received && _.Status != EnumReceiptOrderStatus.Completed))
                && (string.IsNullOrEmpty(data.ProductCode) ? true : _dbContext.WarehouseReceiptOrderLines.Any(line => line.ReceiptNo == _.ReceiptNo && line.ProductCode.Contains(data.ProductCode))))
                join location in _dbContext.Locations.Where(_ => _.IsDeleted != true) on receipts.Location equals location.Id.ToString() into locationReceipts
                from location in locationReceipts.DefaultIfEmpty()
                join tenant in _dbContext.TenantAuth on receipts.TenantId equals tenant.TenantId into tenantReceipts
                from tenant in tenantReceipts.DefaultIfEmpty()
                join supplier in _dbContext.Suppliers on receipts.SupplierId equals supplier.Id into supplierReceipts
                from supplier in supplierReceipts.DefaultIfEmpty()
                join person in _dbContext.Users on receipts.PersonInCharge equals person.Id into personReceipts
                from person in personReceipts.DefaultIfEmpty()
                join receiptLines in (from receiptLines in _dbContext.WarehouseReceiptOrderLines.Where(r => r.IsDeleted != true)
                                      join products in _dbContext.Products.Where(p => p.IsDeleted != true) on receiptLines.ProductCode equals products.ProductCode into productReceiptLines
                                      from products in productReceiptLines
                                      select new WarehouseReceiptOrderLineDto
                                      {
                                          Id = receiptLines.Id,
                                          ReceiptNo = receiptLines.ReceiptNo,
                                          ProductCode = receiptLines.ProductCode,
                                          UnitName = receiptLines.UnitName,
                                          OrderQty = receiptLines.OrderQty,
                                          TransQty = receiptLines.TransQty,
                                          Bin = receiptLines.Bin,
                                          LotNo = receiptLines.LotNo,
                                          ExpirationDate = receiptLines.ExpirationDate,
                                          Putaway = receiptLines.Putaway,
                                          UnitId = products.UnitId,
                                          ProductName = products.ProductName,
                                          StockAvailableQuantity = products.StockAvailableQuanitty,
                                      })
                on receipts.ReceiptNo equals receiptLines.ReceiptNo into receiptOrderLines
                select new WarehouseReceiptOrderDto
                {
                    Id = receipts.Id,
                    ReceiptNo = receipts.ReceiptNo,
                    Location = receipts.Location,
                    ExpectedDate = receipts.ExpectedDate,
                    TenantId = receipts.TenantId,
                    ScheduledArrivalNumber = receipts.ScheduledArrivalNumber == default ? 0 : (int)receipts.ScheduledArrivalNumber,
                    DocumentNo = receipts.DocumentNo,
                    SupplierId = receipts.SupplierId,
                    PersonInCharge = receipts.PersonInCharge,
                    ConfirmedBy = receipts.ConfirmedBy,
                    ConfirmedDate = receipts.ConfirmedDate,
                    Status = receipts.Status,
                    LocationName = location.LocationName,
                    TenantFullName = tenant.TenantFullName,
                    WarehouseReceiptOrderLines = receiptOrderLines.ToList(),
                    SupplierName = supplier.SupplierName,
                    PersonInChargeName = person.FullName,
                    ReferenceNo = receipts.ReferenceNo
                }).OrderByDescending(_ => _.ReceiptNo).AsQueryable();

                if (receipt != null)
                {
                    var pagedList = PageList<WarehouseReceiptOrderDto>.PagedResult(receipt, model.PageNumber, model.PageSize);
                    return await Result<PageList<WarehouseReceiptOrderDto>>.SuccessAsync(pagedList);
                }
                else
                {
                    return await Result<PageList<WarehouseReceiptOrderDto>>.FailAsync();
                }
            }
            catch (Exception ex)
            {
                return await Result<PageList<WarehouseReceiptOrderDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Tuple<int, int>> GenerateReceiptFromReceiptPlan(string tenantIds)
        {
            try
            {
                List<string> tenantList = new List<string>();
                if (!string.IsNullOrEmpty(tenantIds))
                    tenantList = tenantIds.Split(',').ToList();
                if (tenantIds.Contains("*") || tenantIds.Contains("All") || string.IsNullOrEmpty(tenantIds))
                    tenantList = new List<string>();
                // Get existing arrival numbers from receipt orders

                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var arrivalNoExists = await _dbContext.WarehouseReceiptOrders
                   .AsNoTracking()
                   .Select(x => x.ScheduledArrivalNumber)
                   .Distinct()
                   .ToListAsync();

                        // Query arrival instructions that don't have receipt orders yet
                        var rePlan = await (from ai in _dbContext.ArrivalInstructions
                                            where ai.IsDeleted == false
                                                && !arrivalNoExists.Contains(ai.Id)
                                                && !string.IsNullOrEmpty(ai.ProductCode)
                                            join p in _dbContext.Products
                                            on ai.ProductCode equals p.ProductCode
                                            where (tenantList.Count == 0 || tenantList.Contains(ai.CompanyId.ToString()))
                                                    && (p.CompanyId == ai.CompanyId)
                                            join s in _dbContext.Suppliers
                                            on p.SupplierId equals s.Id into sGroup
                                            from s in sGroup.DefaultIfEmpty()
                                            join u in _dbContext.Units
                                            on p.UnitId equals u.Id into units
                                            from unit in units.DefaultIfEmpty()
                                            select new
                                            {
                                                ArrivalInstruction = ai,
                                                Product = p,
                                                Supplier = s,
                                                Unit = unit
                                            }).ToListAsync();

                        Console.WriteLine($"Total receipt plan:{rePlan.Count}");

                        // Get sequence number info and warehouse parameters
                        var receiptNoInfo = await _dbContext.SequencesNumber.FirstOrDefaultAsync(x => x.JournalType == "Receipt");
                        var param = await _dbContext.WarehouseParameters.FirstOrDefaultAsync(x => x.ReceivingLocation != default);

                        int currentReceiptNo = receiptNoInfo.CurrentSequenceNo.Value;
                        List<WarehouseReceiptOrder> newReceipts = new();
                        List<WarehouseReceiptOrderLine> newReceiptLines = new();

                        // Group by arrival instruction ID
                        foreach (var re in rePlan)
                        {
                            string receiptNo = $"{receiptNoInfo.Prefix}{currentReceiptNo++.ToString().PadLeft(receiptNoInfo.SequenceLength.Value, '0')}";

                            // Create receipt header
                            var newReceipt = new WarehouseReceiptOrder
                            {
                                ReceiptNo = receiptNo,
                                TenantId = re.ArrivalInstruction.CompanyId,
                                SupplierId = re.Product.SupplierId,
                                Location = param?.ReceivingLocation ?? "",
                                ScheduledArrivalNumber = re.ArrivalInstruction.Id,
                                ExpectedDate = DateOnly.FromDateTime(re.ArrivalInstruction.ScheduledArrivalDate),
                                CreateAt = DateTime.Now,
                                CreateOperatorId = "system",
                                Status = FBT.ShareModels.EnumReceiptOrderStatus.Open
                            };
                            newReceipts.Add(newReceipt);

                            var newReceiptLine = new WarehouseReceiptOrderLine
                            {
                                Id = Guid.NewGuid(),
                                ReceiptNo = receiptNo,
                                ProductCode = re.ArrivalInstruction.ProductCode,
                                UnitName = re.Unit?.UnitName,
                                OrderQty = re.ArrivalInstruction.Quantity,
                                UnitId = re.Product.UnitId,
                                Status = FBT.ShareModels.EnumStatus.Activated,
                                ArrivalNo = re.ArrivalInstruction.Id,
                                CreateAt = DateTime.Now,
                                CreateOperatorId = "system"
                            };
                            newReceiptLines.Add(newReceiptLine);
                        }

                        // Save changes
                        await _dbContext.AddRangeAsync(newReceipts);
                        await _dbContext.AddRangeAsync(newReceiptLines);
                        await _dbContext.SaveChangesAsync();

                        // Update sequence number
                        await _dbContext.SequencesNumber
                            .Where(x => x.JournalType == "Receipt")
                            .ExecuteUpdateAsync(x => x.SetProperty(xx => xx.CurrentSequenceNo, currentReceiptNo));

                        await transaction.CommitAsync();
                        return new Tuple<int, int>(newReceipts.Count, rePlan.Count);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelpers.LogFile("GENERATE_PLAN", $"ERROR: {ex.Message}");
                return new Tuple<int, int>(0, 0);
            }
        }
        public async Task<Result<List<ReceiptReportDTO>>> GetReceiptReportAsync()
        {
            try
            {
                var receipts = await _dbContext.WarehouseReceiptOrders
                    .Where(x => x.IsDeleted != true && x.ExpectedDate.HasValue)
                    .Select(x => new
                    {
                        x.TenantId,
                        x.ExpectedDate,
                        x.ReceiptNo
                    })
                    .ToListAsync();

                var tenantIds = receipts.Select(x => x.TenantId).Distinct().ToList();
                var tenantNames = await _dbContext.TenantAuth
                    .Where(t => tenantIds.Contains(t.TenantId))
                    .ToDictionaryAsync(t => t.TenantId, t => t.TenantFullName);

                var reports = receipts
                    .GroupBy(x => new { x.TenantId, x.ExpectedDate })
                    .Select(g =>
                    {
                        var firstReceipt = g.First();
                        var receiptNo = firstReceipt.ReceiptNo;

                        return new ReceiptReportDTO
                        {
                            Tenant = tenantNames.GetValueOrDefault(g.Key.TenantId, string.Empty),
                            Period = g.Key.ExpectedDate.Value,
                            NumberPutaway = ReceiptNumberPutaway(receiptNo),
                            RemainingNumber = ReceiptRemainingNumber(receiptNo),
                            ProgressRateString = ReceiptProgressRateString(receiptNo),
                            ProgressRate = ReceiptProgressRate(receiptNo),
                            TotalOrderQty = ReceiptTotalOrderQty(receiptNo),
                            TotalTransQty = ReceiptTotalTransQty(receiptNo),
                            TotalRemainingNumber = TotalReceiptRemainingNumber(receiptNo),
                            Productivity = "0" // Implement this method
                        };
                    })
                    .ToList();

                return Result<List<ReceiptReportDTO>>.Success(reports);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting receipt report: {ex.Message}");
            }
        }
        public async Task<Result<ResponseCompleteModel>> CompleteAndPutAwayReceipts()
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var orders = await (
                        from receipt in _dbContext.WarehouseReceiptOrders.Where(_ =>
                        _.IsDeleted != true && _.HHTStatus == EnumHHTStatus.Done && _.Status == EnumReceiptOrderStatus.Open)
                        join line in _dbContext.WarehouseReceiptOrderLines on receipt.ReceiptNo equals line.ReceiptNo into lines
                        where lines.Any()
                        select new WarehouseReceiptOrderDto(receipt, lines.ToList())).ToListAsync();

                if (orders.Count == 0)
                    return Result<ResponseCompleteModel>.Success(new ResponseCompleteModel(0, 0));

                int success = 0;

                var receiptLinesDto = await (
                    from staging in _dbContext.WarehouseReceiptStagings.Where(_ => orders.Select(o => o.ReceiptNo).Contains(_.ReceiptNo))
                    join product in _dbContext.Products on staging.ProductCode equals product.ProductCode into products
                    from product in products.DefaultIfEmpty()
                    join unit in _dbContext.Units on product.UnitId equals unit.Id into units
                    from unit in units.DefaultIfEmpty()
                    select new WarehouseReceiptOrderLineDto
                    {
                        Id = staging.OrderQty == 0 ? Guid.NewGuid() : staging.ReceiptLineId,
                        ReceiptNo = staging.ReceiptNo,
                        ProductCode = staging.ProductCode,
                        UnitName = unit.UnitName,
                        OrderQty = staging.OrderQty,
                        TransQty = staging.TransQty,
                        Bin = staging.Bin,
                        LotNo = staging.LotNo,
                        ExpirationDate = staging.ExpirationDate,
                        UnitId = product.UnitId,
                        ProductName = product.ProductName,
                        StockAvailableQuantity = product.StockAvailableQuanitty,
                        ReceiptLineIdParent = staging.OrderQty == 0 ? staging.ReceiptLineId : Guid.Empty,
                    }).ToListAsync();
                //var productHHTSet = receiptLinesDto.Select(_ => _.ProductCode).ToHashSet();
                var newReceiptLines = receiptLinesDto.Where(_ => _.OrderQty == 0).Select(_ => new WarehouseReceiptOrderLine
                {
                    Id = _.Id,
                    ReceiptNo = _.ReceiptNo,
                    ProductCode = _.ProductCode,
                    UnitName = _.UnitName,
                    OrderQty = _.OrderQty,
                    TransQty = _.TransQty,
                    Bin = _.Bin,
                    LotNo = _.LotNo,
                    ExpirationDate = _.ExpirationDate,
                    UnitId = _.UnitId,
                    CreateAt = now,
                    UpdateAt = now
                });
                var updateReceiptLines = _dbContext.WarehouseReceiptOrderLines.Where(_ => orders.Select(o => o.ReceiptNo).Contains(_.ReceiptNo));
                foreach (var receiptLine in updateReceiptLines)
                {
                    var line = receiptLinesDto.FirstOrDefault(x => x.Id == receiptLine.Id);
                    if (line != null)
                    {
                        receiptLine.TransQty = line.TransQty;
                        receiptLine.Bin = line.Bin;
                        receiptLine.LotNo = line.LotNo;
                        receiptLine.ExpirationDate = line.ExpirationDate;
                        receiptLine.UpdateAt = now;

                    }
                }
                //_dbContext.WarehouseReceiptOrderLines.RemoveRange(_dbContext.WarehouseReceiptOrderLines.Where(_ => orders.Select(o => o.ReceiptNo).Contains(_.ReceiptNo) && productHHTSet.Contains(_.ProductCode)));
                _dbContext.WarehouseReceiptOrderLines.UpdateRange(updateReceiptLines);
                _dbContext.WarehouseReceiptOrderLines.AddRange(newReceiptLines);

                List<WarehouseTran> warehouseTrans = new();
                foreach (var tran in orders)
                {
                    tran.WarehouseReceiptOrderLines = receiptLinesDto.Where(_ => _.ReceiptNo == tran.ReceiptNo).ToList();
                    var warehouseTranPayload = tran.WarehouseReceiptOrderLines.Select(_ => new WarehouseTran
                    {
                        TransType = EnumWarehouseTransType.Receipt,
                        TransNumber = tran.ReceiptNo,
                        StatusReceipt = EnumStatusReceipt.Received,
                        TransId = tran.Id,
                        TransLineId = _.Id,
                        DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                        ProductCode = _.ProductCode,
                        Qty = _.TransQty ?? 0,
                        TenantId = tran.TenantId,
                        Location = tran.Location,
                        Bin = String.IsNullOrWhiteSpace(_.Bin) ? "N/A" : _.Bin,
                        LotNo = String.IsNullOrWhiteSpace(_.LotNo) ? CommonHelpers.ParseLotno(_.ExpirationDate, tran.ReceiptNo) : _.LotNo,
                        CreateAt = DateTime.Now,
                        UpdateAt = DateTime.Now
                    });

                    warehouseTrans.AddRange(warehouseTranPayload);
                }

                _dbContext.WarehouseReceiptOrders.Where(_ => orders.Select(r => r.Id).Contains(_.Id)).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReceiptOrderStatus.OnPutaway));
                _dbContext.WarehouseTrans.AddRange(warehouseTrans);

                var payload = orders.Select(order => new WarehousePutAwayDto(order));

                await _warehousePutAwayServices.InsertWarehousePutAwayOrder(payload);
                await _dbContext.SaveChangesAsync();

                transaction.Commit();
                success++;
                return Result<ResponseCompleteModel>.Success(new ResponseCompleteModel(orders.Count, success));
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result<ResponseCompleteModel>.Fail(ex);
            }
        }

        private int ReceiptTotalOrderQty(string receiptNo)
        {
            if (string.IsNullOrEmpty(receiptNo))
            {
                return 0;
            }

            var totalOrderQty = _dbContext.WarehouseReceiptOrderLines
                .Where(wpl => wpl.ReceiptNo == receiptNo)
                .Sum(wpl => (int?)wpl.OrderQty) ?? 0;

            return totalOrderQty;
        }
        private int ReceiptTotalTransQty(string receiptNo)
        {
            if (string.IsNullOrEmpty(receiptNo))
            {
                return 0;
            }

            var totalTransQty = _dbContext.WarehouseReceiptOrderLines
                .Where(wpl => wpl.ReceiptNo == receiptNo)
                .Sum(wpl => (int?)wpl.TransQty) ?? 0;

            return totalTransQty;
        }
        private string? ReceiptRemainingNumber(string receiptNo)
        {
            if (string.IsNullOrEmpty(receiptNo))
            {
                return "0/0";
            }

            var receiptLines = _dbContext.WarehouseReceiptOrderLines
                .Where(wpl => wpl.ReceiptNo == receiptNo)
                .Select(wpl => new
                {
                    wpl.OrderQty,
                    wpl.TransQty
                })
                .ToList();

            if (!receiptLines.Any())
            {
                return "0/0";
            }

            int orderQty = receiptLines.Sum(wpl => (int?)wpl.OrderQty) ?? 0;
            int transQty = receiptLines.Sum(wpl => (int?)wpl.TransQty) ?? 0;
            int remainingQty = orderQty - transQty;

            return $"{remainingQty}/{transQty}";
        }
        private string ReceiptNumberPutaway(string receiptNo)
        {
            if (string.IsNullOrEmpty(receiptNo))
            {
                return "0/0";
            }

            var receiptLines = _dbContext.WarehouseReceiptOrderLines
                .Where(wpl => wpl.ReceiptNo == receiptNo)
                .Select(wpl => new
                {
                    wpl.OrderQty,
                    wpl.TransQty
                })
                .ToList();

            if (!receiptLines.Any())
            {
                return "0/0";
            }

            int orderQty = receiptLines.Sum(wpl => (int?)wpl.OrderQty) ?? 0;
            int transQty = receiptLines.Sum(wpl => (int?)wpl.TransQty) ?? 0;

            return $"{orderQty}/{transQty}";
        }
        private int TotalReceiptRemainingNumber(string receiptNo)
        {
            if (string.IsNullOrEmpty(receiptNo))
            {
                return 0;
            }

            var quantities = _dbContext.WarehouseReceiptOrderLines
                .Where(wpl => wpl.ReceiptNo == receiptNo)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    OrderQty = g.Sum(wpl => (int?)wpl.OrderQty) ?? 0,
                    TransQty = g.Sum(wpl => (int?)wpl.TransQty) ?? 0
                })
                .FirstOrDefault();

            if (quantities == null)
            {
                return 0;
            }

            return quantities.OrderQty - quantities.TransQty;
        }
        private string? ReceiptProgressRateString(string receiptNo)
        {
            if (string.IsNullOrEmpty(receiptNo))
            {
                return "0%";
            }

            var quantities = _dbContext.WarehouseReceiptOrderLines
                .Where(wpl => wpl.ReceiptNo == receiptNo)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    OrderQty = g.Sum(wpl => (int?)wpl.OrderQty) ?? 0,
                    TransQty = g.Sum(wpl => (int?)wpl.TransQty) ?? 0
                })
                .FirstOrDefault();

            if (quantities == null || quantities.TransQty == 0)
            {
                return "0%";
            }

            var remainingNumber = quantities.OrderQty - quantities.TransQty;
            decimal percentage = Math.Round((decimal)remainingNumber / quantities.TransQty * 100, 2);

            return $"{percentage}%";
        }
        private decimal ReceiptProgressRate(string receiptNo)
        {
            if (string.IsNullOrEmpty(receiptNo))
            {
                return 0;
            }

            var quantities = _dbContext.WarehouseReceiptOrderLines
                .Where(wpl => wpl.ReceiptNo == receiptNo)
                .GroupBy(x => 1)
                .Select(g => new
                {
                    OrderQty = g.Sum(wpl => (int?)wpl.OrderQty) ?? 0,
                    TransQty = g.Sum(wpl => (int?)wpl.TransQty) ?? 0
                })
                .FirstOrDefault();

            if (quantities == null || quantities.TransQty == 0)
            {
                return 0;
            }

            var remainingNumber = quantities.OrderQty - quantities.TransQty;
            return Math.Round((decimal)remainingNumber / quantities.TransQty * 100, 2);
        }
    }

    public class ReceiptLineComparer : IEqualityComparer<WarehouseReceiptOrderLine>
    {
        public bool Equals(WarehouseReceiptOrderLine x, WarehouseReceiptOrderLine y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(WarehouseReceiptOrderLine obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
