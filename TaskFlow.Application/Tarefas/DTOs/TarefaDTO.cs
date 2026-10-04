using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Tarefas.DTOs
{
    public record TarefaDTO
    (
        int Id,
        string Titulo,
        string Descricao,
        StatusTarefa Status,
        PrioridadeTarefa Prioridade,
        DateOnly DataCriacao,
        DateOnly? DataInicio = null,
        DateOnly? DataConclusao = null
    );
}
