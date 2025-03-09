using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure;

public class CustomRequirement : IAuthorizationRequirement
{
    public string Permission { get; set; }

    public CustomRequirement()
    {
    }
}
public class CustomRequirementHandler : AuthorizationHandler<CustomRequirement>
{


    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequirement requirement)
    {

        // context.User.HasClaim()

        if (true)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, ""));
        }

        return Task.CompletedTask;
    }
}