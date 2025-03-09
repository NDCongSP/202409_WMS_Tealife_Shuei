using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Models;
using Application.Services;
using Bogus;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;

namespace Infrastructure.Repos
{
    public class RepositoryCommonServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : ICommon
    {
        public static List<ShippingInfoCSV> GenerateFakeShippingInfo(int count)
        {
            var faker = new Faker<ShippingInfoCSV>()
                .RuleFor(s => s.CustomerManagementNumber, f => f.Random.AlphaNumeric(10))
                .RuleFor(s => s.ScheduledShippingDate, f => f.Date.Future())
                .RuleFor(s => s.ShippingTimeCategory, f => f.PickRandom("Morning", "Afternoon", "Evening"))
                .RuleFor(s => s.ShippingDeadline, f => f.Date.Future())
                .RuleFor(s => s.ArrivalDeadline, f => f.Date.Future())
                .RuleFor(s => s.MailType, f => f.PickRandom("Standard", "Express", "Registered"))
                .RuleFor(s => s.RefrigerationType, f => f.PickRandom("None", "Chilled", "Frozen"))
                .RuleFor(s => s.PaymentType, f => f.PickRandom("Original", "PaymentToDelivery", "CashOnDelivery"))
                .RuleFor(s => s.DeliveryAddressCode, f => f.Random.AlphaNumeric(5))
                .RuleFor(s => s.DeliveryAddressPostalCode, f => f.Address.ZipCode())
                .RuleFor(s => s.DeliveryAddress1, f => f.Address.StreetAddress())
                .RuleFor(s => s.DeliveryAddress2, f => f.Address.SecondaryAddress())
                .RuleFor(s => s.DeliveryAddressName1, f => f.Name.FullName())
                .RuleFor(s => s.DeliveryAddressTelephoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(s => s.OrderNumber, f => f.Random.AlphaNumeric(12))
                .RuleFor(s => s.OrderDate, f => f.Date.Past())
                .RuleFor(s => s.BreakdownCategory, f => f.Random.Bool())
                .RuleFor(s => s.RawFoodCategory, f => f.Random.Bool())
                .RuleFor(s => s.BinClassification, f => f.Random.Bool())
                .RuleFor(s => s.TotalWeight_G, f => f.Random.Int(500, 5000))
                .RuleFor(s => s.RegisteredMailDamagesRequired, f => f.Random.Decimal(1000, 10000))
                .RuleFor(s => s.DeliveryTimeCategory, f => f.PickRandom("Morning", "Noon", "Evening"))
                .RuleFor(s => s.ProductName, f => f.Commerce.ProductName());

            return faker.Generate(count);
        }

        public async Task<Result<string>> GetDataForExportCsvAsync()
        {
            try
            {
                //var response = await dbContext.Database.SqlQueryRaw<ShippingInfoCSV>("sp_getOrderToExportCSV").ToListAsync();
                List<ShippingInfoCSV> response = GenerateFakeShippingInfo(10);

                if (response.Count == 0 || response == null)
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "Empty.");
                    return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                }

                string tempPath = Path.GetTempFileName();

                CsvHelpers<ShippingInfoCSV>.WriteInfoToCsv(response, tempPath);

                byte[] bytes = await File.ReadAllBytesAsync(tempPath);
                string base64String = Convert.ToBase64String(bytes);

                File.Delete(tempPath);

                return await Result<string>.SuccessAsync(base64String, "");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<string>> ImportCsvAndUpdateTrackingNoAsync1(IFormFile file)
        {
            try
            {
                // Create temp file path
                string tempPath = Path.GetTempFileName();

                // Copy form file to temp file
                using (var stream = file.OpenReadStream())
                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    await stream.CopyToAsync(fileStream);
                }

                // Read CSV into ShippingInfoCSV list
                List<ImportCSVForJPYPModel> csvData = CsvHelpers<ImportCSVForJPYPModel>.ReadFromCsv<ImportCSVForJPYPModel>(tempPath);

                // Delete temp file
                File.Delete(tempPath);

                if (csvData == null || !csvData.Any())
                {
                    var err = new ErrorResponse();
                    err.Errors.Add("Warning", "CSV file is empty or invalid");
                    return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                }

                // Update TrackingNo in OrderDispatches
                var customerManagementNumbers = csvData.Select(x => x.CustomerManagementNumber.ToString()).ToList();
                var ef = await dbContext.OrderDispatches.Where(x=>x.DeliveryId == "D20000002195").ToListAsync();
                var res = await dbContext.OrderDispatches.FirstOrDefaultAsync(_ => _.DeliveryId == "D20000002195");

                using (var transaction=await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var orderDispatches = await dbContext.OrderDispatches
                   .Where(x => customerManagementNumbers.Contains(x.DeliveryId) && (x.IsMarketUpdated == default || x.IsMarketUpdated == false))
                   .ToListAsync();
                        var shipments = await dbContext.WarehouseShipments.Where(x => orderDispatches.Select(xx => xx.OrderId).Contains(x.SalesNo) && x.ShippingCarrierCode == "JP-YP" && x.Status == EnumShipmentOrderStatus.Packed)
                            .ToListAsync();
                        foreach (var dispatch in orderDispatches)
                        {
                            var matchingCsvRow = csvData.FirstOrDefault(x => x.CustomerManagementNumber == dispatch.DeliveryId);
                            if (matchingCsvRow != null)
                            {
                                dispatch.TrackingNo = matchingCsvRow.InquiryNumber;
                                dbContext.Update(dispatch);

                                var matchingShipments = shipments.Where(x => x.SalesNo == dispatch.OrderId).ToList();
                                foreach (var shipment in matchingShipments)
                                {
                                    shipment.TrackingNo = matchingCsvRow.InquiryNumber;
                                }
                                dbContext.UpdateRange(matchingShipments);
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return await Result<string>.SuccessAsync("Successfully updated tracking numbers");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();

                        var err = new ErrorResponse();
                        err.Errors.Add("Error", ex.Message);
                        return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                    }
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<string>> GetLaterPaymentAsync([Body] LaterPayRequest model)
        {
            try
            {
                var domain = "https://www.api-mypage.post.japanpost.jp/webapi/servlet/WEBAPI";
                var handler = new HttpClientHandler
                {
                    UseProxy = false, // Nếu đang gặp vấn đề với proxy
                    UseDefaultCredentials = true
                };
                using (var downloadClient = new HttpClient(handler))
                {
                    var queryParams = new Dictionary<string, string>
                    {
                        ["ctCode"] = model.ctCode.ToString(),
                        ["ctId"] = model.ctId,
                        ["ctPw"] = model.ctPw,
                        ["laterPayNumber1"] = model.laterPayNumber1,
                        ["laterPayNumber2"] = model.laterPayNumber2,
                        ["laterPayNumber3"] = model.laterPayNumber3,
                        ["laterPayNumber4"] = model.laterPayNumber4,
                        ["laterPayNumbers"] = model.laterPayNumbers
                    };

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri($"{domain}?{string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}")
                    };
                    // Remove port if server doesn't accept it
                    request.RequestUri = new Uri($"{request.RequestUri.Scheme}://{request.RequestUri.Host}{request.RequestUri.PathAndQuery}");

                    var response = await downloadClient.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                    {

                        var err = new ErrorResponse();
                        err.Errors.Add("Warning", $"Get Later Payment Fail.");
                        return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    EmsApiResultInfo resultInfo = FileProcessor.DeserializeXml(responseContent);

                    //var url = resultInfo.DownLoadUrl ?? "https://www.api-mypage.post.japanpost.jp/webapi/servlet/DOWNLOAD?fName=0pqgatlconpvqepnlim04nxtznblzosgpzhcr3vjo6agbg1x0b3r77dnux3ih3wm.pdf";
                    var url = resultInfo.DownLoadUrl ?? null;

                    try
                    {
                        if (url == null)
                        {
                            var err = new ErrorResponse();
                            err.Errors.Add("Warning", $"Download URL is empty.");
                            return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                        }

                        // Download the data from the specified URL
                        byte[] bytes = await downloadClient.GetByteArrayAsync(url);
                        Console.WriteLine("File downloaded successfully.");

                        // Convert the byte array to a Base64 string
                        string document = Convert.ToBase64String(bytes);
                        Console.WriteLine("File converted to Base64 format.");
                        return await Result<string>.SuccessAsync(document, "Successful");
                    }
                    catch (HttpRequestException ex)
                    {
                        // If download fails, return the URL instead
                        Console.WriteLine($"Failed to download file: {ex.Message}");
                        var err = new ErrorResponse();
                        err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                        return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to download file: {ex.Message}");
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<HhtUpdateStatusRequestDTO>> UpdateHHTStatusAsync([Body] HhtUpdateStatusRequestDTO model)
        {
            try
            {
                HhtUpdateStatusRequestDTO reponse = null;

                switch (model.EntityName)
                {
                    case EnumEntitiesName.WarehousePutAway:
                        var res = await dbContext.WarehousePutAways.FirstOrDefaultAsync(_ => _.Id == model.Id);

                        if (res == null) throw new Exception($"ID {model.Id} could not be found in {model.EntityName.ToString()} entity.");

                        res.HHTStatus = model.HHTStatus;
                        res.HHTInfo = model.HHTInfo;
                        break;
                    case EnumEntitiesName.WarehouseReceiptOrder:
                        var res1 = await dbContext.WarehouseReceiptOrders.FirstOrDefaultAsync(_ => _.Id == model.Id);

                        if (res1 == null) throw new Exception($"ID {model.Id} could not be found in {model.EntityName.ToString()} entity.");

                        res1.HHTStatus = model.HHTStatus;
                        res1.HHTInfo = model.HHTInfo;
                        break;
                    case EnumEntitiesName.WarehousePickingList:
                        var res2 = await dbContext.WarehousePickingLists.FirstOrDefaultAsync(_ => _.Id == model.Id);

                        if (res2 == null) throw new Exception($"ID {model.Id} could not be found in {model.EntityName.ToString()} entity.");

                        res2.HHTStatus = model.HHTStatus;
                        res2.HHTInfo = model.HHTInfo;
                        break;
                    case EnumEntitiesName.InventTransfer:
                        var res3 = await dbContext.InventTransfers.FirstOrDefaultAsync(_ => _.Id == model.Id);

                        if (res3 == null) throw new Exception($"ID {model.Id} could not be found in {model.EntityName.ToString()} entity.");

                        res3.HHTStatus = model.HHTStatus;
                        res3.HHTInfo = model.HHTInfo;
                        break;
                    case EnumEntitiesName.InventStockTake:
                        var res4 = await dbContext.InventStockTakes.FirstOrDefaultAsync(_ => _.Id == model.Id);

                        if (res4 == null) throw new Exception($"ID {model.Id} could not be found in {model.EntityName.ToString()} entity.");

                        res4.HHTStatus = model.HHTStatus;
                        res4.HHTInfo = model.HHTInfo;
                        break;
                    case EnumEntitiesName.InventStockTakeRecording:
                        var res5 = await dbContext.InventStockTakeRecordings.FirstOrDefaultAsync(_ => _.Id == model.Id);

                        if (res5 == null) throw new Exception($"ID {model.Id} could not be found in {model.EntityName.ToString()} entity.");

                        res5.HHTStatus = model.HHTStatus;
                        res5.HHTInfo = model.HHTInfo;
                        break;
                    case EnumEntitiesName.WarehousePutAwayLine:
                        var res6 = await dbContext.WarehousePutAwayLines.FirstOrDefaultAsync(_ => _.Id == model.Id);

                        if (res6 == null) throw new Exception($"ID {model.Id} could not be found in {model.EntityName.ToString()} entity.");

                        res6.HHTStatus = model.HHTStatus;
                        res6.HHTInfo = model.HHTInfo;
                        break;
                    case EnumEntitiesName.InventBundles:
                        var res7 = await dbContext.InventBundles.FirstOrDefaultAsync(_ => _.Id == model.Id);

                        if (res7 == null) throw new Exception($"ID {model.Id} could not be found in {model.EntityName.ToString()} entity.");

                        res7.HHTStatus = model.HHTStatus;
                        res7.HHTInfo = model.HHTInfo;
                        break;
                }

                await dbContext.SaveChangesAsync();
                return await Result<HhtUpdateStatusRequestDTO>.SuccessAsync(model, "Success");
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message} | {ex.InnerException}");
                return await Result<HhtUpdateStatusRequestDTO>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }

        public async Task<Result<string>> ImportCsvAndUpdateTrackingNoAsync([Body] List<ImportCSVForJPYPModel> models)
        {
            try
            {
                using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var orderDispatches = await dbContext.OrderDispatches
                   .Where(x => models.Select(_=>_.CustomerManagementNumber).Contains(x.DeliveryId) && (x.IsMarketUpdated == default || x.IsMarketUpdated == false))
                   .ToListAsync();
                        var shipments = await dbContext.WarehouseShipments.Where(x => orderDispatches.Select(xx => xx.OrderId).Contains(x.SalesNo) && x.ShippingCarrierCode == "JP-YP" && x.Status == EnumShipmentOrderStatus.Packed)
                            .ToListAsync();
                        foreach (var dispatch in orderDispatches)
                        {
                            var matchingCsvRow = models.FirstOrDefault(x => x.CustomerManagementNumber == dispatch.DeliveryId);
                            if (matchingCsvRow != null)
                            {
                                dispatch.TrackingNo = matchingCsvRow.InquiryNumber;
                                dbContext.Update(dispatch);

                                var matchingShipments = shipments.Where(x => x.SalesNo == dispatch.OrderId).ToList();
                                foreach (var shipment in matchingShipments)
                                {
                                    shipment.TrackingNo = matchingCsvRow.InquiryNumber;
                                }
                                dbContext.UpdateRange(matchingShipments);
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return await Result<string>.SuccessAsync("Successfully updated tracking numbers");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();

                        var err = new ErrorResponse();
                        err.Errors.Add("Error", ex.Message);
                        return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
                    }
                }
            }
            catch (Exception ex)
            {
                var err = new ErrorResponse();
                err.Errors.Add("Error", $"{ex.Message}|{ex.InnerException}");
                return await Result<string>.FailAsync(JsonConvert.SerializeObject(err));
            }
        }
    }
}
