using Application.Extentions;
using Application.Services.Base;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestEase;
using System.Reflection;
using System.Security.Claims;

namespace API.Controllers.Base
{
    //[Authorize]//không cần xét role, login vào là gọi đc API    
    [Route("api/[controller]")]
    [ApiController]
    //[ApiController,JsonifyErrors]
    public class BaseController<TId, T> : ControllerBase, IRepository<TId, T> where T : class
    {
        readonly IRepository<TId, T> _repository;

        public BaseController(IRepository<TId, T> repository)
        {
            _repository = repository;
        }

        [HttpPost(ApiRoutes.AddRange)]
        public async Task<Result<List<T>>> AddRangeAsync([Body] List<T> model)
        {
            return await _repository.AddRangeAsync(model);
        }

        [HttpPost(ApiRoutes.Delete)]
        public async Task<Result<T>> DeleteAsync([Body] T model)
        {
            return await _repository.DeleteAsync(model);
        }

        [HttpPost(ApiRoutes.DeleteRange)]
        public async Task<Result<T>> DeleteRangeAsync([Body] List<T> model)
        {
            return await _repository.DeleteRangeAsync(model);
        }

        [HttpGet(ApiRoutes.GetAll)]
        public async Task<Result<List<T>>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        [HttpGet(ApiRoutes.GetById)]
        public async Task<Result<T>> GetByIdAsync([Path] TId id)
        {
            return await _repository.GetByIdAsync(id);
        }

        [HttpPost(ApiRoutes.Insert)]
        public async Task<Result<T>> InsertAsync([Body] T model)
        {
            var user = User.Identity;
            Type myType = model.GetType();
            //We will be defining a PropertyInfo Object which contains details about the class property 
            PropertyInfo[] arrayPropertyInfos = myType.GetProperties();
            foreach (PropertyInfo property in arrayPropertyInfos)
            {
                if (property.Name == "CreateOperatorId")
                {
                    property.SetValue(model, user.Name);
                }
                else if (property.Name == "CreateAt")
                {
                    property.SetValue(model, DateTime.Now);
                }
            }

            return await _repository.InsertAsync(model);
        }

        [HttpPost(ApiRoutes.Update)]
        public async Task<Result<T>> UpdateAsync([Body] T model)
        {
            var user = User.Identity;
            Type myType = model.GetType();
            //We will be defining a PropertyInfo Object which contains details about the class property 
            PropertyInfo[] arrayPropertyInfos = myType.GetProperties();
            foreach (PropertyInfo property in arrayPropertyInfos)
            {
                if (property.Name == "UpdateOperatorId")
                {
                    property.SetValue(model, user.Name);
                }
                else if (property.Name == "UpdateAt")
                {
                    property.SetValue(model, DateTime.Now);
                }
            }
            return await _repository.UpdateAsync(model);
        }
    }
}