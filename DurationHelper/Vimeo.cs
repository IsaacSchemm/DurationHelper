using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DurationHelper {
    public static class Vimeo {
        public static async Task<TimeSpan> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            HttpWebRequest req = WebRequest.CreateHttp($"https://vimeo.com/api/oembed.json?url={WebUtility.UrlEncode(url.AbsoluteUri)}");
            req.UserAgent = Shared.UserAgent;
            using (var resp = await req.GetResponseAsync()) {
                using (var sr = new StreamReader(resp.GetResponseStream())) {
                    var obj = JsonConvert.DeserializeAnonymousType(await sr.ReadToEndAsync(), new {
                        duration = 0
                    });
                    return TimeSpan.FromSeconds(obj.duration);
                }
            }
        }
    }
}
