using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace WebUIFinal
{
    public static class AuthExtensions
    {
        public static async Task<bool> HasPermissionAsync(this Task<AuthenticationState> authState, string permission)
        {
            var user = (await authState).User;
            return user.HasClaim("Permission", permission);
        }

        public static bool HasPermission(this AuthenticationState authState, string permission)
        {
            if (authState == null)
                return false;
            var user = authState.User;
            return user.HasClaim("Permission", permission);
        }

        public static bool HasRole(this AuthenticationState authState, string role)
        {
            if (authState == null)
                return false;
            var user = authState.User;
            var r= user.HasClaim(ClaimTypes.Role, role);
            return user.HasClaim(ClaimTypes.Role, role);
        }
    }
}
