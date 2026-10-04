using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Application.Projetos.DTOs;
using TaskFlow.Application.Projetos.UseCases;

public class ProjetoService : IProjetoService
{
    private readonly AdicionarProjetoUseCase _criar;
    private readonly BuscarProjetoUseCase _buscar;
    private readonly EditarProjetoUseCase _editar;
    private readonly ExcluirProjetoUseCase _excluir;
    private readonly ListarProjetosUseCase _listar;

    public ProjetoService(
        AdicionarProjetoUseCase criar,
        BuscarProjetoUseCase buscar,
        EditarProjetoUseCase editar,
        ExcluirProjetoUseCase excluir,
        ListarProjetosUseCase listar)
    {
        _criar = criar;
        _buscar = buscar;
        _editar = editar;
        _excluir = excluir;
        _listar = listar;
    }

    public Task<ProjetoDTO> AdicionarProjetoAsync(AdicionarProjetoCommand request)
        => _criar.ExecuteAsync(request);

    public Task<ProjetoDTO> ObterProjetoPorIdAsync(BuscarProjetoCommand command)
        => _buscar.ExecuteAsync(command);

    public Task<ProjetoDTO> EditarProjetoAsync(EditarProjetoCommand request)
        => _editar.ExecuteAsync(request);

    public Task ExcluirProjetoAsync(BuscarProjetoCommand command)
        => _excluir.ExecuteAsync(command);

    public Task<IEnumerable<ProjetoDTO>> ListarTodosProjetosAsync()
        => _listar.ExecuteAsync();
}