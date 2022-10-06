﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop
{
    internal class InvalidHashException : Exception
    {
        // Constructor
        public InvalidHashException()
        {

        }

        public InvalidHashException(string message)
            : base(message)
        {

        }

        public InvalidHashException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
