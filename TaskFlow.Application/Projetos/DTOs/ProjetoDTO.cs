using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Projetos.DTOs
{
    public record ProjetoDTO(
        int Id,
        int UsuarioId,
        string Nome,
        string Descricao,
        StatusProjeto Status,
        DateOnly DataCriacao,
        List<TarefaDTO> Tarefas
    );
}
