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
    public class BuscarProjetoUseCaseTests
    {
        private readonly IProjetoRepository _projetoRepository = Substitute.For<IProjetoRepository>();
        private readonly BuscarProjetoUseCase _useCase;

        public BuscarProjetoUseCaseTests()
        {
            _useCase = new BuscarProjetoUseCase(_projetoRepository);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ExecuteAsync_ProjetoIdInvalido_LancaDomainException(int projetoId)
        {
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: projetoId);

            await Assert.ThrowsAsync<DomainException>(() => _useCase.ExecuteAsync(command));
        }

        [Fact]
        public async Task ExecuteAsync_ProjetoNaoEncontrado_LancaNotFoundException()
        {
            _projetoRepository.ObterPorIdAsync(10, 1).Returns(Task.FromResult<Projeto>(null));
            var command = new BuscarProjetoCommand(UsuarioId: 1, ProjetoId: 10);

            var excecao = await Assert.ThrowsAsync<NotFoundException>(
                () => _useCase.ExecuteAsync(command));

            Assert.Contains("10", excecao.Message);
        }

        [Fact]
        public async Task ExecuteAsync_ProjetoEncontrado_RetornaDtoComDadosMapeados()
        {
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", 1);
            projeto.AdicionarTarefa(new Tarefa("Tarefa 1", "Descrição da tarefa", PrioridadeTarefa.Alta));
            _projetoRepository.ObterPorIdAsync(1, 2).Returns(Task.FromResult(projeto));
            var command = new BuscarProjetoCommand(UsuarioId: 2, ProjetoId: 1);

            var dto = await _useCase.ExecuteAsync(command);

            Assert.Equal("Projeto de Testes", dto.Nome);
            Assert.Equal("Descrição do projeto", dto.Descricao);
            Assert.Equal(StatusProjeto.Planejado, dto.Status);
            var tarefaDto = Assert.Single(dto.Tarefas);
            Assert.Equal("Tarefa 1", tarefaDto.Titulo);
            Assert.Equal(PrioridadeTarefa.Alta, tarefaDto.Prioridade);
        }
    }
}
