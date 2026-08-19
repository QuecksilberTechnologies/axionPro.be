namespace axionpro.api.Middlewares
{
    using System.Net.WebSockets;
    using axionpro.infrastructure.DeviceServices;

    public class WebSocketMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Constructor

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Request handling

        /// <summary>
        /// Handles only legacy device WebSocket upgrades so mapped SignalR upgrades can continue to their endpoint.
        /// </summary>
        /// <param name="context">The current HTTP request context.</param>
        /// <param name="handler">The legacy device WebSocket handler.</param>
        public async Task InvokeAsync(HttpContext context, WebSocketHandler handler)
        {

            Console.WriteLine("👉 Middleware Hit");
            // Device sockets use /ws; all other WebSocket upgrades continue to their mapped endpoint.
            if (context.WebSockets.IsWebSocketRequest &&
                context.Request.Path.StartsWithSegments("/ws"))
            {
                await handler.HandleAsync(context);
                return;
            }

            await _next(context);
        }

        #endregion
    }
}
