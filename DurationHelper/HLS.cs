using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DurationHelper {
    public static class HLS {
        /// <summary>
        /// Find the duration of an HLS playlist (if any.)
        /// </summary>
        /// <param name="url">The URL of the playlist file</param>
        /// <returns>The duration, or null if the file refers to a live stream</returns>
        /// <exception cref="ArgumentNullException">url is null.</exception>
        /// <exception cref="WebException">The HTTP request failed or returned a status outside of the 200 range.</exception>
        /// <exception cref="ProtocolViolationException">The HTTP response did not include a response stream.</exception>
        /// <exception cref="FormatException">The chunklist contains an invalid chunk length.</exception>
        /// <exception cref="OverflowException">The chunklist contains a chunk length that is out of range.</exception>
        public static async Task<TimeSpan?> GetPlaylistDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.UserAgent = Shared.UserAgent;
            using (var resp = await req.GetResponseAsync()) {
                using (var sr = new StreamReader(resp.GetResponseStream())) {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null) {
                        if (!line.StartsWith("#") && line.EndsWith(".m3u8")) {
                            // get chunklist
                            return await GetChunklistDurationAsync(new Uri(url, line));
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Find the duration of an HLS chunklist (if any.)
        /// </summary>
        /// <param name="url">The URL of the chunklist file</param>
        /// <returns>The duration, or null if the file refers to a live stream</returns>
        /// <exception cref="ArgumentNullException">url is null.</exception>
        /// <exception cref="WebException">The HTTP request failed or returned a status outside of the 200 range.</exception>
        /// <exception cref="ProtocolViolationException">The HTTP response did not include a response stream.</exception>
        /// <exception cref="FormatException">The chunklist contains an invalid chunk length.</exception>
        /// <exception cref="OverflowException">The chunklist contains a chunk length that is out of range.</exception>
        public static async Task<TimeSpan?> GetChunklistDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            TimeSpan ts = TimeSpan.Zero;

            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.UserAgent = Shared.UserAgent;
            using (var resp = await req.GetResponseAsync()) {
                using (var sr = new StreamReader(resp.GetResponseStream())) {
                    return await GetChunklistDurationAsync(sr);
                }
            }
        }

        /// <summary>
        /// Find the duration of an HLS chunklist (if any.)
        /// </summary>
        /// <param name="reader">A TextReader containing the contents of the chunklist file</param>
        /// <returns>The duration, or null if the file refers to a live stream</returns>
        /// <exception cref="ArgumentNullException">reader is null.</exception>
        /// <exception cref="FormatException">The chunklist contains an invalid chunk length.</exception>
        /// <exception cref="OverflowException">The chunklist contains a chunk length that is out of range.</exception>
        public static async Task<TimeSpan?> GetChunklistDurationAsync(TextReader reader) {
            if (reader == null) throw new ArgumentNullException();

            TimeSpan ts = TimeSpan.Zero;
            
            string line;
            while ((line = await reader.ReadLineAsync()) != null) {
                if (line.StartsWith("#EXTINF:")) {
                    string[] split = line.Substring("#EXTINF:".Length).Split(',');
                    ts += TimeSpan.FromSeconds(double.Parse(split[0]));
                } else if (line == "#EXT-X-ENDLIST") {
                    return ts;
                }
            }

            return null;
        }
    }
}
