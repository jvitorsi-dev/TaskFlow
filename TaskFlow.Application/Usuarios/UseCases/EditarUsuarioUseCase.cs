using TaskFlow.Application.Usuarios.Commands;
using TaskFlow.Application.Usuarios.DTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Usuarios.UseCases
{
    public class EditarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditarUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDTO> ExecuteAsync(EditarUsuarioCommand input)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(input.Id);

            if (usuario is null)
                throw new DomainException($"Usuário com ID {input.Id} não encontrado.");

            if (input.Email is not null)
            {
                var usuarioComMesmoEmail = await _usuarioRepository.ObterPorEmailAsync(input.Email);
                if (usuarioComMesmoEmail is not null && usuarioComMesmoEmail.Id != input.Id)
                    throw new DomainException("Já existe um usuário cadastrado com esse e-mail.");
            }

            usuario.Editar(input.Nome, input.Email, input.Senha);
            await _unitOfWork.CommitAsync();

            return new UsuarioDTO(usuario.Id, usuario.Nome, usuario.Email);
        }
    }
}