using System;
using System.Collections.Generic;
using System.Text;

namespace DurationHelper.Exceptions {
    public class VideoNotFoundException : Exception {
        public VideoNotFoundException() : base("Video not found") { }

        public VideoNotFoundException(Exception innerException) : base("Video not found", innerException) { }
    }
}
