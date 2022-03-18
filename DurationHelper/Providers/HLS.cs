using ISchemm.DurationFinder;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DurationHelper.Providers {
    public static class HLS {
        [Obsolete]
        public static async Task<TimeSpan?> GetPlaylistDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            return await new HlsDurationProvider().GetDurationAsync(url);
        }

        [Obsolete]
        public static async Task<TimeSpan?> GetChunklistDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            return await HlsDurationProvider.ChunklistProvider.GetDurationAsync(url);
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
