using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DurationHelper {
    public static class Duration {
        public static async Task<TimeSpan?> GetAsync(Uri url, string youTubeKey = null) {
            if (url == null) throw new ArgumentNullException();

            return await GetByNameAsync(url, youTubeKey: youTubeKey) ?? await GetByContentTypeAsync(url);
        }

        private static async Task<TimeSpan?> GetByNameAsync(Uri url, string youTubeKey = null) {
            if (url == null) throw new ArgumentNullException();

            if (url.AbsolutePath.EndsWith(".mp4", StringComparison.InvariantCultureIgnoreCase)) {
                // Assume MP4
                return await MP4.GetDurationAsync(url);
            } else if (url.AbsolutePath.EndsWith(".m3u8", StringComparison.InvariantCultureIgnoreCase)) {
                // Assume HLS
                return await HLS.GetPlaylistDurationAsync(url);
            } else if (url.Authority.EndsWith("vimeo.com")) {
                return await Vimeo.GetDurationAsync(url);
            } else if (url.Authority.EndsWith("youtube.com") || url.Authority.EndsWith("youtu.be")) {
                return youTubeKey == null
                    ? null
                    : await new YouTube(youTubeKey).GetDurationAsync(url);
            } else {
                return null;
            }
        }

        private static async Task<TimeSpan?> GetByContentTypeAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            // Get content type
            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.UserAgent = Shared.UserAgent;
            req.Method = "HEAD";
            using (var resp = await req.GetResponseAsync()) {
                switch (resp.ContentType) {
                    case "application/vnd.apple.mpegurl":
                        return await HLS.GetPlaylistDurationAsync(url);
                    case "video/mp4":
                        return await MP4.GetDurationAsync(url);
                    default:
                        return null;
                }
            }
        }
    }
}
