using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Usuarios.Commands;
using TaskFlow.Application.Usuarios.DTOs;
using Xunit;

namespace TaskFlow.Tests.Api.Controllers
{
    public class UsuarioControllerTests
    {
        private readonly IUsuarioService _usuarioService = Substitute.For<IUsuarioService>();
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _controller = new UsuarioController(_usuarioService);
        }

        [Fact]
        public async Task ObterPorId_ServicoRetornaDto_RetornaOkComDto()
        {
            var dto = new UsuarioDTO(1, "Ana Souza", "ana@teste.com");
            _usuarioService.ObterUsuarioPorIdAsync(1).Returns(Task.FromResult(dto));

            var resultado = await _controller.ObterPorId(1);

            var ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Equal(dto, Assert.IsType<UsuarioDTO>(ok.Value));
        }

        [Fact]
        public async Task ObterPorId_ServicoRetornaNulo_RetornaNotFound()
        {
            _usuarioService.ObterUsuarioPorIdAsync(99).Returns(Task.FromResult<UsuarioDTO>(null));

            var resultado = await _controller.ObterPorId(99);

            Assert.IsType<NotFoundResult>(resultado.Result);
        }

        [Fact]
        public async Task Adicionar_Valido_RetornaCreatedAtActionComDto()
        {
            var command = new AdicionarUsuarioCommand("Ana Souza", "ana@teste.com", "senha12345");
            var dto = new UsuarioDTO(1, "Ana Souza", "ana@teste.com");
            _usuarioService.AdicionarUsuarioAsync(command).Returns(Task.FromResult(dto));

            var resultado = await _controller.Adicionar(command);

            var created = Assert.IsType<CreatedAtActionResult>(resultado.Result);
            Assert.Equal(nameof(UsuarioController.ObterPorId), created.ActionName);
            Assert.Equal(dto, created.Value);
        }

        [Fact]
        public async Task Editar_ServicoRetornaDto_RetornaOk()
        {
            var command = new EditarUsuarioCommand(1, "Ana Souza", "ana@teste.com", "senha12345");
            var dto = new UsuarioDTO(1, "Ana Souza", "ana@teste.com");
            _usuarioService.EditarUsuarioAsync(command).Returns(Task.FromResult(dto));

            var resultado = await _controller.Editar(command);

            var ok = Assert.IsType<OkObjectResult>(resultado.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task Excluir_Valido_RetornaNoContentEChamaServico()
        {
            var resultado = await _controller.Excluir(1);

            Assert.IsType<NoContentResult>(resultado);
            await _usuarioService.Received(1).ExcluirUsuarioAsync(1);
        }
    }
}
