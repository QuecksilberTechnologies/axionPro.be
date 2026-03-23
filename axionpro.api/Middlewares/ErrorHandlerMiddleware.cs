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

            // ✅ 401 - Unauthorized (Token fail)
            catch (UnauthorizedAccessException ex)
            {
                await HandleExceptionAsync(context, 401, ex.Message);
            }

            // ✅ 400 - Validation error
            catch (ValidationErrorException ex)
            {
                await HandleExceptionAsync(context, 400, ex.Message, ex.Errors);
            }

            // ✅ Custom ApiException (dynamic status)
            catch (ApiException ex)
            {
                await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
            }

            // ✅ 404 - Not Found (optional)
            catch (KeyNotFoundException ex)
            {
                await HandleExceptionAsync(context, 404, ex.Message);
            }

            // ✅ 500 - Internal Server Error
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(
                    context,
                    500,
                    "Internal Server Error"
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