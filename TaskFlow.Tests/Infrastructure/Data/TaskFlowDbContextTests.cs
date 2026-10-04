using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Tests.TestSupport;
using Xunit;

namespace TaskFlow.Tests.Infrastructure.Data
{
    public class TaskFlowDbContextTests
    {
        [Fact]
        public async Task SalvarUsuario_RecuperaPorId()
        {
            await using var context = InMemoryDbFactory.CriarContexto();

            context.Usuarios.Add(new Usuario("Ana Souza", "ana@teste.com", "senha12345"));
            await context.SaveChangesAsync();

            var usuario = await context.Usuarios.SingleAsync();

            Assert.Equal("Ana Souza", usuario.Nome);
            Assert.Equal("ana@teste.com", usuario.Email);
            Assert.True(usuario.Id > 0, "O Id deve ser gerado pelo banco.");
        }

        [Fact]
        public async Task SalvarProjetoComTarefas_CarregaColecaoComInclude()
        {
            var nomeBanco = Guid.NewGuid().ToString();

            await using (var contextEscrita = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", usuarioId: 1);
                projeto.AdicionarTarefa(new Tarefa("Tarefa 1", "Descrição da tarefa 1", PrioridadeTarefa.Alta));
                projeto.AdicionarTarefa(new Tarefa("Tarefa 2", "Descrição da tarefa 2", PrioridadeTarefa.Baixa));

                contextEscrita.Projetos.Add(projeto);
                await contextEscrita.SaveChangesAsync();
            }

            await using (var contextLeitura = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var projeto = await contextLeitura.Projetos
                    .Include(p => p.Tarefas)
                    .SingleAsync();

                Assert.Equal("Projeto de Testes", projeto.Nome);
                Assert.Equal(2, projeto.Tarefas.Count);
                Assert.Contains(projeto.Tarefas, t => t.Titulo == "Tarefa 1");
            }
        }

        [Fact]
        public async Task SalvarProjeto_StatusEhRecuperadoCorretamente()
        {
            var nomeBanco = Guid.NewGuid().ToString();

            await using (var contextEscrita = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                contextEscrita.Projetos.Add(
                    new Projeto("Projeto de Testes", "Descrição do projeto", usuarioId: 1));
                await contextEscrita.SaveChangesAsync();
            }

            await using (var contextLeitura = InMemoryDbFactory.CriarContexto(nomeBanco))
            {
                var projeto = await contextLeitura.Projetos.SingleAsync();

                Assert.Equal(StatusProjeto.Planejado, projeto.Status);
            }
        }
    }
}
