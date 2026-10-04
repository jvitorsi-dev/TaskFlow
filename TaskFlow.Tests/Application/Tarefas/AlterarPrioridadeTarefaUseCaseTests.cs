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
    public class AlterarPrioridadeTarefaUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly AlterarPrioridadeTarefaUseCase _useCase;

        public AlterarPrioridadeTarefaUseCaseTests()
        {
            _useCase = new AlterarPrioridadeTarefaUseCase(_unitOfWork, _projetoRepository);
        }

        [Fact]
        public async Task AlterarPrioridadeTarefa_TarefaNaoEncontrada_LancaNotFoundException()
        {
            _projetoRepository.ObterTarefaProjeto(10, 5).Returns(Task.FromResult<Tarefa>(null));
            var command = new AlterarPrioridadeTarefaCommand(10, 5, PrioridadeTarefa.Alta);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _useCase.AlterarPrioridadeTarefa(command));
        }

        [Theory]
        [InlineData(PrioridadeTarefa.Baixa)]
        [InlineData(PrioridadeTarefa.Media)]
        [InlineData(PrioridadeTarefa.Alta)]
        public async Task AlterarPrioridadeTarefa_Valido_AtualizaPrioridadeEPersiste(PrioridadeTarefa prioridade)
        {
            var tarefa = new Tarefa("Tarefa 1", "Descrição da tarefa", PrioridadeTarefa.Media);
            _projetoRepository.ObterTarefaProjeto(10, 5).Returns(Task.FromResult(tarefa));
            var command = new AlterarPrioridadeTarefaCommand(10, 5, prioridade);

            await _useCase.AlterarPrioridadeTarefa(command);

            Assert.Equal(prioridade, tarefa.Prioridade);
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
