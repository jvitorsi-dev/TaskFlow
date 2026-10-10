using System.ComponentModel.DataAnnotations;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities
{
    public class Tarefa
    {
        public int Id { get; private set; }
        public int ProjetoId { get; private set; }
        public string Titulo { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public StatusTarefa Status { get; private set; } = StatusTarefa.Pendente;
        public PrioridadeTarefa Prioridade { get; private set; }
        public DateOnly DataCriacao { get; private set; } 
        public DateOnly? DataInicio { get; private set; }
        public DateOnly? DataConclusao { get; private set; }

        private Tarefa() { } // Construtor privado para Entity Framework}

        public Tarefa(string titulo, string descricao, PrioridadeTarefa prioridade)
        {
            Validar(titulo, descricao);

            Titulo = titulo;
            Descricao = descricao;
            Prioridade = prioridade;
            Status = StatusTarefa.Pendente;
            DataCriacao = DateOnly.FromDateTime(DateTime.Now);
        }

        public void Iniciar()
        {
            if (Status != StatusTarefa.Pendente)
                throw new DomainException(
                    "A tarefa só pode ser iniciada se estiver no status 'Pendente'.");
            Status = StatusTarefa.EmAndamento;
            DataInicio = DateOnly.FromDateTime(DateTime.Now);
        }

        public void Editar(string? titulo, string? descricao, PrioridadeTarefa? prioridade)
        {
            if (Status == StatusTarefa.Concluida)
                throw new DomainException("Não é possível editar uma tarefa concluída.");

            var novoTitulo = titulo ?? Titulo;
            var novaDescricao = descricao ?? Descricao;

            Validar(novoTitulo, novaDescricao);

            Titulo = novoTitulo;
            Descricao = novaDescricao;
            Prioridade = prioridade ?? Prioridade;
        }

        public void Concluir()
        {
            if (Status != StatusTarefa.EmAndamento)
                throw new DomainException(
                    "A tarefa só pode ser concluída se estiver no status 'Em Andamento'.");
            Status = StatusTarefa.Concluida;
            DataConclusao = DateOnly.FromDateTime(DateTime.Now);
        }

        public void AlterarPrioridade(PrioridadeTarefa novaPrioridade)
        {
            Prioridade = novaPrioridade;
        }

        private static void Validar(string nome, string descricao)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome da Tarefa não pode ser nulo ou vazio.");

            if (nome.Length < 3 || nome.Length > 100)
                throw new DomainException("O nome da Tarefa deve ter entre 3 e 100 caracteres.");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new DomainException("A descrição da Tarefa não pode ser nula ou vazia.");

            if (descricao.Length < 3 || descricao.Length > 500)
                throw new DomainException("A descrição da Tarefa deve ter entre 3 e 500 caracteres.");
        }
    }
}
