using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Services;

namespace TaskFlow.Application.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTaskFlowServices(
       this IServiceCollection services)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Registra automaticamente todos os UseCases
            services.Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes.Where(type =>
                    type.Name.EndsWith("UseCase")))
                .AsSelf()
                .WithScopedLifetime());

            // Services
            // Services
            services.Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes.Where(type =>
                    type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
