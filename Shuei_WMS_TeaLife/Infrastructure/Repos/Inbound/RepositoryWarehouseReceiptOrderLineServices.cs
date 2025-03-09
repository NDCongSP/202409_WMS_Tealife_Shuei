using Application.DTOs.Response;
using Application.DTOs;
using Application.Extentions;
using Application.Services.Inbound;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestEase;
using Application.DTOs.Request;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Infrastructure.Repos
{
    public class RepositoryWarehouseReceiptOrderLineServices(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor) : IWarehouseReceiptOrderLine
    {
        public async Task<Result<List<WarehouseReceiptOrderLine>>> AddRangeAsync([Body] List<WarehouseReceiptOrderLine> model)
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

                await dbContext.WarehouseReceiptOrderLines.AddRangeAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<List<WarehouseReceiptOrderLine>>.SuccessAsync(model, "Add range WarehouseReceiptOrderLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrderLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLine>> DeleteRangeAsync([Body] List<WarehouseReceiptOrderLine> model)
        {
            try
            {
                dbContext.WarehouseReceiptOrderLines.RemoveRange(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync("Delete range WarehouseReceiptOrderLine successfull");
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLine>> DeleteAsync([Body] WarehouseReceiptOrderLine model)
        {
            try
            {
                dbContext.WarehouseReceiptOrderLines.Remove(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrderLine>>> GetAllAsync()
        {
            try
            {
                return await Result<List<WarehouseReceiptOrderLine>>.SuccessAsync(await dbContext.WarehouseReceiptOrderLines.ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrderLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLine>> GetByIdAsync([Path] Guid id)
        {
            try
            {
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync(await dbContext.WarehouseReceiptOrderLines.FindAsync(id));
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLine>> InsertAsync([Body] WarehouseReceiptOrderLine model)
        {
            try
            {
                await dbContext.WarehouseReceiptOrderLines.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLine>> UpdateAsync([Body] WarehouseReceiptOrderLine model)
        {
            try
            {
                dbContext.WarehouseReceiptOrderLines.Update(model);
                await dbContext.SaveChangesAsync();
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync(model);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrderLine>>> GetByMasterCodeAsync([Path] string receiptNo)
        {
            try
            {
                return await Result<List<WarehouseReceiptOrderLine>>.SuccessAsync(await dbContext.WarehouseReceiptOrderLines.Where(x => x.ReceiptNo == receiptNo).ToListAsync());
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrderLine>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<List<WarehouseReceiptOrderLineResponseDTO>>> GetAllDTOAsync()
        {
            try
            {
                var response = new List<WarehouseReceiptOrderLineResponseDTO>();
                var res = await dbContext.WarehouseReceiptOrderLines.ToListAsync();


                // Cấu hình AutoMapper
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<WarehouseReceiptOrderLine, WarehouseReceiptOrderLineResponseDTO>();
                    //.ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Price));
                });
                var mapper = config.CreateMapper();

                foreach (var item in res)
                {
                    var d = mapper.Map<WarehouseReceiptOrderLineResponseDTO>(item);
                    if (d.ErrorImagesName != null)
                    {
                        foreach (var item1 in d.ErrorImagesName)
                        {
                            var iRes = LoadImage(item1.Name);
                            d.LoadErrorImagesResult.Add(new ImageInfoDTO()
                            {
                                FilePath = iRes.FilePath,
                                FileName = iRes.FileName,
                                ImageBase64 = iRes.ImageBase64
                            });
                        }
                    }

                    response.Add(d);
                }

                return await Result<List<WarehouseReceiptOrderLineResponseDTO>>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                return await Result<List<WarehouseReceiptOrderLineResponseDTO>>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        public async Task<Result<WarehouseReceiptOrderLineResponseDTO>> GetByIdDTOAsync([Path] Guid id)
        {
            try
            {
                var response = new WarehouseReceiptOrderLineResponseDTO();
                var res = await dbContext.WarehouseReceiptOrderLines.FindAsync(id);

                // Cấu hình AutoMapper
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<WarehouseReceiptOrderLine, WarehouseReceiptOrderLineResponseDTO>();
                });
                var mapper = config.CreateMapper();

                response = mapper.Map<WarehouseReceiptOrderLineResponseDTO>(res);

                if (response.ErrorImagesName != null)
                {
                    foreach (var item in response.ErrorImagesName)
                    {
                        var iRes = LoadImage(item.Name);
                        response.LoadErrorImagesResult.Add(new ImageInfoDTO()
                        {
                            FilePath = iRes.FilePath,
                            FileName = iRes.FileName,
                            ImageBase64 = iRes.ImageBase64
                        });
                    }
                }

                return await Result<WarehouseReceiptOrderLineResponseDTO>.SuccessAsync(response);
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLineResponseDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        private GeneralResponse UploadImage(ImageInfoDTO model)
        {
            GeneralResponse response = new GeneralResponse();
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
                    File.WriteAllBytes(Path.Combine(folderPath, model.FileName), Convert.FromBase64String(s));
                }

                response.Flag = true;
                response.Message = "Save image successfully.";
                return response;
            }
            catch (Exception ex)
            {
                response.Flag = false;
                response.Message = $"{ex.Message}{Environment.NewLine}{ex.InnerException}";
                return response;
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

        public async Task<Result<WarehouseReceiptOrderLine>> GetByProducCodetAsync([Path] string productCode)
        {
            try
            {
                return await Result<WarehouseReceiptOrderLine>.SuccessAsync(await dbContext.WarehouseReceiptOrderLines.Where(x => x.ProductCode == productCode).FirstOrDefaultAsync());
            }
            catch (Exception ex)
            {
                return await Result<WarehouseReceiptOrderLine>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }
    }
}
