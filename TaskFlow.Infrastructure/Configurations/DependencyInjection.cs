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
            IConfiguration configuration,
            Action<IBusRegistrationConfigurator>? configurarConsumers = null)
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
            services.AddTaskFlowMessaging(configuration);

            services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

            return services;
        }

        public static IServiceCollection AddTaskFlowMessaging(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IBusRegistrationConfigurator>? configurarConsumers = null)
        {
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                // Quem chamar este método pode registrar consumers aqui
                configurarConsumers?.Invoke(x);

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

                    // Cria uma fila para cada consumer registrado e liga essa fila à exchange do evento
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
