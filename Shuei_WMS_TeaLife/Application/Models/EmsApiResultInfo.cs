using System.Xml.Serialization;

namespace Application.Models
{
    [XmlRoot("EmsApiResultInfo")]
    public class EmsApiResultInfo
    {
        [XmlElement("csId")]
        public string CsId { get; set; }

        [XmlElement("functionCode")]
        public string FunctionCode { get; set; }

        [XmlElement("ResultCode")]
        public string ResultCode { get; set; }

        [XmlElement("ResultText")]
        public string ResultText { get; set; }

        [XmlElement("DownLoadUrl")]
        public string DownLoadUrl { get; set; } = null;
    }

    public class LaterPayRequest
    {
        public int ctCode { get; set; } = 60;
        public string ctId { get; set; } = "tlf2407";
        public string ctPw { get; set; } = "240719tlfapi";
        public string laterPayNumber1 { get; set; } = "1000009363";
        public string laterPayNumber2 { get; set; } = "000016";
        public string laterPayNumber3 { get; set; } = "0000000001";
        public string laterPayNumber4 { get; set; } = "000001";
        public string laterPayNumbers { get; set; } = "RN073814037JP";
    }

    public class FileProcessor
    {
        public static EmsApiResultInfo DeserializeXml(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EmsApiResultInfo));
            using (StringReader reader = new StringReader(xml))
            {
                return (EmsApiResultInfo)serializer.Deserialize(reader);
            }
        }

        public static async Task DownloadFileAsync(string url, string filePath)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                Console.WriteLine($"Downloading file from: {url}");
                byte[] fileBytes = await httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(filePath, fileBytes);
                Console.WriteLine($"File saved to: {filePath}");
            }
        }
    }
}
