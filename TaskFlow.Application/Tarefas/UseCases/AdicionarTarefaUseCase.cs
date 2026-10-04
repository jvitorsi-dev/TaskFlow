using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tarefas.UseCases
{
    public class AdicionarTarefaUseCase
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdicionarTarefaUseCase(IProjetoRepository projetoRepository,
            IUnitOfWork unitOfWork)
        {
            _projetoRepository = projetoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TarefaDTO> ExecuteAsync(AdicionarTarefaCommand command)
        {
            var projeto = await _projetoRepository
                .ObterPorIdAsync(command.ProjetoId, command.UsuarioId) ??
                throw new NotFoundException("Projeto não encontrado");

            var tarefa = new Tarefa(
                command.Titulo,
                command.Descricao,
                command.Prioridade
                );

            projeto.AdicionarTarefa(tarefa);
            await _unitOfWork.CommitAsync();

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
