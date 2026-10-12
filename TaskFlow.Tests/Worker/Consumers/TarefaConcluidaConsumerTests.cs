using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Tarefas.Events;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Data;
using TaskFlow.Worker.Consumers;
using Xunit;

namespace TaskFlow.Tests.Worker.Consumers
{
    public class TarefaConcluidaConsumerTests : IAsyncLifetime
    {
        private readonly string _nomeBanco = Guid.NewGuid().ToString();
        private ServiceProvider _provider;
        private ITestHarness _harness;

        // Roda ANTES de cada teste: monta a DI com banco em memória + harness
        public async Task InitializeAsync()
        {
            _provider = new ServiceCollection()
                .AddLogging()
                .AddDbContext<TaskFlowDbContext>(o => o.UseInMemoryDatabase(_nomeBanco))
                .AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<TarefaConcluidaConsumer>();
                })
                .BuildServiceProvider(true);

            _harness = _provider.GetRequiredService<ITestHarness>();
            await _harness.Start();
        }

        // Roda DEPOIS de cada teste: desliga tudo
        public async Task DisposeAsync()
        {
            await _harness.Stop();
            await _provider.DisposeAsync();
        }

        [Fact]
        public async Task Consume_EventoNovo_CriaNotificacaoParaOUsuario()
        {
            var evento = CriarEvento(tarefaId: 7);

            await _harness.Bus.Publish(evento);

            // Espera o consumer terminar de processar
            var consumer = _harness.GetConsumerHarness<TarefaConcluidaConsumer>();
            Assert.True(await consumer.Consumed.Any<TarefaConcluidaEvent>());

            var notificacao = Assert.Single(await ObterNotificacoesAsync());
            Assert.Equal(1, notificacao.UsuarioId);
            Assert.Equal(7, notificacao.TarefaId);
            Assert.Contains("Minha tarefa", notificacao.Mensagem);
        }

        [Fact]
        public async Task Consume_TarefaJaNotificada_IgnoraDuplicataSemErro()
        {
            // Simula que esta tarefa JÁ foi processada antes (como no cenário B)
            await AdicionarNotificacaoAsync(new Notificacao(1, 7, "Notificação original"));

            await _harness.Bus.Publish(CriarEvento(tarefaId: 7));

            var consumer = _harness.GetConsumerHarness<TarefaConcluidaConsumer>();
            Assert.True(await consumer.Consumed.Any<TarefaConcluidaEvent>());

            // Continua existindo UMA notificação, e é a original
            var notificacao = Assert.Single(await ObterNotificacoesAsync());
            Assert.Equal("Notificação original", notificacao.Mensagem);

            // Duplicata NÃO é erro: nenhuma falha deve ter sido publicada
            Assert.False(await _harness.Published.Any<Fault<TarefaConcluidaEvent>>());
        }

        [Fact]
        public async Task Consume_TarefasDiferentes_CriaUmaNotificacaoParaCada()
        {
            await _harness.Bus.Publish(CriarEvento(tarefaId: 7));
            await _harness.Bus.Publish(CriarEvento(tarefaId: 8));

            var consumer = _harness.GetConsumerHarness<TarefaConcluidaConsumer>();
            Assert.True(await consumer.Consumed.Any<TarefaConcluidaEvent>(m => m.Context.Message.TarefaId == 7));
            Assert.True(await consumer.Consumed.Any<TarefaConcluidaEvent>(m => m.Context.Message.TarefaId == 8));

            var notificacoes = await ObterNotificacoesAsync();
            Assert.Equal(2, notificacoes.Count);
        }

        // ---------- helpers ----------

        private static TarefaConcluidaEvent CriarEvento(int tarefaId) => new(
            TarefaId: tarefaId,
            ProjetoId: 3,
            UsuarioId: 1,
            Titulo: "Minha tarefa",
            ConcluidaEmUtc: DateTime.UtcNow);

        // Usa um escopo NOVO (um DbContext novo), assim como o consumer faz
        private async Task<List<Notificacao>> ObterNotificacoesAsync()
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
            return await db.Notificacoes.ToListAsync();
        }

        private async Task AdicionarNotificacaoAsync(Notificacao notificacao)
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
            db.Notificacoes.Add(notificacao);
            await db.SaveChangesAsync();
        }
    }
}