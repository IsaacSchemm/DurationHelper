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
    public static class SoundCloud {
        private readonly static Regex REGEX_ID = new Regex(@"soundcloud\.com\/(?:([\w-]+)\/(sets\/)?)([\w-]+)", RegexOptions.IgnoreCase);
        private readonly static Regex DURATION = new Regex(@"<meta +itemprop=['""]duration['""] +content=['""](P[A-Z0-9]+)['""]");

        public static async Task<TimeSpan?> GetDurationAsync(Uri url) {
            var req = WebRequest.CreateHttp(url);
            req.Method = "GET";
            req.UserAgent = Shared.UserAgent;
            try {
                using (var resp = await req.GetResponseAsync())
                using (var sr = new StreamReader(resp.GetResponseStream())) {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null) {
                        var match = DURATION.Match(line);
                        if (match.Success) {
                            return XmlConvert.ToTimeSpan(match.Groups[1].Value);
                        }
                    }
                    return null;
                }
            } catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) {
                throw new VideoNotFoundException(ex);
            }
        }
    }
}
