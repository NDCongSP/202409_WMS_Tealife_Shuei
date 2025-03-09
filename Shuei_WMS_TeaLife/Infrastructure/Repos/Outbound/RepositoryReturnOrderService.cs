using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Extentions.Pagings;
using Application.Services.Outbound;
using Infrastructure.Data;
using Infrastructure.Validators;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;

namespace Infrastructure.Repos.Outbound
{
    class RepositoryReturnOrderService : IReturnOrder
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public RepositoryReturnOrderService(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
        {
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
        }

        public async Task<Result<List<ReturnOrder>>> AddRangeAsync([Body] List<ReturnOrder> model)
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

                await _dbContext.ReturnOrders.AddRangeAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<List<ReturnOrder>>.SuccessAsync(model, "Add range ReturnOrder successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<ReturnOrder>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrder>> DeleteAsync([Body] ReturnOrder model)
        {
            try
            {
                _dbContext.ReturnOrders.RemoveRange(model);
                _dbContext.ReturnOrderLines.Where(x => x.ReturnOrderNo == model.ReturnOrderNo).ExecuteDelete();
                await _dbContext.SaveChangesAsync();
                return await Result<ReturnOrder>.SuccessAsync("Delete range ReturnOrders successfull");
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrder>> DeleteRangeAsync([Body] List<ReturnOrder> model)
        {
            try
            {
                _dbContext.ReturnOrders.RemoveRange(model);
                await _dbContext.SaveChangesAsync();
                return await Result<ReturnOrder>.SuccessAsync("Delete range ReturnOrder successfull");
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ReturnOrder>>> GetAllAsync()
        {
            try
            {
                return await Result<List<ReturnOrder>>.SuccessAsync(await _dbContext.ReturnOrders.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ReturnOrder>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrder>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<ReturnOrder>.SuccessAsync(await _dbContext.ReturnOrders.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrder>> InsertAsync([Body] ReturnOrder model)
        {
            try
            {
                await _dbContext.ReturnOrders.AddAsync(model);
                await _dbContext.SaveChangesAsync();
                return await Result<ReturnOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrder>> UpdateAsync([Body] ReturnOrder model)
        {
            try
            {
                _dbContext.ReturnOrders.Update(model);
                await _dbContext.SaveChangesAsync();
                return await Result<ReturnOrder>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrder>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ReturnOrderDto>>> GetAllReturnOrdersAsync()
        {
            try
            {
                var result =( from returnOrder in _dbContext.ReturnOrders.Where(_ => _.IsDeleted != true)
                    join pic in _dbContext.Users on returnOrder.PersonInCharge equals pic.Id into pics
                    from pic in pics.DefaultIfEmpty()
                    join receipt in _dbContext.WarehouseReceiptOrders on returnOrder.ReturnOrderNo equals receipt.ReferenceNo into receipts
                    from receipt in receipts.DefaultIfEmpty()
                    join returnOrderLines in _dbContext.ReturnOrderLines.Where(_ => _.IsDeleted != true) on returnOrder.ReturnOrderNo equals returnOrderLines.ReturnOrderNo into returnOrders
                    select new ReturnOrderDto(returnOrder, returnOrders.ToList(), pic.FullName ?? string.Empty, receipt.ReferenceNo ?? string.Empty)
                );

                return await Result<List<ReturnOrderDto>>.SuccessAsync(await result.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<ReturnOrderDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrderDto>> GetReturnOrderByReturnNoAsync(string returnOrderNo)
        {
            try
            {
                var result = await (from returnOrder in _dbContext.ReturnOrders
                              join returnOrderLine in (from line in _dbContext.ReturnOrderLines.Where(_ => _.IsDeleted != true && _.ReturnOrderNo == returnOrderNo)
                                                       join product in _dbContext.Products.Where(p => p.IsDeleted != true) on line.ProductCode equals product.ProductCode into productLines
                                                       from product in productLines
                                                       join unit in _dbContext.Units.Where(p => p.IsDeleted != true) on line.UnitId equals unit.Id into unitLines
                                                       from unit in unitLines
                                                       join location in _dbContext.Locations.Where(p => p.IsDeleted != true) on line.Location equals location.Id.ToString() into locationLines
                                                       from location in locationLines.DefaultIfEmpty()
                                                       select new ReturnOrderLineDto()
                                                       {
                                                           Id = line.Id,
                                                           ReturnOrderNo = line.ReturnOrderNo,
                                                           Location = line.Location,
                                                           Qty = line.Qty,
                                                           Status = line.Status,
                                                           CreateOperatorId = line.CreateOperatorId,
                                                           CreateAt = line.CreateAt,
                                                           UpdateOperatorId = line.UpdateOperatorId,
                                                           UpdateAt = line.UpdateAt,
                                                           IsDeleted = line.IsDeleted,
                                                           ProductCode = line.ProductCode,
                                                           ProductName = product.ProductName,
                                                           UnitId = line.UnitId,
                                                           UnitName = unit.UnitName,
                                                           LocationName = location.LocationName
                                                       })
                              on returnOrder.ReturnOrderNo equals returnOrderLine.ReturnOrderNo into returnOrderLines
                              select new ReturnOrderDto()
                              {
                                  Id = returnOrder.Id,
                                  ReturnOrderNo = returnOrder.ReturnOrderNo,
                                  ShipmentNo = returnOrder.ShipmentNo,
                                  ReturnDate = returnOrder.ReturnDate,
                                  Reason = returnOrder.Reason,
                                  PersonInCharge = returnOrder.PersonInCharge,
                                  ShipDate = returnOrder.ShipDate,
                                  Status = returnOrder.Status,
                                  CreateOperatorId = returnOrder.CreateOperatorId,
                                  CreateAt = returnOrder.CreateAt,
                                  UpdateOperatorId = returnOrder.UpdateOperatorId,
                                  UpdateAt = returnOrder.UpdateAt,
                                  IsDeleted = returnOrder.IsDeleted,
                                  ReturnOrderLines = returnOrderLines.ToList(),
                              }).FirstOrDefaultAsync(_ => _.IsDeleted != true && _.ReturnOrderNo == returnOrderNo);

                return await Result<ReturnOrderDto>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<ReturnOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrderDto>> InsertReturnOrderAsync([Body] ReturnOrderDto dto)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var sequenceIndex = await _dbContext.SequencesNumber.Where(_ => _.JournalType == "Return").FirstOrDefaultAsync();
                if (sequenceIndex != null)
                {
                    sequenceIndex.CurrentSequenceNo += 1;
                    dto.ReturnOrderNo = $"{sequenceIndex.Prefix}{sequenceIndex.CurrentSequenceNo.ToString().PadLeft((int)sequenceIndex.SequenceLength, '0')}";
                    _dbContext.SequencesNumber.Update(sequenceIndex);
                }

                var validator = new ReturnOrderValidator();
                var result = validator.Validate(dto);
                if (!result.IsValid)
                {
                    return Result<ReturnOrderDto>.Fail(string.Join(',', result.Errors));
                }

                bool isReturnOrderExisted = await _dbContext.ReturnOrders.AnyAsync(_ => _.ReturnOrderNo == dto.ReturnOrderNo);
                if (isReturnOrderExisted)
                {
                    return Result<ReturnOrderDto>.Fail("This ReturnOrderDto have already existed");
                }

                var userInfo = _contextAccessor?.HttpContext?.User.FindFirst("UserId");
                var returnOrder = dto.Adapt<ReturnOrder>();
                returnOrder.CreateAt = DateTime.Now;
                returnOrder.CreateOperatorId = userInfo?.Value;
                await _dbContext.ReturnOrders.AddAsync(returnOrder);

                if (dto.ReturnOrderLines.Any())
                {
                    var returnOrderLines = dto.ReturnOrderLines.Adapt<List<ReturnOrderLine>>();
                    foreach (var returnOrderLine in returnOrderLines)
                    {
                        returnOrderLine.ReturnOrderNo = dto.ReturnOrderNo;
                        returnOrderLine.CreateAt = DateTime.Now;
                        returnOrderLine.CreateOperatorId = userInfo?.Value;
                    }
                    await _dbContext.ReturnOrderLines.AddRangeAsync(returnOrderLines);
                }

                await _dbContext.SaveChangesAsync();

                transaction.Commit();
                return await Result<ReturnOrderDto>.SuccessAsync(dto);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<ReturnOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<ReturnOrderDto>> UpdateReturnOrderAsync([Body] ReturnOrderDto dto)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var validator = new ReturnOrderValidator();
                var result = validator.Validate(dto);
                if (!result.IsValid)
                {
                    return Result<ReturnOrderDto>.Fail(string.Join(',', result.Errors));
                }

                var userInfo = _contextAccessor?.HttpContext?.User.FindFirst("UserId");
                var returnOrder = dto.Adapt<ReturnOrder>();
                returnOrder.UpdateAt = DateTime.Now;
                returnOrder.UpdateOperatorId = userInfo?.Value;
                _dbContext.ReturnOrders.Update(returnOrder);

                if (dto.ReturnOrderLines.Any())
                {
                    var returnOrderLines = dto.ReturnOrderLines.Adapt<List<ReturnOrderLine>>();
                    var updateList = new List<ReturnOrderLine>();
                    var createList = new List<ReturnOrderLine>();
                    foreach (var returnOrderLine in returnOrderLines)
                    {
                        if (String.IsNullOrEmpty(returnOrderLine.ReturnOrderNo))
                        {
                            returnOrderLine.ReturnOrderNo = dto.ReturnOrderNo;
                            returnOrderLine.CreateAt = DateTime.Now;
                            returnOrderLine.CreateOperatorId = userInfo?.Value;
                            createList.Add(returnOrderLine);
                        }
                        else
                        {
                            returnOrderLine.ReturnOrderNo = dto.ReturnOrderNo;
                            returnOrderLine.UpdateAt = DateTime.Now;
                            returnOrderLine.UpdateOperatorId = userInfo?.Value;
                            updateList.Add(returnOrderLine);
                        }

                    }
                    if (createList.Count > 0)
                    { 
                        _dbContext.ReturnOrderLines.AddRange(createList);
                    }
                    _dbContext.ReturnOrderLines.UpdateRange(updateList);
                    var lines = await _dbContext.ReturnOrderLines.Where(_ => _.ReturnOrderNo == dto.ReturnOrderNo).ToListAsync();
                    var deletedReceiptLines = lines.Except(returnOrderLines, new ReturnOrderLineComparer());
                    _dbContext.ReturnOrderLines.RemoveRange(deletedReceiptLines);
                }

                await _dbContext.SaveChangesAsync();

                transaction.Commit();
                return await Result<ReturnOrderDto>.SuccessAsync(dto);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<ReturnOrderDto>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<bool>> DeleteReturnOrderAsync([Path] Guid id)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var returnOrder = await _dbContext.ReturnOrders.FirstOrDefaultAsync(_ => _.Id == id);
                if (returnOrder == null) return Result<bool>.Fail("This ReturnOrderDto have already existed");

                _dbContext.ReturnOrders.Remove(returnOrder);
                var lines = await _dbContext.ReturnOrders.Where(_ => _.ReturnOrderNo == returnOrder.ReturnOrderNo).ToListAsync();
                if (lines.Count > 0)
                {
                    _dbContext.ReturnOrders.RemoveRange(lines);
                }
                await _dbContext.SaveChangesAsync();

                transaction.Commit();
                return await Result<bool>.SuccessAsync(true);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Result<bool>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<PageList<ReturnOrderDto>>> SearchReturnOrder([Body] QueryModel<ReturnOrderSearchModel> model)
        {
            try
            {
                var data = model.Entity ?? new ReturnOrderSearchModel();
                var query = (from returnOrder in _dbContext.ReturnOrders
                            .AsEnumerable()
                            .Where(_ => _.IsDeleted == false 
                            && _.ShipmentNo.Contains(data.ShipmentNo ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                            && _.ReturnOrderNo.Contains(data.ReturnOrderNo ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                            && (data.ReturnOrderFrom == default || data.ReturnOrderFrom <= _.ReturnDate)
                            && (data.ReturnOrderTo == default || _.ReturnDate <= data.ReturnOrderTo)
                            && (data.Status == default || _.Status == data.Status))
                            join pic in _dbContext.Users on returnOrder.PersonInCharge equals pic.Id into pics
                            from pic in pics.DefaultIfEmpty()
                            join receipt in _dbContext.WarehouseReceiptOrders on returnOrder.ReturnOrderNo equals receipt.ReferenceNo into receipts
                            from receipt in receipts.DefaultIfEmpty()
                            join returnOrderLines in _dbContext.ReturnOrderLines.Where(_ => _.IsDeleted != true) on returnOrder.ReturnOrderNo equals returnOrderLines.ReturnOrderNo into returnOrders
                            select new ReturnOrderDto(returnOrder, returnOrders.ToList(), pic == null? string.Empty : pic.FullName ?? string.Empty, receipt == null ? string.Empty : (receipt.ReceiptNo ?? string.Empty))).AsQueryable();

                if (!string.IsNullOrEmpty(data.ReceiptNo))
                {
                    var receiptQuery = query.AsEnumerable().Where(_ => !string.IsNullOrEmpty(_.ReferenceNo) && _.ReferenceNo.Contains(data.ReceiptNo ?? string.Empty, StringComparison.OrdinalIgnoreCase));
                    query = receiptQuery.AsEnumerable().Join(query, x => x.Id, y => y.Id, (x, y) => x).AsQueryable();
                }

                var pagedList = PageList<ReturnOrderDto>.PagedResult(query, model.PageNumber, model.PageSize);
                return await Result<PageList<ReturnOrderDto>>.SuccessAsync(pagedList);
            }
            catch (Exception ex)
            {
                return await Result<PageList<ReturnOrderDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<ReturnOrderDto>>> GetReturnByShipmentNo(string shipmentNo)
        {
            try
            {
                var result = await (from returnOrder in _dbContext.ReturnOrders
                                    join returnOrderLine in (from line in _dbContext.ReturnOrderLines.Where(_ => _.IsDeleted != true)
                                                             select new ReturnOrderLineDto()
                                                             {
                                                                 Id = line.Id,
                                                                 ReturnOrderNo = line.ReturnOrderNo,
                                                                 Location = line.Location,
                                                                 Qty = line.Qty,
                                                                 Status = line.Status,
                                                                 CreateOperatorId = line.CreateOperatorId,
                                                                 CreateAt = line.CreateAt,
                                                                 UpdateOperatorId = line.UpdateOperatorId,
                                                                 UpdateAt = line.UpdateAt,
                                                                 IsDeleted = line.IsDeleted,
                                                                 ProductCode = line.ProductCode
                                                             })
                                    on returnOrder.ReturnOrderNo equals returnOrderLine.ReturnOrderNo into returnOrderLines
                                    select new ReturnOrderDto()
                                    {
                                        Id = returnOrder.Id,
                                        ReturnOrderNo = returnOrder.ReturnOrderNo,
                                        ShipmentNo = returnOrder.ShipmentNo,
                                        ReturnDate = returnOrder.ReturnDate,
                                        Reason = returnOrder.Reason,
                                        PersonInCharge = returnOrder.PersonInCharge,
                                        ShipDate = returnOrder.ShipDate,
                                        Status = returnOrder.Status,
                                        CreateOperatorId = returnOrder.CreateOperatorId,
                                        CreateAt = returnOrder.CreateAt,
                                        UpdateOperatorId = returnOrder.UpdateOperatorId,
                                        UpdateAt = returnOrder.UpdateAt,
                                        IsDeleted = returnOrder.IsDeleted,
                                        ReturnOrderLines = returnOrderLines.ToList(),
                                    }).Where(_ => _.IsDeleted != true && _.ShipmentNo == shipmentNo).ToListAsync();
               return  await Result<List<ReturnOrderDto>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<List<ReturnOrderDto>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }


    }

    public class ReturnOrderLineComparer : IEqualityComparer<ReturnOrderLine>
    {
        public bool Equals(ReturnOrderLine x, ReturnOrderLine y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ReturnOrderLine obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
