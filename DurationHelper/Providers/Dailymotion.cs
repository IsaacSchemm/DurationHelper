using ISchemm.DurationFinder;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace DurationHelper.Providers {
    public static class Dailymotion {
        private readonly static Regex REGEX_ID = new Regex(@"(?:/video|ly)/([A-Za-z0-9]+)", RegexOptions.IgnoreCase);

        public static string GetIdFromUrl(Uri url) {
            var match = REGEX_ID.Match(url.AbsoluteUri);
            return match.Success ? match.Groups[1].Value : throw new FormatException("Could not extract video ID from URL");
        }

        public static async Task<TimeSpan> GetDurationAsync(string id) {
            return (await new OpenGraphDurationProvider().GetDurationAsync(new Uri($"https://www.dailymotion.com/video/{Uri.EscapeDataString(id)}"))).Value;
        }

        [Obsolete]
        public static async Task<TimeSpan> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            return (await new OpenGraphDurationProvider().GetDurationAsync(url)).Value;
        }
    }
}
