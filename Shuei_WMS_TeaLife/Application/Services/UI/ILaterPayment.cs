using Application.Extentions;
using Application.Models;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    //[BaseAddress(ApiRoutes.LaterPayment.BasePath)]
    //public interface ILaterPaymentClient
    //{
    //    [Get(ApiRoutes.LaterPayment.GetLaterPayment)]
    //    Task<HttpResponseMessage> GetLaterPayment(
    //        [Query] int ctCode,
    //        [Query] string ctId,
    //        [Query] string ctPw,
    //        [Query] string laterPayNumber1,
    //        [Query] string laterPayNumber2,
    //        [Query] string laterPayNumber3,
    //        [Query] string laterPayNumber4,
    //        [Query] string laterPayNumbers);
    //}

    public interface ILaterPayment
    {
        Task<Result<string>> GetLaterPayment(LaterPayRequest model);
    }
}
