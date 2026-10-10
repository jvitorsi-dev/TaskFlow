using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Repositories;

namespace TaskFlow.Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTaskFlowInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Banco de dados 
            services.AddDbContext<TaskFlowDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // Repositórios 
            services.AddTaskFlowRepositories();

            // UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            return services;
        }
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
