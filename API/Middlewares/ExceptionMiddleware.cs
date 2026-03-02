using Application.Common;
using Application.Exceptions;
using Newtonsoft.Json;

namespace API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _next = next;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, _webHostEnvironment);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, IWebHostEnvironment webHostEnvironment)
        {
            if (context.Response.HasStarted)
                throw exception;

            string reference = "unavailable";
            if (exception.InnerException is HCBException ex && ex.ReferenceId != Guid.Empty)
            {
                reference = ex.ReferenceId.ToString();
            }
            else if (exception is HCBException hex && hex.ReferenceId != Guid.Empty)
            {
                reference = hex.ReferenceId.ToString();
            }

            var exceptionSummary = new ExceptionSummary
            {
                UserMessage = "There was an error processing your request.",
                SupportMessages = $"There reference for this exception is {reference} ",
                ReferenceId = reference
            };

            var errorStatusCode = StatusCodes.Status500InternalServerError;
            var appException = exception as HCBException;
            if (appException != null)
            {
                switch (appException.ExceptionCode)
                {
                    case ExceptionCodes.Validation:
                        errorStatusCode = StatusCodes.Status400BadRequest;
                        break;
                    case ExceptionCodes.Restriction:
                    case ExceptionCodes.UnAuthorized:
                        errorStatusCode = StatusCodes.Status403Forbidden;
                        break;
                    case ExceptionCodes.ItemNotFound:
                        errorStatusCode = StatusCodes.Status404NotFound;
                        break;
                    default:
                        break;
                }
            }
            else
            {

                switch (exception)
                {
                    case ArgumentNullException _:
                    case ArgumentException _:
                        errorStatusCode = StatusCodes.Status400BadRequest;
                        break;
                    case UnauthorizedAccessException _:
                        errorStatusCode = StatusCodes.Status401Unauthorized;
                        break;
                    case NotImplementedException _:
                        errorStatusCode = StatusCodes.Status501NotImplemented;
                        break;
                    case NotSupportedException _:
                        errorStatusCode = StatusCodes.Status405MethodNotAllowed;
                        break;
                    case KeyNotFoundException _:
                        errorStatusCode = StatusCodes.Status404NotFound;
                        break;
                    default:
                        errorStatusCode = StatusCodes.Status500InternalServerError;
                        break;
                }
            }

            // Add validation message for custom exceptions
            var current = exception;
            while (current != null)
            {
                var exceptionCode = (current as HCBException)?.ExceptionCode;
                if (exceptionCode == ExceptionCodes.Validation ||
                    exceptionCode == ExceptionCodes.Restriction ||
                    exceptionCode == ExceptionCodes.Operation ||
                    exceptionCode == ExceptionCodes.ItemNotFound ||
                    exceptionCode == ExceptionCodes.UnAuthorized)
                {
                    exceptionSummary.ValidationMessage = $"{current.Message}";
                    break;
                }

                if (webHostEnvironment.IsDevelopment())
                    exceptionSummary.SupportMessages = $"{Environment.NewLine}{current.Message}";

                current = current.InnerException;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorStatusCode;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(exceptionSummary));
        }
    }
}
