using Application.Common.Behaviors;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register MediatR and FluentValidation services
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
            services.AddValidatorsFromAssemblies(assemblies);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Register Mapster services
            var config = new TypeAdapterConfig();
            config.Scan(typeof(DependencyInjection).Assembly);
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
    }
}
