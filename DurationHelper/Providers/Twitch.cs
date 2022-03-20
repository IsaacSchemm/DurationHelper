using DurationHelper.Credentials;
using ISchemm.DurationFinder;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DurationHelper.Providers {
    public class Twitch {
        private readonly static Regex REGEX_ID = new Regex(@"(clips\.)?twitch\.tv/(?:(?:videos/(\d+))|(\w+))?", RegexOptions.IgnoreCase);

        public static string GetVideoIdFromUrl(Uri url) {
            var match = REGEX_ID.Match(url.AbsoluteUri);
            return match.Success ? match.Groups[2].Value : throw new FormatException("Could not extract video ID from URL");
        }

        public Twitch(ITwitchCredentials credentials) { }

        public async Task<TimeSpan?> GetDurationAsync(string id) {
            return await new OpenGraphDurationProvider().GetDurationAsync(new Uri($"https://www.twitch.tv/videos/{Uri.EscapeDataString(id)}"));
        }

        [Obsolete]
        public async Task<TimeSpan?> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            return await new OpenGraphDurationProvider().GetDurationAsync(url);
        }
    }
}
