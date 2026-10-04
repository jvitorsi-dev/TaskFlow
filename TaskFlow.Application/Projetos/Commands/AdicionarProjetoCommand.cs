using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Projetos.Commands
{
    public record AdicionarProjetoCommand(
        string Nome,
        string Descricao,
        int UsuarioId
    );
}
