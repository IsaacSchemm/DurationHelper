using System;
using System.Collections.Generic;
using System.Text;

namespace DurationHelper {
    public class YouTubeURLException : Exception {
        public YouTubeURLException() : base("Invalid YouTube URL") { }
    }
}
