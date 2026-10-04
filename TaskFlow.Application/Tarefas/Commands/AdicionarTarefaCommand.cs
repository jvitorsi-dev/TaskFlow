using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Tarefas.Commands
{
    public record AdicionarTarefaCommand
    ( 
        int UsuarioId,
        int ProjetoId,
        string Titulo,
        string Descricao,
        PrioridadeTarefa Prioridade
    );
}
