﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Exceptions
{
    public class InvalidClientException : Exception
    {
        public InvalidClientException()
        {
            
        }

        public InvalidClientException(string message): base(message)
        {
            
        }
    }
}
