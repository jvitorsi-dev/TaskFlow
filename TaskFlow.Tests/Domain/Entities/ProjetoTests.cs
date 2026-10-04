using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using Xunit;

namespace TaskFlow.Tests.Domain.Entities
{
    public class ProjetoTests
    {
        private const string NomeValido = "Projeto de Testes";
        private const string DescricaoValida = "Descrição do projeto de testes";

        private static Projeto CriarProjetoValido(int usuarioId = 1)
            => new(NomeValido, DescricaoValida, usuarioId);

        private static Tarefa CriarTarefaValida(string titulo = "Tarefa de teste")
            => new(titulo, "Descrição da tarefa de teste", PrioridadeTarefa.Media);

        private static Projeto CriarProjetoConcluido()
        {
            var projeto = CriarProjetoValido();
            projeto.Iniciar();
            projeto.Concluir();
            return projeto;
        }

        #region Construtor

        [Fact]
        public void Construtor_Valido_CriaProjetoNoStatusPlanejado()
        {
            var projeto = CriarProjetoValido(usuarioId: 42);

            Assert.Equal(NomeValido, projeto.Nome);
            Assert.Equal(DescricaoValida, projeto.Descricao);
            Assert.Equal(42, projeto.UsuarioId);
            Assert.Equal(StatusProjeto.Planejado, projeto.Status);
            Assert.Empty(projeto.Tarefas);
        }

        [Fact]
        public void Construtor_Valido_DefineDataCriacaoComDataAtual()
        {
            var projeto = CriarProjetoValido();

            Assert.Equal(DateOnly.FromDateTime(DateTime.Today), projeto.DataCriacao);
            Assert.Null(projeto.DataInicio);
            Assert.Null(projeto.DataConclusao);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_NomeNuloOuVazio_LancaDomainException(string nome)
        {
            var excecao = Assert.Throws<DomainException>(() => new Projeto(nome, DescricaoValida, 1));

            Assert.Contains("nome do projeto", excecao.Message);
        }

        [Theory]
        [InlineData("ab")] // 2 caracteres (abaixo do mínimo)
        public void Construtor_NomeCurtoDemais_LancaDomainException(string nome)
        {
            var excecao = Assert.Throws<DomainException>(() => new Projeto(nome, DescricaoValida, 1));

            Assert.Contains("entre 3 e 100 caracteres", excecao.Message);
        }

        [Fact]
        public void Construtor_NomeCom101Caracteres_LancaDomainException()
        {
            var nome = new string('a', 101);

            var excecao = Assert.Throws<DomainException>(() => new Projeto(nome, DescricaoValida, 1));

            Assert.Contains("entre 3 e 100 caracteres", excecao.Message);
        }

        [Theory]
        [InlineData("abc")] // mínimo
        public void Construtor_NomeComTamanhoValido_NaoLancaExcecao(string nome)
        {
            var projeto = new Projeto(nome, DescricaoValida, 1);

            Assert.Equal(nome, projeto.Nome);
        }

        [Fact]
        public void Construtor_NomeCom100Caracteres_NaoLancaExcecao()
        {
            var nome = new string('a', 100);

            var projeto = new Projeto(nome, DescricaoValida, 1);

            Assert.Equal(100, projeto.Nome.Length);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_DescricaoNulaOuVazia_LancaDomainException(string descricao)
        {
            var excecao = Assert.Throws<DomainException>(() => new Projeto(NomeValido, descricao, 1));

            Assert.Contains("descrição do projeto", excecao.Message);
        }

        [Fact]
        public void Construtor_DescricaoCom501Caracteres_LancaDomainException()
        {
            var descricao = new string('d', 501);

            var excecao = Assert.Throws<DomainException>(() => new Projeto(NomeValido, descricao, 1));

            Assert.Contains("entre 3 e 500 caracteres", excecao.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Construtor_UsuarioIdInvalido_LancaDomainException(int usuarioId)
        {
            var excecao = Assert.Throws<DomainException>(() => new Projeto(NomeValido, DescricaoValida, usuarioId));

            Assert.Contains("Id do usuário", excecao.Message);
        }

        #endregion

        #region Ciclo de vida (Iniciar / Concluir)

        [Fact]
        public void Iniciar_ProjetoPlanejado_MudaParaEmAndamentoEDefineDataInicio()
        {
            var projeto = CriarProjetoValido();

            projeto.Iniciar();

            Assert.Equal(StatusProjeto.EmAndamento, projeto.Status);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Today), projeto.DataInicio);
        }

        [Fact]
        public void Iniciar_ProjetoJaEmAndamento_LancaDomainException()
        {
            var projeto = CriarProjetoValido();
            projeto.Iniciar();

            var excecao = Assert.Throws<DomainException>(() => projeto.Iniciar());

            Assert.Contains("'Planejado'", excecao.Message);
        }

        [Fact]
        public void Iniciar_ProjetoConcluido_LancaDomainException()
        {
            var projeto = CriarProjetoConcluido();

            Assert.Throws<DomainException>(() => projeto.Iniciar());
        }

        [Fact]
        public void Concluir_ProjetoEmAndamento_MudaParaConcluidoEDefineDataConclusao()
        {
            var projeto = CriarProjetoValido();
            projeto.Iniciar();

            projeto.Concluir();

            Assert.Equal(StatusProjeto.Concluido, projeto.Status);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Today), projeto.DataConclusao);
        }

        [Fact]
        public void Concluir_ProjetoPlanejado_LancaDomainException()
        {
            var projeto = CriarProjetoValido();

            var excecao = Assert.Throws<DomainException>(() => projeto.Concluir());

            Assert.Contains("'Em Andamento'", excecao.Message);
        }

        #endregion

        #region Editar

        [Fact]
        public void Editar_ProjetoPlanejado_AlteraNomeEDescricao()
        {
            var projeto = CriarProjetoValido();

            projeto.Editar("Novo nome", "Nova descrição", usuarioId: 1);

            Assert.Equal("Novo nome", projeto.Nome);
            Assert.Equal("Nova descrição", projeto.Descricao);
        }

        [Fact]
        public void Editar_ValoresNulos_MantemValoresAtuais()
        {
            var projeto = CriarProjetoValido();

            projeto.Editar(null, null, usuarioId: 1);

            Assert.Equal(NomeValido, projeto.Nome);
            Assert.Equal(DescricaoValida, projeto.Descricao);
        }

        [Fact]
        public void Editar_ProjetoConcluido_LancaDomainException()
        {
            var projeto = CriarProjetoConcluido();

            var excecao = Assert.Throws<DomainException>(
                () => projeto.Editar("Novo nome", "Nova descrição", usuarioId: 1));

            Assert.Contains("concluído", excecao.Message);
        }

        [Fact]
        public void Editar_NomeInvalido_LancaDomainException()
        {
            var projeto = CriarProjetoValido();

            Assert.Throws<DomainException>(() => projeto.Editar("", "Nova descrição", usuarioId: 1));
        }

        #endregion

        #region Tarefas

        [Fact]
        public void AdicionarTarefa_Valida_AdicionaAColecao()
        {
            var projeto = CriarProjetoValido();
            var tarefa = CriarTarefaValida();

            projeto.AdicionarTarefa(tarefa);

            var tarefaAdicionada = Assert.Single(projeto.Tarefas);
            Assert.Same(tarefa, tarefaAdicionada);
        }

        [Fact]
        public void AdicionarTarefa_TarefaNula_LancaDomainException()
        {
            var projeto = CriarProjetoValido();

            Assert.Throws<DomainException>(() => projeto.AdicionarTarefa(null));
        }

        [Fact]
        public void AdicionarTarefa_ProjetoConcluido_LancaDomainException()
        {
            var projeto = CriarProjetoConcluido();

            var excecao = Assert.Throws<DomainException>(() => projeto.AdicionarTarefa(CriarTarefaValida()));

            Assert.Contains("concluído", excecao.Message);
        }

        [Theory]
        [InlineData("tarefa de teste")] // mesmo título, caixa diferente
        [InlineData("TAREFA DE TESTE")]
        [InlineData("Tarefa de teste")]
        public void AdicionarTarefa_TituloDuplicado_LancaDomainException(string tituloDuplicado)
        {
            var projeto = CriarProjetoValido();
            projeto.AdicionarTarefa(CriarTarefaValida("Tarefa de teste"));

            var excecao = Assert.Throws<DomainException>(
                () => projeto.AdicionarTarefa(CriarTarefaValida(tituloDuplicado)));

            Assert.Contains("Já existe uma tarefa", excecao.Message);
        }

        [Fact]
        public void EditarTarefa_TarefaExistente_AlteraDadosDaTarefa()
        {
            var projeto = CriarProjetoValido();
            var tarefa = CriarTarefaValida();
            projeto.AdicionarTarefa(tarefa);

            // Tarefas recém-criadas (não persistidas) possuem Id 0.
            projeto.EditarTarefa(tarefaId: 0, "Título novo", "Descrição nova", PrioridadeTarefa.Alta);

            Assert.Equal("Título novo", tarefa.Titulo);
            Assert.Equal("Descrição nova", tarefa.Descricao);
            Assert.Equal(PrioridadeTarefa.Alta, tarefa.Prioridade);
        }

        [Fact]
        public void EditarTarefa_TarefaInexistente_LancaNotFoundException()
        {
            var projeto = CriarProjetoValido();

            Assert.Throws<NotFoundException>(() => projeto.EditarTarefa(999, "Título", "Descrição", null));
        }

        [Fact]
        public void EditarTarefa_ProjetoConcluido_LancaDomainException()
        {
            var projeto = CriarProjetoConcluido();

            Assert.Throws<DomainException>(() => projeto.EditarTarefa(0, "Título", "Descrição", null));
        }

        [Fact]
        public void ExcluirTarefa_TarefaExistente_RemoveDaColecao()
        {
            var projeto = CriarProjetoValido();
            projeto.AdicionarTarefa(CriarTarefaValida());

            projeto.ExcluirTarefa(tarefaId: 0);

            Assert.Empty(projeto.Tarefas);
        }

        [Fact]
        public void ExcluirTarefa_TarefaInexistente_LancaNotFoundException()
        {
            var projeto = CriarProjetoValido();

            Assert.Throws<NotFoundException>(() => projeto.ExcluirTarefa(999));
        }

        [Fact]
        public void ExcluirTarefa_ProjetoConcluido_LancaDomainException()
        {
            var projeto = CriarProjetoConcluido();

            Assert.Throws<DomainException>(() => projeto.ExcluirTarefa(0));
        }

        #endregion
    }
}
