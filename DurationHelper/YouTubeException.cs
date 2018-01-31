using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DurationHelper {
    public class YouTubeException : Exception {
        public readonly HttpStatusCode? StatusCode;
        public readonly string ResponseText;
        public readonly IReadOnlyList<string> Reasons;
        
        public YouTubeException(HttpStatusCode code, string response, string message, IEnumerable<string> reasons) : base(message) {
            StatusCode = code;
            ResponseText = response;
            Reasons = reasons?.ToList();
        }

        public static async Task<YouTubeException> FromHttpWebResponseAsync(HttpWebResponse r) {
            using (var sr = new StreamReader(r.GetResponseStream())) {
                string json = await sr.ReadToEndAsync();
                var o = JsonConvert.DeserializeAnonymousType(json, new {
                    error = new {
                        errors = new[] {
                                new {
                                    domain = "",
                                    reason = "",
                                    message = "",
                                    extendedHelp = ""
                                }
                            },
                        code = 0,
                        message = ""
                    }
                });
                return new YouTubeException(
                    r.StatusCode,
                    json,
                    o?.error?.message ?? "A YouTube Data API error occurred",
                    o?.error?.errors?.Select(x => x.reason));
            }
        }
    }
}
