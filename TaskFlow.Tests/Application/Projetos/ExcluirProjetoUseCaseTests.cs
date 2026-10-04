using NSubstitute;
using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.UseCases;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using Xunit;

namespace TaskFlow.Tests.Application.Projetos
{
    public class ExcluirProjetoUseCaseTests
    {
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly ExcluirProjetoUseCase _useCase;

        public ExcluirProjetoUseCaseTests()
        {
            _useCase = new ExcluirProjetoUseCase(_projetoRepository, _unitOfWork);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ExecuteAsync_ProjetoIdInvalido_LancaDomainException(int projetoId)
        {
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: projetoId);

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));
        }

        [Fact]
        public async Task ExecuteAsync_ProjetoNaoEncontrado_LancaNotFoundException()
        {
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult<Projeto>(null));
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: 10);

            await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(command));

            await _projetoRepository.DidNotReceive().ExcluirAsync(Arg.Any<Projeto>());
            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_Valido_ChamaExclusaoEPersiste()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: 10);

            await _useCase.ExecuteAsync(command);

            await _projetoRepository.Received(1).ExcluirAsync(projeto);
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
