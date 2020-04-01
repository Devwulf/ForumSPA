using ForumSPA.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using ForumSPA.Shared.Models;

namespace ForumSPA.Shared.Authorization
{
    public class PostIsOwnerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, PostModel>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            PostModel resource)
        {
            if (context.User == null || resource == null)
                return Task.CompletedTask;

            if (requirement.Name != ForumConstants.CreatePostOperationName &&
                requirement.Name != ForumConstants.ReadPostOperationName &&
                requirement.Name != ForumConstants.UpdatePostOperationName &&
                requirement.Name != ForumConstants.DeletePostOperationName)
                return Task.CompletedTask;

            var userId = context.User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
            if (!userId.IsNullOrWhiteSpace() &&
                !resource.UserId.IsNullOrWhiteSpace() &&
                resource.UserId.Equals(userId))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public class PostAdministratorAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, PostModel>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement,
            PostModel resource)
        {
            if (context.User == null)
                return Task.CompletedTask;

            if (context.User.IsInRole(ForumConstants.AdministratorRole))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
