using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    [Serializable]
    public class BaseServiceException : HCBException
    {
        public BaseServiceException(string message, Exception innerException = null, ExceptionCodes exceptionCodes = ExceptionCodes.Default)
            : base($"Service Exception: {message}", innerException, exceptionCodes)
        {
        }

        public BaseServiceException(string message, ExceptionCodes exceptionCodes = ExceptionCodes.Default)
           : base($"Service Exception: {message}", null, exceptionCodes)
        {
        }
    }
}
