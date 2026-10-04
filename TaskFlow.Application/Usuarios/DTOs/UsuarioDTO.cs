using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Usuarios.DTOs
{
    public record UsuarioDTO(
        int Id,
        string Nome,
        string Email
    );
}
