using BarcodeStandard;
using QRCoder.Core;
using System.Drawing.Imaging;
using SkiaSharp;
using System.Drawing;

namespace Infrastructure
{
    public static class GlobalVariable
    {
        /// <summary>
        /// Method to generate QR code base64.
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public static string GenerateQRCode(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return null;

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
            }
        }
        public static string GenerateBarcodeBase64(string data, int width = 300, int height = 100)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                    data = "empty";
                // Tạo một đối tượng mã vạch
                Barcode barcode = new Barcode
                {
                    IncludeLabel = true,
                    Alignment = AlignmentPositions.Center,
                    Width = width,
                    Height = height,
                };
                var bar = new Barcode();
                SKImage img = bar.Encode(BarcodeStandard.Type.Code128, data, width, height);
                return $"data:image/png;base64,{Convert.ToBase64String(img.Encode().ToArray())}";
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        //public static string GenerateBarcode(string inputText)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(inputText))
        //            return null;

        //        var barcode = BarcodeWriter.CreateBarcode(inputText, BarcodeEncoding.Code128);

        //        using var ms = new MemoryStream();
        //        //barcode.SaveAsPng(ms, 300); // Thêm DPI parameter 300
        //        byte[] byteImage = ms.ToArray();

        //        // Trả về chuỗi Base64
        //        return $"data:image/png;base64,{Convert.ToBase64String(byteImage)}";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}
    }
}
