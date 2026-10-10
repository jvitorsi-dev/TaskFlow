using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Senha { get; private set; } = string.Empty;
        private readonly List<Projeto> _projetos = new List<Projeto>();
        public IReadOnlyCollection<Projeto> Projetos => _projetos.AsReadOnly();

        public Usuario() { } // Construtor privado para Entity Framework

        public Usuario(string nome, string email, string senha)
        {
            if (string.IsNullOrEmpty(nome))
                throw new DomainException("O nome do usuário não pode ser nulo ou vazio.");
            if (string.IsNullOrEmpty(email))
                throw new DomainException("O email do usuário não pode ser nulo ou vazio.");
            if (string.IsNullOrEmpty(senha))
                throw new DomainException("A senha do usuário não pode ser nula ou vazia.");
            Nome = nome;
            Email = email;
            Senha = senha;
        }

        public void AdicionarProjeto(Projeto projeto)
        {
            if (projeto is null)
                throw new DomainException("O projeto não pode ser nulo.");

            _projetos.Add(projeto);
        }

        public void RemoverProjeto(Projeto projeto)
        {
            _projetos.Remove(projeto);
        }

        public void Editar(string? nome, string? email, string? senha)
        {
            var novoNome = nome ?? Nome;
            var novoEmail = email ?? Email;
            var novaSenha = senha ?? Senha;

            Validar(novoNome, novoEmail, novaSenha);

            Nome = novoNome;
            Email = novoEmail;
            Senha = novaSenha;
        }

        public void Validar(string? nome, string? email, string? senha)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException(
                    "O nome do usuário não pode ser nulo ou vazio.");

            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException(
                    "O email do usuário não pode ser nulo ou vazio.");

            if (string.IsNullOrWhiteSpace(senha))
                throw new DomainException(
                    "A senha do usuário não pode ser nula ou vazia.");

            if (senha.Length < 8)
                throw new DomainException(
                    "A senha deve possuir no mínimo 8 caracteres.");

            if (senha.Length > 15)
                throw new DomainException(
                    "A senha deve possuir no máximo 15 caracteres.");

        }
    }
}
