﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DurationHelper {
    public static class Duration {
        /// <summary>
        /// Find the duration of a video from its URL. This function can throw
        /// several exceptions if the process fails, but it can also return
        /// null if the process succeeds and the duration simply can't be
        /// determined.
        /// </summary>
        /// <param name="url">A public URL pointing to an MP4, HLS, youTube, or Vimeo video</param>
        /// <returns>The duration, or null if the duration could not be determined.</returns>
        /// <exception cref="ArgumentNullException">url is null.</exception>
        /// <exception cref="WebException">An HTTP request failed or returned a status outside of the 200 range.</exception>
        /// <exception cref="ProtocolViolationException">An HTTP response did not include a response stream.</exception>
        /// <exception cref="FormatException">The duration could not be parsed.</exception>
        /// <exception cref="OverflowException">An HLS chunklist contains a chunk length that is out of range.</exception>
        /// <exception cref="JsonReaderException">A JSON response could not be deserialized.</exception>
        /// <exception cref="YouTubeException">A YouTube API error occurred.</exception>
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
