using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.DTOs;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projetos.UseCases
{
    public class EditarProjetoUseCase
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IUnitOfWork _unityOfWork;

        public EditarProjetoUseCase(IProjetoRepository projetoRepository,
            IUnitOfWork unitOfWork)
        {
            _projetoRepository = projetoRepository;
            _unityOfWork = unitOfWork;
        }

        public async Task<ProjetoDTO> ExecuteAsync(
            EditarProjetoCommand command)
        {   
            var projeto = await _projetoRepository
                .ObterPorIdAsync(command.Id, command.UsuarioId) ??
                throw new NotFoundException(
                    $"Projeto com Id {command.Id} não encontrado.");

            projeto.Editar
                (command.Nome,
                command.Descricao,
                command.UsuarioId);

            await _projetoRepository.EditarAsync(projeto);
            await _unityOfWork.CommitAsync();

            return new ProjetoDTO(
                projeto.Id,
                projeto.UsuarioId,
                projeto.Nome,
                projeto.Descricao,
                projeto.Status,
                projeto.DataCriacao,
                projeto.Tarefas.Select(t => new TarefaDTO(
                    t.Id,
                    t.Titulo,
                    t.Descricao,
                    t.Status,
                    t.Prioridade,
                    t.DataCriacao)).ToList()
            );
        }
    }
}
