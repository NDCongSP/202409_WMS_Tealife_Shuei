using Application.Extentions;
using Application.Services.Base;
using RestEase;

namespace Application.Services.Authen
{
    [BasePath(ApiRoutes.PermissionTenant.BasePath)]
    public interface IPermissionTenant : IRepository<Guid, PermissionsTenant>
    {
    }
}
