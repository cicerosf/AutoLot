using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace AutoLot.Dal.Exceptions
{
    public class CustomRetryLimitExceededException : Exception
    {
        public CustomRetryLimitExceededException() { }
    
        public CustomRetryLimitExceededException(string message) : base(message) { }

        public CustomRetryLimitExceededException(string message, RetryLimitExceededException innerMessage)
            : base(message, innerMessage)
        {

        }

    }
}
