using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

public class EditarTarefaUseCase
{
    private readonly IProjetoRepository _projetoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditarTarefaUseCase(
        IProjetoRepository projetoRepository,
        IUnitOfWork unitOfWork)
    {
        _projetoRepository = projetoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TarefaDTO> ExecuteAsync(EditarTarefaCommand command)
    {
        var projeto = await _projetoRepository.ObterPorIdAsync(command.ProjetoId, command.UsuarioId)
            ?? throw new NotFoundException($"Projeto com Id {command.ProjetoId} não encontrado.");

        projeto.EditarTarefa(command.Id, command.Titulo, command.Descricao, command.Prioridade);

        await _unitOfWork.CommitAsync();

        var tarefa = projeto.Tarefas.First(t => t.Id == command.Id);

        return new TarefaDTO(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Descricao,
            tarefa.Status,
            tarefa.Prioridade,
            tarefa.DataCriacao,
            tarefa.DataInicio,
            tarefa.DataConclusao
        );
    }
}