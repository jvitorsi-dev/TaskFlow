using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Tarefas.Commands
{
    public record BuscarTarefaCommand
    (
        int UsuarioId,
        int ProjetoId,
        int TarefaId
    );    
}
