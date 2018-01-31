using System;
using System.Collections.Generic;
using System.Text;

namespace DurationHelper.Exceptions {
    public class VideoURLParseException : Exception {
        public VideoURLParseException() : base("Invalid YouTube URL") { }
    }
}
