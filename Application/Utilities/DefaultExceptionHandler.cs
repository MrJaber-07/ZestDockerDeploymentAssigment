using Application.Common;
using Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utilities
{
    public static class DefaultExceptionHandler
    {
        public static string DefaultErrorMessage { get; set; } = "An error occured while performing requested operation.";

        static DefaultExceptionHandler()
        {

        }
        public static HCBException HandleException<TTargetException>(Exception exception, string message = null, ExceptionCodes exceptionCode = ExceptionCodes.Default) where TTargetException : HCBException
        {
            if (exception is AggregateException && exception.InnerException != null)
            {
                return HandleException<TTargetException>(exception.InnerException, message ?? DefaultErrorMessage, exceptionCode);
            }

            if (exception is HCBException nextEnsignException)
            {
                return nextEnsignException;
            }

            return (TTargetException)Activator.CreateInstance(typeof(TTargetException), message ?? DefaultErrorMessage, exception, exceptionCode);
        }
    }
}
