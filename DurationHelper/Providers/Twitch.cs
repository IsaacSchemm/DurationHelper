using DurationHelper.Credentials;
using ISchemm.DurationFinder;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DurationHelper.Providers {
    public class Twitch : IDurationProvider {
        private readonly ITwitchCredentials _credentials;

        private readonly static Regex REGEX_ID = new Regex(@"(clips\.)?twitch\.tv/(?:(?:videos/(\d+))|(\w+))?", RegexOptions.IgnoreCase);

        public static string GetVideoIdFromUrl(Uri url) {
            var match = REGEX_ID.Match(url.AbsoluteUri);
            return match.Success ? match.Groups[2].Value : throw new FormatException("Could not extract video ID from URL");
        }

        public Twitch(ITwitchCredentials credentials) {
            _credentials = credentials;
        }

        public async Task<TimeSpan?> GetDurationAsync(string id) {
            if (_credentials == null)
                return null;

            var req = WebRequest.CreateHttp($"https://api.twitch.tv/helix/videos?id={id}");
            req.Method = "GET";
            req.Headers.Add("Authorization", "Bearer " + await _credentials.GetAccessTokenAsync());
            using (var resp = await req.GetResponseAsync()) {
                using (var sr = new StreamReader(resp.GetResponseStream())) {
                    string json = await sr.ReadToEndAsync();
                    var obj = JsonConvert.DeserializeAnonymousType(json, new {
                        data = new[] {
                            new {
                                duration = ""
                            }
                        }
                    });
                    return XmlConvert.ToTimeSpan("PT" + obj.data?.Select(x => x.duration.ToUpperInvariant())?.FirstOrDefault());
                }
            }
        }

        public async Task<TimeSpan?> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            return REGEX_ID.IsMatch(url.AbsoluteUri)
                ? await GetDurationAsync(GetVideoIdFromUrl(url))
                : null;
        }

        async Task<TimeSpan?> IDurationProvider.GetDurationAsync(Uri originalLocation, HttpContent httpContent) {
            return await GetDurationAsync(originalLocation);
        }
    }
}
