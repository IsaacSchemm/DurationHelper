using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DurationHelper {
    public class MP4 {
        public static async Task<TimeSpan?> GetDurationAsync(Stream stream) {
            if (stream == null) throw new ArgumentNullException();
            byte[] searchFor = Encoding.UTF8.GetBytes("mvhd");
            byte[] tag = new byte[4];
            while (true) {
                int r = await stream.ReadAsync(tag, 0, 4);
                if (r < 4) break;

                if (tag.SequenceEqual(searchFor)) {
                    for (int i = 0; i < 12; i++) stream.ReadByte();

                    uint timeScale = 0;
                    for (int i = 0; i < 4; i++) {
                        timeScale <<= 8;
                        timeScale |= (byte)stream.ReadByte();
                    }
                    uint duration = 0;
                    for (int i = 0; i < 4; i++) {
                        duration <<= 8;
                        duration |= (byte)stream.ReadByte();
                    }
                    return TimeSpan.FromSeconds((double)duration / timeScale);
                }
            }
            return null;
        }
        
        public static async Task<TimeSpan?> GetDurationAsync(byte[] data) {
            if (data == null) throw new ArgumentNullException();
            using (MemoryStream ms = new MemoryStream(data, false)) {
                return await GetDurationAsync(ms);
            }
        }

        public static async Task<TimeSpan?> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.UserAgent = $"DurationHelper/1.0 (https://github.com/IsaacSchemm/DurationHelper)";
            req.AddRange(0, 256);
            using (var resp = await req.GetResponseAsync()) {
                using (var stream = resp.GetResponseStream()) {
                    return await GetDurationAsync(stream);
                }
            }
        }
    }
}
