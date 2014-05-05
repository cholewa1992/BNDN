using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Exceptions
{
    public class InvalidUserException : Exception
    {
        public InvalidUserException()
        {
            
        }

        public InvalidUserException(string message) : base(message)
        {
            
        }
    }
}
