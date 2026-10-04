// TaskFlow.Domain/Entities/Projeto.cs
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities
{
    public class Projeto
    {
        public int Id { get; private set; }
        public int UsuarioId { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public StatusProjeto Status { get; private set; } = StatusProjeto.Planejado;
        public DateOnly DataCriacao { get; private set; }
        public DateOnly? DataInicio { get; private set; }
        public DateOnly? DataConclusao { get; private set; }

        private readonly List<Tarefa> _tarefas = new();
        public IReadOnlyCollection<Tarefa> Tarefas => _tarefas.AsReadOnly();

        private Projeto() { } // EF Core

        public Projeto(string nome, string descricao, int usuarioId)
        {
            Validar(nome, descricao, usuarioId);

            Nome = nome;
            Descricao = descricao;
            UsuarioId = usuarioId;
            Status = StatusProjeto.Planejado;
            DataCriacao = DateOnly.FromDateTime(DateTime.Now);
        }

        public void Iniciar()
        {
            if (Status != StatusProjeto.Planejado)
                throw new DomainException(
                    "O projeto só pode ser iniciado se estiver no status 'Planejado'.");

            Status = StatusProjeto.EmAndamento;
            DataInicio = DateOnly.FromDateTime(DateTime.Now);
        }

        public void Concluir()
        {
            if (Status != StatusProjeto.EmAndamento)
                throw new DomainException(
                    "O projeto só pode ser concluído se estiver no status 'Em Andamento'.");

            Status = StatusProjeto.Concluido;
            DataConclusao = DateOnly.FromDateTime(DateTime.Now);
        }



        public void Editar(string? nome, string? descricao, int usuarioId)
        {
            if (Status == StatusProjeto.Concluido)
                throw new DomainException(
                    "Não é possível editar um projeto concluído.");

            var novoNome = nome ?? Nome;
            var novaDescricao = descricao ?? Descricao;

            Validar(novoNome, novaDescricao, usuarioId);

            Nome = novoNome;
            Descricao = novaDescricao;
        }

        private static void Validar(string nome, string descricao, int usuarioId)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do projeto não pode ser nulo ou vazio.");

            if (nome.Length < 3 || nome.Length > 100)
                throw new DomainException("O nome do projeto deve ter entre 3 e 100 caracteres.");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new DomainException("A descrição do projeto não pode ser nula ou vazia.");

            if (descricao.Length < 3 || descricao.Length > 500)
                throw new DomainException("A descrição do projeto deve ter entre 3 e 500 caracteres.");

            if (usuarioId <= 0)
                throw new DomainException("O Id do usuário deve ser maior que zero.");
        }

        //TAREFAS

        public void AdicionarTarefa(Tarefa tarefa)
        {
            if (tarefa is null)
                throw new DomainException("A tarefa não pode ser nula.");

            if (Status == StatusProjeto.Concluido)
                throw new DomainException(
                    "Não é possível adicionar tarefas a um projeto concluído.");

            if (_tarefas.Any(t => t.Titulo.Equals(tarefa.Titulo, StringComparison.OrdinalIgnoreCase)))
                throw new DomainException(
                    "Já existe uma tarefa com esse título neste projeto.");

            _tarefas.Add(tarefa);
        }

        public void EditarTarefa(int tarefaId, string? titulo, string? descricao, PrioridadeTarefa? prioridade)
        {
            if (Status == StatusProjeto.Concluido)
                throw new DomainException("Não é possível editar tarefas de um projeto concluído.");

            var tarefa = _tarefas.FirstOrDefault(t => t.Id == tarefaId)
                ?? throw new NotFoundException($"Tarefa com Id {tarefaId} não encontrada no projeto.");

            tarefa.Editar(titulo, descricao, prioridade);
        }

        public void ExcluirTarefa(int tarefaId)
        {
            if (Status == StatusProjeto.Concluido)
                throw new DomainException("Não é possível excluir tarefas de um projeto concluído.");
            var tarefa = _tarefas.FirstOrDefault(t => t.Id == tarefaId)
                ?? throw new NotFoundException($"Tarefa com Id {tarefaId} não encontrada no projeto.");
            _tarefas.Remove(tarefa);
        }
    }
}