using NSubstitute;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;
using TaskFlow.Tests.TestSupport;
using Xunit;

namespace TaskFlow.Tests.Application.Tarefas
{
    // Observação: a classe EditarTarefaUseCase está declarada no namespace global.
    public class EditarTarefaUseCaseTests
    {
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly EditarTarefaUseCase _useCase;

        public EditarTarefaUseCaseTests()
        {
            _useCase = new EditarTarefaUseCase(_projetoRepository, _unitOfWork);
        }

        private static EditarTarefaCommand Command(
            int tarefaId = 5,
            string titulo = "Título novo",
            string descricao = null,
            PrioridadeTarefa? prioridade = PrioridadeTarefa.Alta)
            => new(Id: tarefaId, ProjetoId: 10, UsuarioId: 1, Titulo: titulo, Descricao: descricao,
                   Status: null, Prioridade: prioridade, Data: default);

        [Fact]
        public async Task ExecuteAsync_ProjetoNaoEncontrado_LancaNotFoundException()
        {
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult<Projeto>(null));

            await Assert.ThrowsAsync<NotFoundException>(() => _useCase.ExecuteAsync(Command()));

            await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_TarefaNaoEncontradaNoProjeto_LancaNotFoundException()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));

            await Assert.ThrowsAsync<NotFoundException>(
                () => _useCase.ExecuteAsync(Command(tarefaId: 999)));
        }

        [Fact]
        public async Task ExecuteAsync_Valido_RetornaDtoAtualizadoEPersiste()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            var tarefa = new Tarefa("Tarefa original", "Descrição original", PrioridadeTarefa.Baixa)
                .ComId(5);
            projeto.AdicionarTarefa(tarefa);
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));

            var dto = await _useCase.ExecuteAsync(Command());

            Assert.Equal("Título novo", dto.Titulo);
            Assert.Equal("Descrição original", dto.Descricao); // null mantém o valor atual
            Assert.Equal(PrioridadeTarefa.Alta, dto.Prioridade);
            await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ExecuteAsync_ProjetoConcluido_LancaDomainException()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            var tarefa = new Tarefa("Tarefa original", "Descrição original", PrioridadeTarefa.Baixa)
                .ComId(5);
            projeto.AdicionarTarefa(tarefa);
            projeto.Iniciar();
            projeto.Concluir();
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult(projeto));

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(Command()));
        }
    }
}
