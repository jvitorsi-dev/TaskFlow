using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(p => p.Nome)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Senha)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Metadata
                   .FindNavigation(nameof(Usuario.Projetos))!
                   .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(p => p.Projetos)
                   .WithOne()
                   .HasForeignKey(p => p.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
