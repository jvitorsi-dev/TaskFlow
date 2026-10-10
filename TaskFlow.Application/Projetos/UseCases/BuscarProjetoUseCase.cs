using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.DTOs;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projetos.UseCases
{
    public class BuscarProjetoUseCase
    {
        private readonly IProjetoRepository _projetoRepository;

        public BuscarProjetoUseCase(IProjetoRepository projetoRepository)
        {
            _projetoRepository = projetoRepository;
        }

        public async Task<ProjetoDTO> ExecuteAsync(BuscarProjetoCommand command)
        {
            if (command.ProjetoId <= 0)
                throw new DomainException("O id do projeto deve ser maior que zero.");

            var projeto = await _projetoRepository.ObterPorIdAsync(command.ProjetoId, command.UsuarioId)
                ?? throw new NotFoundException($"Projeto com Id {command.ProjetoId} não encontrado.");

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
                    t.DataCriacao
                )).ToList()
            );
        }
    }
}