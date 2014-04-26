using System;

namespace DataAccessLayer
{
    ///<author>
    /// Jacob Cholewa (jbec@itu.dk)
    /// </author>
    public class InternalDbException : Exception
    {
        public InternalDbException() { }
        public InternalDbException(string message) : base(message) { }
        public InternalDbException(string message, Exception inner) : base(message, inner) { }
    }
}
