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
    public class ThreadIsOwnerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Thread>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ThreadIsOwnerAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            Thread resource)
        {
            if (context.User == null || resource == null)
                return Task.CompletedTask;

            if (requirement.Name != ForumConstants.CreateThreadOperationName &&
                requirement.Name != ForumConstants.ReadThreadOperationName &&
                requirement.Name != ForumConstants.UpdateThreadOperationName &&
                requirement.Name != ForumConstants.DeleteThreadOperationName)
                return Task.CompletedTask;

            // Checking if UserId is null because it is possible to be null
            // from the database
            if (!resource.UserId.IsNullOrWhiteSpace() &&
                resource.UserId.Equals(_userManager.GetUserId(context.User)))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public class ThreadAdministratorAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Thread>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            Thread resource)
        {
            if (context.User == null)
                return Task.CompletedTask;

            if (context.User.IsInRole(ForumConstants.AdministratorRole))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
