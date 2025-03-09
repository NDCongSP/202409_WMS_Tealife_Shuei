using Application.DTOs;
using Application.DTOs.Response.Account;
using Application.Extentions;
using Application.Services.Base;

using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Suppliers
{
    [BasePath(ApiRoutes.Suppliers.BasePath)]
    public interface ISuppliers:IRepository<int,Supplier>
    {
        [Get(ApiRoutes.Suppliers.GetSupplierWithTenantAsync)]
        Task<Result<List<SupplierTenantDTO>>> GetSupplierWithTenantAsync();
    }
}
