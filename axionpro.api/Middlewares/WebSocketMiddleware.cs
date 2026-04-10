namespace axionpro.api.Middlewares
{
    using System.Net.WebSockets;
    using axionpro.infrastructure.DeviceServices;

    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, WebSocketHandler handler)
        {
            // 🔥 Only handle /ws requests
            if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
            {
                await handler.HandleAsync(context);
                return;
            }

            await _next(context);
        }
    }
}
