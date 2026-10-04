using NSubstitute;
using TaskFlow.Application.Usuarios.Commands;
using TaskFlow.Application.Usuarios.UseCases;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using Xunit;

namespace TaskFlow.Tests.Application.Usuarios
{
    public class AdicionarUsuarioUseCaseTests
    {
        private readonly IUsuarioRepository _usuarioRepository = Substitute.For<IUsuarioRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly AdicionarUsuarioUseCase _useCase;

        public AdicionarUsuarioUseCaseTests()
        {
            _useCase = new AdicionarUsuarioUseCase(_usuarioRepository, _unitOfWork);
        }

        [Fact]
        public async Task ExecuteAsync_EmailJaCadastrado_LancaDomainException()
        {
            var usuarioExistente = new Usuario("Carlos Lima", "ana@teste.com", "senha12345");
            _usuarioRepository.ObterPorEmailAsync("ana@teste.com").Returns(Task.FromResult(usuarioExistente));
            var command = new AdicionarUsuarioCommand("Ana Souza", "ana@teste.com", "senha12345");

            var excecao = await Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(command));

            Assert.Contains("Já existe um usuário", excecao.Message);
            await _usuarioRepository.DidNotReceive().AdicionarAsync(Arg.Any<Usuario>());
        }

        [Fact]
        public async Task ExecuteAsync_Valido_RetornaDtoComNomeEEmail()
        {
            var command = new AdicionarUsuarioCommand("Ana Souza", "ana@teste.com", "senha12345");

            var dto = await _useCase.ExecuteAsync(command);

            Assert.Equal("Ana Souza", dto.Nome);
            Assert.Equal("ana@teste.com", dto.Email);
        }

        [Fact]
        public async Task ExecuteAsync_Valido_AdicionaNoRepositorioEPersiste()
        {
            var command = new AdicionarUsuarioCommand("Ana Souza", "ana@teste.com", "senha12345");

            await _useCase.ExecuteAsync(command);

            await _usuarioRepository.Received(1)
                .AdicionarAsync(Arg.Is<Usuario>(u => u.Nome == "Ana Souza" && u.Email == "ana@teste.com"));
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_NomeVazio_LancaDomainException()
        {
            var command = new AdicionarUsuarioCommand("", "ana@teste.com", "senha12345");

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));

            await _usuarioRepository.DidNotReceive().AdicionarAsync(Arg.Any<Usuario>());
        }

        [Fact]
        public async Task ExecuteAsync_SenhaVazia_LancaDomainException()
        {
            var command = new AdicionarUsuarioCommand("Ana Souza", "ana@teste.com", "");

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));
        }
    }
}
