using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Tarefas.Commands
{
    public record EditarTarefaCommand
    (
        int Id,
        int ProjetoId,
        int UsuarioId,
        string? Titulo,
        string? Descricao,
        StatusTarefa? Status,
        PrioridadeTarefa? Prioridade,
        DateOnly Data
    );
}
