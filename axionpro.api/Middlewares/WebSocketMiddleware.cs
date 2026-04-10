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

            Console.WriteLine("👉 Middleware Hit");
            // 🔥 Only handle /ws requests
            if (context.WebSockets.IsWebSocketRequest)
            {
                await handler.HandleAsync(context);
                return;
            }

            await _next(context);
        }
    }
}
