using TaskFlow.Application.Interfaces;
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
        private readonly IniciarTarefaUseCase _iniciar;
        private readonly ConcluirTarefaUseCase _concluir;

        public TarefaService(
            AdicionarTarefaUseCase criar,
            BuscarTarefaUseCase buscar,
            EditarTarefaUseCase editar,
            ExcluirTarefaUseCase excluir,
            IniciarTarefaUseCase iniciar,
            ConcluirTarefaUseCase concluir)
        {
            _criar = criar;
            _buscar = buscar;
            _editar = editar;
            _excluir = excluir;
            _iniciar = iniciar;
            _concluir = concluir;
        }

        public Task<TarefaDTO> AdicionarTarefaAsync(AdicionarTarefaCommand request)
            => _criar.ExecuteAsync(request);

        public Task<TarefaDTO> ObterTarefaPorIdAsync(BuscarTarefaCommand command)
            => _buscar.ExecuteAsync(command);

        public Task<TarefaDTO> EditarTarefaAsync(EditarTarefaCommand request)
            => _editar.ExecuteAsync(request);

        public Task ExcluirTarefaAsync(BuscarTarefaCommand command)
            => _excluir.ExecuteAsync(command);

        public Task IniciarTarefa(BuscarTarefaCommand command)
            => _iniciar.ExecuteAsync(command);

        public Task ConcluirTarefa(BuscarTarefaCommand command) 
            => _concluir.ExecuteAsync(command);
    }
}
