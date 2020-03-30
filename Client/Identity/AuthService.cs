using Blazored.LocalStorage;
using ForumSPA.Shared.Models.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ForumSPA.Client.Identity
{
    public class AuthService : IAuthService, IStateChangeAsync
    {
        public event Func<Task> OnStateChange;

        private readonly HttpClient _httpClient;
        private readonly ApiAuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                           NavigationManager navManager,
                           ApiAuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            /*
            var baseUri = new Uri(navManager.BaseUri + "api/account/");
            httpClient.BaseAddress = baseUri;
            /**/

            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<RegisterResult> Register(RegisterModel registerModel)
        {
            var registerAsJson = JsonSerializer.Serialize(registerModel);
            var response = await _httpClient.PostAsync("api/account/register", new StringContent(registerAsJson, Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RegisterResult>(responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            return result;
        }

        public async Task<LoginResult> Login(LoginModel loginModel)
        {
            /* Unlike PostJsonAsync, this returns the error object
             * instead of throwing an exception */
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var response = await _httpClient.PostAsync("api/account/login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<LoginResult>(responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            
            if (!response.IsSuccessStatusCode)
            {
                return result;
            }
            /**/

            /*
            var result = await _httpClient.PostJsonAsync<LoginResult>("api/account/login", loginModel);

            if (!result.Succeeded)
            {
                return result;
            }
            /**/

            // Store token as cookie in local browser
            await _localStorage.SetItemAsync("authToken", result.Token);
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
            
            await NotifyStateChange();

            return result;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;

            await NotifyStateChange();
        }

        public async Task NotifyStateChange()
        {
            var handler = OnStateChange;
            if (handler == null)
                return;

            var invokeList = handler.GetInvocationList();
            var tasks = new Task[invokeList.Length];

            for (int i = 0; i < invokeList.Length; i++)
                tasks[i] = ((Func<Task>)invokeList[i])();

            await Task.WhenAll(tasks);
        }
    }
}
