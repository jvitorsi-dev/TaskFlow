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
    public class AdicionarProjetoUseCaseTests
    {
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IUsuarioRepository _usuarioRepository = Substitute.For<IUsuarioRepository>();
        private readonly AdicionarProjetoUseCase _useCase;

        public AdicionarProjetoUseCaseTests()
        {
            _useCase = new AdicionarProjetoUseCase(_unitOfWork, _usuarioRepository);
        }

        [Fact]
        public async Task ExecuteAsync_UsuarioNaoEncontrado_LancaNotFoundException()
        {
            _usuarioRepository.ObterPorIdAsync(99).Returns(Task.FromResult<Usuario>(null));
            var command = new AdicionarProjetoCommand("Projeto Novo", "Descrição do projeto", 99);

            var excecao = await Assert.ThrowsAsync<NotFoundException>(
                () => _useCase.ExecuteAsync(command));

            Assert.Contains("Usuário não encontrado", excecao.Message);
        }

        [Fact]
        public async Task ExecuteAsync_Valido_RetornaDtoComDadosDoProjeto()
        {
            var usuario = new Usuario("Ana Souza", "ana@teste.com", "senha12345");
            _usuarioRepository.ObterPorIdAsync(1).Returns(Task.FromResult(usuario));
            var command = new AdicionarProjetoCommand("Projeto Novo", "Descrição do projeto", 1);

            var dto = await _useCase.ExecuteAsync(command);

            Assert.Equal("Projeto Novo", dto.Nome);
            Assert.Equal("Descrição do projeto", dto.Descricao);
            Assert.Equal(StatusProjeto.Planejado, dto.Status);
            Assert.Empty(dto.Tarefas);
        }

        [Fact]
        public async Task ExecuteAsync_Valido_PersisteAsAlteracoes()
        {
            var usuario = new Usuario("Ana Souza", "ana@teste.com", "senha12345");
            _usuarioRepository.ObterPorIdAsync(1).Returns(Task.FromResult(usuario));
            var command = new AdicionarProjetoCommand("Projeto Novo", "Descrição do projeto", 1);

            await _useCase.ExecuteAsync(command);

            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData("")]
        [InlineData("ab")]
        public async Task ExecuteAsync_NomeInvalido_LancaDomainException(string nome)
        {
            var command = new AdicionarProjetoCommand(nome, "Descrição do projeto", 1);

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));
        }

        [Fact]
        public async Task ExecuteAsync_DescricaoInvalida_LancaDomainException()
        {
            var command = new AdicionarProjetoCommand("Projeto Novo", "", 1);

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));
        }

        [Fact]
        public async Task ExecuteAsync_Valido_NaoPersisteQuandoUsuarioNaoExiste()
        {
            _usuarioRepository.ObterPorIdAsync(99).Returns(Task.FromResult<Usuario>(null));
            var command = new AdicionarProjetoCommand("Projeto Novo", "Descrição do projeto", 99);

            await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(command));

            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
