using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Tests.TestSupport;
using Xunit;

namespace TaskFlow.Tests.Infrastructure.Repositories
{
    public class UnitOfWorkTests
    {
        [Fact]
        public async Task CommitAsync_PersisteAlteracoesPendentes()
        {
            var nomeBanco = Guid.NewGuid().ToString();

            await using (var context = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var unitOfWork = new UnitOfWork(context);

                context.Usuarios.Add(new Usuario("Ana Souza", "ana@teste.com", "senha12345"));
                var entidadesAfetadas = await unitOfWork.CommitAsync();

                Assert.True(entidadesAfetadas >= 1);
            }

            await using (var contextLeitura = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                Assert.True(await contextLeitura.Usuarios.AnyAsync(u => u.Email == "ana@teste.com"));
            }
        }

        [Fact]
        public async Task CommitAsync_SemAlteracoes_RetornaZero()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var unitOfWork = new UnitOfWork(context);

            var entidadesAfetadas = await unitOfWork.CommitAsync();

            Assert.Equal(0, entidadesAfetadas);
        }
    }
}
