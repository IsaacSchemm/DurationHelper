using ISchemm.DurationFinder;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DurationHelper.Providers {
    public class YouTube {
        private readonly static Regex REGEX_ID = new Regex(@"(?:(?:v|vi|be|videos|embed)/(?!videoseries)|(?:v|ci)=)([\w-]{11})", RegexOptions.IgnoreCase);

        public YouTube(string key) { }

        public static string GetIdFromUrl(Uri url) {
            var match = REGEX_ID.Match(url.AbsoluteUri);
            return match.Success ? match.Groups[1].Value : throw new FormatException("Could not extract video ID from URL");
        }

        public async Task<TimeSpan> GetDurationAsync(string id) {
            if (id == null) throw new ArgumentNullException();

            return (await new SchemaOrgDurationProvider().GetDurationAsync(new Uri($"https://www.youtube.com/watch?v={Uri.EscapeDataString(id)}"))).Value;
        }

        [Obsolete]
        public async Task<TimeSpan?> GetDurationAsync(Uri url) {
            if (url == null) throw new ArgumentNullException();

            return await new SchemaOrgDurationProvider().GetDurationAsync(url);
        }
    }
}
