using NSubstitute;
using TaskFlow.Application.Usuarios.Commands;
using TaskFlow.Application.Usuarios.UseCases;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using TaskFlow.Tests.TestSupport;
using Xunit;

namespace TaskFlow.Tests.Application.Usuarios
{
    public class EditarUsuarioUseCaseTests
    {
        private readonly IUsuarioRepository _usuarioRepository = Substitute.For<IUsuarioRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly EditarUsuarioUseCase _useCase;

        public EditarUsuarioUseCaseTests()
        {
            _useCase = new EditarUsuarioUseCase(_usuarioRepository, _unitOfWork);
        }

        [Fact]
        public async Task ExecuteAsync_UsuarioNaoEncontrado_LancaDomainException()
        {
            _usuarioRepository.ObterPorIdAsync(99).Returns(Task.FromResult<Usuario>(null));
            var command = new EditarUsuarioCommand(99, "Ana Souza", "ana@teste.com", "senha12345");

            var excecao = await Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(command));

            Assert.Contains("não encontrado", excecao.Message);
        }

        [Fact]
        public async Task ExecuteAsync_EmailJaUsadoPorOutroUsuario_LancaDomainException()
        {
            var usuario = new Usuario("Ana Souza", "ana@teste.com", "senha12345").ComId(1);
            var outroUsuario = new Usuario("Carlos Lima", "carlos@teste.com", "senha12345").ComId(2);
            _usuarioRepository.ObterPorIdAsync(1).Returns(Task.FromResult(usuario));
            _usuarioRepository.ObterPorEmailAsync("carlos@teste.com").Returns(Task.FromResult(outroUsuario));
            var command = new EditarUsuarioCommand(1, "Ana Souza", "carlos@teste.com", "senha12345");

            var excecao = await Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(command));

            Assert.Contains("Já existe um usuário", excecao.Message);
        }

        [Fact]
        public async Task ExecuteAsync_MantendoOProprioEmail_PermiteAEdicao()
        {
            var usuario = new Usuario("Ana Souza", "ana@teste.com", "senha12345").ComId(1);
            _usuarioRepository.ObterPorIdAsync(1).Returns(Task.FromResult(usuario));
            _usuarioRepository.ObterPorEmailAsync("ana@teste.com").Returns(Task.FromResult(usuario));
            var command = new EditarUsuarioCommand(1, "Ana Souza Lima", "ana@teste.com", "senha12345");

            var dto = await _useCase.ExecuteAsync(command);

            Assert.Equal("Ana Souza Lima", dto.Nome);
            Assert.Equal("ana@teste.com", dto.Email);
        }

        [Fact]
        public async Task ExecuteAsync_Valido_AtualizaDadosEPersiste()
        {
            var usuario = new Usuario("Ana Souza", "ana@teste.com", "senha12345").ComId(1);
            _usuarioRepository.ObterPorIdAsync(1).Returns(Task.FromResult(usuario));
            var command = new EditarUsuarioCommand(1, "Ana Souza", "novo@teste.com", "novasenha123");

            var dto = await _useCase.ExecuteAsync(command);

            Assert.Equal("novo@teste.com", dto.Email);
            Assert.Equal("Ana Souza", usuario.Nome);
            Assert.Equal("novasenha123", usuario.Senha);
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_SenhaInvalida_LancaDomainException()
        {
            var usuario = new Usuario("Ana Souza", "ana@teste.com", "senha12345").ComId(1);
            _usuarioRepository.ObterPorIdAsync(1).Returns(Task.FromResult(usuario));
            var command = new EditarUsuarioCommand(1, "Ana Souza", "ana@teste.com", "123"); // < 8 caracteres

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));

            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
