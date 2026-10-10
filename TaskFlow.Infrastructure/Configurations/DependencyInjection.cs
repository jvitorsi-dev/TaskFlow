using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TaskFlow.Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTaskFlowRepositories(
       this IServiceCollection services)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            services.Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes => classes.Where(type =>
                    type.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
