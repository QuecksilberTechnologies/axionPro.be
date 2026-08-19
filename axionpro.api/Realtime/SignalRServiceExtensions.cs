// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Registers and maps the authenticated AxionPro SignalR
//               notification transport boundary.
// ============================================================================

using axionpro.api.Realtime.Hubs;
using axionpro.api.Realtime.Identity;
using axionpro.api.Realtime.Services;
using axionpro.application.Interfaces.IRealTimeNotification;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;

namespace axionpro.api.Realtime
{
    /// <summary>Registers and maps the API-owned SignalR notification services.</summary>
    public static class SignalRServiceExtensions
    {
        #region Registration

        /// <summary>Registers SignalR and its tenant-safe application notification adapter.</summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection AddAxionProSignalR(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddScoped<SignalRConnectionIdentityResolver>();
            services.AddSingleton<IUserIdProvider, AxionProUserIdProvider>();
            services.AddSingleton<IRealTimeNotificationService, SignalRRealTimeNotificationService>();

            return services;
        }

        #endregion

        #region Endpoint mapping

        /// <summary>Maps the authenticated notification hub to its fixed API route.</summary>
        /// <param name="endpoints">The endpoint route builder.</param>
        /// <returns>The configured endpoint route builder.</returns>
        public static IEndpointRouteBuilder MapAxionProSignalR(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<NotificationHub>(SignalRRouteConstants.NotificationHub);
            return endpoints;
        }

        #endregion
    }
}
