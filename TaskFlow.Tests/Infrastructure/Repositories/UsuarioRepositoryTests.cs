using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Tests.TestSupport;
using Xunit;

namespace TaskFlow.Tests.Infrastructure.Repositories
{
    public class UsuarioRepositoryTests
    {
        [Fact]
        public async Task AdicionarAsync_PersisteUsuario()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var repository = new UsuarioRepository(context);

            await repository.AdicionarAsync(new Usuario("Ana Souza", "ana@teste.com", "senha12345"));
            await context.SaveChangesAsync();

            var usuarios = (await repository.ListarTodosAsync()).ToList();
            var usuario = Assert.Single(usuarios);
            Assert.Equal("Ana Souza", usuario.Nome);
            Assert.Equal("ana@teste.com", usuario.Email);
        }

        [Fact]
        public async Task ObterPorIdAsync_RetornaUsuarioExistente()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var repository = new UsuarioRepository(context);
            await repository.AdicionarAsync(new Usuario("Ana Souza", "ana@teste.com", "senha12345"));
            await context.SaveChangesAsync();

            var usuarios = (await repository.ListarTodosAsync()).ToList();
            var usuario = await repository.ObterPorIdAsync(usuarios[0].Id);

            Assert.NotNull(usuario);
            Assert.Equal("Ana Souza", usuario.Nome);
        }

        [Fact]
        public async Task ObterPorIdAsync_IdInexistente_RetornaNulo()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var repository = new UsuarioRepository(context);

            var usuario = await repository.ObterPorIdAsync(999);

            Assert.Null(usuario);
        }

        [Fact]
        public async Task ObterPorEmailAsync_RetornaUsuarioComAqueleEmail()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var repository = new UsuarioRepository(context);
            await repository.AdicionarAsync(new Usuario("Ana Souza", "ana@teste.com", "senha12345"));
            await repository.AdicionarAsync(new Usuario("Carlos Lima", "carlos@teste.com", "senha12345"));
            await context.SaveChangesAsync();

            var usuario = await repository.ObterPorEmailAsync("carlos@teste.com");

            Assert.NotNull(usuario);
            Assert.Equal("Carlos Lima", usuario.Nome);
        }

        [Fact]
        public async Task ObterPorEmailAsync_EmailInexistente_RetornaNulo()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var repository = new UsuarioRepository(context);

            var usuario = await repository.ObterPorEmailAsync("ninguem@teste.com");

            Assert.Null(usuario);
        }

        [Fact]
        public async Task ExcluirUsuario_RemoveUsuarioDoBanco()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var repository = new UsuarioRepository(context);
            await repository.AdicionarAsync(new Usuario("Ana Souza", "ana@teste.com", "senha12345"));
            await context.SaveChangesAsync();
            var usuarios = (await repository.ListarTodosAsync()).ToList();

            await repository.ExcluirUsuario(usuarios[0].Id);
            await context.SaveChangesAsync();

            Assert.Empty(await repository.ListarTodosAsync());
        }

        [Fact]
        public async Task ExcluirUsuario_IdInexistente_NaoLancaExcecao()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var repository = new UsuarioRepository(context);

            await repository.ExcluirUsuario(999);
            await context.SaveChangesAsync();
        }
    }
}
