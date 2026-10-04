using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Usuarios.Commands;
using TaskFlow.Application.Usuarios.DTOs;

namespace TaskFlow.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> AdicionarUsuarioAsync(AdicionarUsuarioCommand request);
        Task<UsuarioDTO> ObterUsuarioPorIdAsync(int usuarioId);
        Task<UsuarioDTO> EditarUsuarioAsync(EditarUsuarioCommand request);
        Task ExcluirUsuarioAsync(int usuarioId);
    }
}
