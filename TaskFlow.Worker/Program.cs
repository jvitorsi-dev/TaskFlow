using TaskFlow.Infrastructure.Configurations;
using TaskFlow.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTaskFlowPersistence(builder.Configuration);  

builder.Services.AddTaskFlowMessaging(builder.Configuration, x =>
{
    x.AddConsumer<TarefaConcluidaConsumer>();
});

var host = builder.Build();
host.Run();