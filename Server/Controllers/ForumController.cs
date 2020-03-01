using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ForumSPA.Server.Authorization;
using ForumSPA.Server.Data;
using ForumSPA.Server.Data.Models;
using ForumSPA.Server.Services;
using ForumSPA.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumSPA.Server.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authService;
        private readonly ForumServerService _forumService;

        public ForumController(
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authService,
            ForumServerService forumService)
        {
            _userManager = userManager;
            _authService = authService;
            _forumService = forumService;
        }

        /// <summary>
        /// Gets the list of hubs in HubModel form.
        /// </summary>
        /// <returns>A GenericGetResult that contains the list of hubs in HubModel form.</returns>
        [HttpGet("hubs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHubModels()
        {
            var hubModels = new List<HubModel>();
            var hubs = await _forumService.GetAllHubs();

            foreach (var hub in hubs)
            {
                var model = await ConvertHubToHubModel(hub);
                if (model != null)
                    hubModels.Add(model);
            }

            return Ok(new GenericGetResult<List<HubModel>>()
            {
                Succeeded = true,
                Value = hubModels
            });
        }

        // Test
        [HttpGet("user")]
        public IActionResult GetUsername()
        {
            return Ok(new GenericGetResult<string>()
            {
                Succeeded = true,
                Value = User.Identity.Name
            });
        }

        [HttpGet("threads/{hubId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetThreadModelsByHub(int hubId)
        {
            var threadModels = new List<ThreadModel>();
            var threads = await _forumService.GetAllThreadsByHub(hubId);

            foreach (var thread in threads)
            {
                var model = await ConvertThreadToThreadModel(thread);
                if (model != null)
                    threadModels.Add(model);
            }

            return Ok(new GenericGetResult<List<ThreadModel>>()
            {
                Succeeded = true,
                Value = threadModels
            });
        }

        [HttpGet("threads/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetThreadModelsByUser(string userId)
        {
            var threadModels = new List<ThreadModel>();
            var threads = await _forumService.GetAllThreadsByUser(userId);

            foreach (var thread in threads)
            {
                var model = await ConvertThreadToThreadModel(thread);
                if (model != null)
                    threadModels.Add(model);
            }

            return Ok(new GenericGetResult<List<ThreadModel>>()
            {
                Succeeded = true,
                Value = threadModels
            });
        }

        [HttpGet("posts/{threadId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostModelsByThread(int threadId)
        {
            var postModels = new List<PostModel>();
            var posts = await _forumService.GetAllPostsByThread(threadId);

            foreach (var post in posts)
            {
                var model = await ConvertPostToPostModel(post);
                if (model != null)
                    postModels.Add(model);
            }

            return Ok(new GenericGetResult<List<PostModel>>() 
            { 
                Succeeded = true,
                Value = postModels
            });
        }

        [HttpGet("posts/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostModelsByUser(string userId)
        {
            var postModels = new List<PostModel>();
            var posts = await _forumService.GetAllPostsByUser(userId);

            foreach (var post in posts)
            {
                var model = await ConvertPostToPostModel(post);
                if (model != null)
                    postModels.Add(model);
            }

            return Ok(new GenericGetResult<List<PostModel>>()
            {
                Succeeded = true,
                Value = postModels
            });
        }

        [HttpGet("hub/{hubId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHub(int hubId)
        {
            var hub = await _forumService.GetHub(hubId);
            if (hub == null)
                return BadRequest(new GenericGetResult<HubModel>()
                {
                    Succeeded = false,
                    Error = $"Hub of id '{hubId}' was not found and not retrieved."
                });

            var hubModel = await ConvertHubToHubModel(hub);
            return Ok(new GenericGetResult<HubModel>()
            {
                Succeeded = true,
                Value = hubModel
            });
        }

        /// <summary>
        /// Creates a new Hub.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/Forum/hub
        ///     {
        ///         "name": "Fight Club",
        ///         "rules": "1. You do NOT talk about Fight Club",
        ///         "description": "Fight!"
        ///     }
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("hub")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateHub([FromBody] HubModel model)
        {
            var newHub = new Hub()
            {
                Name = model.Name,
                Rules = model.Rules,
                Description = model.Description
            };
            if (!newHub.IsValid())
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = "The created hub has an empty/whitespace title."
                });

            var authResult = await _authService.AuthorizeAsync(User, newHub, ForumOperations.CreateHub);
            if (!authResult.Succeeded)
                return Unauthorized(new GenericResult()
                {
                    Succeeded = false,
                    Error = "Not authorized"
                });

            var createdHub = await _forumService.CreateHub(newHub);

            model.Id = createdHub.Id;
            model.ThreadCount = 0;

            // This returns a 201 Created Success status code with
            // the hub model and the location for getting the hub
            return CreatedAtAction(nameof(GetHub), new { hubId = model.Id }, model);
        }

        [HttpPut("hub")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateHub([FromBody] HubModel model)
        {
            var hub = await _forumService.GetHub(model.Id);
            if (hub == null)
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = $"Hub of id '{model.Id}' was not found and not updated."
                }); 
            
            var authResult = await _authService.AuthorizeAsync(User, hub, ForumOperations.UpdateHub);
            if (!authResult.Succeeded)
                return Unauthorized(new GenericResult()
                {
                    Succeeded = false,
                    Error = "Not authorized"
                });

            if (hub.Name.Equals(model.Name) &&
                hub.Rules.Equals(model.Rules) &&
                hub.Description.Equals(model.Description))
                return Ok(new GenericResult() { Succeeded = true, Error = $"Nothing changed with hub of id '{model.Id}'!" });

            hub.Name = model.Name;
            hub.Rules = model.Rules;
            hub.Description = model.Description;
            if (!hub.IsValid())
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = "The created hub has an empty/whitespace title."
                });

            await _forumService.UpdateHub(hub);

            return Ok(new GenericResult() { Succeeded = true });
        }

        [HttpDelete("hub/{hubId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteHub(int hubId)
        {
            var hub = await _forumService.GetHub(hubId);
            if (hub == null)
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = $"Hub of id '{hubId}' was not found and not deleted."
                });

            var authResult = await _authService.AuthorizeAsync(User, hub, ForumOperations.DeleteHub);
            if (!authResult.Succeeded)
                return Unauthorized(new GenericResult()
                {
                    Succeeded = false,
                    Error = "Not authorized"
                });

            await _forumService.DeleteHub(hubId);

            return Ok(new GenericResult() { Succeeded = true });
        }

        [HttpGet("thread/{threadId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetThread(int threadId)
        {
            var thread = await _forumService.GetThread(threadId);
            if (thread == null)
                return BadRequest(new GenericGetResult<ThreadModel>()
                {
                    Succeeded = false,
                    Error = $"Thread of id '{threadId}' was not found and not retrieved."
                });

            var threadModel = await ConvertThreadToThreadModel(thread);
            return Ok(new GenericGetResult<ThreadModel>()
            {
                Succeeded = true,
                Value = threadModel
            });
        }

        [HttpPost("thread")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateThread([FromBody] ThreadModel model)
        {
            var newThread = new Thread()
            {
                Name = model.Name,
                HubId = model.HubId,
                UserId = model.UserId
            };
            if (!newThread.IsValid())
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = "The created thread has an empty/whitespace title."
                });
            
            var authResult = await _authService.AuthorizeAsync(User, newThread, ForumOperations.CreateThread);
            if (!authResult.Succeeded)
                return Unauthorized(new GenericResult()
                {
                    Succeeded = false,
                    Error = "Not Authorized"
                });

            var createdThread = await _forumService.CreateThread(newThread);

            var firstPost = new Post()
            {
                Body = model.Body,
                IsMainPost = true,
                ThreadId = createdThread.Id,
                UserId = createdThread.UserId
            };
            if (!firstPost.IsValid())
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = "The created post has an empty/whitespace body."
                });

            await _forumService.CreatePost(firstPost);

            model.Id = createdThread.Id;
            model.DateModified = createdThread.DateModified;

            return CreatedAtAction(nameof(GetThread), new { threadId = model.Id }, model);
        }

        [HttpPut("thread")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateThread([FromBody] ThreadModel model)
        {
            var thread = await _forumService.GetThread(model.Id);
            if (thread == null)
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = $"Thread of id '{model.Id}' was not found and not updated."
                });

            var authResult = await _authService.AuthorizeAsync(User, thread, ForumOperations.UpdateThread);
            if (!authResult.Succeeded)
                return Unauthorized(new GenericResult()
                {
                    Succeeded = false,
                    Error = "Not Authorized"
                });

            if (thread.Name.Equals(model.Name))
                return Ok(new GenericResult() { Succeeded = true, Error = $"Nothing changed with thread of id '{model.Id}'!" });

            // Only the thread title gets edited
            thread.Name = model.Name;
            if (!thread.IsValid())
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = "The edited thread has an empty/whitespace title."
                });

            await _forumService.UpdateThread(thread);

            return Ok(new GenericResult() { Succeeded = true });
        }

        [HttpDelete("thread/{threadId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteThread(int threadId)
        {
            var thread = await _forumService.GetThread(threadId);
            if (thread == null)
                return BadRequest(new GenericResult() 
                { 
                    Succeeded = false, 
                    Error = $"Thread of id '{threadId}' was not found and not deleted." 
                });

            var authResult = await _authService.AuthorizeAsync(User, thread, ForumOperations.DeleteThread);
            if (!authResult.Succeeded)
                return Unauthorized(new GenericResult()
                {
                    Succeeded = false,
                    Error = "Not Authorized"
                });

            await _forumService.DeleteThread(threadId);

            return Ok(new GenericResult() { Succeeded = true });
        }

        [HttpGet("post/{postId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPost(int postId)
        {
            var post = await _forumService.GetPost(postId);
            if (post == null)
                return BadRequest(new GenericGetResult<PostModel>()
                {
                    Succeeded = false,
                    Error = $"Post of id '{postId}' was not found and not retrieved."
                });

            var postModel = await ConvertPostToPostModel(post);
            return Ok(new GenericGetResult<PostModel>()
            {
                Succeeded = true,
                Value = postModel
            });
        }

        [HttpPost("post")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreatePost([FromBody] PostModel model)
        {
            var newPost = new Post()
            {
                Body = model.Body,
                IsMainPost = false,
                ThreadId = model.ThreadId,
                UserId = model.UserId
            };
            if (!newPost.IsValid())
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = "The created post has an empty/whitespace body."
                });

            var authResult = await _authService.AuthorizeAsync(User, newPost, ForumOperations.CreatePost);
            if (!authResult.Succeeded)
                return Unauthorized(new GenericResult()
                {
                    Succeeded = false,
                    Error = "Not authorized"
                });

            var createdPost = await _forumService.CreatePost(newPost);

            model.Id = createdPost.Id;
            model.DateCreated = createdPost.DateCreated;
            model.DateModified = createdPost.DateModified;

            return CreatedAtAction(nameof(GetPost), new { postId = model.Id }, model);
        }

        [HttpPut("post")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePost([FromBody] PostModel model)
        {
            var post = await _forumService.GetPost(model.Id);
            if (post == null)
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = $"Post of id '{model.Id}' was not found and not updated."
                });

            var authResult = await _authService.AuthorizeAsync(User, post, ForumOperations.UpdatePost);
            if (!authResult.Succeeded)
                return Unauthorized(new GenericResult()
                {
                    Succeeded = false,
                    Error = "Not authorized"
                });

            if (post.Body.Equals(model.Body))
                return Ok(new GenericResult() { Succeeded = true, Error = $"Nothing changed with post of id '{model.Id}'!" });

            post.Body = model.Body;
            if (!post.IsValid())
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = "The edited post has an empty/whitespace body."
                });

            await _forumService.UpdatePost(post);

            return Ok(new GenericResult() { Succeeded = true });
        }

        [HttpDelete("post/{postId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var post = await _forumService.GetPost(postId);
            if (post == null)
                return BadRequest(new GenericResult()
                {
                    Succeeded = false,
                    Error = $"Post of id '{postId}' was not found and not deleted."
                });

            var authResult = await _authService.AuthorizeAsync(User, post, ForumOperations.DeletePost);
            if (!authResult.Succeeded)
                return Unauthorized(new GenericResult()
                {
                    Succeeded = false,
                    Error = "Not authorized"
                });

            await _forumService.DeletePost(postId);

            return Ok(new GenericResult() { Succeeded = true });
        }

        private async Task<HubModel> ConvertHubToHubModel(Hub hub)
        {
            if (hub == null)
                return null;

            var threadCount = (await _forumService.GetAllThreadsByHub(hub.Id)).Count();

            return new HubModel()
            {
                Id = hub.Id,
                Name = hub.Name,
                Description = hub.Description,
                Rules = hub.Rules,
                ThreadCount = threadCount
            };
        }

        private async Task<ThreadModel> ConvertThreadToThreadModel(Thread thread)
        {
            if (thread == null)
                return null;

            var user = await _userManager.FindByIdAsync(thread.UserId);
            var userName = user == null ? "<deleted>" : user.UserName;

            var replyCount = (await _forumService.GetAllPostsByThread(thread.Id)).Count();

            return new ThreadModel()
            {
                Id = thread.Id,
                Name = thread.Name,
                HubId = thread.HubId,
                UserId = thread.UserId,
                UserName = userName,
                ReplyCount = replyCount - 1,
                DateModified = thread.DateModified
            };
        }

        private async Task<PostModel> ConvertPostToPostModel(Post post)
        {
            if (post == null)
                return null;

            var user = await _userManager.FindByIdAsync(post.UserId);
            var userName = user == null ? "<deleted>" : user.UserName;

            return new PostModel()
            {
                Id = post.Id,
                Body = post.Body,
                IsMainPost = post.IsMainPost,
                ThreadId = post.ThreadId,
                UserId = post.UserId,
                UserName = userName,
                DateCreated = post.DateCreated,
                DateModified = post.DateModified
            };
        }
    }
}