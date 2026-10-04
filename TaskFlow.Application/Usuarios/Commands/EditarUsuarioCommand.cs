using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Usuarios.Commands
{
    public record EditarUsuarioCommand(
        int Id,
        string Nome,
        string Email,
        string Senha
    );  
}
