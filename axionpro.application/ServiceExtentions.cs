using axionpro.application.Interfaces.ILogger;
using axionpro.application.Mappings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace axionpro.application
{
    public static class ServiceExtentions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            // Registration the services
            //
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(conf => conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient( typeof(MediatR.IPipelineBehavior<,>),typeof(Features.EmployeeCmd.EmployeeTenantPermissionBehavior<,>));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Features.DepartmentCmd.DepartmentPermissionBehavior<,>));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Features.DesignationCmd.DesignationPermissionBehavior<,>));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Features.RoleCmd.RolePermissionBehavior<,>));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Features.TenantManagementCmd.TenantManagementPermissionBehavior<,>));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Features.TenantConfigurationCmd.TenantLocationPermissionBehavior<,>));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Features.DefaultEmailConfigCmd.DefaultEmailConfigPermissionBehavior<,>));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Features.TenantEmailConfigCmd.TenantEmailConfigPermissionBehavior<,>));
            services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(Features.DeviceCommandCmd.DeviceCommandPermissionBehavior<,>));
           

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
             //registration of fluent validation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviors<,>));

        }
    }
}
