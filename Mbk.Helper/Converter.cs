using System;
using System.Globalization;

namespace Mbk.Helper
{
    public static class Converter
    {
        private static string DateFormat = "yyyyMMdd";
        private static string TimeFormat = "HHmm";
        private static string DateTimeFormat = $"{DateFormat} {TimeFormat}";

        public static DateTime ConvertToDateTime(string input)
        {
            return DateTime.ParseExact(input, DateTimeFormat, null);
        }

        public static DateTime ConvertToDate(string input)
        {
            return DateTime.ParseExact(input, DateFormat, null);
        }

        public static string ToDateString(DateTime input)
        {
            return input.ToString(DateFormat);
        }

        public static string ToTimeString(DateTime input)
        {
            return input.ToString(TimeFormat);
        }

        public static TimeSpan ConvertToTime(string input)
        {
            input = input.Replace(":", "");
            return TimeSpan.ParseExact(
                input,
                new string[] { "hhmm", @"\+hhmm", @"\-hhmm" },
                null,
                input.StartsWith("-") ? TimeSpanStyles.AssumeNegative : TimeSpanStyles.None);
        }

        public static string ToTimeString(TimeSpan input)
        {
            return ((input.Hours < 0) ? "-" : "+") + input.ToString("hhmm");
        }
    }
}
