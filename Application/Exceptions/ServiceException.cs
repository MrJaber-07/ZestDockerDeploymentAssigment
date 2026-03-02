using Application.Common;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{

    public abstract class ServiceException
    {
        protected HCBException HandleException(Exception exception, string message = null, ExceptionCodes exceptionCode = ExceptionCodes.Default)
        {
            return DefaultExceptionHandler.HandleException<BaseServiceException>(exception, message, exceptionCode);
        }
    }
}
