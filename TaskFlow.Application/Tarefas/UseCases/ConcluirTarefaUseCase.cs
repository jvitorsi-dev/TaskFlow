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

            tarefa.Concluir();              // 1. regra de negócio (pode lançar DomainException)
            await _unitOfWork.CommitAsync(); // 2. grava no banco

            // 3. só depois de gravar, avisa os interessados
            await _eventPublisher.PublishAsync(new TarefaConcluidaEvent(
                TarefaId: tarefa.Id,
                ProjetoId: tarefa.ProjetoId,
                UsuarioId: command.UsuarioId,
                Titulo: tarefa.Titulo,
                ConcluidaEmUtc: DateTime.UtcNow));
        }
    }
}