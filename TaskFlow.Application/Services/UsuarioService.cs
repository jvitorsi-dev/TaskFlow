using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Application.Tarefas.UseCases;
using TaskFlow.Application.Usuarios.Commands;
using TaskFlow.Application.Usuarios.DTOs;
using TaskFlow.Application.Usuarios.UseCases;

namespace TaskFlow.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AdicionarUsuarioUseCase _criar;
        private readonly BuscarUsuarioUseCase _buscar;
        private readonly EditarUsuarioUseCase _editar;
        private readonly ExcluirUsuarioUseCase _excluir;

        public UsuarioService(
            AdicionarUsuarioUseCase criar,
            BuscarUsuarioUseCase buscar,
            EditarUsuarioUseCase editar,
            ExcluirUsuarioUseCase    excluir)
        {
            _criar = criar;
            _buscar = buscar;
            _editar = editar;
            _excluir = excluir;
        }

        public Task<UsuarioDTO> AdicionarUsuarioAsync(AdicionarUsuarioCommand request)
            => _criar.ExecuteAsync(request);

        public Task<UsuarioDTO> ObterUsuarioPorIdAsync(int usuarioId)
            => _buscar.ExecuteAsync(usuarioId);

        public Task<UsuarioDTO> EditarUsuarioAsync(EditarUsuarioCommand  request)
            => _editar.ExecuteAsync(request);

        public Task ExcluirUsuarioAsync(int usuarioId)
            => _excluir.ExecuteAsync(usuarioId);
    }
}
