using System;

namespace AsyncStreams
{
    [Serializable]
    public class DuplicateMovieException : Exception
    {
        public DuplicateMovieException() { }
        public DuplicateMovieException(string message) : base(message) { }
        public DuplicateMovieException(string message, Exception inner) : base(message, inner) { }
        protected DuplicateMovieException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
