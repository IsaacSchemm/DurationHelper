using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DurationHelper {
    public static class MP4 {
        public static async Task<TimeSpan?> GetDurationAsync(Stream stream) {
            if (stream == null) throw new ArgumentNullException();
            byte[] searchFor = Encoding.UTF8.GetBytes("mvhd");
            byte[] b = new byte[1];
            int lookingFor = 0;
            while (true) {
                int r = await stream.ReadAsync(b, 0, 1);
                if (r < 1) break;

                if (b[0] != searchFor[lookingFor]) {
                    lookingFor = 0;
                    continue;
                }

                lookingFor++;
                if (lookingFor == searchFor.Length) {
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

        public static async Task<TimeSpan?> GetDurationAsync(Uri url, int range = 256) {
            if (url == null) throw new ArgumentNullException();

            HttpWebRequest req = WebRequest.CreateHttp(url);
            req.UserAgent = Shared.UserAgent;
            req.AddRange(0, range);
            using (var resp = await req.GetResponseAsync()) {
                using (var stream = resp.GetResponseStream()) {
                    return await GetDurationAsync(stream);
                }
            }
        }
    }
}
