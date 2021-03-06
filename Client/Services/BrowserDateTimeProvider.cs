﻿using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazored.Localisation.Services
{
    // TODO: Temporary measure until globalization is implemeted for blazor
    public class BrowserDateTimeProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private BrowserDateTime _browserDateTime;

        public BrowserDateTimeProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<BrowserDateTime> GetInstance()
        {
            if (_browserDateTime == null)
            {
                var timeZoneOffSet = await _jsRuntime.InvokeAsync<int>("eval", "new Date().getTimezoneOffset()");
                var browserTimeZoneIdentifier = await _jsRuntime.InvokeAsync<string>("eval", "Intl.DateTimeFormat().resolvedOptions().timeZone");
                var timeZoneIdentifier = string.IsNullOrWhiteSpace(browserTimeZoneIdentifier) ? "BrowserTZ" : browserTimeZoneIdentifier;
                var browserTimeZone = TimeZoneInfo.CreateCustomTimeZone(timeZoneIdentifier, new TimeSpan(0, 0 - timeZoneOffSet, 0), timeZoneIdentifier, timeZoneIdentifier);

                _browserDateTime = new BrowserDateTime(browserTimeZone);
            }

            return _browserDateTime;
        }
    }
}
