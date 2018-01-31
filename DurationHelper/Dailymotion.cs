using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DurationHelper {
    public static class Dailymotion {
        private readonly static Regex REGEX_ID = new Regex(@"(?:/video|ly)/([A-Za-z0-9]+)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Extract a video ID from a Dailymotion URL.
        /// </summary>
        /// <param name="url">The Dailymotion URL</param>
        /// <returns>The video ID</returns>
        /// <exception cref="VideoURLParseException">The URL format was not recognized as a Dailymotion URL.</exception>
        public static string GetIdFromUrl(Uri url) {
            var match = REGEX_ID.Match(url.AbsoluteUri);
            return match.Success ? match.Groups[1].Value : throw new VideoURLParseException();
        }

        /// <summary>
        /// Find the duration of a Vimeo video by its ID.
        /// </summary>
        /// <param name="id">The ID of the video</param>
        /// <returns>The video duration</returns>
        /// <exception cref="ArgumentNullException">id is null.</exception>
        /// <exception cref="WebException">The Dailymotion API request failed or returned a status outside of the 200 range.</exception>
        /// <exception cref="JsonReaderException">The Dailymotion API response could not be deserialized.</exception>
        /// <exception cref="VideoNotFoundException">No video was found at the given URL.</exception>
        public static async Task<TimeSpan> GetDurationAsync(string id) {
            if (id == null) throw new ArgumentNullException();

            try {
                HttpWebRequest req = WebRequest.CreateHttp($"https://api.dailymotion.com/video/{WebUtility.UrlEncode(id)}?fields=duration");
                req.UserAgent = Shared.UserAgent;
                using (var resp = await req.GetResponseAsync()) {
                    using (var sr = new StreamReader(resp.GetResponseStream())) {
                        var obj = JsonConvert.DeserializeAnonymousType(await sr.ReadToEndAsync(), new {
                            duration = 0.0
                        });
                        return TimeSpan.FromSeconds(obj.duration);
                    }
                }
            } catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) {
                throw new VideoNotFoundException(ex);
            }
        }

        /// <summary>
        /// Find the duration of a Vimeo video by its URL.
        /// </summary>
        /// <param name="url">The URL of the video</param>
        /// <returns>The video duration</returns>
        /// <exception cref="ArgumentNullException">url is null.</exception>
        /// <exception cref="WebException">The Dailymotion API request failed or returned a status outside of the 200 range.</exception>
        /// <exception cref="JsonReaderException">The Dailymotion API response could not be deserialized.</exception>
        public static async Task<TimeSpan> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            return await GetDurationAsync(GetIdFromUrl(url));
        }
    }
}
