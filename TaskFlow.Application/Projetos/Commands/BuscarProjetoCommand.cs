using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Projetos.Commands
{
    public record BuscarProjetoCommand
    (
        int UsuarioId,
        int ProjetoId
    );
}
