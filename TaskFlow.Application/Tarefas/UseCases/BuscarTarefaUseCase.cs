using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tarefas.UseCases
{
    public class BuscarTarefaUseCase
    {
        private readonly IProjetoRepository _projetoRepository;

        public BuscarTarefaUseCase(IProjetoRepository projetoRepository)
        {
            _projetoRepository = projetoRepository;
        }

        public async Task<TarefaDTO> ExecuteAsync(BuscarTarefaCommand command)
        {
            if (command.TarefaId <= 0)
                throw new DomainException("O id da tarefa deve ser maior que zero.");

            var tarefa = await _projetoRepository.ObterTarefaProjeto(
                command.ProjetoId, command.TarefaId);

            if (tarefa is null)
                throw new DomainException("Tarefa não encontrada.");

            return new TarefaDTO(
                tarefa.Id,
                tarefa.Titulo,
                tarefa.Descricao,
                tarefa.Status,
                tarefa.Prioridade,
                tarefa.DataCriacao
            );
        }
    }
}
