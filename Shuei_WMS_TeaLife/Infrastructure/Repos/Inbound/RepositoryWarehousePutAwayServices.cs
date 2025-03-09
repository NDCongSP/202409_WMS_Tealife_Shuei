using Application.DTOs;
using Application.Extentions;
using Application.Services;
using Application.Services.Inbound;
using DocumentFormat.OpenXml.Spreadsheet;
using FBT.ShareModels.Entities;
using FBT.ShareModels.WMS;
using Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestEase;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using Azure.Core;
using Application.DTOs.Response;
using Application.Models;
using AutoMapper;
using WebUIFinal.Core;

namespace Infrastructure.Repos
{
    public class RepositoryWarehousePutAwayServices : IWarehousePutAway
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly INumberSequences _numberSequences;
        private readonly DateTime now = DateTime.Now;

        public RepositoryWarehousePutAwayServices(
            ApplicationDbContext dbContext,
            IHttpContextAccessor contextAccessor,
            INumberSequences numberSequences)
        {
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
            _numberSequences = numberSequences;
        }

        public async Task<Result<List<WarehousePutAway>>> AddRangeAsync([Body] List<WarehousePutAway> model)
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

                await _dbContext.WarehousePutAways.AddRangeAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<List<WarehousePutAway>>.SuccessAsync(model, "Add range WarehousePutAway successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAway>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> DeleteRangeAsync([Body] List<WarehousePutAway> model)
        {
            try
            {
                _dbContext.WarehousePutAways.RemoveRange(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync("Delete range WarehousePutAway successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> DeleteAsync([Body] WarehousePutAway model)
        {
            try
            {
                _dbContext.WarehousePutAways.Remove(model);
                _dbContext.WarehousePutAwayLines.Where(x => x.PutAwayNo == model.PutAwayNo).ExecuteDelete();
                _dbContext.WarehouseReceiptOrders.Where(x => x.ReceiptNo == model.ReceiptNo).ExecuteUpdate(x => x.SetProperty(b => b.Status, b => EnumReceiptOrderStatus.Received));
                await _dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePutAway>>> GetAllAsync()
        {
            try
            {
                var result = await _dbContext.WarehousePutAways.Where(x => x.IsDeleted == false).ToListAsync();
                var locations = await _dbContext.Locations.Where(x => result.Select(xx => Guid.Parse(xx.Location)).Contains(x.Id)).ToListAsync();
                if (result.Count() > 0 && locations.Count > 0)
                {
                    result.ForEach(item =>
                    {
                        var locationInfo = locations.FirstOrDefault(x => x.Id.ToString() == item.Location);
                        if (locationInfo != null)
                            item.Location = locationInfo?.LocationName;
                    });
                }
                return await Result<List<WarehousePutAway>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAway>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehousePutAway>.SuccessAsync(await _dbContext.WarehousePutAways.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> InsertAsync([Body] WarehousePutAway model)
        {
            try
            {
                await _dbContext.WarehousePutAways.AddAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAway>> UpdateAsync([Body] WarehousePutAway model)
        {
            try
            {
                _dbContext.WarehousePutAways.Update(model);
                await _dbContext.SaveChangesAsync();
                return await Result<WarehousePutAway>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAway>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehousePutAway>>> GetByMasterCodeAsync([Path] string putAwayNo)
        {
            try
            {
                return await Result<List<WarehousePutAway>>.SuccessAsync(await _dbContext.WarehousePutAways.Where(x => x.PutAwayNo == putAwayNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehousePutAway>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<IEnumerable<WarehousePutAwayDto>>> InsertWarehousePutAwayOrder([Body] IEnumerable<WarehousePutAwayDto> request)
        {
            try
            {
                var userInfo = _contextAccessor.HttpContext?.User.FindFirst("UserId");
                var seqResult = await _dbContext.SequencesNumber
                    .FirstOrDefaultAsync(_ => _.JournalType == "Putaway");

                if (seqResult == null)
                    return await Result<IEnumerable<WarehousePutAwayDto>>.FailAsync("PutawayNo's prefix does not exist");

                int currentIndex = seqResult.CurrentSequenceNo ?? 6;
                var receiptNos = new List<string>();
                var warehouseReceiptOrderLines = _dbContext.WarehouseReceiptOrderLines.Where(w => request.Select(s => s.ReceiptNo).Contains(w.ReceiptNo)).ToList();
                var warehousePutAways = new List<WarehousePutAway>();
                var warehousePutAwayLines = new List<WarehousePutAwayLine>();
                var warehouseTrans = new List<WarehouseTran>();
                Dictionary<string, WarehousePutAwayDto> receiptDict = new Dictionary<string, WarehousePutAwayDto>();
                var whsParam = _dbContext.WarehouseParameters.FirstOrDefault();
                var updatedBatches = new List<Batches>();
                foreach (var r in request)
                {
                    var putAwayIndex = $"{seqResult.Prefix}{currentIndex.ToString().PadLeft(seqResult.SequenceLength ?? 6, '0')}";
                    currentIndex++;
                    receiptDict.Add(r.ReceiptNo, r);
                    var warehousePutAway = new WarehousePutAway
                    {
                        Id = r.Id,
                        PutAwayNo = putAwayIndex,
                        ReceiptNo = r.ReceiptNo,
                        Description = r.Description,
                        TenantId = r.TenantId,

                        DocumentDate = r.DocumentDate,
                        DocumentNo = r.DocumentNo,
                        Location = string.IsNullOrEmpty(whsParam.PutawayLocation) ? r.Location : whsParam.PutawayLocation,
                        PostedDate = r.PostedDate,
                        PostedBy = r.PostedBy,
                        CreateOperatorId = userInfo == null ? "" : userInfo.Value,
                        CreateAt = now,
                        IsDeleted = r.IsDeleted ?? false,
                        TransDate = DateOnly.FromDateTime(DateTime.Now)
                    };
                    receiptDict.Add(putAwayIndex, r);
                    var receiptLines = warehouseReceiptOrderLines.Where(x => x.ReceiptNo == r.ReceiptNo && x.TransQty > 0).ToList();
                    var products = _dbContext.Products.Where(x => receiptLines.Select(s => s.ProductCode).Contains(x.ProductCode)).ToList();
                    if (receiptLines.Count > 0)
                    {
                        warehousePutAways.Add(warehousePutAway);
                        foreach (var receiptLine in receiptLines)
                        {
                            warehousePutAwayLines.Add(new WarehousePutAwayLine
                            {
                                Id = Guid.NewGuid(),
                                PutAwayNo = putAwayIndex,
                                ProductCode = receiptLine.ProductCode,
                                UnitId = products.Where(x => x.ProductCode == receiptLine.ProductCode).FirstOrDefault().UnitId, //receiptLine.UnitId,
                                JournalQty = receiptLine.TransQty,
                                TransQty = receiptLine.TransQty,
                                ExpirationDate = receiptLine.ExpirationDate,
                                Bin = receiptLine.Bin,
                                LotNo = receiptLine.LotNo,
                                CreateAt = now,
                                CreateOperatorId = userInfo == null ? "" : userInfo.Value,
                                ReceiptLineId = receiptLine.Id,
                            });
                            var lotNo = String.IsNullOrEmpty(receiptLine.LotNo) ? CommonHelpers.ParseLotno(receiptLine.ExpirationDate, receiptLine.ReceiptNo) : receiptLine.LotNo;
                            //await _dbContext.Batches.Where(x => x.TenantId == r.TenantId && x.ProductCode == receiptLine.ProductCode && x.LotNo == lotNo).ExecuteDeleteAsync();
                            int count = updatedBatches.Where(x => x.TenantId == r.TenantId && x.ProductCode == receiptLine.ProductCode && x.LotNo == lotNo).ToList().Count;
                            if (count == 0)
                            {
                                updatedBatches.Add(new Batches
                                {
                                    Id = Guid.NewGuid(),
                                    TenantId = r.TenantId,
                                    LotNo = lotNo,
                                    ProductCode = receiptLine.ProductCode,
                                    ExpirationDate = receiptLine.ExpirationDate,
                                    CreateAt = now,
                                    CreateOperatorId = userInfo == null ? "" : userInfo.Value,
                                    IsDeleted = false
                                });
                            }
                           

                        }
                    }

                }
                if (warehousePutAways.Count > 0)
                {
                    foreach(var batch in updatedBatches)
                    {
                        _dbContext.Batches.Where(x => x.TenantId == batch.TenantId && x.ProductCode == batch.ProductCode && x.LotNo == batch.LotNo).ExecuteDelete();
                    }
                    await _dbContext.WarehousePutAways.AddRangeAsync(warehousePutAways);
                    await _dbContext.WarehousePutAwayLines.AddRangeAsync(warehousePutAwayLines);
                    await _dbContext.Batches.AddRangeAsync(updatedBatches);
                    seqResult.CurrentSequenceNo = currentIndex;
                    _dbContext.SequencesNumber.Update(seqResult);

                    await _dbContext.SaveChangesAsync();
                    return await Result<IEnumerable<WarehousePutAwayDto>>.SuccessAsync(request);
                }
                else
                {
                    return await Result<IEnumerable<WarehousePutAwayDto>>.FailAsync($"Cannot create Putaway because missing product line.");
                }
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<WarehousePutAwayDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayDto>> GetPutAwayAsync(string PutAwayNo)
        {
            /*
            try
            {
                
                if (putAway == null)
                    return await Result<WarehousePutAwayDto>.FailAsync("Putaway is not existed");
                var result = putAway.Adapt<WarehousePutAwayDto>();
                result.WarehousePutAwayLines = _dbContext.WarehousePutAwayLines.Where(x => x.PutAwayNo == PutAwayNo).AsEnumerable().Adapt<List<WarehousePutAwayLineDto>>();

                return await Result<WarehousePutAwayDto>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                throw;
            }
            */
            var tenantId =  _dbContext.WarehousePutAways.Where(x => x.PutAwayNo == PutAwayNo).FirstOrDefault().TenantId;
            try
            {
                var putAway = await (
                from putaways in _dbContext.WarehousePutAways
                join putawayLines in (from putawayLines in _dbContext.WarehousePutAwayLines.Where(r => r.PutAwayNo == PutAwayNo)
                                      join products in _dbContext.Products.Where(p => p.IsDeleted != true && p.CompanyId == tenantId) on putawayLines.ProductCode equals products.ProductCode into productPutAwayLines
                                      from products in productPutAwayLines
                                      join units in _dbContext.Units.Where(p => p.IsDeleted != true) on putawayLines.UnitId equals units.Id into unitPutAwayLines
                                      from units in unitPutAwayLines.DefaultIfEmpty()
                                      select new WarehousePutAwayLineDto
                                      {
                                          Id = putawayLines.Id,
                                          PutAwayNo = putawayLines.PutAwayNo,
                                          ProductCode = putawayLines.ProductCode,
                                          ProductShortCode = String.IsNullOrEmpty(products.ProductShortCode) ? putawayLines.ProductCode : products.ProductShortCode,
                                          UnitId = putawayLines.UnitId,
                                          UnitName = units != null ? units.UnitName : "",
                                          JournalQty = putawayLines.JournalQty,
                                          TransQty = putawayLines.TransQty,
                                          Bin = putawayLines.Bin,
                                          LotNo = putawayLines.LotNo,
                                          ExpirationDate = putawayLines.ExpirationDate,
                                          ReceiptLineId = putawayLines.ReceiptLineId,
                                          ProductName = products.ProductName,
                                          ProductUrl = products.ProductUrl,
                                          ProductJanCodes = _dbContext.ProductJanCodes.Where(x => x.ProductId == products.Id).Select(x => x.JanCode).ToList(),
                                      })
                on putaways.PutAwayNo equals putawayLines.PutAwayNo into putAwayOrderLines
                select new WarehousePutAwayDto
                {
                    Id = putaways.Id,
                    ReceiptNo = putaways.ReceiptNo,
                    Description = putaways.Description,
                    TransDate = putaways.TransDate,
                    DocumentDate = putaways.DocumentDate,
                    PutAwayNo = putaways.PutAwayNo,
                    Location = putaways.Location,
                    TenantId = putaways.TenantId,
                    DocumentNo = putaways.DocumentNo,
                    IsDeleted = putaways.IsDeleted,
                    Status = putaways.Status,
                    WarehousePutAwayLines = putAwayOrderLines.ToList(),
                }).FirstOrDefaultAsync(_ => _.PutAwayNo == PutAwayNo && _.IsDeleted != true);

                if (putAway != null)
                {
                    return await Result<WarehousePutAwayDto>.SuccessAsync(putAway);
                }
                else
                {
                    return await Result<WarehousePutAwayDto>.FailAsync();
                }
            }
            catch (Exception ex)
            {
                return await Result<WarehousePutAwayDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayDto>> SyncHTData(WarehousePutAwayDto putAwayDto)
        {
            try
            {
                var userInfo = _contextAccessor.HttpContext?.User.FindFirst("UserId");
                var putAwayStagings = await _dbContext.WarehousePutAwayStagings.Where(_ => _.IsDeleted != true && _.PutAwayNo == putAwayDto.PutAwayNo).ToListAsync();
                /*
                var putAwayLineIds = putAwayDto.WarehousePutAwayLines.Select(line => line.Id).ToList();
                var hasValidReceiptStaging = putAwayStagings
                    .Any(staging => putAwayLineIds.Contains(staging.PutAwayLineId));

                if (!hasValidReceiptStaging)
                {
                    return Result<WarehousePutAwayDto>.Fail("No data found for this Receipt No");
                }

                var putAwayLinesDto = (
                    from putAwayLine in putAwayDto.WarehousePutAwayLines
                    join putAwayStaging in putAwayStagings on putAwayLine.Id equals putAwayStaging.PutAwayLineId into putAwayLineStaging
                    from putAwayStaging in putAwayLineStaging.DefaultIfEmpty()
                    select new WarehousePutAwayLineDto(putAwayLine, putAwayStaging.JournalQty, putAwayStaging.TransQty, putAwayStaging.Bin));

                var putAwayLines = putAwayLinesDto.Select(_ => 
                new WarehousePutAwayLine(_.Id, _.PutAwayNo, _.ProductCode, _.UnitId, _.JournalQty, _.TransQty, _.Bin, _.LotNo, _.ExpirationDate,
                _.TenantId, _.Status, _.CreateAt, now));
                putAwayDto.WarehousePutAwayLines = putAwayLinesDto.ToList();
                _dbContext.WarehousePutAwayLines.UpdateRange(putAwayLines);
                */
                if (putAwayStagings.Count == 0)
                {
                    return Result<WarehousePutAwayDto>.Fail("No data found in HT");
                }
                var putAwayLinesDto = (
                    from staging in _dbContext.WarehousePutAwayStagings.Where(_ => _.PutAwayNo == putAwayDto.PutAwayNo)
                    join product in _dbContext.Products on staging.ProductCode equals product.ProductCode into products
                    from product in products.DefaultIfEmpty()
                    select new WarehousePutAwayLineDto
                    {
                        Id = Guid.NewGuid(),
                        PutAwayNo = putAwayDto.PutAwayNo,
                        TenantId = putAwayDto.TenantId,
                        ProductCode = staging.ProductCode,
                        UnitId = product.UnitId,
                        JournalQty = staging.JournalQty,
                        TransQty = staging.TransQty,
                        Bin = staging.Bin,
                        LotNo = staging.LotNo,
                        ExpirationDate = staging.ExpiryDate,
                        ReceiptLineId = staging.ReceiptLineId,
                        CreateAt = now,
                        CreateOperatorId = userInfo == null ? "" : userInfo.Value
                    });
                var productHHTSet = putAwayLinesDto.Select(_ => _.ProductCode).ToHashSet();
                var putAwayLines = putAwayLinesDto.Select(_ => new WarehousePutAwayLine
                {
                    Id = _.Id,
                    PutAwayNo = _.PutAwayNo,
                    TenantId = _.TenantId,
                    ProductCode = _.ProductCode,
                    UnitId = _.UnitId,
                    JournalQty = _.JournalQty,
                    TransQty = _.TransQty,
                    Bin = _.Bin,
                    LotNo = _.LotNo,
                    ExpirationDate = _.ExpirationDate,
                    ReceiptLineId = _.ReceiptLineId,
                    CreateAt = now,
                    CreateOperatorId = userInfo == null ? "" : userInfo.Value
                });
                var missingProducts = putAwayDto.WarehousePutAwayLines.Where(_ => _.PutAwayNo == putAwayDto.PutAwayNo && !productHHTSet.Contains(_.ProductCode));
                putAwayDto.WarehousePutAwayLines = putAwayLinesDto.ToList();
                putAwayDto.WarehousePutAwayLines.AddRange(missingProducts);

                _dbContext.WarehousePutAwayLines.RemoveRange(_dbContext.WarehousePutAwayLines.Where(_ => _.PutAwayNo == putAwayDto.PutAwayNo && productHHTSet.Contains(_.ProductCode)));
                _dbContext.WarehousePutAwayLines.AddRange(putAwayLines);
                await _dbContext.SaveChangesAsync();
                return Result<WarehousePutAwayDto>.Success(putAwayDto);
            }
            catch (Exception ex)
            {
                return Result<WarehousePutAwayDto>.Fail($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehousePutAwayDto>> AdjustActionPutAway([Body] WarehousePutAwayDto request)
        {
            //using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var userInfo = _contextAccessor.HttpContext?.User.FindFirst("UserId");
                switch (request.ReferenceType)
                {
                    case EnumWarehouseTransType.Bundle:

                        var bundle = await _dbContext.InventBundles.FirstOrDefaultAsync(_ => _.TransNo == request.ReceiptNo);

                        if (bundle == null)
                        {
                            return Result<WarehousePutAwayDto>.Fail($"this bundle {request.ReceiptNo} does not exist!");
                        }

                        break;

                    default:
                        var receipt = await _dbContext.WarehouseReceiptOrders.FirstOrDefaultAsync(_ => _.ReceiptNo == request.ReceiptNo);

                        if (receipt == null)
                        {
                            return Result<WarehousePutAwayDto>.Fail($"this receipt {request.ReceiptNo} does not exist!");
                        }
                        break;
                }

                //Validation to make sure all recieved qty has ben putaway
                var warehouseTransLines = _dbContext.WarehouseTrans.Where(_ => _.TransNumber == request.ReceiptNo).ToList();
                var receivedProduct = new HashSet<string>(warehouseTransLines.Select(_ => _.ProductCode));
                var wrongProducts = request.WarehousePutAwayLines.Where(_ => !receivedProduct.Contains(_.ProductCode)).ToList();
                if (wrongProducts.Count > 0)
                {
                    return await Result<WarehousePutAwayDto>.FailAsync("Validation.WrongProduct");
                }
                var lines = request.WarehousePutAwayLines.Where(x => String.IsNullOrEmpty(x.Bin));
                if (lines.Count() > 0)
                {
                    return await Result<WarehousePutAwayDto>.FailAsync("RequiredBin");
                }
                /*
                foreach (var warehouseTransLine in warehouseTransLines)
                {
                    var totalPutAway = _dbContext.WarehousePutAwayLines.Where(_ => _.PutAwayNo == request.PutAwayNo && _.ProductCode == warehouseTransLine.ProductCode &&
                        _.ReceiptLineId == warehouseTransLine.TransLineId).Sum(x => x.TransQty);
                    if (warehouseTransLine.Qty != totalPutAway)
                    {
                        return await Result<WarehousePutAwayDto>.FailAsync("Validation.UnmatchQty");
                    }
                }
                */
                if (request.ReferenceType == EnumWarehouseTransType.Bundle)
                {

                    var Bundle = await _dbContext.InventBundles.Where(_ => _.TransNo == request.ReferenceNo).FirstOrDefaultAsync();
                    Bundle.Status = EnumStatusBundle.Completed;
                    _dbContext.InventBundles.Update(Bundle);
                }

                else
                {
                    var receiptPayload = await _dbContext.WarehouseReceiptOrders.Where(_ => _.ReceiptNo == request.ReceiptNo).FirstOrDefaultAsync();
                    if (receiptPayload != null)
                    {
                        receiptPayload.Status = EnumReceiptOrderStatus.Completed;
                        _dbContext.WarehouseReceiptOrders.Update(receiptPayload);

                        await AdjustPutAwayStatusByReferenceType(receiptPayload);
                    }
                }

                //update warehousePutaway --> complete
                request.PostedDate = DateOnly.FromDateTime(DateTime.Now);
                request.PostedBy = userInfo.Value;
                request.Status = EnumPutAwayStatus.Completed;

                var configMap = new MapperConfiguration(cfg =>{
                    cfg.CreateMap<WarehousePutAwayDto,WarehousePutAway>();
                });
                var mapper = configMap.CreateMapper();

                var dataUpdate = mapper.Map<WarehousePutAway>(request);
                _dbContext.WarehousePutAways.Update(dataUpdate);

                //_dbContext.WarehousePutAways.Where(_ => _.PutAwayNo == request.PutAwayNo).ExecuteUpdate(_ => _
                //                .SetProperty(r => r.Status, EnumPutAwayStatus.Completed)
                //                .SetProperty(r => r.Location, request.Location));

                var warehouseTrans = new List<WarehouseTran>();
                var updatedBatches = new List<FBT.ShareModels.WMS.Batches>();
                var putAwaysLines = _dbContext.WarehousePutAwayLines.Where(_ => _.PutAwayNo == request.PutAwayNo).ToList();

                foreach (var warehouseTransLine in warehouseTransLines)
                {

                    if (request.ReferenceType == EnumWarehouseTransType.Bundle)
                    {
                        warehouseTrans.Add(new WarehouseTran
                        {
                            Id = Guid.NewGuid(),
                            TransType = EnumWarehouseTransType.PutAway,
                            TransNumber = request.PutAwayNo,
                            StatusIssue = EnumStatusIssue.Delivered,
                            DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                            ProductCode = warehouseTransLine.ProductCode,
                            Qty = -1 * warehouseTransLine.Qty,
                            TenantId = warehouseTransLine.TenantId,
                            Location = warehouseTransLine.Location,
                            Bin = String.IsNullOrWhiteSpace(warehouseTransLine.Bin) ? "N/A" : warehouseTransLine.Bin,
                            LotNo = warehouseTransLine.LotNo,
                            CreateAt = now,
                            CreateOperatorId = userInfo == null ? "" : userInfo.Value
                        }
                        );
                    }
                    else
                    {
                        warehouseTrans.Add(new WarehouseTran
                        {
                            Id = Guid.NewGuid(),
                            TransType = EnumWarehouseTransType.PutAway,
                            TransNumber = request.PutAwayNo,
                            StatusIssue = EnumStatusIssue.Delivered,
                            TransId = request.Id,
                            //TransLineId = warehouseTransLine.TransLineId,
                            DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                            ProductCode = warehouseTransLine.ProductCode,
                            Qty = -1 * warehouseTransLine.Qty,
                            TenantId = warehouseTransLine.TenantId,
                            Location = warehouseTransLine.Location,
                            Bin = String.IsNullOrWhiteSpace(warehouseTransLine.Bin) ? "N/A" : warehouseTransLine.Bin,
                            LotNo =  warehouseTransLine.LotNo,
                            CreateAt = now,
                            CreateOperatorId = userInfo == null ? "" : userInfo.Value
                        }
                    );
                    }
                }

                foreach (var putAwaysLine in putAwaysLines)
                {

                    if (request.ReferenceType == EnumWarehouseTransType.Bundle)
                    {
                        warehouseTrans.Add(new WarehouseTran
                        {
                            Id = Guid.NewGuid(),
                            TransNumber = putAwaysLine.PutAwayNo,
                            TransId = request.Id,
                            TransLineId = putAwaysLine.Id,
                            StatusReceipt = EnumStatusReceipt.Received,
                            DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                            ProductCode = putAwaysLine.ProductCode,
                            Qty = putAwaysLine.TransQty ?? 0,
                            Location = request.Location,
                            TenantId = request.TenantId,
                            Bin = String.IsNullOrWhiteSpace(putAwaysLine.Bin) ? "N/A" : putAwaysLine.Bin,
                            LotNo = String.IsNullOrWhiteSpace(putAwaysLine.LotNo) ? CommonHelpers.ParseLotno(putAwaysLine.ExpirationDate, request.ReceiptNo) : putAwaysLine.LotNo,
                            CreateAt = now,
                            CreateOperatorId = userInfo == null ? "" : userInfo.Value
                        }
                        );
                    }
                    else
                    {
                        warehouseTrans.Add(new WarehouseTran
                        {
                            Id = Guid.NewGuid(),
                            TransType = EnumWarehouseTransType.PutAway,
                            TransNumber = putAwaysLine.PutAwayNo,
                            StatusReceipt = EnumStatusReceipt.Received,
                            TransId = request.Id,
                            TransLineId = putAwaysLine.Id,
                            DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                            ProductCode = putAwaysLine.ProductCode,
                            Qty = putAwaysLine.TransQty ?? 0,
                            Location = request.Location,
                            TenantId = request.TenantId,
                            Bin = String.IsNullOrWhiteSpace(putAwaysLine.Bin) ? "N/A" : putAwaysLine.Bin,
                            LotNo = String.IsNullOrWhiteSpace(putAwaysLine.LotNo) ? CommonHelpers.ParseLotno(putAwaysLine.ExpirationDate, request.ReceiptNo) : putAwaysLine.LotNo,
                            CreateAt = now,
                            CreateOperatorId = userInfo == null ? "" : userInfo.Value
                        }
                        );
                        

                    }
                    //putAwaysLine.LotNo = String.IsNullOrEmpty(putAwaysLine.LotNo) ? CommonHelpers.ParseLotno(_.ExpirationDate, request.ReceiptNo) : putAwaysLine.LotNo;
                    var lotNo = String.IsNullOrWhiteSpace(putAwaysLine.LotNo) ? CommonHelpers.ParseLotno(putAwaysLine.ExpirationDate, request.ReceiptNo) : putAwaysLine.LotNo;
                    //await _dbContext.Batches.Where(x => x.TenantId == request.TenantId&&  x.ProductCode == putAwaysLine.ProductCode && x.LotNo == lotNo).ExecuteDeleteAsync();
                    int count = updatedBatches.Where(x => x.TenantId == request.TenantId && x.ProductCode == putAwaysLine.ProductCode && x.LotNo == lotNo).ToList().Count();
                    if (count == 0)
                    {
                        updatedBatches.Add(new FBT.ShareModels.WMS.Batches
                        {
                            Id = Guid.NewGuid(),
                            TenantId = request.TenantId,
                            LotNo = lotNo,
                            ProductCode = putAwaysLine.ProductCode,
                            ExpirationDate = putAwaysLine.ExpirationDate,
                            CreateAt = now,
                            CreateOperatorId = userInfo == null ? "" : userInfo.Value,
                            IsDeleted = false
                        }
                        );
                    }
                    
                   
                }
                foreach (var batch in updatedBatches)
                {
                    _dbContext.Batches.Where(x => x.TenantId == request.TenantId && x.ProductCode == batch.ProductCode && x.LotNo == batch.LotNo).ExecuteDelete();
                }
                _dbContext.WarehouseTrans.AddRange(warehouseTrans);
                _dbContext.Batches.AddRange(updatedBatches);
                await _dbContext.SaveChangesAsync();
                //transaction.Commit();

                return Result<WarehousePutAwayDto>.Success(request);
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                return Result<WarehousePutAwayDto>.Fail(ex.ToString());
            }
        }
        public async Task<Result<WarehousePutAway>> UpdateWarehousePutAwaysStatus([Body] WarehousePutAway request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var userInfo = _contextAccessor.HttpContext?.User.FindFirst("UserId");

                switch (request.ReferenceType)
                {
                    case EnumWarehouseTransType.Bundle:

                        var bundle = await _dbContext.InventBundles.FirstOrDefaultAsync(_ => _.TransNo == request.ReceiptNo);

                        if (bundle == null)
                        {
                            return Result<WarehousePutAway>.Fail($"this bundle {request.ReceiptNo} does not exist!");
                        }

                        break;

                    default:
                        var receipt = await _dbContext.WarehouseReceiptOrders.FirstOrDefaultAsync(_ => _.ReceiptNo == request.ReceiptNo);

                        if (receipt == null)
                        {
                            return Result<WarehousePutAway>.Fail($"this receipt {request.ReceiptNo} does not exist!");
                        }
                        break;
                }
                //Validation to make sure all recieved qty has ben putaway
                var warehouseTransLines = _dbContext.WarehouseTrans.Where(_ => _.TransNumber == request.ReceiptNo).ToList();
                //var receivedProduct = new HashSet<string>(warehouseTransLines.Select(_ => _.ProductCode));
                //var wrongProducts = request.WarehousePutAwayLines.Where(_ => !receivedProduct.Contains(_.ProductCode)).ToList();
                //if (wrongProducts.Count > 0)
                //{
                //    return await Result<WarehousePutAway>.FailAsync("Validation.WrongProduct");
                //}
                /*
                foreach (var warehouseTransLine in warehouseTransLines)
                {
                    var totalPutAway = _dbContext.WarehousePutAwayLines.Where(_ => _.PutAwayNo == request.PutAwayNo && _.ProductCode == warehouseTransLine.ProductCode &&
                        _.ReceiptLineId == warehouseTransLine.TransLineId).Sum(x => x.TransQty);
                    if (warehouseTransLine.Qty != totalPutAway)
                    {
                        return await Result<WarehousePutAway>.FailAsync("Validation.UnmatchQty");
                    }
                }
                */
                var lines = _dbContext.WarehousePutAwayLines.Where(x => x.PutAwayNo == request.PutAwayNo && String.IsNullOrEmpty(x.Bin));
                if (lines.Count() > 0)
                {
                    return await Result<WarehousePutAway>.FailAsync("RequiredBin");
                }

                if (request.ReferenceType == EnumWarehouseTransType.Bundle)
                {

                    var Bundle = await _dbContext.InventBundles.Where(_ => _.TransNo == request.ReferenceNo).FirstOrDefaultAsync();
                    Bundle.Status = EnumStatusBundle.Completed;
                    _dbContext.InventBundles.Update(Bundle);
                }

                else
                {
                    var receiptPayload = await _dbContext.WarehouseReceiptOrders.Where(_ => _.ReceiptNo == request.ReceiptNo).FirstOrDefaultAsync();
                    if (receiptPayload != null)
                    {
                        receiptPayload.Status = EnumReceiptOrderStatus.Completed;
                        _dbContext.WarehouseReceiptOrders.Update(receiptPayload);

                        await AdjustPutAwayStatusByReferenceType(receiptPayload);
                    }
                }

                //update warehousePutaway --> complete
                request.PostedDate = DateOnly.FromDateTime(DateTime.Now);
                request.PostedBy = userInfo.Value;
                request.Status = EnumPutAwayStatus.Completed;
                var location = await _dbContext.Locations.FirstOrDefaultAsync(x => x.LocationName == request.Location);
                request.Location = location?.Id.ToString();

                _dbContext.WarehousePutAways.Update(request);
                //_dbContext.WarehousePutAways.Where(_ => _.PutAwayNo == request.PutAwayNo).ExecuteUpdate(_ => _
                //                .SetProperty(r => r.Status, EnumPutAwayStatus.Completed));


                var warehouseTrans = new List<WarehouseTran>();
                var putAwaysLines = _dbContext.WarehousePutAwayLines.Where(_ => _.PutAwayNo == request.PutAwayNo).ToList();

                foreach (var warehouseTransLine in warehouseTransLines)
                {

                    if (request.ReferenceType == EnumWarehouseTransType.Bundle)
                    {
                        warehouseTrans.Add(new WarehouseTran
                        {
                            Id = Guid.NewGuid(),
                            TransType = EnumWarehouseTransType.PutAway,
                            TransNumber = request.PutAwayNo,
                            StatusIssue = EnumStatusIssue.Delivered,
                            DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                            ProductCode = warehouseTransLine.ProductCode,
                            Qty = -1 * warehouseTransLine.Qty,
                            TenantId = warehouseTransLine.TenantId,
                            Location = warehouseTransLine.Location,
                            Bin = String.IsNullOrWhiteSpace(warehouseTransLine.Bin) ? "N/A" : warehouseTransLine.Bin,
                            LotNo =  warehouseTransLine.LotNo,
                            CreateAt = now,
                            CreateOperatorId = userInfo == null ? "" : userInfo.Value
                        }
                        );
                    }
                    else
                    {
                        warehouseTrans.Add(new WarehouseTran
                        {
                            Id = Guid.NewGuid(),
                            TransType = EnumWarehouseTransType.PutAway,
                            TransNumber = request.PutAwayNo,
                            StatusIssue = EnumStatusIssue.Delivered,
                            TransId = request.Id,
                            //TransLineId = warehouseTransLine.TransLineId,
                            DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                            ProductCode = warehouseTransLine.ProductCode,
                            Qty = -1 * warehouseTransLine.Qty,
                            TenantId = warehouseTransLine.TenantId,
                            Location = warehouseTransLine.Location,
                            Bin = String.IsNullOrWhiteSpace(warehouseTransLine.Bin) ? "N/A" : warehouseTransLine.Bin,
                            LotNo = warehouseTransLine.LotNo,
                            CreateAt = now,
                            CreateOperatorId = userInfo == null ? "" : userInfo.Value
                        }
                    );
                    }
                }

                foreach (var putAwaysLine in putAwaysLines)
                {
                    
                    if (request.ReferenceType == EnumWarehouseTransType.Bundle)
                    {
                        warehouseTrans.Add(new WarehouseTran
                        {
                            Id = Guid.NewGuid(),
                            TransNumber = putAwaysLine.PutAwayNo,
                            TransId = request.Id,
                            TransLineId = putAwaysLine.Id,
                            StatusReceipt = EnumStatusReceipt.Received,
                            DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                            ProductCode = putAwaysLine.ProductCode,
                            Qty = putAwaysLine.TransQty ?? 0,
                            Location = request.Location,
                            TenantId = request.TenantId,
                            Bin = String.IsNullOrWhiteSpace(putAwaysLine.Bin) ? "N/A" : putAwaysLine.Bin,
                            LotNo = String.IsNullOrWhiteSpace(putAwaysLine.LotNo) ? CommonHelpers.ParseLotno(putAwaysLine.ExpirationDate, request.ReceiptNo) : putAwaysLine.LotNo,
                            CreateAt = now,
                            CreateOperatorId = userInfo == null ? "" : userInfo.Value
                        }
                        );
                    }
                    else
                    {
                        warehouseTrans.Add(new WarehouseTran
                        {
                            Id = Guid.NewGuid(),
                            TransType = EnumWarehouseTransType.PutAway,
                            TransNumber = putAwaysLine.PutAwayNo,
                            StatusReceipt = EnumStatusReceipt.Received,
                            TransId = request.Id,
                            TransLineId = putAwaysLine.Id,
                            DatePhysical = DateOnly.FromDateTime(DateTime.Now),
                            ProductCode = putAwaysLine.ProductCode,
                            Qty = putAwaysLine.TransQty ?? 0,
                            Location = request.Location,
                            TenantId = request.TenantId,
                            Bin = String.IsNullOrWhiteSpace(putAwaysLine.Bin) ? "N/A" : putAwaysLine.Bin,
                            LotNo = String.IsNullOrWhiteSpace(putAwaysLine.LotNo) ? CommonHelpers.ParseLotno(putAwaysLine.ExpirationDate, request.ReceiptNo) : putAwaysLine.LotNo,
                            CreateAt = now,
                            CreateOperatorId = userInfo == null ? "" : userInfo.Value
                        }
                        );
                    }

                    
                }
                _dbContext.WarehouseTrans.AddRange(warehouseTrans);
                await _dbContext.SaveChangesAsync();
                transaction.Commit();

                return Result<WarehousePutAway>.Success(request);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Result<WarehousePutAway>.Fail(ex.ToString());
            }
        }

        public async Task<Result<ResponseCompleteModel>> CompletePutawayFlow()
        {
            try
            {
                var putaways = await _dbContext.WarehousePutAways
                .Where(p => p.HHTStatus == EnumHHTStatus.Done && p.Status != EnumPutAwayStatus.Completed)
                .ToListAsync();
                if (putaways.Count == 0)
                    return Result<ResponseCompleteModel>.Success(new ResponseCompleteModel(0, 0));
                int success = 0;
                foreach (var putaway in putaways)
                {
                    using var transaction = await _dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var putawayDto = putaway.Adapt<WarehousePutAwayDto>();
                        putawayDto.WarehousePutAwayLines = (
                            from putawayLines in _dbContext.WarehousePutAwayLines.Where(_ => _.PutAwayNo == putaway.PutAwayNo)
                            join product in _dbContext.Products on putawayLines.ProductCode equals product.ProductCode into products
                            from product in products.DefaultIfEmpty()
                            select new WarehousePutAwayLineDto
                            {
                                Id = putawayLines.Id,
                                PutAwayNo = putawayLines.PutAwayNo,
                                TenantId = putawayLines.TenantId,
                                ProductCode = putawayLines.ProductCode,
                                UnitId = product.UnitId,
                                JournalQty = putawayLines.JournalQty,
                                TransQty = putawayLines.TransQty,
                                Bin = putawayLines.Bin,
                                LotNo = putawayLines.LotNo,
                                ExpirationDate = putawayLines.ExpirationDate,
                                ReceiptLineId = putawayLines.ReceiptLineId,
                                CreateAt = putawayLines.CreateAt,
                                CreateOperatorId = putawayLines.CreateOperatorId,
                                UpdateAt = putawayLines.UpdateAt,
                                UpdateOperatorId = putawayLines.UpdateOperatorId
                            }).ToList();

                        // Call SyncHTData
                        var syncResult = await SyncHTData(putawayDto);
                        if (!syncResult.Succeeded)
                        {
                            LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"Failed to sync HT data for putaway {putaway.PutAwayNo}: {string.Join(", ", syncResult.Messages)}");
                            await transaction.RollbackAsync();
                            continue; // Skip to the next putaway
                        }

                        // Call AdjustActionPutAway
                        var adjustResult = await AdjustActionPutAway(putawayDto);
                        if (!adjustResult.Succeeded)
                        {
                            LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"Failed to adjust putaway {putaway.PutAwayNo}: {string.Join(", ", adjustResult.Messages)}");
                            await transaction.RollbackAsync();
                            continue; // Skip to the next putaway
                        }

                        await transaction.CommitAsync();
                        success++;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        LogHelpers.LogFile("COMPLETE_PUTAWAYS", $"Transaction error for putaway {putaway.PutAwayNo}: {ex.Message}");
                    }
                }
                return Result<ResponseCompleteModel>.Success(new ResponseCompleteModel(putaways.Count, success)); // Return the last processed putAwayDto or adjust as needed
            }
            catch (Exception ex)
            {
                return Result<ResponseCompleteModel>.Fail(ex); // Return the last processed putAwayDto or adjust as needed
            }
        }

        public async Task AdjustPutAwayStatusByReferenceType(WarehouseReceiptOrder model)
        {
            switch (model.ReferenceType)
            {
                case EnumWarehouseTransType.Return:
                    _dbContext.ReturnOrders.Where(_ => _.ReturnOrderNo == model.ReferenceNo).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumReturnOrderStatus.Completed));
                    return;
                case EnumWarehouseTransType.Movement:
                    _dbContext.WarehouseShipments.Where(_ => _.ShipmentNo == model.ReferenceNo).ExecuteUpdate(_ => _.SetProperty(r => r.Status, EnumShipmentOrderStatus.Completed));
                    return;
                default:
                    return;
            }
        }
        // ...

        public async Task<Result<List<PutawayReportDTO>>> GetPutawayReportAsync()
        {
            try
            {
                var resInvent = await _dbContext.Database.SqlQueryRaw<PutawayReportDTO>("sp_dashboardGetPutawayData")
                          .ToListAsync();

                return await Result<List<PutawayReportDTO>>.SuccessAsync(resInvent);
            }
            catch (Exception ex)
            {
                return await Result<List<PutawayReportDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
            //var putAways = await _dbContext.WarehousePutAways
            //    .Where(x => x.IsDeleted != true)
            //    .Select(x => new
            //    {
            //        x.TenantId,
            //        x.TransDate,
            //        x.PutAwayNo
            //    })
            //    .ToListAsync(); 

            //var tenantIds = putAways.Select(x => x.TenantId).Distinct().ToList();
            //var tenantNames = await _dbContext.TenantAuth
            //    .Where(t => tenantIds.Contains(t.TenantId))
            //    .ToDictionaryAsync(t => t.TenantId, t => t.TenantFullName);

            //var reports = putAways
            //    .GroupBy(x => new { x.TenantId, x.TransDate })
            //    .Select(g =>
            //    {
            //        var firstPutAway = g.First();
            //        var putAwayNo = firstPutAway.PutAwayNo;

            //        return new PutawayReportDTO
            //        {
            //            Tenant = tenantNames.GetValueOrDefault(g.Key.TenantId, string.Empty),
            //            Period = g.Key.TransDate.Value,
            //            ExpectedStock = CalculateExpectedStock(putAwayNo),
            //            RemainingNumber = CalculateRemainingNumber(putAwayNo),
            //            ProgressRate = ProgressRate(putAwayNo),
            //            TotalJournalQty = TotalJournalQty(putAwayNo),
            //            TotalTransQty = TotalTransQty(putAwayNo),
            //            TotalRemainingNumber = TotalRemainingNumber(putAwayNo),
            //            Productivity = "0" // Implement this method
            //        };
            //    })
            //    .ToList();
        }

        private int TotalJournalQty(string putawayno)
        {
            var putAways = _dbContext.WarehousePutAways
                .FirstOrDefault(wp => wp.PutAwayNo == putawayno);

            if (putAways == null)
            {

                return 0; // Default value if shipment not found
            }

            var putAwayLines = _dbContext.WarehousePutAwayLines
                .Where(wpl => wpl.PutAwayNo == putawayno)
                .ToList();
            int JournalQty = putAwayLines.Sum(wpl => (int?)wpl.JournalQty) ?? 0;
            return JournalQty;
        }
        private int TotalTransQty(string putawayno)
        {
            var putAways = _dbContext.WarehousePutAways
                .FirstOrDefault(wp => wp.PutAwayNo == putawayno);

            if (putAways == null)
            {

                return 0; // Default value if shipment not found
            }

            var putAwayLines = _dbContext.WarehousePutAwayLines
                .Where(wpl => wpl.PutAwayNo == putawayno)
                .ToList();
            int TransQty = putAwayLines.Sum(wpl => (int?)wpl.TransQty) ?? 0;
            return TransQty;
        }
        private int TotalRemainingNumber(string putawayno)
        {
            var putAways = _dbContext.WarehousePutAways
                .FirstOrDefault(wp => wp.PutAwayNo == putawayno);

            if (putAways == null)
            {

                return 0; // Default value if shipment not found
            }

            var putAwayLines = _dbContext.WarehousePutAwayLines
                .Where(wpl => wpl.PutAwayNo == putawayno)
                .ToList();

            int JournalQty = putAwayLines.Sum(wpl => (int?)wpl.JournalQty) ?? 0;
            int TransQty = putAwayLines.Sum(wpl => (int?)wpl.TransQty) ?? 0;
            var RemainingNumber = JournalQty - TransQty;
            return RemainingNumber;
        }
        private string? CalculateExpectedStock(string putawayno)
        {
            var putAways = _dbContext.WarehousePutAways
                .FirstOrDefault(wp => wp.PutAwayNo == putawayno);

            if (putAways == null)
            {

                return "0/0"; // Default value if shipment not found
            }

            var putAwayLines = _dbContext.WarehousePutAwayLines
                .Where(wpl => wpl.PutAwayNo == putawayno)
                .ToList();

            int JournalQty = putAwayLines.Sum(wpl => (int?)wpl.JournalQty) ?? 0;
            int TransQty = putAwayLines.Sum(wpl => (int?)wpl.TransQty) ?? 0;



            return $"{JournalQty}/{TransQty}";
        }
        private string? CalculateRemainingNumber(string putawayno)
        {
            var putAways = _dbContext.WarehousePutAways
                .Where(wp => wp.Status == EnumPutAwayStatus.PutAway &&
                             wp.TransDate.HasValue && wp.TransDate.Value <= DateOnly.FromDateTime(DateTime.Now))
                .FirstOrDefault(wp => wp.PutAwayNo == putawayno);

            if (putAways == null)
            {

                return "0/0"; // Default value if shipment not found
            }

            var putAwayLines = _dbContext.WarehousePutAwayLines
                .Where(wpl => wpl.PutAwayNo == putawayno)
                .ToList();

            int JournalQty = putAwayLines.Sum(wpl => (int?)wpl.JournalQty) ?? 0;
            int TransQty = putAwayLines.Sum(wpl => (int?)wpl.TransQty) ?? 0;
            var RemainingNumber = JournalQty - TransQty;

            return $"{RemainingNumber}/{TransQty}";
        }
        private string? CalculateProgressRate(string putawayno)
        {
            var putAways = _dbContext.WarehousePutAways
                .FirstOrDefault(wp => wp.PutAwayNo == putawayno);

            if (putAways == null)
            {
                return "0/0"; // Default value if shipment not found
            }

            var putAwayLines = _dbContext.WarehousePutAwayLines
                .Where(wpl => wpl.PutAwayNo == putawayno)
                .ToList();

            int JournalQty = putAwayLines.Sum(wpl => (int?)wpl.JournalQty) ?? 0;
            int TransQty = putAwayLines.Sum(wpl => (int?)wpl.TransQty) ?? 0;
            var RemainingNumber = JournalQty - TransQty;

            double percentage = Math.Round((double)RemainingNumber / TransQty * 100, 2);


            return $"{percentage}%";
        }
        private double ProgressRate(string putawayno)
        {
            var putAways = _dbContext.WarehousePutAways
                .FirstOrDefault(wp => wp.PutAwayNo == putawayno);

            if (putAways == null)
            {
                return 0; // Default value if shipment not found
            }

            var putAwayLines = _dbContext.WarehousePutAwayLines
                .Where(wpl => wpl.PutAwayNo == putawayno)
                .ToList();

            int JournalQty = putAwayLines.Sum(wpl => (int?)wpl.JournalQty) ?? 0;
            int TransQty = putAwayLines.Sum(wpl => (int?)wpl.TransQty) ?? 0;
            var RemainingNumber = JournalQty - TransQty;

            double percentage = Math.Round((double)RemainingNumber / TransQty * 100, 2);

            return percentage;
        }

        private string? CalculateProductivity(string putawayno)
        {
            // Implement logic to calculate productivity
            return "0/0"; // Placeholder
        }

    }
}
