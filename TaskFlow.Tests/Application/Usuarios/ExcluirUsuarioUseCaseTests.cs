using NSubstitute;
using TaskFlow.Application.Usuarios.UseCases;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using Xunit;

namespace TaskFlow.Tests.Application.Usuarios
{
    public class ExcluirUsuarioUseCaseTests
    {
        private readonly IUsuarioRepository _usuarioRepository = Substitute.For<IUsuarioRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly ExcluirUsuarioUseCase _useCase;

        public ExcluirUsuarioUseCaseTests()
        {
            _useCase = new ExcluirUsuarioUseCase(_usuarioRepository, _unitOfWork);
        }

        [Fact]
        public async Task ExecuteAsync_UsuarioNaoEncontrado_LancaDomainException()
        {
            _usuarioRepository.ObterPorIdAsync(99).Returns(Task.FromResult<Usuario>(null));

            var excecao = await Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(99));

            Assert.Contains("não encontrado", excecao.Message);
            await _usuarioRepository.DidNotReceive().ExcluirUsuario(Arg.Any<int>());
        }

        [Fact]
        public async Task ExecuteAsync_Valido_ChamaExclusaoEPersiste()
        {
            var usuario = new Usuario("Ana Souza", "ana@teste.com", "senha12345");
            _usuarioRepository.ObterPorIdAsync(1).Returns(Task.FromResult(usuario));

            await _useCase.ExecuteAsync(1);

            await _usuarioRepository.Received(1).ExcluirUsuario(1);
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
