using axionpro.application.Interfaces.ICacheService;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IQRService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;

using axionpro.infrastructure.BackgroundJob;
using axionpro.infrastructure.CacheMemory;
using axionpro.infrastructure.CommonRequest;
using axionpro.infrastructure.DeviceCommunication.Mqtt;
using axionpro.infrastructure.EncryptionService;
using axionpro.infrastructure.FileStoringService;
using axionpro.infrastructure.Logging;
using axionpro.infrastructure.MailService;
using axionpro.infrastructure.Permission;
using axionpro.infrastructure.QRServies;
using axionpro.infrastructure.Security.HashedService;
using axionpro.infrastructure.Token;
using axionpro.persistance.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
 

namespace axionpro.infrastructure
{
    public static class ServiceExtentions
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register background service
             services.AddHostedService<CommonBackgroundService>();  // ✅ This is mandatory
             services.AddMemoryCache();

            // One shared broker client serves every Tenant and device. Broker credentials are
            // bound from secure application/environment configuration, not Tenant settings.
            services.AddOptions<AxionProMqttOptions>()
                .Bind(configuration.GetSection(AxionProMqttOptions.SectionName));
            services.AddSingleton<DeviceMqttMessageRouter>();
            services.AddSingleton<AxionProMqttClient>();
            services.AddSingleton<IAxionProMqttPublisher>(serviceProvider =>
                serviceProvider.GetRequiredService<AxionProMqttClient>());
            services.AddHostedService<AxionProMqttHostedService>();
            services.AddHostedService<DeviceCommandDispatcherWorker>();

            // Register repositories & services

            // Register repositories & services
            services.AddScoped<ILoggerService, LoggerService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITenantEmailConfigRepository, TenantEmailConfigRepository>();
            services.AddScoped<IQRService, QRService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICommonRequestService, CommonRequestService>();
            

            // ✅ Add missing repository registration
            services.AddScoped<IPermissionRepository, PermissionRepository>();

            // Register permission + encryption
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();

            services.AddScoped<IEncryptionService, AesEncryptionService>();
            services.AddScoped<IIdEncoderService, IdEncoderService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ITenantKeyCache, TenantKeyMemoryCache>();
            services.AddScoped<ITenantKeyResolver, TenantKeyResolver>();

        }
    }


}
