
using TaskFlow.Application.Usuarios.DTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Usuarios.UseCases
{
    public class BuscarUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public BuscarUsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioDTO> ExecuteAsync(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);

            if (usuario is null)
                throw new NotFoundException($"Usuário com ID {id} não encontrado.");

            return new UsuarioDTO(usuario.Id, usuario.Nome, usuario.Email);
        }
    }
}