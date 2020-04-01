using ForumSPA.Shared.Utils;
using ForumSPA.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using ForumSPA.Shared.Models;

namespace ForumSPA.Shared.Authorization
{
    public class ThreadIsOwnerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, ThreadModel>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement,
            ThreadModel resource)
        {
            if (context.User == null || resource == null)
                return Task.CompletedTask;

            if (requirement.Name != ForumConstants.CreateThreadOperationName &&
                requirement.Name != ForumConstants.ReadThreadOperationName &&
                requirement.Name != ForumConstants.UpdateThreadOperationName &&
                requirement.Name != ForumConstants.DeleteThreadOperationName)
                return Task.CompletedTask;

            var userId = context.User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
            // Checking if UserId is null because it is possible to be null
            // from the database
            if (!userId.IsNullOrWhiteSpace() &&
                !resource.UserId.IsNullOrWhiteSpace() &&
                resource.UserId.Equals(userId))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public class ThreadAdministratorAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, ThreadModel>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement,
            ThreadModel resource)
        {
            if (context.User == null)
                return Task.CompletedTask;

            if (context.User.IsInRole(ForumConstants.AdministratorRole))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
