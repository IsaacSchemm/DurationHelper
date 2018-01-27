using System;
using System.Collections.Generic;
using System.Text;

namespace DurationHelper {
    public class YouTubeException : Exception {
        public YouTubeException(string message)
            : base(message) { }

        public YouTubeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
