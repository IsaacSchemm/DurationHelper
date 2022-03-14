using ISchemm.DurationFinder;
using System;
using System.Threading.Tasks;

namespace DurationHelper.Providers {
    [Obsolete]
    public static class Vimeo {
        public static async Task<TimeSpan> GetDurationAsync(Uri url) {
            return (await new OEmbedDurationProvider().GetDurationAsync(url)).Value;
        }
    }
}
