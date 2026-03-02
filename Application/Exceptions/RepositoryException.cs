using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    [Serializable]
    public class RepositoryException : HCBException
    {
        public RepositoryException(string message, Exception innerException = null, ExceptionCodes exceptionCodes = ExceptionCodes.Default)
            : base($"RepositoryException: {message}", innerException, exceptionCodes)
        {
        }
    }
}
