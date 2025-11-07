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
           

            services.AddAutoMapper(typeof(MappingProfile).Assembly);
             //registration of fluent validation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviors<,>));

        }
    }
}
