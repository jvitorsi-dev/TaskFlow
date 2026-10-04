using NSubstitute;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.UseCases;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using Xunit;

namespace TaskFlow.Tests.Application.Tarefas
{
    public class BuscarTarefaUseCaseTests
    {
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly BuscarTarefaUseCase _useCase;

        public BuscarTarefaUseCaseTests()
        {
            _useCase = new BuscarTarefaUseCase(_projetoRepository);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ExecuteAsync_TarefaIdInvalido_LancaDomainException(int tarefaId)
        {
            var command = new BuscarTarefaCommand(UsuarioId: 1, ProjetoId: 10, TarefaId: tarefaId);

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));
        }

        [Fact]
        public async Task ExecuteAsync_TarefaNaoEncontrada_LancaDomainException()
        {
            _projetoRepository.ObterTarefaProjeto(10, 5).Returns(Task.FromResult<Tarefa>(null));
            var command = new BuscarTarefaCommand(UsuarioId: 1, ProjetoId: 10, TarefaId: 5);

            var excecao = await Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(command));

            Assert.Contains("Tarefa não encontrada", excecao.Message);
        }

        [Fact]
        public async Task ExecuteAsync_TarefaEncontrada_RetornaDtoComDadosMapeados()
        {
            var tarefa = new Tarefa("Tarefa 1", "Descrição da tarefa", PrioridadeTarefa.Alta);
            _projetoRepository.ObterTarefaProjeto(10, 5).Returns(Task.FromResult(tarefa));
            var command = new BuscarTarefaCommand(UsuarioId: 1, ProjetoId: 10, TarefaId: 5);

            var dto = await _useCase.ExecuteAsync(command);

            Assert.Equal("Tarefa 1", dto.Titulo);
            Assert.Equal("Descrição da tarefa", dto.Descricao);
            Assert.Equal(StatusTarefa.Pendente, dto.Status);
            Assert.Equal(PrioridadeTarefa.Alta, dto.Prioridade);
        }
    }
}
