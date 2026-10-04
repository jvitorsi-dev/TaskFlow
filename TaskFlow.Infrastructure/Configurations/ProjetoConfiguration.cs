using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Configurations
{
    public class ProjetoConfiguration : IEntityTypeConfiguration<Projeto>
    {
        public void Configure(EntityTypeBuilder<Projeto> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Descricao)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(p => p.Status)
                   .HasConversion<string>();

            builder.Metadata
                   .FindNavigation(nameof(Projeto.Tarefas))!
                   .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(p => p.Tarefas)
                   .WithOne()
                   .HasForeignKey(t => t.ProjetoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
