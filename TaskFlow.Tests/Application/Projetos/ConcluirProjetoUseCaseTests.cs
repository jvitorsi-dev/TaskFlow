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
    public class ConcluirProjetoUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly ConcluirProjetoUseCase _useCase;

        public ConcluirProjetoUseCaseTests()
        {
            _useCase = new ConcluirProjetoUseCase(_unitOfWork, _projetoRepository);
        }

        [Fact]
        public async Task ConcluirProjeto_ProjetoNaoEncontrado_LancaNotFoundException()
        {
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult<Projeto>(null));
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: 10);

            await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ConcluirProjeto(command));
        }

        [Fact]
        public async Task ConcluirProjeto_ProjetoEmAndamento_FicaConcluidoEPersiste()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            projeto.Iniciar();
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: 10);

            await _useCase.ConcluirProjeto(command);

            Assert.Equal(StatusProjeto.Concluido, projeto.Status);
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ConcluirProjeto_ProjetoPlanejado_LancaDomainException()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: 10);

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ConcluirProjeto(command));

            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
