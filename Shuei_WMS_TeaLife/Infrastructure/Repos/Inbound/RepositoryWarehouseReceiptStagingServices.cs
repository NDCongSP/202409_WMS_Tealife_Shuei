using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions;
using Application.Models;
using Application.Services.Inbound;
using AutoMapper;
using DocumentFormat.OpenXml.Vml;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestEase;
using System.Diagnostics;
using RazorEngine.Compilation.ImpromptuInterface.InvokeExt;
using Infrastructure.Extensions;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Vml;

namespace Infrastructure.Repos
{
    public class RepositoryWarehouseReceiptStagingServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehouseReceiptStaging
    {
        public async Task<Result<List<WarehouseReceiptStaging>>> AddRangeAsync([Body] List<WarehouseReceiptStaging> model)
        {
            try
            {
                //lay thong tin user
                var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                foreach (var item in model)
                {
                    item.CreateAt = DateTime.Now;
                    item.CreateOperatorId = userInfo?.Id;
                }

                await dbContext.WarehouseReceiptStagings.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<WarehouseReceiptStaging>>.SuccessAsync(model, "Add range WarehouseReceiptStaging successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptStaging>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> DeleteRangeAsync([Body] List<WarehouseReceiptStaging> model)
        {
            try
            {
                dbContext.WarehouseReceiptStagings.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptStaging>.SuccessAsync("Delete range WarehouseReceiptStaging successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> DeleteAsync([Body] WarehouseReceiptStaging model)
        {
            try
            {
                dbContext.WarehouseReceiptStagings.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptStaging>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseReceiptStaging>>.SuccessAsync(await dbContext.WarehouseReceiptStagings.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptStaging>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseReceiptStaging>.SuccessAsync(await dbContext.WarehouseReceiptStagings.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> InsertAsync([Body] WarehouseReceiptStaging model)
        {
            try
            {
                await dbContext.WarehouseReceiptStagings.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> UpdateAsync([Body] WarehouseReceiptStaging model)
        {
            try
            {
                dbContext.WarehouseReceiptStagings.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptStaging>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptStaging>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            try
            {
                return await Result<List<WarehouseReceiptStaging>>.SuccessAsync(await dbContext.WarehouseReceiptStagings.Where(x => x.ReceiptNo == receiptNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptStaging>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStaging>> GetByReceiptLineIdAsync([Path] Guid receiptLineId)
        {
            try
            {
                return await Result<WarehouseReceiptStaging>.SuccessAsync(await dbContext.WarehouseReceiptStagings.Where(x => x.ReceiptLineId == receiptLineId).FirstOrDefaultAsync());
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStaging>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<string>> UploadProductErrorImageAsync([Body] List<UpdateReceiptImageRequestDTO> model)
        {
            try
            {
                using (var tran = dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        //lay thong tin user
                        var userInfo = await dbContext.Users.FirstOrDefaultAsync(x => x.UserName == contextAccessor.HttpContext.User.Identity.Name);

                        foreach (var item in model)
                        {
                            //check exist receiptLineId in warehoue receipt staging
                            var stagingInfo = await dbContext.WarehouseReceiptStagings.Where(x => x.ReceiptLineId == item.ReceiptLineId).FirstOrDefaultAsync();
                            if (stagingInfo == null) throw new Exception($"ReceiptId: {item.ReceiptLineId} could be not found.");

                            //generate model imagename
                            var imageNames = new ImagesName();
                            var imageErrorTasks = new ImagesNameBase64();

                            //upload error image
                            foreach (var item2 in item.ErrorImages)
                            {
                                var isData = new ImageStorage();
                                string receiptLineId = item.ReceiptLineId.ToString().ToLower() ?? string.Empty;

                                var checkExist = await dbContext.ImageStorages.Where(x => x.ResourcesId.ToLower() == receiptLineId && x.FileName == item2.FileName).FirstOrDefaultAsync();
                                if (checkExist == null)
                                {
                                    isData.ResourcesId = item.ReceiptLineId.ToString();
                                    isData.Type = EnumImageStorageType.ReceiptProductError;
                                    isData.FileName = item2.FileName;
                                    isData.ImageBase64 = item2.ImageBase64;
                                    isData.CreateAt = DateTime.Now;
                                    isData.CreateOperatorId = userInfo.Id;
                                    isData.IsDeleted = false;

                                    await dbContext.ImageStorages.AddAsync(isData);
                                }
                                else
                                {
                                    //checkExist.FileName = item2.FileName;
                                    //checkExist.ImageBase64 = item2.ImageBase64;
                                    //checkExist.UpdateAt = DateTime.Now;
                                    //checkExist.UpdateOperatorId = userInfo.Id;
                                }

                                //create list model log into receiptStaging errorImage
                                imageNames.Add(new ImageName() { FilePath = "Database", Name = item2.FileName });
                                imageErrorTasks.Add(new ImageNameBase64() { Name = item2.FileName, ImageBase64String = item2.ImageBase64 });
                            }

                            stagingInfo.ErrorImages = JsonConvert.SerializeObject(imageNames);
                            stagingInfo.StatusError = item.StatusError;

                            Debug.WriteLine($"ErrImage= {stagingInfo.ErrorImages}");

                            #region insert/update Tasks
                            var taskInfo = await dbContext.TaskModels.Where(x => x.ReceiptOrderLineId == item.ReceiptLineId).FirstOrDefaultAsync();

                            var receiptLineInfo = await dbContext.WarehouseReceiptOrders.Where(x => x.ReceiptNo == stagingInfo.ReceiptNo).FirstOrDefaultAsync();
                            if (receiptLineInfo == null) throw new Exception($"ReceiptId: {item.ReceiptLineId} could be not found.");

                            if (taskInfo == null)//insert
                            {
                                var taskData = new TaskModel()
                                {
                                    CompanyId = receiptLineInfo.TenantId,
                                    Title = "画像に欠陥がある商品を確認する",
                                    TaskContent = "商品画像に欠陥があります。エラー画像と商品ステータスを確認します.",
                                    TaskType = 4,
                                    Priority = 2,
                                    Status = -1,
                                    StartTime = DateTime.Now,
                                    StatusProductError = item.StatusError,
                                    ImageUrl = stagingInfo.ErrorImages,
                                    ProductCode = stagingInfo.ProductCode,
                                    LotNo = stagingInfo.LotNo,
                                    ReceiptNo = stagingInfo.ReceiptNo,
                                    ReceiptOrderLineId = stagingInfo.ReceiptLineId,
                                    CreateAt = DateTime.Now,
                                    CreateOperatorId = userInfo.Id,
                                    ImagesBase64 = JsonConvert.SerializeObject(imageErrorTasks)
                                };

                                await dbContext.TaskModels.AddAsync(taskData);
                            }
                            else//update
                            {
                                taskInfo.UpdateAt = DateTime.Now;
                                taskInfo.UpdateOperatorId = userInfo.Id;
                                taskInfo.LotNo = stagingInfo.LotNo;
                                taskInfo.StatusProductError = stagingInfo.StatusError;
                                taskInfo.ImageUrl = stagingInfo.ErrorImages;
                                taskInfo.ImagesBase64 = JsonConvert.SerializeObject(imageErrorTasks);

                                //dbContext.TaskModels.Update(taskData);
                            }
                            #endregion
                        }

                        await dbContext.SaveChangesAsync();
                        await tran.CommitAsync();
                        return await Result<string>.SuccessAsync("Successfull.");
                    }
                    catch (Exception ex)
                    {
                        await tran.RollbackAsync();
                        return await Result<string>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
                    }
                }
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptStagingResponseDTO>>> GetAllDTOAsync()
        {
            try
            {
                var response = new List<WarehouseReceiptStagingResponseDTO>();
                var res = await dbContext.WarehouseReceiptStagings.ToListAsync();


                // Cấu hình AutoMapper
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<WarehouseReceiptStaging, WarehouseReceiptStagingResponseDTO>();
                    //.ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Price));
                });
                var mapper = config.CreateMapper();

                foreach (var item in res)
                {
                    var d = mapper.Map<WarehouseReceiptStagingResponseDTO>(item);
                    if (d.ErrorImagesName != null)
                    {
                        var ImageData = await dbContext.ImageStorages.Where(x => x.ResourcesId.ToLower() == d.ReceiptLineId.ToString().ToLower() && x.Type == EnumImageStorageType.ReceiptProductError).ToListAsync();
                        if (ImageData != null && ImageData.Count > 0)
                        {
                            foreach (var item1 in ImageData)
                            {
                                d.ImageStorage.Add(item1);
                            }
                        }
                    }

                    response.Add(d);
                }

                return await Result<List<WarehouseReceiptStagingResponseDTO>>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptStagingResponseDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptStagingResponseDTO>> GetByReceiptLineIdDTOAsync([Path] Guid receiptLineId)
        {
            try
            {
                var response = new WarehouseReceiptStagingResponseDTO();
                var res = await dbContext.WarehouseReceiptStagings.Where(x => x.ReceiptLineId == receiptLineId).FirstOrDefaultAsync();

                // Cấu hình AutoMapper
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<WarehouseReceiptStaging, WarehouseReceiptStagingResponseDTO>();
                });
                var mapper = config.CreateMapper();

                response = mapper.Map<WarehouseReceiptStagingResponseDTO>(res);

                if (response.ErrorImagesName != null)
                {
                    //foreach (var item in response.ErrorImagesName)
                    //{
                    //    var iRes = LoadImage(item.Name);
                    //    response.LoadErrorImagesResult.Add(new ImageInfoDTO()
                    //    {
                    //        FilePath = iRes.FilePath,
                    //        FileName = iRes.FileName,
                    //        ImageBase64 = iRes.ImageBase64
                    //    });
                    //}

                    var ImageData = await dbContext.ImageStorages.Where(x => x.ResourcesId.ToLower() == receiptLineId.ToString().ToLower() && x.Type == EnumImageStorageType.ReceiptProductError).ToListAsync();
                    if (ImageData != null && ImageData.Count > 0)
                    {
                        foreach (var item in ImageData)
                        {
                            response.ImageStorage.Add(item);
                        }
                    }
                }

                return await Result<WarehouseReceiptStagingResponseDTO>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptStagingResponseDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        private async Task<Result<ImageInfoDTO>> UploadImage(ImageInfoDTO model)
        {
            try
            {
                var folderPath = @"C:\Images\Receipt\Error";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = @"C:\Images\Receipt\Error\" + model.FileName;
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                if (model.ImageBase64.Length > 0)
                {
                    var s = model.ImageBase64.Split(',')[1];
                    File.WriteAllBytes(System.IO.Path.Combine(folderPath, model.FileName), Convert.FromBase64String(s));
                }

                model.FilePath = folderPath;
                return await Result<ImageInfoDTO>.SuccessAsync(model, "Successful.");
            }
            catch (Exception ex)
            {
                return await Result<ImageInfoDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}"); ;
            }
        }

        private ImageInfoDTO LoadImage(string ProductImageName)
        {
            ImageInfoDTO response = new ImageInfoDTO();
            try
            {
                var fileName = @"C:\Images\Receipt\Error\" + ProductImageName;
                if (File.Exists(fileName))
                {
                    var imageArray = File.ReadAllBytes(fileName);
                    var base64Image = Convert.ToBase64String(imageArray);

                    //dồn chung ImageName và string base64 của ảnh trả về cho client cắt ra xử
                    var typeImage = ProductImageName.Split('.')[1];
                    if (typeImage == "png")
                    {
                        ProductImageName = $"data:image/png;base64,{base64Image}";
                    }
                    else if (typeImage == "jpeg" || typeImage == "jpg")
                    {
                        ProductImageName = $"data:image/jpeg;base64,{base64Image}";
                    }
                    else if (typeImage == "svg")
                    {
                        ProductImageName = $"data:image/svg+xml;base64,{base64Image}";
                    }
                }
                else ProductImageName = string.Empty;

                response.FilePath = fileName;
                response.FileName = ProductImageName;
                response.ImageBase64 = ProductImageName;

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
