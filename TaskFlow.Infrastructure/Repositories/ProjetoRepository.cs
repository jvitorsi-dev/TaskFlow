using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories
{
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly TaskFlowDbContext _context;

        public ProjetoRepository(TaskFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Projeto?> ObterPorIdAsync(int projetoId, int usuarioId)
        {
            return await _context.Projetos
            .Include(p => p.Tarefas)   
            .FirstOrDefaultAsync(p => p.Id == projetoId && p.UsuarioId == usuarioId);
        }

        public async Task<IEnumerable<Projeto>> ListarTodosAsync()
        {
            return await _context.Projetos.ToListAsync();
        }

        public async Task EditarAsync(Projeto projeto)
        {
            _context.Projetos.Update(projeto);
        }

        public async Task ExcluirAsync(Projeto projeto)
        {
            _context.Projetos.Remove(projeto);
        }

        public async Task<Tarefa?> ObterTarefaProjeto(int projetoId, int tarefaId)
        {
            return await _context.Projetos
                .Where(p => p.Id == projetoId)
                .SelectMany(p => p.Tarefas)
                .FirstOrDefaultAsync(t => t.Id == tarefaId);
        }

    }
}
