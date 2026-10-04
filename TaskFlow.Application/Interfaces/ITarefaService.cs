using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.DTOs;

namespace TaskFlow.Application.Interfaces
{
    public interface ITarefaService
    {
        Task<TarefaDTO> AdicionarTarefaAsync(AdicionarTarefaCommand request);
        Task<TarefaDTO> ObterTarefaPorIdAsync(BuscarTarefaCommand command);
        Task<TarefaDTO> EditarTarefaAsync(EditarTarefaCommand request);
        Task ExcluirTarefaAsync(BuscarTarefaCommand command);
    }
}
