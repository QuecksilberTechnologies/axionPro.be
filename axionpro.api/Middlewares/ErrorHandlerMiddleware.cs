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
                if (context.WebSockets.IsWebSocketRequest)
                {
                    await _next(context);
                    return;
                }

                await _next(context);
            }

            catch (UnauthorizedAccessException ex)
            {
                await HandleExceptionAsync(context, 401, ex.Message);
            }

            catch (ValidationErrorException ex)
            {
                await HandleExceptionAsync(context, 400, ex.Message, ex.Errors);
            }

            catch (ApiException ex)
            {
                await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
            }

            catch (KeyNotFoundException ex)
            {
                await HandleExceptionAsync(context, 404, ex.Message);
            }

            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Null reference error");

                await HandleExceptionAsync(
                    context,
                    500,
                    "Required data is missing"
                );
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                await HandleExceptionAsync(
                    context,
                    500,
                    "Something went wrong. Please try again."
                );
            }
        }
        private static async Task HandleExceptionAsync(
            HttpContext context,
            int statusCode,
            string message,
            List<string>? errors = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ApiResponse<object>
            {
                IsSucceeded = false,
                Message = message,
                Data = null,
                Errors = errors ?? new List<string>()
            };

            var result = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(result);
        }
    }
}