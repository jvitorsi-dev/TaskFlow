using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using Xunit;

namespace TaskFlow.Tests.Domain.Entities
{
    public class UsuarioTests
    {
        private const string NomeValido = "Ana Souza";
        private const string EmailValido = "ana@teste.com";
        private const string SenhaValida = "senha12345"; // 10 caracteres

        [Fact]
        public void Construtor_Valido_DefineNomeEmailESenha()
        {
            var usuario = new Usuario(NomeValido, EmailValido, SenhaValida);

            Assert.Equal(NomeValido, usuario.Nome);
            Assert.Equal(EmailValido, usuario.Email);
            Assert.Equal(SenhaValida, usuario.Senha);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Construtor_NomeNuloOuVazio_LancaDomainException(string nome)
        {
            var excecao = Assert.Throws<DomainException>(() => new Usuario(nome, EmailValido, SenhaValida));

            Assert.Contains("nome do usuário", excecao.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Construtor_EmailNuloOuVazio_LancaDomainException(string email)
        {
            var excecao = Assert.Throws<DomainException>(() => new Usuario(NomeValido, email, SenhaValida));

            Assert.Contains("email do usuário", excecao.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Construtor_SenhaNulaOuVazia_LancaDomainException(string senha)
        {
            var excecao = Assert.Throws<DomainException>(() => new Usuario(NomeValido, EmailValido, senha));

            Assert.Contains("senha do usuário", excecao.Message);
        }

        [Fact]
        public void Editar_TodosOsCampos_AtualizaOsValores()
        {
            var usuario = new Usuario(NomeValido, EmailValido, SenhaValida);

            usuario.Editar("Carlos Lima", "carlos@teste.com", "novasenha123");

            Assert.Equal("Carlos Lima", usuario.Nome);
            Assert.Equal("carlos@teste.com", usuario.Email);
            Assert.Equal("novasenha123", usuario.Senha);
        }

        [Fact]
        public void Editar_SenhaComMenosDe8Caracteres_LancaDomainException()
        {
            var usuario = new Usuario(NomeValido, EmailValido, SenhaValida);

            var excecao = Assert.Throws<DomainException>(
                () => usuario.Editar(NomeValido, EmailValido, "1234567")); // 7 caracteres

            Assert.Contains("no mínimo 8 caracteres", excecao.Message);
        }

        [Fact]
        public void Editar_SenhaComMaisDe15Caracteres_LancaDomainException()
        {
            var usuario = new Usuario(NomeValido, EmailValido, SenhaValida);

            var excecao = Assert.Throws<DomainException>(
                () => usuario.Editar(NomeValido, EmailValido, new string('s', 16)));

            Assert.Contains("no máximo 15 caracteres", excecao.Message);
        }

        [Theory]
        [InlineData("12345678")]      // 8 caracteres (mínimo)
        [InlineData("123456789012345")] // 15 caracteres (máximo)
        public void Editar_SenhaNosLimites_NaoLancaExcecao(string senha)
        {
            var usuario = new Usuario(NomeValido, EmailValido, SenhaValida);

            usuario.Editar(NomeValido, EmailValido, senha);

            Assert.Equal(senha, usuario.Senha);
        }

        [Fact]
        public void AdicionarProjeto_ProjetoValido_NaoLancaExcecao()
        {
            var usuario = new Usuario(NomeValido, EmailValido, SenhaValida);
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto de testes", usuarioId: 1);

            usuario.AdicionarProjeto(projeto);
            usuario.RemoverProjeto(projeto);
        }
    }
}
