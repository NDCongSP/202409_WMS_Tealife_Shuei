using Application.Extentions;
using Application.Services.Authen.UI;
using Blazored.LocalStorage;
using RestEase.Implementation;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Application.Models;
using System.Net.Http.Json;
using System.Xml.Linq;
using System.Reflection;

namespace Application.Services
{
    public class LaterPaymentServices : ILaterPayment
    {
        //readonly ILaterPaymentClient _paymentClient;
        readonly ILocalStorageService _localStorage;

        //readonly ApiAuthenticationStateProvider _authStateProvider;
        //public LaterPaymentServices( ILaterPaymentClient paymentClient, ILocalStorageService localStorage)
        public LaterPaymentServices(ILocalStorageService localStorage)
        {
            //_paymentClient = paymentClient;
            _localStorage = localStorage;
        }

        public async Task<Result<string>> GetLaterPayment(LaterPayRequest model)
        {
            try
            {
                //var result = await _paymentClient.GetLaterPayment(model.ctCode, model.ctId, model.ctPw, model.laterPayNumber1, model.laterPayNumber2, model.laterPayNumber3, model.laterPayNumber4, model.laterPayNumbers);

                //if (!result.IsSuccessStatusCode)
                //    throw new Exception("GetLaterPayment Fail.");

                //var content = await result.Content.ReadAsStringAsync();
                ////var response = JsonConvert.DeserializeObject<GeneralResponse>(content);
                //EmsApiResultInfo resultInfo = FileProcessor.DeserializeXml(content);

                //using var downloadClient = new HttpClient();
                //byte[] bytes = await downloadClient.GetByteArrayAsync(resultInfo.DownLoadUrl);

                //// Convert the byte array to a Base64 string
                //string document = Convert.ToBase64String(bytes);
                //Console.WriteLine("File converted to Base64 format.");
                //return await Result<string>.SuccessAsync(document);

                ////Base URL
                using var downloadClient = new HttpClient();
                string baseUrl = "https://api-mypage.post.japanpost.jp/webapi/servlet/WEBAPI";

                // Sử dụng UriBuilder để thêm query parameters
                var uriBuilder = new UriBuilder(baseUrl);
                var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
                query["ctCode"] = model.ctCode.ToString();
                query["ctId"] = model.ctId;
                query["ctPw"] = model.ctPw;
                query["laterPayNumber1"] = model.laterPayNumber1;
                query["laterPayNumber2"] = model.laterPayNumber2;
                query["laterPayNumber3"] = model.laterPayNumber3;
                query["laterPayNumber4"] = model.laterPayNumber4;
                query["laterPayNumbers"] = model.laterPayNumbers;
                uriBuilder.Query = query.ToString();

                // Xây dựng URL hoàn chỉnh
                string fullUrl = uriBuilder.ToString();

                var result = await downloadClient.GetAsync(fullUrl);

                if (!result.IsSuccessStatusCode)
                    throw new Exception("GetLaterPayment Fail.");

                var content = await result.Content.ReadAsStringAsync();
                //var response = JsonConvert.DeserializeObject<GeneralResponse>(content);
                EmsApiResultInfo resultInfo = FileProcessor.DeserializeXml(content);

                var pdfUrl = "https://www.api-mypage.post.japanpost.jp/webapi/servlet/DOWNLOAD?fName=0pqgatlconpvqepnlim04nxtznblzosgpzhcr3vjo6agbg1x0b3r77dnux3ih3wm.pdf";
                
                return await Result<string>.SuccessAsync(resultInfo.DownLoadUrl ?? pdfUrl);

                //// Download the data from the specified URL
                //byte[] bytes = await downloadClient.GetByteArrayAsync(resultInfo.DownLoadUrl);
                //Console.WriteLine("File downloaded successfully.");

                //// Convert the byte array to a Base64 string
                //string document = Convert.ToBase64String(bytes);
                //Console.WriteLine("File converted to Base64 format.");
                //return await Result<string>.SuccessAsync(document, "Successful");
            }

            catch (Exception ex)
            {
                return await Result<string>.FailAsync(ex.Message);
            }
        }
    }
}
