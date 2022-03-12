using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DurationHelper.Tests {
    [TestClass]
    public class DurationHelperTests {
        private async Task<bool> Exists(string url) {
            try {
                var req = WebRequest.Create(url);
                req.Method = "HEAD";
                req.Timeout = 3000;
                using (var resp = await req.GetResponseAsync()) {
                    if ((resp as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound) {
                        return true;
                    }
                }
            } catch (Exception) { }
            return false;
        }

        private const string youTubeKey = null;

        private async Task TestUrl(double? expected, string url) {
            TimeSpan? duration = await Duration.GetAsync(new Uri(url), youTubeKey: youTubeKey);
            if (expected is double d) {
                if (duration is TimeSpan ts) {
                    Assert.AreEqual(d, ts.TotalSeconds, 0.1);
                } else if (await Exists(url)) {
                    Assert.Inconclusive();
                } else {
                    Assert.IsNotNull(duration);
                }
            } else {
                Assert.IsNull(duration);
            }
        }

        [TestMethod]
        public async Task TestYouTube() {
            if (youTubeKey == null) Assert.Inconclusive();
            await TestUrl(424, "https://youtu.be/bMZIG-iMS9k");
            await TestUrl(424, "https://www.youtube.com/watch?v=bMZIG-iMS9k");
            await TestUrl(424, "https://www.youtube.com/embed/bMZIG-iMS9k");
            await TestUrl(null, "https://www.example.com/?v=bMZIG-iMS9k");
        }

        [TestMethod]
        public async Task TestMP4_1() {
            await TestUrl(596.474195, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4");
        }

        [TestMethod]
        public async Task TestMP4_2() {
            await TestUrl(653.804263, "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4");
        }

        [TestMethod]
        public async Task TestHLS_1() {
            await TestUrl(210, "https://bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8");
        }

        [TestMethod]
        public async Task TestHLS_2() {
            await TestUrl(600, "https://devstreaming-cdn.apple.com/videos/streaming/examples/img_bipbop_adv_example_ts/master.m3u8");
        }

        [TestMethod]
        public async Task TestHLS_3() {
            await TestUrl(null, "https://cph-p2p-msl.akamaized.net/hls/live/2000341/test/master.m3u8");
        }

        [TestMethod]
        public async Task TestVimeo_1() {
            await TestUrl(57, "https://vimeo.com/241309009");
        }

        [TestMethod]
        public async Task TestVimeo_2() {
            await TestUrl(536, "https://vimeo.com/181964440");
        }

        [TestMethod]
        public async Task TestDailymotion_1() {
            await TestUrl(56, "http://www.dailymotion.com/video/x6h5cqp");
        }

        [TestMethod]
        public async Task TestDailymotion_2() {
            await TestUrl(181, "http://www.dailymotion.com/video/x6gxyji?playlist=x5np2u");
        }

        [TestMethod]
        public async Task TestSoundCloud_1() {
            await TestUrl(280, "https://soundcloud.com/jamieirl/pass-feat-marcy-nabors");
        }
    }
}
