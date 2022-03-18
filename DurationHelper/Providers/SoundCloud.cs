using ISchemm.DurationFinder;
using System;
using System.Threading.Tasks;

namespace DurationHelper.Providers {
    [Obsolete]
    public static class SoundCloud {
        public static async Task<TimeSpan?> GetDurationAsync(Uri url) {
            return await new SchemaOrgDurationProvider().GetDurationAsync(url);
        }
    }
}
