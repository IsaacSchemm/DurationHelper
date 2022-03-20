using DurationHelper.Credentials;
using DurationHelper.Providers;
using ISchemm.DurationFinder;
using System;
using System.Threading.Tasks;

namespace DurationHelper {
    public static class Duration {
        [Obsolete()]
        public static async Task<TimeSpan?> GetAsync(Uri url, string youTubeKey = null, ITwitchCredentials twitchCredentials = null) {
            if (url == null) throw new ArgumentNullException();

            return await ISchemm.DurationFinder.Providers.All.GetDurationAsync(url);
        }

        public static async Task<TimeSpan?> GetAsync(string provider, string id, string youTubeKey = null, ITwitchCredentials twitchCredentials = null) {
            if (id == null) throw new ArgumentNullException(nameof(id));

            switch (provider) {
                case "youtube":
                    return await new SchemaOrgDurationProvider().GetDurationAsync(new Uri($"https://www.youtube.com/watch?v={Uri.EscapeDataString(id)}"));
                case "vimeo":
                    return await new OEmbedDurationProvider().GetDurationAsync(new Uri($"https://vimeo.com/{Uri.EscapeUriString(id)}"));
                case "dailymotion":
                    return await new OpenGraphDurationProvider().GetDurationAsync(new Uri($"https://www.dailymotion.com/video/{Uri.EscapeDataString(id)}"));
                case "twitch":
                    return await new OpenGraphDurationProvider().GetDurationAsync(new Uri($"https://www.twitch.tv/videos/{Uri.EscapeDataString(id)}"));
                default:
                    // Unrecognized provider
                    return null;
            }
        }
    }
}
