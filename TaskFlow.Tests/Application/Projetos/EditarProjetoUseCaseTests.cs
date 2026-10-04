using NSubstitute;
using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.UseCases;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using Xunit;

namespace TaskFlow.Tests.Application.Projetos
{
    public class EditarProjetoUseCaseTests
    {
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly EditarProjetoUseCase _useCase;

        public EditarProjetoUseCaseTests()
        {
            _useCase = new EditarProjetoUseCase(_projetoRepository, _unitOfWork);
        }

        private static EditarProjetoCommand Command(string nome = "Nome novo", string descricao = "Descrição nova")
            => new(Id: 1, Nome: nome, Descricao: descricao, Status: null, dataConclusao: null, UsuarioId: 2);

        [Fact]
        public async Task ExecuteAsync_ProjetoNaoEncontrado_LancaNotFoundException()
        {
            _projetoRepository.ObterPorIdAsync(1, 2).Returns(Task.FromResult<Projeto>(null));

            await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(Command()));

            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_Valido_RetornaDtoAtualizado()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 2);
            _projetoRepository.ObterPorIdAsync(1, 2).Returns(Task.FromResult(projeto));

            var dto = await _useCase.ExecuteAsync(Command());

            Assert.Equal("Nome novo", dto.Nome);
            Assert.Equal("Descrição nova", dto.Descricao);
        }

        [Fact]
        public async Task ExecuteAsync_Valido_PersisteAsAlteracoes()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 2);
            _projetoRepository.ObterPorIdAsync(1, 2).Returns(Task.FromResult(projeto));

            await _useCase.ExecuteAsync(Command());

            await _projetoRepository.Received(1).EditarAsync(projeto);
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_ProjetoConcluido_LancaDomainException()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 2);
            projeto.Iniciar();
            projeto.Concluir();
            _projetoRepository.ObterPorIdAsync(1, 2).Returns(Task.FromResult(projeto));

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(Command()));
        }

        [Fact]
        public async Task ExecuteAsync_NomeInvalido_LancaDomainException()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 2);
            _projetoRepository.ObterPorIdAsync(1, 2).Returns(Task.FromResult(projeto));

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(Command(nome: "")));
        }
    }
}
