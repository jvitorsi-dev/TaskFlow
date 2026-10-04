// TaskFlow.Application/UseCases/Usuario/DeletarUsuarioUseCase.cs
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Usuarios.UseCases
{
    public class ExcluirUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExcluirUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);

            if (usuario is null)
                throw new DomainException($"Usuário com ID {id} não encontrado.");

            await _usuarioRepository.ExcluirUsuario(id);
            await _unitOfWork.CommitAsync();
        }
    }
}