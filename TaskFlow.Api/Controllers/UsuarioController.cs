using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Usuarios.Commands;
using TaskFlow.Application.Usuarios.DTOs;
// Inclua os namespaces de IUsuarioService, UsuarioDTO e dos Commands.

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet("{usuarioId}")]
    public async Task<ActionResult<UsuarioDTO>> ObterPorId(int usuarioId)
    {
        var usuario = await _usuarioService.ObterUsuarioPorIdAsync(usuarioId);

        if (usuario is null)
            return NotFound();

        return Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult<UsuarioDTO>> Adicionar(
        [FromBody] AdicionarUsuarioCommand request)
    {
        var usuario = await _usuarioService.AdicionarUsuarioAsync(request);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { usuarioId = usuario.Id },
            usuario);
    }

    [HttpPut]
    public async Task<ActionResult<UsuarioDTO>> Editar(
        [FromBody] EditarUsuarioCommand request)
    {
        var usuario = await _usuarioService.EditarUsuarioAsync(request);

        if (usuario is null)
            return NotFound();

        return Ok(usuario);
    }

    [HttpDelete("{usuarioId}")]
    public async Task<IActionResult> Excluir(int usuarioId)
    {
        await _usuarioService.ExcluirUsuarioAsync(usuarioId);

        return Ok();
    }
}