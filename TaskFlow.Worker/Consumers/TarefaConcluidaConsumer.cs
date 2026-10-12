using MassTransit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Tarefas.Events;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Worker.Consumers
{
    public class TarefaConcluidaConsumer : IConsumer<TarefaConcluidaEvent>
    {
        private readonly TaskFlowDbContext _db;
        private readonly ILogger<TarefaConcluidaConsumer> _logger;

        public TarefaConcluidaConsumer(TaskFlowDbContext db, ILogger<TarefaConcluidaConsumer> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TarefaConcluidaEvent> context)
        {
            var evento = context.Message;

            _logger.LogInformation(
                "Recebido TarefaConcluidaEvent | MessageId: {MessageId} | Tarefa {TarefaId} '{Titulo}'",
                context.MessageId, evento.TarefaId, evento.Titulo);

            var jaNotificada = await _db.Notificacoes
                .AnyAsync(n => n.TarefaId == evento.TarefaId);

            if (jaNotificada)
            {
                _logger.LogWarning(
                    "DUPLICADA ignorada | MessageId: {MessageId} | Tarefa {TarefaId} já foi notificada.",
                    context.MessageId, evento.TarefaId);
                return; // retornar sem exceção = ack → a mensagem sai da fila
            }

            _db.Notificacoes.Add(new Notificacao(
                usuarioId: evento.UsuarioId,
                tarefaId: evento.TarefaId,
                mensagem: $"Sua tarefa '{evento.Titulo}' foi concluída."));

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                // 🛡️ 2ª BARREIRA: outro Worker gravou a mesma notificação ao mesmo tempo
                _logger.LogWarning(
                    "DUPLICADA barrada pelo índice único | Tarefa {TarefaId}.", evento.TarefaId);
                return;
            }

            _logger.LogInformation(
                "Notificacao criada para o usuario {UsuarioId} | Tarefa {TarefaId}.",
                evento.UsuarioId, evento.TarefaId);
        }
    }
}