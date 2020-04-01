using ForumSPA.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ForumSPA.Client.Services
{
    public class ForumClientService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authProvider;

        public ForumClientService(HttpClient httpClient,
                                  NavigationManager navManager,
                                  AuthenticationStateProvider authProvider)
        {
            /*
            var baseUri = new Uri(navManager.BaseUri + "api/forum/");
            httpClient.BaseAddress = baseUri;
            /**/

            _httpClient = httpClient;
            _authProvider = authProvider;
        }

        public async Task<GenericGetResult<List<HubModel>>> GetHubs()
        {
            var response = await _httpClient.GetAsync("api/forum/hubs");
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GenericGetResult<List<HubModel>>>(responseContent,
                                                                                      new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return result;
        }
        
        public async Task<GenericGetResult<HubModel>> GetHub(int hubId)
        {
            var response = await _httpClient.GetAsync($"api/forum/hub/{hubId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GenericGetResult<HubModel>>(responseContent, 
                                                                                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return result;
        }

        public async Task<GenericGetResult<List<ThreadModel>>> GetThreads(int hubId)
        {
            var response = await _httpClient.GetAsync($"api/forum/threads/{hubId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GenericGetResult<List<ThreadModel>>>(responseContent,
                                                                                         new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return result;
        }

        public async Task<GenericGetResult<ThreadModel>> GetThread(int threadId)
        {
            var response = await _httpClient.GetAsync($"api/forum/thread/{threadId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GenericGetResult<ThreadModel>>(responseContent,
                                                                                   new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return result;
        }

        public async Task<GenericGetResult<List<PostModel>>> GetPosts(int threadId)
        {
            var response = await _httpClient.GetAsync($"api/forum/posts/{threadId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GenericGetResult<List<PostModel>>>(responseContent,
                                                                                       new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return result;
        }

        public async Task<GenericResult> CreateThread(ThreadModel thread)
        {
            var threadJson = JsonSerializer.Serialize(thread);
            var response = await _httpClient.PostAsync("api/forum/thread", new StringContent(threadJson, Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GenericResult>(responseContent,
                                                                   new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            if (response.IsSuccessStatusCode)
                return new GenericResult() { Succeeded = true };

            return result;
        }

        public async Task<GenericResult> UpdateThread(ThreadEditModel model)
        {
            var threadJson = JsonSerializer.Serialize(model);
            var response = await _httpClient.PutAsync("api/forum/thread", new StringContent(threadJson, Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GenericResult>(responseContent,
                                                                   new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return result;
        }

        public async Task<GenericResult> DeleteThread(int threadId)
        {
            var response = await _httpClient.DeleteAsync($"api/forum/thread/{threadId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GenericResult>(responseContent,
                                                                   new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return result;
        }
    }
}
