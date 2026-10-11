using MassTransit;
using TaskFlow.Application.Tarefas.Events;

namespace TaskFlow.Worker.Consumers
{
    public class TarefaConcluidaConsumer : IConsumer<TarefaConcluidaEvent>
    {
        private readonly ILogger<TarefaConcluidaConsumer> _logger;

        public TarefaConcluidaConsumer(ILogger<TarefaConcluidaConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TarefaConcluidaEvent> context)
        {
            var evento = context.Message;

            _logger.LogInformation(
                "Recebido TarefaConcluidaEvent | MessageId: {MessageId} | Tarefa {TarefaId} '{Titulo}' | Projeto {ProjetoId} | Usuario {UsuarioId}",
                context.MessageId, evento.TarefaId, evento.Titulo, evento.ProjetoId, evento.UsuarioId);

            // Simula um envio de e-mail lento (2 segundos)
            await Task.Delay(TimeSpan.FromSeconds(2));

            _logger.LogInformation(
                "Notificacao enviada ao usuario {UsuarioId}: a tarefa '{Titulo}' foi concluida.",
                evento.UsuarioId, evento.Titulo);
        }
    }
}