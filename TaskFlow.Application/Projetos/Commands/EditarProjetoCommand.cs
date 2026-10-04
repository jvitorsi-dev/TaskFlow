using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Projetos.Commands
{
    public record EditarProjetoCommand
    (
        int Id,
        string? Nome,
        string? Descricao,
        StatusProjeto? Status,
        DateOnly? dataConclusao,
        int UsuarioId
    );
}
