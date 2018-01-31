using DurationHelper.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DurationHelper.Providers {
    public static class Vimeo {
        /// <summary>
        /// Find the duration of a Vimeo video by its URL.
        /// </summary>
        /// <param name="url">The URL of the video</param>
        /// <returns>The video duration</returns>
        /// <exception cref="ArgumentNullException">url is null.</exception>
        /// <exception cref="WebException">The Vimeo oEmbed request failed or returned a status outside of the 200 range.</exception>
        /// <exception cref="JsonReaderException">The Vimeo oEmbed response could not be deserialized.</exception>
        /// <exception cref="VideoNotFoundException">No video was found at the given URL.</exception>
        public static async Task<TimeSpan> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            HttpWebRequest req = WebRequest.CreateHttp($"https://vimeo.com/api/oembed.json?url={WebUtility.UrlEncode(url.AbsoluteUri)}");
            req.UserAgent = Shared.UserAgent;
            try {
                using (var resp = await req.GetResponseAsync()) {
                    using (var sr = new StreamReader(resp.GetResponseStream())) {
                        var obj = JsonConvert.DeserializeAnonymousType(await sr.ReadToEndAsync(), new {
                            duration = 0.0
                        });
                        return TimeSpan.FromSeconds(obj.duration);
                    }
                }
            } catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) {
                throw new VideoNotFoundException(ex);
            }
        }
    }
}
