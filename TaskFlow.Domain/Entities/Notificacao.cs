using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities
{
    public class Notificacao
    {
        public int Id { get; private set; }
        public int UsuarioId { get; private set; }
        public int TarefaId { get; private set; }
        public string Mensagem { get; private set; } = string.Empty;
        public DateTime CriadaEmUtc { get; private set; }

        private Notificacao() { } // Construtor privado para Entity Framework

        public Notificacao(int usuarioId, int tarefaId, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem))
                throw new DomainException("A mensagem da notificação não pode ser vazia.");

            UsuarioId = usuarioId;
            TarefaId = tarefaId;
            Mensagem = mensagem;
            CriadaEmUtc = DateTime.UtcNow;
        }
    }
}