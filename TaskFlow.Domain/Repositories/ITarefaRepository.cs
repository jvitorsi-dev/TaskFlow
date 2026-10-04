using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Repositories
{
    public interface ITarefaRepository
    {
        Task CriarTarefaAasync(Tarefa tarefa);
        Task ObterTarefaPorIdAsync(int id);
        Task EditarTarefaAsync(Tarefa tarefa);
        Task DeletarTarefaAsync(int id);
        Task ListarTodasTarefas();
        Task SaveChangesAsync();
    }
}
