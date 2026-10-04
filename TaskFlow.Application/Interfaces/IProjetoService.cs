using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Interfaces
{
    public interface IProjetoService
    {
        Task<IEnumerable<ProjetoDTO>> ListarTodosProjetosAsync();
        Task<ProjetoDTO> ObterProjetoPorIdAsync(BuscarProjetoCommand command);
        Task<ProjetoDTO> AdicionarProjetoAsync(AdicionarProjetoCommand projeto);
        Task<ProjetoDTO> EditarProjetoAsync(EditarProjetoCommand projeto);
        Task ExcluirProjetoAsync(BuscarProjetoCommand projeto);
    }
}
