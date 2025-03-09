using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using QRCoder.Core;

namespace WebUIFinal
{
    public static class GlobalVariable
    {
        public static BreadCumb BreadCrumbData { get; set; } = new BreadCumb();
        public static BreadCumb BreadCrumbDataMaster { get; set; } = new BreadCumb();

        public static UserAuthorizationInfo UserAuthorizationInfo { get; set; } = new UserAuthorizationInfo();
        //public static ClaimsPrincipal UserAuthorizationInfo { get; set; }

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
        [CascadingParameter] public static AuthenticationState AuthenticationStateTask { get; set; }

        public static string FilePathTemporary { get; set; }
        public static string ApiURL { get; set; }
    }
}
