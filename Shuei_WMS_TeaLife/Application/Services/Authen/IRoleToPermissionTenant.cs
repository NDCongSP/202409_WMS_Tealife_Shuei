using Application.Extentions;
using Application.Services.Base;

using RestEase;

namespace Application.Services.Authen
{
    [BasePath(ApiRoutes.RoleToPermissionTenant.BasePath)]
    public interface IRoleToPermissionTenant:IRepository<Guid,RoleToPermissionTenant>
    {
    }
}
