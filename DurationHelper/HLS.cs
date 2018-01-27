using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DurationHelper {
    public static class HLS {
        public static async Task<TimeSpan?> GetPlaylistDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.UserAgent = $"DurationHelper/1.0 (https://github.com/IsaacSchemm/DurationHelper)";
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

        public static async Task<TimeSpan?> GetChunklistDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            TimeSpan ts = TimeSpan.Zero;

            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.UserAgent = $"DurationHelper/1.0 (https://github.com/IsaacSchemm/DurationHelper)";
            using (var resp = await req.GetResponseAsync()) {
                using (var sr = new StreamReader(resp.GetResponseStream())) {
                    return await GetChunklistDurationAsync(sr);
                }
            }
        }

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
