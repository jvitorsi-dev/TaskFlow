using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.DTOs;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projetos.UseCases
{
    public class AdicionarProjetoUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuarioRepository _usuarioRepository;

        public AdicionarProjetoUseCase(
            IUnitOfWork unitOfWork,
            IUsuarioRepository usuarioRepository)
        {
            _unitOfWork = unitOfWork;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<ProjetoDTO> ExecuteAsync(AdicionarProjetoCommand command)
        {
            var projeto = new Projeto(
                command.Nome,
                command.Descricao,
                command.UsuarioId);

            var usuario = await _usuarioRepository.ObterPorIdAsync(command.UsuarioId);
            if(usuario == null)
                throw new NotFoundException("Usuário não encontrado.");

            usuario.AdicionarProjeto(projeto);
            await _unitOfWork.CommitAsync();

            return new ProjetoDTO(
                projeto.Id,
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