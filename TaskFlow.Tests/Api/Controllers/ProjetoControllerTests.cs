using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.DTOs;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Domain.Enums;
using Xunit;

namespace TaskFlow.Tests.Api.Controllers
{
    public class ProjetoControllerTests
    {
        private readonly IProjetoService _projetoService = Substitute.For<IProjetoService>();
        private readonly ProjetoController _controller;

        public ProjetoControllerTests()
        {
            _controller = new ProjetoController(_projetoService);
        }

        private static ProjetoDTO CriarDto()
            => new(1, "Projeto de Testes", "Descrição do projeto",
                   StatusProjeto.Planejado, DataCriacao: default, Tarefas: new List<TarefaDTO>());

        [Fact]
        public async Task ListarTodos_ServicoRetornaLista_RetornaOkComLista()
        {
            var projetos = new[] { CriarDto() };
            _projetoService.ListarTodosProjetosAsync()
                .Returns(Task.FromResult<IEnumerable<ProjetoDTO>>(projetos));

            var resultado = await _controller.ListarTodos();

            var ok = Assert.IsType<OkObjectResult>(resultado.Result);
            var lista = Assert.IsAssignableFrom<IEnumerable<ProjetoDTO>>(ok.Value);
            Assert.Single(lista);
        }

        [Fact]
        public async Task ObterPorId_ServicoRetornaDto_RetornaOk()
        {
            var dto = CriarDto();
            _projetoService.ObterProjetoPorIdAsync(Arg.Any<BuscarProjetoCommand>())
                .Returns(Task.FromResult(dto));

            var resultado = await _controller.ObterPorId(idProjeto: 1, idUsuario: 2);

            var ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task ObterPorId_ServicoRetornaNulo_RetornaNotFound()
        {
            _projetoService.ObterProjetoPorIdAsync(Arg.Any<BuscarProjetoCommand>())
                .Returns(Task.FromResult<ProjetoDTO>(null));

            var resultado = await _controller.ObterPorId(idProjeto: 99, idUsuario: 2);

            Assert.IsType<NotFoundResult>(resultado.Result);
        }

        [Fact]
        public async Task Adicionar_Valido_RetornaCreatedAtActionComDto()
        {
            var command = new AdicionarProjetoCommand("Projeto Novo", "Descrição do projeto", 1);
            var dto = CriarDto();
            _projetoService.AdicionarProjetoAsync(command).Returns(Task.FromResult(dto));

            var resultado = await _controller.Adicionar(command);

            var created = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            Assert.Equal(nameof(ProjetoController.ObterPorId), created.ActionName);
            Assert.Equal(dto, created.Value);
        }

        [Fact]
        public async Task Editar_Valido_RetornaOkComDto()
        {
            var command = new EditarProjetoCommand(1, "Nome novo", "Descrição nova", null, null, 2);
            var dto = CriarDto();
            _projetoService.EditarProjetoAsync(command).Returns(Task.FromResult(dto));

            var resultado = await _controller.Editar(command);

            var ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task Excluir_Valido_RetornaNoContentEChamaServico()
        {
            var resultado = await _controller.Excluir(idProjeto: 1, idUsuario: 2);

            Assert.IsType<NoContentResult>(resultado);
            await _projetoService.Received(1).ExcluirProjetoAsync(Arg.Any<BuscarProjetoCommand>());
        }
    }
}
