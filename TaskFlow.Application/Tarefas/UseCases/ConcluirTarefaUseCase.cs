using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.Events;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tarefas.UseCases
{
    public class ConcluirTarefaUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjetoRepository _projetoRepository;
        private readonly IEventPublisher _eventPublisher;

        public ConcluirTarefaUseCase(
            IUnitOfWork unitOfWork,
            IProjetoRepository projetoRepository,
            IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _projetoRepository = projetoRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task ExecuteAsync(BuscarTarefaCommand command)
        {
            var tarefa = await _projetoRepository.ObterTarefaProjeto(command.ProjetoId, command.TarefaId)
                ?? throw new NotFoundException(
                    $"Tarefa com Id {command.TarefaId} não encontrada no projeto {command.ProjetoId}.");

            tarefa.Concluir();  // 1. regra de negócio

            // 2. Com o Outbox, o Publish NÃO envia ao RabbitMQ:
            //    só "anota" a mensagem no DbContext (tabela OutboxMessage)
            await _eventPublisher.PublishAsync(new TarefaConcluidaEvent(
                TarefaId: tarefa.Id,
                ProjetoId: tarefa.ProjetoId,
                UsuarioId: command.UsuarioId,
                Titulo: tarefa.Titulo,
                ConcluidaEmUtc: DateTime.UtcNow));

            // 3. Grava a tarefa E a mensagem na MESMA transação
            await _unitOfWork.CommitAsync();
        }
    }
}