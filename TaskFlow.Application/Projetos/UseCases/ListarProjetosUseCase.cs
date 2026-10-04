using TaskFlow.Application.Projetos.DTOs;
using TaskFlow.Application.Tarefas.DTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projetos.UseCases
{
    public class ListarProjetosUseCase
    {
        private readonly IProjetoRepository _projetoRepository;

        public ListarProjetosUseCase(IProjetoRepository projetoRepository)
        {
            _projetoRepository = projetoRepository;
        }

        public async Task<IEnumerable<ProjetoDTO>> ExecuteAsync()
        {
            var projetos = await _projetoRepository.ListarTodosAsync();
            return projetos.Select(p => new ProjetoDTO
            (
                p.Id,
                p.Nome,
                p.Descricao,
                p.Status,
                p.DataCriacao,
                p.Tarefas.Select(t => new TarefaDTO(
                    t.Id,
                    t.Titulo,
                    t.Descricao,
                    t.Status,
                    t.Prioridade,
                    t.DataCriacao,
                    t.DataInicio,
                    t.DataConclusao
                )).ToList()
            ));
        }
    }
}
