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
    public class AdicionarTarefaUseCaseTests
    {
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly AdicionarTarefaUseCase _useCase;

        public AdicionarTarefaUseCaseTests()
        {
            _useCase = new AdicionarTarefaUseCase(_projetoRepository, _unitOfWork);
        }

        private static AdicionarTarefaCommand Command(
            string titulo = "Tarefa nova",
            string descricao = "Descrição da tarefa nova",
            PrioridadeTarefa prioridade = PrioridadeTarefa.Media)
            => new(UsuarioId: 1, ProjetoId: 10, Titulo: titulo, Descricao: descricao, Prioridade: prioridade);

        [Fact]
        public async Task ExecuteAsync_ProjetoNaoEncontrado_LancaNotFoundException()
        {
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult<Projeto>(null));

            await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(Command()));

            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_Valido_AdicionaTarefaAoProjetoERetornaDto()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));

            var dto = await _useCase.ExecuteAsync(Command(prioridade: PrioridadeTarefa.Alta));

            Assert.Equal("Tarefa nova", dto.Titulo);
            Assert.Equal("Descrição da tarefa nova", dto.Descricao);
            Assert.Equal(StatusTarefa.Pendente, dto.Status);
            Assert.Equal(PrioridadeTarefa.Alta, dto.Prioridade);
            Assert.Single(projeto.Tarefas);
        }

        [Fact]
        public async Task ExecuteAsync_Valido_PersisteAsAlteracoes()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));

            await _useCase.ExecuteAsync(Command());

            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_TituloDuplicadoNoProjeto_LancaDomainException()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            projeto.AdicionarTarefa(new Tarefa("Tarefa existente", "Descrição da tarefa", PrioridadeTarefa.Baixa));
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));

            await Assert.ThrowsAsync<DomainException>(
                () => _useCase.ExecuteAsync(Command(titulo: "TAREFA EXISTENTE")));
        }

        [Fact]
        public async Task ExecuteAsync_ProjetoConcluido_LancaDomainException()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            projeto.Iniciar();
            projeto.Concluir();
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(Command()));
        }
    }
}
