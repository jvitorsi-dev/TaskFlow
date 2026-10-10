using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projetos.UseCases
{
    public class ConcluirProjetoUseCase
    {
        private readonly IUnitOfWork _unityOfWork;
        private readonly IProjetoRepository _projetoRepository;
        public ConcluirProjetoUseCase(IUnitOfWork unityOfWork, IProjetoRepository projetoRepository)
        {
            _unityOfWork = unityOfWork;
            _projetoRepository = projetoRepository;
        }
        public async Task ExecuteAsync(BuscarProjetoCommand command)
        {
            var projeto = await _projetoRepository.ObterPorIdAsync(command.ProjetoId, command.UsuarioId)
                ?? throw new NotFoundException($"Projeto com Id {command.ProjetoId} não encontrado.");

            projeto.Concluir();
            await _unityOfWork.CommitAsync();
        }   
    }
}
