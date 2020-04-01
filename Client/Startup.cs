using Blazored.Localisation.Services;
using Blazored.LocalStorage;
using ForumSPA.Client.Identity;
using ForumSPA.Client.Services;
using ForumSPA.Client.Utils;
using ForumSPA.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Blazor.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Reflection;

namespace ForumSPA.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();
            services.AddAuthorizationCore();

            services.AddScoped<ApiAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<ApiAuthenticationStateProvider>());

            // Leaving all these here until there's a way to have different httpclients for each service
            // Necessary hack since the WebAssemblyHttpMessageHandler was deleted with no replacement
            //var wasmHttpMessageHandlerType = Assembly.Load("WebAssembly.Net.Http").GetType("WebAssembly.Net.Http.HttpClient.WasmHttpMessageHandler");
            //services.AddHttpClient<AuthService>()
            //        .ConfigurePrimaryHttpMessageHandler(provider => (HttpMessageHandler)Activator.CreateInstance(wasmHttpMessageHandlerType));
            services.AddScoped<AuthService>();
            services.AddScoped<IAuthService>(provider => provider.GetRequiredService<AuthService>());
            // TODO: Check this link for multiple implementations of the same interface: https://stackoverflow.com/questions/39174989/how-to-register-multiple-implementations-of-the-same-interface-in-asp-net-core
            services.AddScoped<IStateChangeAsync>(provider => provider.GetRequiredService<AuthService>());

            //services.AddHttpClient<ForumClientService>()
            //        .ConfigurePrimaryHttpMessageHandler(provider => (HttpMessageHandler)Activator.CreateInstance(wasmHttpMessageHandlerType));
            services.AddScoped<ForumClientService>();

            services.AddScoped<BrowserDateTimeProvider>();

            services.AddAuthorizationCore(config =>
            {
                config.AddPolicy(ForumConstants.IsThreadOwner, ForumPolicies.IsThreadOwner());
                config.AddPolicy(ForumConstants.IsPostOwner, ForumPolicies.IsPostOwner());
            });

            services.AddScoped<IAuthorizationHandler, ThreadIsOwnerAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, PostIsOwnerAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler, HubAdministratorAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ThreadAdministratorAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, PostAdministratorAuthorizationHandler>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.UseBrowserLocalisation();
            app.AddComponent<App>("app");
        }
    }
}
