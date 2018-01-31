using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DurationHelper {
    public class YouTubeAPIException : Exception {
        public readonly HttpStatusCode? StatusCode;
        public readonly string ResponseText;
        public readonly IReadOnlyList<string> Reasons;
        
        public YouTubeAPIException(HttpStatusCode code, string response, string message, IEnumerable<string> reasons) : base(message) {
            StatusCode = code;
            ResponseText = response;
            Reasons = reasons?.ToList() ?? new List<string>(0);
        }

        public static async Task<Exception> FromHttpWebResponseAsync(HttpWebResponse r) {
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

                var newEx = new YouTubeAPIException(
                    r.StatusCode,
                    json,
                    o?.error?.message ?? "A YouTube Data API error occurred",
                    o?.error?.errors?.Select(x => x.reason));

                if (newEx.Reasons.Contains("quotaExceeded")) {
                    throw new TooManyRequestsException("YouTube API request limit exceeded", newEx);
                } else {
                    throw newEx;
                }
            }
        }
    }
}
