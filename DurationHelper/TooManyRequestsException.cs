using System;
using System.Collections.Generic;
using System.Text;

namespace DurationHelper
{
    public class TooManyRequestsException : Exception {
        public TooManyRequestsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
