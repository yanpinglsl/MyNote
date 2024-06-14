using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YY.Zhihu.UseCases.Common.Exceptions;

namespace YY.Zhihu.Application.Infrastructure
{
    public class UseCaseExceptionHandler : IExceptionHandler
    {
        private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

        public UseCaseExceptionHandler()
        {
            _exceptionHandlers = new Dictionary<Type, Func<HttpContext, Exception, Task>>
        {
            { typeof(ForbiddenException), HandleForbiddenExceptionAsync },
            { typeof(ValidationException), HandleValidationExceptionAsync }
        };
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
            CancellationToken cancellationToken)
        {
            var exceptionType = exception.GetType();

            if (!_exceptionHandlers.TryGetValue(exceptionType, out var handler)) return false;

            await handler.Invoke(httpContext, exception);
            return true;
        }

        private async Task HandleForbiddenExceptionAsync(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Type = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/403"
            });
        }
        private async Task HandleValidationExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var validationException = (ValidationException)exception;
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest ;

            await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(validationException.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/400"
            });
        }
    }
}