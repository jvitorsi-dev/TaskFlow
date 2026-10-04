using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using Xunit;

namespace TaskFlow.Tests.Domain.Entities
{
    public class TarefaTests
    {
        private const string TituloValido = "Tarefa de teste";
        private const string DescricaoValida = "Descrição da tarefa de teste";

        private static Tarefa CriarTarefaValida(PrioridadeTarefa prioridade = PrioridadeTarefa.Media)
            => new(TituloValido, DescricaoValida, prioridade);

        private static Tarefa CriarTarefaConcluida()
        {
            var tarefa = CriarTarefaValida();
            tarefa.Iniciar();
            tarefa.Concluir();
            return tarefa;
        }

        #region Construtor

        [Fact]
        public void Construtor_Valido_CriaTarefaPendenteComDataAtual()
        {
            var tarefa = new Tarefa(TituloValido, DescricaoValida, PrioridadeTarefa.Alta);

            Assert.Equal(TituloValido, tarefa.Titulo);
            Assert.Equal(DescricaoValida, tarefa.Descricao);
            Assert.Equal(PrioridadeTarefa.Alta, tarefa.Prioridade);
            Assert.Equal(StatusTarefa.Pendente, tarefa.Status);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Today), tarefa.DataCriacao);
            Assert.Null(tarefa.DataInicio);
            Assert.Null(tarefa.DataConclusao);
        }

        #endregion

        #region Ciclo de vida (Iniciar / Concluir)

        [Fact]
        public void Iniciar_TarefaPendente_MudaParaEmAndamentoEDefineDataInicio()
        {
            var tarefa = CriarTarefaValida();

            tarefa.Iniciar();

            Assert.Equal(StatusTarefa.EmAndamento, tarefa.Status);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Today), tarefa.DataInicio);
        }

        [Fact]
        public void Iniciar_TarefaJaEmAndamento_LancaDomainException()
        {
            var tarefa = CriarTarefaValida();
            tarefa.Iniciar();

            var excecao = Assert.Throws<DomainException>(() => tarefa.Iniciar());

            Assert.Contains("'Pendente'", excecao.Message);
        }

        [Fact]
        public void Iniciar_TarefaConcluida_LancaDomainException()
        {
            var tarefa = CriarTarefaConcluida();

            Assert.Throws<DomainException>(() => tarefa.Iniciar());
        }

        [Fact]
        public void Concluir_TarefaEmAndamento_MudaParaConcluidaEDefineDataConclusao()
        {
            var tarefa = CriarTarefaValida();
            tarefa.Iniciar();

            tarefa.Concluir();

            Assert.Equal(StatusTarefa.Concluida, tarefa.Status);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Today), tarefa.DataConclusao);
        }

        [Fact]
        public void Concluir_TarefaPendente_LancaDomainException()
        {
            var tarefa = CriarTarefaValida();

            var excecao = Assert.Throws<DomainException>(() => tarefa.Concluir());

            Assert.Contains("'Em Andamento'", excecao.Message);
        }

        #endregion

        #region Editar

        [Fact]
        public void Editar_TarefaPendente_AlteraTituloDescricaoEPrioridade()
        {
            var tarefa = CriarTarefaValida(PrioridadeTarefa.Baixa);

            tarefa.Editar("Título novo", "Descrição nova", PrioridadeTarefa.Alta);

            Assert.Equal("Título novo", tarefa.Titulo);
            Assert.Equal("Descrição nova", tarefa.Descricao);
            Assert.Equal(PrioridadeTarefa.Alta, tarefa.Prioridade);
        }

        [Fact]
        public void Editar_ValoresNulos_MantemValoresAtuais()
        {
            var tarefa = CriarTarefaValida(PrioridadeTarefa.Baixa);

            tarefa.Editar(null, null, null);

            Assert.Equal(TituloValido, tarefa.Titulo);
            Assert.Equal(DescricaoValida, tarefa.Descricao);
            Assert.Equal(PrioridadeTarefa.Baixa, tarefa.Prioridade);
        }

        [Fact]
        public void Editar_TarefaConcluida_LancaDomainException()
        {
            var tarefa = CriarTarefaConcluida();

            var excecao = Assert.Throws<DomainException>(
                () => tarefa.Editar("Título novo", "Descrição nova", null));

            Assert.Contains("concluída", excecao.Message);
        }

        // null não entra aqui de propósito: titulo null mantém o valor atual (coberto por Editar_ValoresNulos_MantemValoresAtuais)
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ab")] // menos de 3 caracteres
        public void Editar_TituloInvalido_LancaDomainException(string titulo)
        {
            var tarefa = CriarTarefaValida();

            Assert.Throws<DomainException>(() => tarefa.Editar(titulo, DescricaoValida, null));
        }

        [Fact]
        public void Editar_TituloCom101Caracteres_LancaDomainException()
        {
            var tarefa = CriarTarefaValida();

            var excecao = Assert.Throws<DomainException>(
                () => tarefa.Editar(new string('t', 101), DescricaoValida, null));

            Assert.Contains("entre 3 e 100 caracteres", excecao.Message);
        }

        // null não entra aqui de propósito: descricao null mantém o valor atual
        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void Editar_DescricaoInvalida_LancaDomainException(string descricao)
        {
            var tarefa = CriarTarefaValida();

            Assert.Throws<DomainException>(() => tarefa.Editar(TituloValido, descricao, null));
        }

        [Fact]
        public void Editar_DescricaoCom501Caracteres_LancaDomainException()
        {
            var tarefa = CriarTarefaValida();

            var excecao = Assert.Throws<DomainException>(
                () => tarefa.Editar(TituloValido, new string('d', 501), null));

            Assert.Contains("entre 3 e 500 caracteres", excecao.Message);
        }

        #endregion

        #region Prioridade

        [Theory]
        [InlineData(PrioridadeTarefa.Baixa)]
        [InlineData(PrioridadeTarefa.Media)]
        [InlineData(PrioridadeTarefa.Alta)]
        public void AlterarPrioridade_AtualizaAPrioridade(PrioridadeTarefa novaPrioridade)
        {
            var tarefa = CriarTarefaValida();

            tarefa.AlterarPrioridade(novaPrioridade);

            Assert.Equal(novaPrioridade, tarefa.Prioridade);
        }

        #endregion
    }
}
