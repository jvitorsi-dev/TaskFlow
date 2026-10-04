using NSubstitute;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Domain.Entities;
using Xunit;

namespace TaskFlow.Tests
{
    /// <summary>
    /// Testes que documentam comportamentos incorretos identificados no código atual.
    /// Eles estão marcados como Skip de propósito: quando o bug correspondente for
    /// corrigido, basta remover o Skip e o teste passará a proteger o comportamento correto.
    /// </summary>
    public class KnownIssuesTests
    {
        [Fact(Skip = "Bug conhecido: ProjetoController.ObterPorId passa os parâmetros da rota invertidos " +
                     "para o BuscarProjetoCommand (idProjeto vai para UsuarioId e idUsuario vai para ProjetoId). " +
                     "O mesmo acontece no método Excluir.")]
        public async Task ProjetoController_ObterPorId_DeveMontarCommandComIdsNaOrdemCorreta()
        {
            var service = Substitute.For<IProjetoService>();
            var controller = new ProjetoController(service);

            _ = await controller.ObterPorId(idProjeto: 10, idUsuario: 20);

            await service.Received(1).ObterProjetoPorIdAsync(
                Arg.Is<BuscarProjetoCommand>(c => c.ProjetoId == 10 && c.UsuarioId == 20));
        }

        [Fact(Skip = "Bug conhecido: Usuario.AdicionarProjeto adiciona o projeto em uma lista privada (_projetos) " +
                     "diferente da coleção exposta pela propriedade Usuario.Projetos, que permanece sempre vazia.")]
        public void Usuario_AdicionarProjeto_DeveExporProjetoNaPropriedadeProjetos()
        {
            var usuario = new Usuario("Ana Souza", "ana@teste.com", "senha12345");
            var projeto = new Projeto("Projeto de Testes", "Descrição do projeto", usuarioId: 1);

            usuario.AdicionarProjeto(projeto);

            Assert.Single(usuario.Projetos);
        }

        [Fact(Skip = "Bug conhecido: Usuario.Editar valida os valores recebidos antes de aplicar a " +
                     "fallback (null ?? atual), então edições parciais passando null sempre lançam " +
                     "DomainException em vez de manter o valor atual.")]
        public void Usuario_Editar_ComCamposNulos_DeveManterValoresAtuais()
        {
            var usuario = new Usuario("Ana Souza", "ana@teste.com", "senha12345");

            usuario.Editar(nome: null, email: null, senha: null);

            Assert.Equal("Ana Souza", usuario.Nome);
            Assert.Equal("ana@teste.com", usuario.Email);
            Assert.Equal("senha12345", usuario.Senha);
        }

        [Fact(Skip = "Bug conhecido: Tarefa.Editar é o único ponto que valida título/descrição; o construtor " +
                     "de Tarefa aceita valores nulos ou vazios sem validação.")]
        public void Tarefa_Construtor_DeveValidarTituloEDescricao()
        {
            var excecao = Assert.Throws<TaskFlow.Domain.Exceptions.DomainException>(
                () => new Tarefa("", "", TaskFlow.Domain.Enums.PrioridadeTarefa.Media));

            Assert.Contains("Tarefa", excecao.Message);
        }
    }
}
