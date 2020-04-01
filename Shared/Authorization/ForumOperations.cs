using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Shared.Authorization
{
    public static class ForumOperations
    {
        public static OperationAuthorizationRequirement CreateHub =
            new OperationAuthorizationRequirement() { Name = ForumConstants.CreateHubOperationName };
        public static OperationAuthorizationRequirement ReadHub =
            new OperationAuthorizationRequirement() { Name = ForumConstants.ReadHubOperationName };
        public static OperationAuthorizationRequirement UpdateHub =
            new OperationAuthorizationRequirement() { Name = ForumConstants.UpdateHubOperationName };
        public static OperationAuthorizationRequirement DeleteHub =
            new OperationAuthorizationRequirement() { Name = ForumConstants.DeleteHubOperationName };

        public static OperationAuthorizationRequirement CreateThread =
               new OperationAuthorizationRequirement() { Name = ForumConstants.CreateThreadOperationName };
        public static OperationAuthorizationRequirement ReadThread =
            new OperationAuthorizationRequirement() { Name = ForumConstants.ReadThreadOperationName };
        public static OperationAuthorizationRequirement UpdateThread =
            new OperationAuthorizationRequirement() { Name = ForumConstants.UpdateThreadOperationName };
        public static OperationAuthorizationRequirement DeleteThread =
            new OperationAuthorizationRequirement() { Name = ForumConstants.DeleteThreadOperationName };

        public static OperationAuthorizationRequirement CreatePost =
            new OperationAuthorizationRequirement() { Name = ForumConstants.CreatePostOperationName };
        public static OperationAuthorizationRequirement ReadPost =
            new OperationAuthorizationRequirement() { Name = ForumConstants.ReadPostOperationName };
        public static OperationAuthorizationRequirement UpdatePost =
            new OperationAuthorizationRequirement() { Name = ForumConstants.UpdatePostOperationName };
        public static OperationAuthorizationRequirement DeletePost =
            new OperationAuthorizationRequirement() { Name = ForumConstants.DeletePostOperationName };
    }

    public static class ForumPolicies
    {
        public static AuthorizationPolicy IsThreadOwner()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .AddRequirements(
                                                       ForumOperations.CreateThread, 
                                                       ForumOperations.UpdateThread, 
                                                       ForumOperations.DeleteThread)
                                                   .Build();
        }

        public static AuthorizationPolicy IsPostOwner()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                   .AddRequirements(
                                                       ForumOperations.CreatePost,
                                                       ForumOperations.UpdatePost,
                                                       ForumOperations.DeletePost)
                                                   .Build();
        }
    }

    public static class ForumConstants
    {
        public const string CreateHubOperationName = "CreateHub";
        public const string ReadHubOperationName = "ReadHub";
        public const string UpdateHubOperationName = "UpdateHub";
        public const string DeleteHubOperationName = "DeleteHub";

        public const string CreateThreadOperationName = "CreateThread";
        public const string ReadThreadOperationName = "ReadThread";
        public const string UpdateThreadOperationName = "UpdateThread";
        public const string DeleteThreadOperationName = "DeleteThread";

        public const string CreatePostOperationName = "CreatePost";
        public const string ReadPostOperationName = "ReadPost";
        public const string UpdatePostOperationName = "UpdatePost";
        public const string DeletePostOperationName = "DeletePost";

        // Roles
        public const string AdministratorRole = "Administrator";
        public const string ModeratorRole = "Moderator";
        public const string UserRole = "User";

        // Policies
        public const string IsThreadOwner = "IsThreadOwner";
        public const string IsPostOwner = "IsPostOwner";
    }
}
