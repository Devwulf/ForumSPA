using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSPA.Shared.Utils
{
    public static class DateHelper
    {
        public static string ToTimeSpan(DateTime dateTime)
        {
            return ToTimeSpan(dateTime, DateTime.Now);
        }

        // TODO: The DateTime.Now is currently set to UTC time, have to wait until time localization is implemented
        public static string ToTimeSpan(DateTime dateTime, DateTime now)
        {
            var span = now - dateTime;

            int secs = (int)span.TotalSeconds;
            int mins = (int)span.TotalMinutes;
            int hours = (int)span.TotalHours;
            int days = (int)span.TotalDays;
            if (secs < 2)
                return "a second ago";
            if (secs < 60)
                return $"{secs} secs ago";

            if (mins < 2)
                return "a minute ago";
            if (mins < 60)
                return $"{mins} mins ago";

            if (hours < 2)
                return "an hour ago";
            if (hours < 24)
                return $"{hours} hours ago";

            if (days < 2)
                return "a day ago";

            return $"{days} days ago";
        }
    }
}
