using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Data
{
    public class TaskFlowDbContext : DbContext
    {
        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options)
            : base(options)
        {
        }

        public DbSet<Projeto> Projetos => Set<Projeto>();
        public DbSet<Tarefa> Tarefas => Set<Tarefa>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Notificacao> Notificacoes => Set<Notificacao>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica todas as configurations da mesma assembly automaticamente
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskFlowDbContext).Assembly);

            // Tabelas do Outbox/Inbox do MassTransit
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
