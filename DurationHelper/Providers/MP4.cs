using ISchemm.DurationFinder;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DurationHelper.Providers {
    [Obsolete]
    public static class MP4 {
        public static async Task<TimeSpan?> GetDurationAsync(Stream stream) {
            return await new MP4DurationProvider().GetDurationAsync(new StreamDataSource(stream));
        }

        public static async Task<TimeSpan?> GetDurationAsync(byte[] data) {
            if (data == null) throw new ArgumentNullException();
            using (MemoryStream ms = new MemoryStream(data, false)) {
                return await GetDurationAsync(ms);
            }
        }

        public static async Task<TimeSpan?> GetDurationAsync(Uri url, int range = 256) {
            return await new MP4DurationProvider().GetDurationAsync(url);
        }
    }
}
