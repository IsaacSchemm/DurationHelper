using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DurationHelper {
    public class YouTube {
        private string _key;

        public YouTube(string key) {
            _key = key ?? throw new ArgumentNullException();
        }

        public static YouTubeUrlInfo ParseUrl(Uri url) {
            string getQueryVariable(Uri u, string variable) {
                var query = u.Query;
                if (query.StartsWith("?")) query = query.Substring(1);
                var vars = query.Split('&');
                for (var i = 0; i < vars.Length; i++) {
                    var pair = vars[i].Split('=');
                    if (WebUtility.UrlDecode(pair[0]) == variable) {
                        return WebUtility.UrlDecode(pair[1]);
                    }
                }
                return null;
            }

            int? asNumber(string s) =>
                s != null && int.TryParse(s, out int i)
                    ? i
                    : (int?)null;

            if (url.Authority.EndsWith("youtube.com")) {
                var v = getQueryVariable(url, "v");
                if (v != null) {
                    return new YouTubeUrlInfo {
                        id = v,
                        start = asNumber(getQueryVariable(url, "start")),
                        end = asNumber(getQueryVariable(url, "end")),
                        autoplay = getQueryVariable(url, "autoplay") == "1",
                    };
                } else if (url.AbsolutePath.StartsWith("/embed/")) {
                    return new YouTubeUrlInfo {
                        id = url.AbsolutePath.Substring("/embed/".Length),
                        start = asNumber(getQueryVariable(url, "start")),
                        end = asNumber(getQueryVariable(url, "end")),
                        autoplay = getQueryVariable(url, "autoplay") == "1",
                    };
                }
            } else if (url.Authority.EndsWith("youtu.be")) {
                var t = getQueryVariable(url, "t");
                int sec = 0;
                if (t != null) {
                    var h = Regex.Match(url.Query, "([0-9]+)h");
                    if (h.Success) {
                        sec += int.Parse(h.Groups[1].Value) * 3600;
                    }
                    var m = Regex.Match(url.Query, "([0-9]+)m");
                    if (m != null) {
                        sec += int.Parse(m.Groups[1].Value) * 60;
                    }
                    var s = Regex.Match(url.Query, "([0-9]+)s");
                    if (s != null) {
                        sec += int.Parse(m.Groups[1].Value);
                    }
                }
                return new YouTubeUrlInfo {
                    id = url.AbsolutePath.Substring(1),
                    start = sec,
                    end = null,
                    autoplay = false
                };
            }
            throw new YouTubeException("Invalid YouTube URL");
        }

        public async Task<TimeSpan> GetDurationAsync(string id) {
            if (id == null) throw new ArgumentNullException();

            HttpWebRequest req = WebRequest.CreateHttp($"https://www.googleapis.com/youtube/v3/videos?part=contentDetails&id={WebUtility.UrlEncode(id)}&key={WebUtility.UrlEncode(_key)}");
            req.UserAgent = Shared.UserAgent;
            try {
                using (var resp = await req.GetResponseAsync()) {
                    using (var sr = new StreamReader(resp.GetResponseStream())) {
                        var obj = JsonConvert.DeserializeAnonymousType(await sr.ReadToEndAsync(), new {
                            items = new[] {
                                new {
                                    id = "",
                                    contentDetails = new {
                                        duration = ""
                                    }
                                }
                            }
                        });
                        return XmlConvert.ToTimeSpan(obj.items[0].contentDetails.duration);
                    }
                }
            } catch (WebException ex) when (ex.Response is HttpWebResponse r) {
                throw await YouTubeException.FromHttpWebResponseAsync(r);
            }
        }

        public async Task<TimeSpan?> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();
            
            return await GetDurationAsync(ParseUrl(url).id);
        }
    }
}
