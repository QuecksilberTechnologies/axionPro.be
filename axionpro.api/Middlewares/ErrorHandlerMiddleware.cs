using axionpro.application.Exceptions;
using axionpro.application.Wrappers;
using System.Net;
using System.Text.Json;

namespace axionpro.api.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, ex.Message);

                await HandleExceptionAsync(
                    context,
                    (int)HttpStatusCode.Unauthorized,
                    ex.Message,
                    null);
            }
            catch (ValidationErrorException ex)
            {
                _logger.LogWarning(ex, ex.Message);

                await HandleExceptionAsync(
                    context,
                    (int)HttpStatusCode.BadRequest,
                    ex.Message,
                    ex.Errors);
            }
            catch (ApiException ex)
            {
                _logger.LogWarning(ex, ex.Message);

                await HandleExceptionAsync(
                    context,
                    (int)HttpStatusCode.BadRequest,
                    ex.Message,
                    null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(
                    context,
                    (int)HttpStatusCode.InternalServerError,
                    "An unexpected server error occurred.",
                    new List<string> { ex.Message });
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            int statusCode,
            string message,
            List<string>? errors)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var responseModel = new ApiResponse<string>
            {
                IsSucceeded = false,
                Message = message,
                Data = null,
                Errors = errors ?? new List<string>()
            };

            var result = JsonSerializer.Serialize(responseModel);
            await context.Response.WriteAsync(result);
        }
    }
}