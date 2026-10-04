using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Tests.TestSupport;
using Xunit;

namespace TaskFlow.Tests.Infrastructure.Repositories
{
    public class ProjetoRepositoryTests
    {
        private static Projeto CriarProjeto(string nome = "Projeto de Testes", int usuarioId = 1)
            => new(nome, "Descrição do projeto de testes", usuarioId);

        [Fact]
        public async Task ObterPorIdAsync_RetornaProjetoComSuasTarefas()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            int projetoId;

            await using (var contextEscrita = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var projeto = CriarProjeto();
                projeto.AdicionarTarefa(new Tarefa("Tarefa 1", "Descrição da tarefa", PrioridadeTarefa.Media));
                contextEscrita.Projetos.Add(projeto);
                await contextEscrita.SaveChangesAsync();
                projetoId = projeto.Id;
            }

            await using (var contextLeitura = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var repository = new ProjetoRepository(contextLeitura);

                var projeto = await repository.ObterPorIdAsync(projetoId, usuarioId: 1);

                Assert.NotNull(projeto);
                Assert.Equal("Projeto de Testes", projeto.Nome);
                var tarefa = Assert.Single(projeto.Tarefas);
                Assert.Equal("Tarefa 1", tarefa.Titulo);
            }
        }

        [Fact]
        public async Task ObterPorIdAsync_ProjetoDeOutroUsuario_RetornaNulo()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            int projetoId;

            await using (var contextEscrita = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var projeto = CriarProjeto(usuarioId: 1);
                contextEscrita.Projetos.Add(projeto);
                await contextEscrita.SaveChangesAsync();
                projetoId = projeto.Id;
            }

            await using (var contextLeitura = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var repository = new ProjetoRepository(contextLeitura);

                // O projeto pertence ao usuário 1; buscando como usuário 2 não deve retornar nada.
                var projeto = await repository.ObterPorIdAsync(projetoId, usuarioId: 2);

                Assert.Null(projeto);
            }
        }

        [Fact]
        public async Task ListarTodosAsync_RetornaTodosOsProjetos()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var repository = new ProjetoRepository(context);
            context.Projetos.AddRange(CriarProjeto("Projeto A"), CriarProjeto("Projeto B", usuarioId: 2));
            await context.SaveChangesAsync();

            var projetos = (await repository.ListarTodosAsync()).ToList();

            Assert.Equal(2, projetos.Count);
        }

        [Fact]
        public async Task EditarAsync_PersisteAsAlteracoes()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            int projetoId;

            await using (var contextEscrita = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var projeto = CriarProjeto();
                contextEscrita.Projetos.Add(projeto);
                await contextEscrita.SaveChangesAsync();
                projetoId = projeto.Id;
            }

            await using (var contextEdicao = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var repository = new ProjetoRepository(contextEdicao);
                var projeto = await repository.ObterPorIdAsync(projetoId, usuarioId: 1);

                projeto.Editar("Nome editado", "Descrição editada", usuarioId: 1);
                await repository.EditarAsync(projeto);
                await contextEdicao.SaveChangesAsync();
            }

            await using (var contextLeitura = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var projeto = await contextLeitura.Projetos.SingleAsync(p => p.Id == projetoId);

                Assert.Equal("Nome editado", projeto.Nome);
                Assert.Equal("Descrição editada", projeto.Descricao);
            }
        }

        [Fact]
        public async Task ExcluirAsync_RemoveProjetoDoBanco()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var repository = new ProjetoRepository(context);
            var projeto = CriarProjeto();
            context.Projetos.Add(projeto);
            await context.SaveChangesAsync();

            await repository.ExcluirAsync(projeto);
            await context.SaveChangesAsync();

            Assert.Empty(await repository.ListarTodosAsync());
        }

        [Fact]
        public async Task ObterTarefaProjeto_TarefaExistente_RetornaTarefa()
        {
            var nomeBanco = Guid.NewGuid().ToString();
            int projetoId;
            int tarefaId;

            await using (var contextEscrita = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var projeto = CriarProjeto();
                var tarefa = new Tarefa("Tarefa 1", "Descrição da tarefa", PrioridadeTarefa.Alta);
                projeto.AdicionarTarefa(tarefa);
                contextEscrita.Projetos.Add(projeto);
                await contextEscrita.SaveChangesAsync();
                projetoId = projeto.Id;
                tarefaId = tarefa.Id;
            }

            await using (var contextLeitura = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var repository = new ProjetoRepository(contextLeitura);

                var tarefa = await repository.ObterTarefaProjeto(projetoId, tarefaId);

                Assert.NotNull(tarefa);
                Assert.Equal("Tarefa 1", tarefa.Titulo);
                Assert.Equal(PrioridadeTarefa.Alta, tarefa.Prioridade);
            }
        }

        [Fact]
        public async Task ObterTarefaProjeto_TarefaInexistente_RetornaNulo()
        {
            await using var context = InMemoryDbFactory.CriarContexto();
            var repository = new ProjetoRepository(context);
            var projeto = CriarProjeto();
            context.Projetos.Add(projeto);
            await context.SaveChangesAsync();

            var tarefa = await repository.ObterTarefaProjeto(projeto.Id, tarefaId: 999);

            Assert.Null(tarefa);
        }
    }
}
