using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Application.Tarefas.DTOs;
// Inclua os namespaces de ITarefaService, TarefaDTO e dos Commands.

[ApiController]
[Route("api/[controller]")]
public class TarefaController : ControllerBase
{
    private readonly ITarefaService _tarefaService;

    public TarefaController(ITarefaService tarefaService)
    {
        _tarefaService = tarefaService;
    }

    [HttpGet("{idUsuario}/{idProjeto}/{idTarefa}")]
    public async Task<ActionResult<TarefaDTO>> ObterPorId(
        int idUsuario, int idProjeto, int idTarefa)
    {
        var command = new BuscarTarefaCommand(idUsuario, idProjeto, idTarefa);
        var tarefa = await _tarefaService.ObterTarefaPorIdAsync(command);

        if (tarefa is null)
            return NotFound();

        return Ok(tarefa);
    }

    [HttpPost]
    public async Task<ActionResult<TarefaDTO>> Adicionar(
        [FromBody] AdicionarTarefaCommand request)
    {
        var tarefa = await _tarefaService.AdicionarTarefaAsync(request);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = tarefa.Id },
            tarefa);
    }

    [HttpPut]
    public async Task<ActionResult<TarefaDTO>> Editar(
        [FromBody] EditarTarefaCommand request)
    {
        var tarefa = await _tarefaService.EditarTarefaAsync(request);

        if (tarefa is null)
            return NotFound();

        return Ok(tarefa);
    }

    [HttpDelete("{idUsuario}/{idProjeto}/{idTarefa}")]
    public async Task<IActionResult> Excluir(
        int idUsuario, int idProjeto, int idTarefa)
    {
        var command = new BuscarTarefaCommand(idUsuario, idProjeto, idTarefa);
        await _tarefaService.ExcluirTarefaAsync(command);

        return NoContent();
    }

    [HttpPost("iniciar/{idUsuario}/{idProjeto}/{idTarefa}")]
    public async Task<IActionResult> IniciarTarefa(
        int idUsuario, int idProjeto, int idTarefa)
    {
        var command = new BuscarTarefaCommand(idUsuario, idProjeto, idTarefa);
        await _tarefaService.IniciarTarefa(command);

        return Ok();
    }

    [HttpPost("concluir/{idUsuario}/{idProjeto}/{idTarefa}")]
    public async Task<IActionResult> ConcluirTarefa(
        int idUsuario, int idProjeto, int idTarefa)
    {
        var command = new BuscarTarefaCommand(idUsuario, idProjeto, idTarefa);
        await _tarefaService.ConcluirTarefa(command);

        return Ok();
    }
}