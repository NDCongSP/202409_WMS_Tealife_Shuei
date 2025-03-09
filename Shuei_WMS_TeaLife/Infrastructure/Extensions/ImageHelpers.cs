using Application.DTOs;
using Application.DTOs.Response;
using Application.Extentions;

namespace Infrastructure.Extensions
{
    public static class ImageHelpers
    {
        public static string LoadImage(string ProductImageName)
        {
            try
            {
                var fileName = @"C:\Images\Products\" + ProductImageName;
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


                return ProductImageName;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static async Task<Result<ImageInfoDTO>> UploadImageAsync(ImageInfoDTO model)
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
                    File.WriteAllBytes(Path.Combine(folderPath, model.FileName), Convert.FromBase64String(s));
                }

                model.FilePath = folderPath;
                return await Result<ImageInfoDTO>.SuccessAsync(model, "Successful.");
            }
            catch (Exception ex)
            {
                return await Result<ImageInfoDTO>.FailAsync($"{ex.Message}{Environment.NewLine}{ex.InnerException}"); ;
            }
        }
    }
}
