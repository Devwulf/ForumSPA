using Microsoft.AspNetCore.Components.Builder;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ForumSPA.Client.Utils
{
    public static class BlazorHelper
    {
        public static void UseBrowserLocalisation(this IComponentsApplicationBuilder app)
        {
            var jsRuntime = app.Services.GetService(typeof(IJSRuntime));
            var browserLocale = ((IJSInProcessRuntime)jsRuntime).Invoke<string>("helperFunctions.getBrowserLocale");
            var culture = new CultureInfo(browserLocale);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}
