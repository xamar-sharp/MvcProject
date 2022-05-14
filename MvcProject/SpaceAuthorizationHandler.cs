using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
namespace MvcProject
{
    public class SpaceAuthorizationHandler:AuthorizationHandler<AuthorizationRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext ctx,AuthorizationRequirement requirement)
        {
            if (requirement.ValidateAvailableSpace())
            {
                ctx.Succeed(requirement);
            }
            if (!ctx.HasSucceeded)
            {
                ctx.Fail();
            }
            await Task.CompletedTask;
        }
    }
}
