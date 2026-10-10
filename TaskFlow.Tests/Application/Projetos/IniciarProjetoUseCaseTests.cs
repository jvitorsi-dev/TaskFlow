using NSubstitute;
using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.UseCases;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using Xunit;

namespace TaskFlow.Tests.Application.Projetos
{
    public class IniciarProjetoUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly IniciarProjetoUseCase _useCase;

        public IniciarProjetoUseCaseTests()
        {
            _useCase = new IniciarProjetoUseCase(_unitOfWork, _projetoRepository);
        }

        [Fact]
        public async Task IniciarProjeto_ProjetoNaoEncontrado_LancaNotFoundException()
        {
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult<Projeto>(null));
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: 10);

            await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(command));
        }

        [Fact]
        public async Task IniciarProjeto_Valido_ProjetoFicaEmAndamentoEPersiste()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: 10);

            await _useCase.ExecuteAsync(command);

            Assert.Equal(StatusProjeto.EmAndamento, projeto.Status);
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task IniciarProjeto_ProjetoJaEmAndamento_LancaDomainException()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            projeto.Iniciar();
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: 10);

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));

            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
