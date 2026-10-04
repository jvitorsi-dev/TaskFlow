using NSubstitute;
using TaskFlow.Application.Usuarios.UseCases;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using Xunit;

namespace TaskFlow.Tests.Application.Usuarios
{
    public class BuscarUsuarioUseCaseTests
    {
        private readonly IUsuarioRepository _usuarioRepository = Substitute.For<IUsuarioRepository>();
        private readonly BuscarUsuarioUseCase _useCase;

        public BuscarUsuarioUseCaseTests()
        {
            _useCase = new BuscarUsuarioUseCase(_usuarioRepository);
        }

        [Fact]
        public async Task ExecuteAsync_UsuarioNaoEncontrado_LancaNotFoundException()
        {
            _usuarioRepository.ObterPorIdAsync(99).Returns(Task.FromResult<Usuario>(null));

            var excecao = await Assert.ThrowsAsync<NotFoundException>(
                () => _useCase.ExecuteAsync(99));

            Assert.Contains("99", excecao.Message);
        }

        [Fact]
        public async Task ExecuteAsync_UsuarioEncontrado_RetornaDtoComNomeEEmail()
        {
            var usuario = new Usuario("Ana Souza", "ana@teste.com", "senha12345");
            _usuarioRepository.ObterPorIdAsync(1).Returns(Task.FromResult(usuario));

            var dto = await _useCase.ExecuteAsync(1);

            Assert.Equal("Ana Souza", dto.Nome);
            Assert.Equal("ana@teste.com", dto.Email);
        }
    }
}
