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

            return await new ChainedDurationProvider(
                ISchemm.DurationFinder.Providers.All,
                new Twitch(twitchCredentials)
            ).GetDurationAsync(url);
        }

        public static async Task<TimeSpan?> GetAsync(string provider, string id, string youTubeKey = null, ITwitchCredentials twitchCredentials = null) {
            if (id == null) throw new ArgumentNullException(nameof(id));

            switch (provider) {
                case "youtube":
                    return youTubeKey == null
                        ? (TimeSpan?)null
                        : await new YouTube(youTubeKey).GetDurationAsync(id);
                case "vimeo":
                    return await new OEmbedDurationProvider().GetDurationAsync(new Uri($"https://vimeo.com/{Uri.EscapeUriString(id)}"));
                case "dailymotion":
                    return await Dailymotion.GetDurationAsync(id);
                case "twitch":
                    return await new Twitch(twitchCredentials).GetDurationAsync(id);
                default:
                    // Unrecognized provider
                    return null;
            }
        }
    }
}
