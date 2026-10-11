using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Infrastructure.Messaging;
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

            // Mensageria (MassTransit + RabbitMQ)
            services.AddMassTransit(x =>
            {
                // nomes de fila legíveis, ex.: "tarefa-concluida"
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(
                        configuration["RabbitMQ:Host"] ?? "localhost",
                        configuration["RabbitMQ:VirtualHost"] ?? "/",
                        h =>
                        {
                            h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                            h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                        });

                    // cria automaticamente as filas dos consumers registrados 
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

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
