using NSubstitute;
using TaskFlow.Application.Projetos.UseCases;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Repositories;
using Xunit;

namespace TaskFlow.Tests.Application.Projetos
{
    public class ListarProjetosUseCaseTests
    {
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly ListarProjetosUseCase _useCase;

        public ListarProjetosUseCaseTests()
        {
            _useCase = new ListarProjetosUseCase(_projetoRepository);
        }

        [Fact]
        public async Task ExecuteAsync_SemProjetos_RetornaListaVazia()
        {
            _projetoRepository.ListarTodosAsync()
                .Returns(Task.FromResult(Enumerable.Empty<Projeto>()));

            var resultado = await _useCase.ExecuteAsync();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ExecuteAsync_ComProjetos_RetornaDtosMapeados()
        {
            var projetoComTarefa = new Projeto("Projeto A", "Descrição do projeto A", 1);
            projetoComTarefa.AdicionarTarefa(
                new Tarefa("Tarefa 1", "Descrição da tarefa 1", PrioridadeTarefa.Alta));
            var projetoSimples = new Projeto("Projeto B", "Descrição do projeto B", 2);
            projetoSimples.Iniciar();

            _projetoRepository.ListarTodosAsync()
                .Returns(Task.FromResult(new[] { projetoComTarefa, projetoSimples }.AsEnumerable()));

            var resultado = (await _useCase.ExecuteAsync()).ToList();

            Assert.Equal(2, resultado.Count);

            Assert.Equal("Projeto A", resultado[0].Nome);
            var tarefaDto = Assert.Single(resultado[0].Tarefas);
            Assert.Equal("Tarefa 1", tarefaDto.Titulo);
            Assert.Equal(PrioridadeTarefa.Alta, tarefaDto.Prioridade);

            Assert.Equal("Projeto B", resultado[1].Nome);
            Assert.Equal(StatusProjeto.EmAndamento, resultado[1].Status);
            Assert.Empty(resultado[1].Tarefas);
        }
    }
}
