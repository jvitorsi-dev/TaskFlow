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
    public class IniciarTarefaUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly IniciarTarefaUseCase _useCase;

        public IniciarTarefaUseCaseTests()
        {
            _useCase = new IniciarTarefaUseCase(_unitOfWork, _projetoRepository);
        }

        [Fact]
        public async Task ExecuteAsync_TarefaNaoEncontrada_LancaNotFoundException()
        {
            _projetoRepository.ObterTarefaProjeto(10, 5).Returns(Task.FromResult<Tarefa>(null));
            var command = new BuscarTarefaCommand(UsuarioId: 1, ProjetoId: 10, TarefaId: 5);

            await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(command));
        }

        [Fact]
        public async Task ExecuteAsync_Valido_TarefaFicaEmAndamentoEPersiste()
        {
            var tarefa = new Tarefa("Tarefa 1", "Descrição da tarefa", PrioridadeTarefa.Media);
            _projetoRepository.ObterTarefaProjeto(10, 5).Returns(Task.FromResult(tarefa));
            var command = new BuscarTarefaCommand(UsuarioId: 1, ProjetoId: 10, TarefaId: 5);

            await _useCase.ExecuteAsync(command);

            Assert.Equal(StatusTarefa.EmAndamento, tarefa.Status);
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_TarefaJaEmAndamento_LancaDomainException()
        {
            var tarefa = new Tarefa("Tarefa 1", "Descrição da tarefa", PrioridadeTarefa.Media);
            tarefa.Iniciar();
            _projetoRepository.ObterTarefaProjeto(10, 5).Returns(Task.FromResult(tarefa));
            var command = new BuscarTarefaCommand(UsuarioId: 1, ProjetoId: 10, TarefaId: 5);

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));

            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
