using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.DTOs;
using TaskFlow.Domain.Entities;
// Inclua aqui os namespaces de IProjetoService, ProjetoDTO e dos Commands.

[ApiController]
[Route("api/[controller]")]
public class ProjetoController : ControllerBase
{
    private readonly IProjetoService _projetoService;

    public ProjetoController(IProjetoService projetoService)
    {
        _projetoService = projetoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjetoDTO>>> ListarTodos()
    {
        var projetos = await _projetoService.ListarTodosProjetosAsync();
        return Ok(projetos);
    }

    [HttpGet("{idUsuario}/{idProjeto}")]
    public async Task<ActionResult<ProjetoDTO>> ObterPorId(int idUsuario, int idProjeto)
    {
        var command = new BuscarProjetoCommand(idUsuario, idProjeto);
        var projeto = await _projetoService.ObterProjetoPorIdAsync(command);

        if (projeto is null)
            return NotFound();

        return Ok(projeto);
    }

    [HttpPost]
    public async Task<ActionResult<ProjetoDTO>> Adicionar(
        [FromBody] AdicionarProjetoCommand command)
    {
        var projeto = await _projetoService.AdicionarProjetoAsync(command);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = projeto.Id },
            projeto);
    }

    [HttpPut]
    public async Task<ActionResult<ProjetoDTO>> Editar(
        [FromBody] EditarProjetoCommand command)
    {
        var projeto = await _projetoService.EditarProjetoAsync(command);

        if (projeto is null)
            return NotFound();

        return Ok(projeto);
    }

    [HttpDelete("{idProjeto}/{idUsuario}")]
    public async Task<IActionResult> Excluir(int idProjeto, int idUsuario)
    {
        var command = new BuscarProjetoCommand(idUsuario, idProjeto);
        await _projetoService.ExcluirProjetoAsync(command);

        return NoContent();
    }
}