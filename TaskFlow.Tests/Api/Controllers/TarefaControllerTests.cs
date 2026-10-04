using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Domain.Enums;
using Xunit;

namespace TaskFlow.Tests.Api.Controllers
{
    public class TarefaControllerTests
    {
        private readonly ITarefaService _tarefaService = Substitute.For<ITarefaService>();
        private readonly TarefaController _controller;

        public TarefaControllerTests()
        {
            _controller = new TarefaController(_tarefaService);
        }

        [Fact]
        public async Task ObterPorId_Valido_MontaCommandNaOrdemCorretaERetornaOk()
        {
            var dto = new TarefaDTO(5, "Tarefa 1", "Descrição da tarefa",
                StatusTarefa.Pendente, PrioridadeTarefa.Alta, DataCriacao: default);
            _tarefaService.ObterTarefaPorIdAsync(Arg.Any<BuscarTarefaCommand>())
                .Returns(Task.FromResult(dto));

            var resultado = await _controller.ObterPorId(idUsuario: 1, idProjeto: 2, idTarefa: 3);

            await _tarefaService.Received(1).ObterTarefaPorIdAsync(
                Arg.Is<BuscarTarefaCommand>(c =>
                    c.UsuarioId == 1 && c.ProjetoId == 2 && c.TarefaId == 3));

            var ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task ObterPorId_ServicoRetornaNulo_RetornaNotFound()
        {
            _tarefaService.ObterTarefaPorIdAsync(Arg.Any<BuscarTarefaCommand>())
                .Returns(Task.FromResult<TarefaDTO>(null));

            var resultado = await _controller.ObterPorId(1, 2, 99);

            Assert.IsType<NotFoundResult>(resultado.Result);
        }

        [Fact]
        public async Task Adicionar_Valido_RetornaCreatedAtActionComDto()
        {
            var command = new AdicionarTarefaCommand(1, 2, "Tarefa nova", "Descrição da tarefa", PrioridadeTarefa.Media);
            var dto = new TarefaDTO(5, "Tarefa nova", "Descrição da tarefa",
                StatusTarefa.Pendente, PrioridadeTarefa.Media, DataCriacao: default);
            _tarefaService.AdicionarTarefaAsync(command).Returns(Task.FromResult(dto));

            var resultado = await _controller.Adicionar(command);

            var created = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            Assert.Equal(nameof(TarefaController.ObterPorId), created.ActionName);
            Assert.Equal(dto, created.Value);
        }

        [Fact]
        public async Task Editar_Valido_RetornaOkComDto()
        {
            var command = new EditarTarefaCommand(5, 2, 1, "Título novo", null, null, PrioridadeTarefa.Alta, default);
            var dto = new TarefaDTO(5, "Título novo", "Descrição da tarefa",
                StatusTarefa.Pendente, PrioridadeTarefa.Alta, DataCriacao: default);
            _tarefaService.EditarTarefaAsync(command).Returns(Task.FromResult(dto));

            var resultado = await _controller.Editar(command);

            var ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task Excluir_Valido_RetornaNoContentEChamaServico()
        {
            var resultado = await _controller.Excluir(1, 2, 3);

            Assert.IsType<NoContentResult>(resultado);
            await _tarefaService.Received(1).ExcluirTarefaAsync(
                Arg.Is<BuscarTarefaCommand>(c =>
                    c.UsuarioId == 1 && c.ProjetoId == 2 && c.TarefaId == 3));
        }
    }
}
