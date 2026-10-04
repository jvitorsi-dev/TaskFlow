using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TaskFlow.API.Handlers;
using TaskFlow.Domain.Exceptions;
using Xunit;

namespace TaskFlow.Tests.Api.Handlers
{
    public class GlobalExceptionHandlerTests
    {
        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web); // tolerante a camelCase/PascalCase

        private static async Task<(int Status, ProblemDetails Problem, bool Tratado)> ExecutarHandler(
            Exception excecao)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().BuildServiceProvider()
            };
            httpContext.Request.Path = "/api/teste";
            httpContext.Response.Body = new MemoryStream();

            var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

            var tratado = await handler.TryHandleAsync(httpContext, excecao, CancellationToken.None);

            httpContext.Response.Body.Position = 0;
            var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(
                httpContext.Response.Body, JsonOptions);

            return (httpContext.Response.StatusCode, problem, tratado);
        }

        [Fact]
        public async Task TryHandleAsync_DomainException_Retorna400ComMensagemOriginal()
        {
            var (status, problem, tratado) = await ExecutarHandler(
                new DomainException("O nome do projeto não pode ser nulo ou vazio."));

            Assert.True(tratado);
            Assert.Equal(StatusCodes.Status400BadRequest, status);
            Assert.Equal("Regra de negócio violada", problem.Title);
            Assert.Equal("O nome do projeto não pode ser nulo ou vazio.", problem.Detail);
        }

        [Fact]
        public async Task TryHandleAsync_NotFoundException_Retorna404ComMensagemOriginal()
        {
            var (status, problem, tratado) = await ExecutarHandler(
                new NotFoundException("Projeto com Id 10 não encontrado."));

            Assert.True(tratado);
            Assert.Equal(StatusCodes.Status404NotFound, status);
            Assert.Equal("Recurso não encontrado", problem.Title);
            Assert.Equal("Projeto com Id 10 não encontrado.", problem.Detail);
        }

        [Fact]
        public async Task TryHandleAsync_ExcecaoDesconhecida_Retorna500SemVazarDetalhes()
        {
            var (status, problem, tratado) = await ExecutarHandler(
                new InvalidOperationException("detalhe sensível do banco de dados"));

            Assert.True(tratado);
            Assert.Equal(StatusCodes.Status500InternalServerError, status);
            Assert.Equal("Erro interno no servidor", problem.Title);
            Assert.Equal("Ocorreu um erro inesperado.", problem.Detail);
        }

        [Fact]
        public async Task TryHandleAsync_DefineInstanceComPathDaRequisicao()
        {
            var (_, problem, _) = await ExecutarHandler(new DomainException("mensagem"));

            Assert.Equal("/api/teste", problem.Instance);
        }
    }
}
