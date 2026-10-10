using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.DTOs;
using TaskFlow.Application.Projetos.UseCases;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Application.Tarefas.UseCases;

namespace TaskFlow.Application.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly AdicionarTarefaUseCase _criar;
        private readonly BuscarTarefaUseCase _buscar;
        private readonly EditarTarefaUseCase _editar;
        private readonly ExcluirTarefaUseCase _excluir;

        public TarefaService(
            AdicionarTarefaUseCase criar,
            BuscarTarefaUseCase buscar,
            EditarTarefaUseCase editar,
            ExcluirTarefaUseCase excluir)
        {
            _criar = criar;
            _buscar = buscar;
            _editar = editar;
            _excluir = excluir;
        }

        public Task<TarefaDTO> AdicionarTarefaAsync(AdicionarTarefaCommand request)
            => _criar.ExecuteAsync(request);

        public Task<TarefaDTO> ObterTarefaPorIdAsync(BuscarTarefaCommand command)
            => _buscar.ExecuteAsync(command);

        public Task<TarefaDTO> EditarTarefaAsync(EditarTarefaCommand request)
            => _editar.ExecuteAsync(request);

        public Task ExcluirTarefaAsync(BuscarTarefaCommand command)
            => _excluir.ExecuteAsync(command);
    }
}
