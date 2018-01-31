using DurationHelper.Exceptions;
using DurationHelper.Providers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DurationHelper {
    public static class Duration {
        /// <summary>
        /// Find the duration of a video from its URL. This function can throw
        /// several exceptions if the process fails, but it can also return
        /// null if the process succeeds and the duration simply can't be
        /// determined.
        /// </summary>
        /// <param name="url">A public URL pointing to an MP4, HLS, youTube, or Vimeo video</param>
        /// <param name="youTubeKey">A YouTube Data API v3 key. If not provided, all YouTube URLs will return a duration of null, as if they were not recognized.</param>
        /// <param name="twitchClientId">A Twitch API client ID. If not provided, all Twitch URLs will return null.</param>
        /// <param name="twitchSecret">A Twitch API client secret. If not provided, all Twitch URLs will return null.</param>
        /// <returns>The duration, or null if the duration could not be determined.</returns>
        /// <exception cref="ArgumentNullException">url is null.</exception>
        /// <exception cref="WebException">An HTTP request failed or returned a status outside of the 200 range.</exception>
        /// <exception cref="FormatException">The duration could not be parsed.</exception>
        /// <exception cref="OverflowException">An HLS chunklist contains a chunk length that is out of range.</exception>
        /// <exception cref="JsonReaderException">A JSON response could not be deserialized.</exception>
        /// <exception cref="YouTubeAPIException">A YouTube API error occurred.</exception>
        /// <exception cref="TooManyRequestsException">An API quota has been exceeded.</exception>
        /// <exception cref="VideoNotFoundException">No video was found at the given URL.</exception>
        /// <exception cref="VideoURLParseException">The URL did not match a known URL format for its site.</exception>
        public static async Task<TimeSpan?> GetAsync(Uri url, string youTubeKey = null, string twitchClientId = null, string twitchSecret = null) {
            if (url == null) throw new ArgumentNullException();

            return await GetByNameAsync(url, youTubeKey: youTubeKey, twitchClientId: twitchClientId, twitchSecret: twitchSecret)
                ?? await GetByContentTypeAsync(url);
        }

        /// <summary>
        /// Find the duration of a video from its provider and ID. This
        /// function can throw several exceptions if the process fails, but it
        /// can also return null if the duration can't be determined or if the
        /// provider is unrecognized.
        /// </summary
        /// <param name="provider">The video provider as given by jsVideoUrlParser, e.g. "youtube" or "vimeo"</param>
        /// <param name="id">The video ID</param>
        /// <param name="youTubeKey">A YouTube Data API v3 key. If not provided, all YouTube URLs will return a duration of null, as if they were not recognized.</param>
        /// <param name="twitchClientId">A Twitch API client ID. If not provided, all Twitch URLs will return null.</param>
        /// <param name="twitchSecret">A Twitch API client secret. If not provided, all Twitch URLs will return null.</param>
        /// <returns>The duration, or null if the duration could not be determined.</returns>
        /// <exception cref="ArgumentNullException">id is null.</exception>
        /// <exception cref="WebException">An HTTP request failed or returned a status outside of the 200 range.</exception>
        /// <exception cref="FormatException">The duration could not be parsed.</exception>
        /// <exception cref="JsonReaderException">A JSON response could not be deserialized.</exception>
        /// <exception cref="YouTubeAPIException">A YouTube API error occurred.</exception>
        /// <exception cref="TooManyRequestsException">An API quota has been exceeded.</exception>
        /// <exception cref="VideoNotFoundException">No video was found at the given URL.</exception>
        public static async Task<TimeSpan?> GetAsync(string provider, string id, string youTubeKey = null, string twitchClientId = null, string twitchSecret = null) {
            if (id == null) throw new ArgumentNullException(nameof(id));

            switch (provider) {
                case "youtube":
                    return youTubeKey == null
                        ? (TimeSpan?)null
                        : await new YouTube(youTubeKey).GetDurationAsync(id);
                case "vimeo":
                    return await Vimeo.GetDurationAsync(new Uri($"https://vimeo.com/{WebUtility.UrlEncode(id)}"));
                case "dailymotion":
                    return await Dailymotion.GetDurationAsync(id);
                case "twitch":
                    var twitch = await Twitch.CreateAsync(twitchClientId, twitchSecret);
                    return await twitch.GetDurationAsync(id);
                default:
                    // Unrecognized provider
                    return null;
            }
        }

        private static async Task<TimeSpan?> GetByNameAsync(Uri url, string youTubeKey = null, string twitchClientId = null, string twitchSecret = null) {
            if (url == null) throw new ArgumentNullException();

            if (url.AbsolutePath.EndsWith(".mp4", StringComparison.InvariantCultureIgnoreCase)) {
                // Assume MP4
                return await MP4.GetDurationAsync(url);
            } else if (url.AbsolutePath.EndsWith(".m3u8", StringComparison.InvariantCultureIgnoreCase)) {
                // Assume HLS
                return await HLS.GetPlaylistDurationAsync(url);
            } else if (url.Authority.EndsWith("vimeo.com")) {
                return await Vimeo.GetDurationAsync(url);
            } else if (url.Authority.EndsWith("youtube.com") || url.Authority.EndsWith("youtu.be")) {
                return youTubeKey == null
                    ? null
                    : await new YouTube(youTubeKey).GetDurationAsync(url);
            } else if (url.Authority.EndsWith("dailymotion.com") || url.Authority.EndsWith("dai.ly")) {
                return await Dailymotion.GetDurationAsync(url);
            } else if (url.Authority.EndsWith("twitch.tv")) {
                var twitch = await Twitch.CreateAsync(twitchClientId, twitchSecret);
                return await twitch.GetDurationAsync(url);
            } else {
                return null;
            }
        }

        private static async Task<TimeSpan?> GetByContentTypeAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            // Get content type
            try { 
                HttpWebRequest req = WebRequest.CreateHttp(url);
                req.UserAgent = Shared.UserAgent;
                req.Method = "HEAD";
                using (var resp = await req.GetResponseAsync()) {
                    switch (resp.ContentType) {
                        case "application/vnd.apple.mpegurl":
                            return await HLS.GetPlaylistDurationAsync(url);
                        case "video/mp4":
                            return await MP4.GetDurationAsync(url);
                        default:
                            return null;
                    }
                }
            } catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) {
                throw new VideoNotFoundException(ex);
            }
        }
    }
}
