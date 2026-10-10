using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Tarefas.Events
{
    public record TarefaConcluidaEvent(
        int TarefaId,
        int ProjetoId,
        int UsuarioId,
        string Titulo,
        DateTime ConcluidaEmUtc
    );
}
