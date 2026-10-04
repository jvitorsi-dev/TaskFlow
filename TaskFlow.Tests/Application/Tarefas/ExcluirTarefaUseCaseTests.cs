using NSubstitute;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.UseCases;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using TaskFlow.Tests.TestSupport;
using Xunit;

namespace TaskFlow.Tests.Application.Tarefas
{
    public class ExcluirTarefaUseCaseTests
    {
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly ExcluirTarefaUseCase _useCase;

        public ExcluirTarefaUseCaseTests()
        {
            _useCase = new ExcluirTarefaUseCase(_projetoRepository, _unitOfWork);
        }

        [Fact]
        public async Task ExecuteAsync_ProjetoNaoEncontrado_LancaNotFoundException()
        {
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult<Projeto>(null));
            var command = new BuscarTarefaCommand(UsuarioId: 1, ProjetoId: 10, TarefaId: 5);

            await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(command));

            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_TarefaNaoEncontrada_LancaNotFoundException()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));
            var command = new BuscarTarefaCommand(UsuarioId: 1, ProjetoId: 10, TarefaId: 999);

            await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(command));
        }

        [Fact]
        public async Task ExecuteAsync_Valido_RemoveTarefaDoProjetoEPersiste()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            projeto.AdicionarTarefa(
                new Tarefa("Tarefa 1", "Descrição da tarefa", PrioridadeTarefa.Media).ComId(5));
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));
            var command = new BuscarTarefaCommand(UsuarioId: 1, ProjetoId: 10, TarefaId: 5);

            await _useCase.ExecuteAsync(command);

            Assert.Empty(projeto.Tarefas);
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
