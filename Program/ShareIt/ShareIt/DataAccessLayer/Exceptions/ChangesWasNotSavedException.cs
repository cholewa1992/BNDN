using System;

namespace DataAccessLayer
{
    /// <author>
    /// Jacob Cholewa (jbec@itu.dk)
    /// </author>
    public class ChangesWasNotSavedException : Exception
    {
        public ChangesWasNotSavedException() { }
        public ChangesWasNotSavedException(string message) : base(message) { }
        public ChangesWasNotSavedException(string message, Exception inner) : base(message, inner) { }
    }
}
