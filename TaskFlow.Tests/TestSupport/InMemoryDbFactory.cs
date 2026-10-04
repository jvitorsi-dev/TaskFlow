using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Tests.TestSupport
{
    /// <summary>
    /// Cria instâncias de <see cref="TaskFlowDbContext"/> apontando para um banco
    /// In-Memory isolado (um nome único por contexto, salvo quando um nome é informado
    /// para compartilhar o banco entre dois contextos).
    /// </summary>
    public static class InMemoryDbFactory
    {
        public static TaskFlowDbContext CriarContexto(string nomeBanco = null)
        {
            var options = new DbContextOptionsBuilder<TaskFlowDbContext>()
                .UseInMemoryDatabase(databaseName: nomeBanco ?? Guid.NewGuid().ToString())
                .Options;

            return new TaskFlowDbContext(options);
        }
    }
}
