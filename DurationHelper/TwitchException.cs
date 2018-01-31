using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DurationHelper {
    public class TwitchException : Exception {
        public readonly HttpStatusCode? StatusCode;
        public readonly string ResponseText;

        public TwitchException(HttpStatusCode code, string response, string message) : base(message) {
            StatusCode = code;
            ResponseText = response;
        }

        public static async Task<TwitchException> FromHttpWebResponseAsync(HttpWebResponse r) {
            using (var sr = new StreamReader(r.GetResponseStream())) {
                string json = await sr.ReadToEndAsync();
                var o = JsonConvert.DeserializeAnonymousType(json, new {
                    error = "",
                    status = 0,
                    message = ""
                });
                return new TwitchException(
                    r.StatusCode,
                    json,
                    o?.message ?? "A Twitch API error occurred");
            }
        }
    }
}
