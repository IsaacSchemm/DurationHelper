using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DurationHelper {
    public static class MP4 {
        /// <summary>
        /// Find the duration of an MP4 file from a Stream.
        /// </summary>
        /// <param name="stream">A readable Stream containing an MP4 file</param>
        /// <returns>The duration, or null if no duration info was found</returns>
        /// <exception cref="ArgumentNullException">stream is null.</exception>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="ObjectDisposedException">The stream has been disposed.</exception>
        /// <exception cref="InvalidOperationException">The stream is currently in use by a previous read operation.</exception>
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

        /// <summary>
        /// Find the duration of an MP4 file from a byte array.
        /// </summary>
        /// <param name="data">A byte array containing an MP4 file</param>
        /// <returns>The duration, or null if no duration info was found</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        public static async Task<TimeSpan?> GetDurationAsync(byte[] data) {
            if (data == null) throw new ArgumentNullException();
            using (MemoryStream ms = new MemoryStream(data, false)) {
                return await GetDurationAsync(ms);
            }
        }

        /// <summary>
        /// Find the duration of an MP4 file from a URL.
        /// </summary>
        /// <param name="url">A public URL pointing to an MP4 file</param>
        /// <param name="range">The maximum number of bytes to download, starting from the beginning of the file</param>
        /// <returns>The duration, or null if no duration info was found</returns>
        /// <exception cref="ArgumentNullException">url is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">range is less than 0.</exception>
        /// <exception cref="WebException">The HTTP request failed or returned a status outside of the 200 range.</exception>
        /// <exception cref="ProtocolViolationException">The HTTP response did not include a response stream.</exception>
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
