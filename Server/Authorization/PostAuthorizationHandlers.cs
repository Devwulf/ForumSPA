using ForumSPA.Server.Data.Models;
using ForumSPA.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Server.Authorization
{
    public class PostIsOwnerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Post>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PostIsOwnerAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            Post resource)
        {
            if (context.User == null || resource == null)
                return Task.CompletedTask;

            if (requirement.Name != ForumConstants.CreatePostOperationName &&
                requirement.Name != ForumConstants.ReadPostOperationName &&
                requirement.Name != ForumConstants.UpdatePostOperationName &&
                requirement.Name != ForumConstants.DeletePostOperationName)
                return Task.CompletedTask;

            if (!resource.UserId.IsNullOrWhiteSpace() &&
                resource.UserId.Equals(_userManager.GetUserId(context.User)))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public class PostAdministratorAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Post>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            Post resource)
        {
            if (context.User == null)
                return Task.CompletedTask;

            if (context.User.IsInRole(ForumConstants.AdministratorRole))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
