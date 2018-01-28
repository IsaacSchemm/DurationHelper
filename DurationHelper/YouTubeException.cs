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
        public readonly IReadOnlyList<string> Reasons;
        public readonly HttpStatusCode? StatusCode;
        
        public YouTubeException(string message) : base(message) { }

        private YouTubeException(HttpStatusCode code, string message, IEnumerable<string> reasons) : base(message) {
            StatusCode = code;
            Reasons = reasons.ToList();
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
                    o?.error?.message ?? "A YouTube Data API error occurred",
                    o?.error?.errors?.Select(x => x.reason));
            }
        }
    }
}
