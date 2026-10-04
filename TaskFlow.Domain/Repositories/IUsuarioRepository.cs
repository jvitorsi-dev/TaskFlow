using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterPorIdAsync(int usuarioId);
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<IEnumerable<Usuario>> ListarTodosAsync();
        Task AdicionarAsync(Usuario usuario);
        Task ExcluirUsuario(int usuarioId);
    }
}
