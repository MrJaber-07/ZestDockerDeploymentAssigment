using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{

    [Serializable]
    public abstract class HCBException : Exception
    {
        public Guid ReferenceId { get; }
        public ExceptionCodes ExceptionCode { get; set; }
        public HCBException(string message, Exception innerException = null, ExceptionCodes exceptionCode = ExceptionCodes.Default)
            : base(message, innerException)
        {
            ExceptionCode = exceptionCode;
            ReferenceId = (innerException as HCBException)?.ReferenceId ?? Guid.NewGuid();
        }

        protected HCBException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
            ExceptionCode = (ExceptionCodes)serializationInfo.GetInt32("ExceptionCode");
            ReferenceId = Guid.Parse(serializationInfo.GetString("ReferenceId"));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ExceptionCode", ExceptionCode);
            info.AddValue("ReferenceId", ReferenceId);
        }
    }
}
