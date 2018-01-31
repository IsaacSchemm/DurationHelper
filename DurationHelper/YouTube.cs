using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DurationHelper {
    public class YouTube {
        private string _key;

        private readonly static Regex REGEX_ID = new Regex(@"(?:(?:v|vi|be|videos|embed)/(?!videoseries)|(?:v|ci)=)([\w-]{11})", RegexOptions.IgnoreCase);

        /// <summary>
        /// Create a new DurationHelper.YouTube object with a YouTube Data API v3 key.
        /// </summary>
        /// <param name="key">The YouTube API key</param>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        public YouTube(string key) {
            _key = key ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Extract a YouTube ID from a YouTube URL.
        /// </summary>
        /// <param name="url">The YouTube URL (youtube.com or youtu.be)</param>
        /// <returns>The YouTube ID</returns>
        /// <exception cref="VideoURLParseException">The URL format was not recognized as a YouTube URL.</exception>
        public static string GetIdFromUrl(Uri url) {
            var match = REGEX_ID.Match(url.AbsoluteUri);
            return match.Success ? match.Groups[1].Value : throw new VideoURLParseException();
        }

        /// <summary>
        /// Get the duration of a YouTube video by its ID.
        /// </summary>
        /// <param name="id">The video's YouTube ID</param>
        /// <returns>The duration of the video</returns>
        /// <exception cref="ArgumentNullException">id is null.</exception>
        /// <exception cref="WebException">The YouTube API request failed.</exception>
        /// <exception cref="JsonReaderException">The YouTube API response could not be deserialized.</exception>
        /// <exception cref="FormatException">The duration could not be parsed from the YouTube API response.</exception>
        /// <exception cref="YouTubeAPIException">A YouTube API error occurred.</exception>
        /// <exception cref="TooManyRequestsException">The YouTube API quota has been exceeded.</exception>
        /// <exception cref="VideoNotFoundException">No video was found at the given URL.</exception>
        public async Task<TimeSpan> GetDurationAsync(string id) {
            if (id == null) throw new ArgumentNullException();

            HttpWebRequest req = WebRequest.CreateHttp($"https://www.googleapis.com/youtube/v3/videos?part=contentDetails&id={WebUtility.UrlEncode(id)}&key={WebUtility.UrlEncode(_key)}");
            req.UserAgent = Shared.UserAgent;
            try {
                using (var resp = await req.GetResponseAsync()) {
                    using (var sr = new StreamReader(resp.GetResponseStream())) {
                        string json = await sr.ReadToEndAsync();
                        var obj = JsonConvert.DeserializeAnonymousType(json, new {
                            items = new[] {
                                new {
                                    id = "",
                                    contentDetails = new {
                                        duration = ""
                                    }
                                }
                            }
                        });
                        string ts = obj?.items?.Select(i => i.contentDetails.duration).FirstOrDefault();
                        if (ts == null) {
                            throw new VideoNotFoundException();
                        } else {
                            return XmlConvert.ToTimeSpan(ts);
                        }
                    }
                }
            } catch (WebException ex) when (ex.Response is HttpWebResponse r) {
                throw await YouTubeAPIException.FromHttpWebResponseAsync(r);
            }
        }

        /// <summary>
        /// Get the duration of a YouTube video by its URL.
        /// </summary>
        /// <param name="url">The video's YouTube URL</param>
        /// <returns>The duration of the video</returns>
        /// <exception cref="ArgumentNullException">url is null.</exception>
        /// <exception cref="VideoURLParseException">The URL format was not recognized as a YouTube URL.</exception>
        /// <exception cref="WebException">The YouTube API request failed.</exception>
        /// <exception cref="JsonReaderException">The YouTube API response could not be deserialized.</exception>
        /// <exception cref="FormatException">The duration could not be parsed from the YouTube API response.</exception>
        /// <exception cref="YouTubeAPIException">A YouTube API error occurred.</exception>
        /// <exception cref="TooManyRequestsException">The YouTube API quota has been exceeded.</exception>
        /// <exception cref="VideoURLParseException">The URL format was not recognized as a YouTube URL.</exception>
        public async Task<TimeSpan?> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();
            
            return await GetDurationAsync(GetIdFromUrl(url));
        }
    }
}
