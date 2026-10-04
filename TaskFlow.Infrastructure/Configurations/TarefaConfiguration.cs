using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Configurations
{
    public class TarefaConfiguration : IEntityTypeConfiguration<Tarefa>
    {
        public void Configure(EntityTypeBuilder<Tarefa> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Titulo)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(t => t.Descricao)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(t => t.Status)
                   .HasConversion<string>();

            builder.Property(t => t.Prioridade)
                   .HasConversion<string>();
        }
    }
}
