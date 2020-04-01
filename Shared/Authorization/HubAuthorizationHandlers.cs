using ForumSPA.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Shared.Authorization
{
    public class HubAdministratorAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, HubModel>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement,
            HubModel resource)
        {
            if (context.User == null)
                return Task.CompletedTask;

            if (context.User.IsInRole(ForumConstants.AdministratorRole))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
