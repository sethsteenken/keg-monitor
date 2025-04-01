using Microsoft.AspNetCore.Authorization;

namespace KegMonitor.Web.Authorization
{
    public class AllowAnonymousAuthorizationHandler : AuthorizationHandler<AllowAnonymousAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            AllowAnonymousAuthorizationRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
