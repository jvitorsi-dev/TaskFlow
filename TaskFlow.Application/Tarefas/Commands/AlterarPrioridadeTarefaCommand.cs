using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Tarefas.Commands
{
    public record AlterarPrioridadeTarefaCommand
    (
        int ProjetoId,
        int TarefaId,
        PrioridadeTarefa Prioridade
    );
}
