using Blazored.Localisation.Services;
using ForumSPA.Client.Components;
using ForumSPA.Client.Services;
using ForumSPA.Shared.Models;
using ForumSPA.Shared.Utils;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ForumSPA.Client.Pages
{
    public partial class MainForum : IDisposable
    {
        public class SearchModel
        {
            [Required]
            public string SearchString { get; set; }
        }

        [Inject]
        public NavigationManager NavManager { get; set; }
        [Inject]
        public ForumClientService ForumService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public BrowserDateTimeProvider DateTimeProvider { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authStateTask { get; set; }

        private int hubId { get; set; }
        private int threadId { get; set; }
        private int postId { get; set; }
        private string threadSearch { get; set; }
        private bool isEditingThread { get; set; } = false;
        private bool isEditingPost { get; set; } = false;

        private HubModel hub;
        private List<ThreadModel> threads;
        private ThreadModel thread;
        private List<PostModel> posts;

        private SearchModel searchModel = new SearchModel();
        private ThreadModel threadModel = new ThreadModel();
        private ThreadEditModel threadEditModel = new ThreadEditModel();
        private PostModel postModel = new PostModel();

        private InputRichText postInputRef;

        private async void OnLocationChanged(object sender, LocationChangedEventArgs e) => await RefreshParams();

        private BrowserDateTime dateTime;

        protected override async Task OnInitializedAsync()
        {
            dateTime = await DateTimeProvider.GetInstance();

            NavManager.LocationChanged += OnLocationChanged;
            await RefreshParams();
        }

        private async Task RefreshParams()
        {
            hub = null;
            thread = null;
            threads = new List<ThreadModel>();
            posts = new List<PostModel>();
            isEditingThread = false;

            var uri = new Uri(NavManager.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);

            hubId = query.TryGetValue("hubId", out var hubStr) ? (int.TryParse(hubStr.First(), out int hubInt) ? hubInt : -1) : -1;
            threadId = query.TryGetValue("threadId", out var threadStr) ? (int.TryParse(threadStr.First(), out int threadInt) ? threadInt : -1) : -1;
            postId = query.TryGetValue("postId", out var postStr) ? (int.TryParse(postStr.First(), out int postInt) ? postInt : -1) : -1;
            threadSearch = query.TryGetValue("threadSearch", out var searchStr) ? searchStr.ToString() : null;

            if (hubId > 0)
            {
                var result = await ForumService.GetHub(hubId);
                if (result.Succeeded)
                    hub = result.Value;
            }

            if (hub != null)
            {
                var result = await ForumService.GetThreads(hub.Id);
                if (result.Succeeded)
                    threads = result.Value;
            }

            if (threadId > 0)
            {
                var result = await ForumService.GetThread(threadId);
                if (result.Succeeded)
                    thread = result.Value;
            }

            if (thread != null)
            {
                var result = await ForumService.GetPosts(thread.Id);
                if (result.Succeeded)
                    posts = result.Value;

                StateHasChanged(); // Wait for all html to load first
                await JSRuntime.InvokeVoidAsync("helperFunctions.IFramelyLoad");
            }

            if (hub != null && !threadSearch.IsNullOrWhiteSpace())
            {
                searchModel.SearchString = threadSearch;
                threads = (from t in threads
                           where t.Name.Contains(threadSearch, StringComparison.OrdinalIgnoreCase)
                           select t).ToList();
            }

            StateHasChanged();
        }

        public void Dispose()
        {
            NavManager.LocationChanged -= OnLocationChanged;
        }

        private void HandleSearch()
        {
            if (searchModel != null && !searchModel.SearchString.IsNullOrWhiteSpace())
            {
                var uri = new Uri(NavManager.Uri);
                var query = QueryHelpers.ParseQuery(uri.Query)
                                        .ToDictionary(pair => pair.Key, pair => pair.Value.ToString());

                if (query.ContainsKey("threadSearch"))
                    query["threadSearch"] = searchModel.SearchString;
                else
                    query.Add("threadSearch", searchModel.SearchString);

                var url = QueryHelpers.AddQueryString(NavManager.BaseUri + "forum", query);
                NavManager.NavigateTo(url);
            }
        }

        private async Task HandleCreateThread()
        {
            var authState = await authStateTask;
            var userId = authState.User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
            if (userId.IsNullOrWhiteSpace())
                return;

            var thread = new ThreadModel()
            {
                Name = threadModel.Name,
                Body = threadModel.Body,
                HubId = hub.Id,
                UserId = userId
            };

            var result = await ForumService.CreateThread(thread);
            if (!result.Succeeded)
                return; // Show a toast (yeah I know, it's such a stupid name) about the error

            await JSRuntime.InvokeVoidAsync("helperFunctions.closeModal", "#newThreadModal");
            await RefreshParams();
        }

        private async Task HandleDeleteThread(int threadId)
        {
            var result = await ForumService.DeleteThread(threadId);
            if (!result.Succeeded)
                return;

            await JSRuntime.InvokeVoidAsync("helperFunctions.closeModal", "#deleteThreadModal");
            await RefreshParams();
        }

        private void HandleEditThread(ThreadModel model)
        {
            isEditingThread = true;
            threadEditModel.Id = model.Id;
            threadEditModel.Name = model.Name;
        }

        private async Task HandleSaveEditThread()
        {
            var result = await ForumService.UpdateThread(threadEditModel);
            if (!result.Succeeded)
                return;

            isEditingThread = false;
            await RefreshParams();
        }

        private void HandleCancelEditThread()
        {
            isEditingThread = false;
        }

        private async Task HandleSaveReplyPost()
        {
            var authState = await authStateTask;
            var userId = authState.User.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
            if (userId.IsNullOrWhiteSpace())
                return;

            if (isEditingPost)
            {
                // Save edited post
                var post = new PostModel()
                {
                    Id = postModel.Id,
                    Body = postModel.Body
                };

                var result = await ForumService.UpdatePost(post);
                if (!result.Succeeded)
                    return;

                isEditingPost = false;
            }
            else
            {
                // Reply with a new post
                var post = new PostModel()
                {
                    Body = postModel.Body,
                    ThreadId = thread.Id,
                    UserId = userId,
                    IsMainPost = false
                };

                var result = await ForumService.CreatePost(post);
                if (!result.Succeeded)
                    return;
            }

            await postInputRef.EditorClearValue();
            await RefreshParams();
        }

        private async Task HandleCancelReplyPost()
        {
            await postInputRef.EditorClearValue();
            isEditingPost = false;
        }

        private async Task HandleQuotePost(PostModel model)
        {
            var quote = $"<blockquote>" +
                            $"<div class=\"d-flex justify-content-end small c-gray-light\">" +
                                $"quoting&nbsp;<a href=\"/\">{model.UserName}</a>&nbsp;(post&nbsp;<a href=\"/\">#{model.Id}</a>)" +
                            $"</div>" +
                            $"{ClearBlockQuotes(model.Body)}" +
                        $"</blockquote>" +
                        $"<p></p>";
            await postInputRef.EditorAddValue(quote);
        }

        private string ClearBlockQuotes(string body)
        {
            var html = new HtmlDocument();
            html.LoadHtml(body);

            var quotes = html.DocumentNode.SelectNodes("blockquote");
            if (quotes != null)
            {
                var quotesList = quotes.ToList();
                foreach (var quote in quotesList)
                    quote.Remove();
            }

            return html.DocumentNode.OuterHtml;
        }

        private async Task HandleReportPost(int postId)
        {

        }

        private async Task HandleEditPost(PostModel model)
        {
            isEditingPost = true;
            postModel.Id = model.Id;
            postModel.Body = model.Body;
            await postInputRef.EditorSetValue(postModel.Body);
        }

        private async Task HandleDeletePost(int postId)
        {
            var result = await ForumService.DeletePost(postId);
            if (!result.Succeeded)
                return;

            await RefreshParams();
        }

        private string Replies(int replies)
        {
            if (replies == 1)
                return "1 Reply";

            return $"{replies} Replies";
        }

        private string EncodeUri(string uri)
        {
            return WebUtility.UrlEncode(uri);
        }
    }
}
