using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Configurations
{
    public class NotificacaoConfiguration : IEntityTypeConfiguration<Notificacao>
    {
        public void Configure(EntityTypeBuilder<Notificacao> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Mensagem)
                   .IsRequired()
                   .HasMaxLength(300);

            // GARANTIA DE IDEMPOTÊNCIA: no máximo UMA notificação por tarefa.
            // Se alguém tentar inserir uma segunda, o próprio banco recusa.
            builder.HasIndex(n => n.TarefaId)
                   .IsUnique();
        }
    }
}