using System;
using System.Collections.Generic;
using System.Text;

namespace DurationHelper {
    public class VideoURLParseException : Exception {
        public VideoURLParseException() : base("Invalid YouTube URL") { }
    }
}
