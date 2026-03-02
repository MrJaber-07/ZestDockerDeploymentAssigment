
using System.Globalization;


namespace Application.Exceptions
{
    public class ExceptionSummary
    {
        public string UserMessage { get; set; }
        public string ValidationMessage { get; set; }
        public string SupportMessages { get; set; }
        public string ReferenceId { get; set; }
        public string TimeStamp => DateTime.Now.ToString(CultureInfo.InvariantCulture);
    }
}
