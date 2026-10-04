using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Repositories
{
    public interface IProjetoRepository
    {
        Task<Projeto?> ObterPorIdAsync(int projetoId, int usuarioId);
        Task<IEnumerable<Projeto>> ListarTodosAsync();
        Task EditarAsync(Projeto projeto);
        Task ExcluirAsync(Projeto projeto);
        Task<Tarefa?> ObterTarefaProjeto(int projetoId, int tarefaId);
    }
}
