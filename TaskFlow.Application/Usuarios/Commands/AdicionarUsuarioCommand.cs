using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Usuarios.Commands
{
    public record AdicionarUsuarioCommand
    (
        string Nome,
        string Email,
        string Senha
    );
}
