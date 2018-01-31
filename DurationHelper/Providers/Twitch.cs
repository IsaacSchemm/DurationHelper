using DurationHelper.Credentials;
using DurationHelper.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DurationHelper.Providers {
    public class Twitch {
        private ITwitchCredentials _credentials;

        private readonly static Regex REGEX_ID = new Regex(@"(clips\.)?twitch\.tv/(?:(?:videos/(\d+))|(\w+))?", RegexOptions.IgnoreCase);

        /// <summary>
        /// Extract a video ID from a Twitch URL.
        /// </summary>
        /// <param name="url">The Twitch URL<param>
        /// <returns>The video ID</returns>
        /// <exception cref="VideoURLParseException">The URL format was not recognized as a Twitch URL.</exception>
        public static string GetVideoIdFromUrl(Uri url) {
            var match = REGEX_ID.Match(url.AbsoluteUri);
            return match.Success ? match.Groups[2].Value : throw new VideoURLParseException();
        }

        public Twitch(ITwitchCredentials credentials) {
            _credentials = credentials;
        }

        public async Task<TimeSpan?> GetDurationAsync(string id) {
            var req = WebRequest.CreateHttp($"https://api.twitch.tv/helix/videos?id={id}");
            req.Method = "GET";
            req.Headers.Add("Authorization", "Bearer " + await _credentials.GetAccessTokenAsync());
            try { 
                using (var resp = await req.GetResponseAsync()) {
                    using (var sr = new StreamReader(resp.GetResponseStream())) {
                        string json = await sr.ReadToEndAsync();
                        var obj = JsonConvert.DeserializeAnonymousType(json, new {
                            data = new [] {
                                new {
                                    duration = ""
                                }
                            }
                        });
                        return XmlConvert.ToTimeSpan("PT" + obj.data?.Select(x => x.duration.ToUpperInvariant())?.FirstOrDefault());
                    }
                }
            } catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) {
                throw new VideoNotFoundException(ex);
            } catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == (HttpStatusCode)429) {
                throw new TooManyRequestsException("Twitch API request limit exceeded", ex);
            }
        }
        
        public async Task<TimeSpan?> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            return await GetDurationAsync(GetVideoIdFromUrl(url));
        }
    }
}
