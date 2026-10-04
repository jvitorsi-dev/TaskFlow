
using TaskFlow.Application.Usuarios.Commands;
using TaskFlow.Application.Usuarios.DTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Usuarios.UseCases
{
    public class AdicionarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdicionarUsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDTO> ExecuteAsync(AdicionarUsuarioCommand input)
        {
            var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(input.Email);
            if (usuarioExistente is not null)
                throw new DomainException("Já existe um usuário cadastrado com esse e-mail.");

            var usuario = new Usuario(
                input.Nome,
                input.Email,
                input.Senha
            );

            await _usuarioRepository.AdicionarAsync(usuario);
            await _unitOfWork.CommitAsync();

            return new UsuarioDTO(usuario.Id, usuario.Nome, usuario.Email);
        }
    }
}