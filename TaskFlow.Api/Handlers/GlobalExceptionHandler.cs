// TaskFlow.API/Handlers/GlobalExceptionHandler.cs
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.API.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (status, titulo) = exception switch
            {
                DomainException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
                NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
                _ => (StatusCodes.Status500InternalServerError, "Erro interno no servidor")
            };

            if (status == 500)
                _logger.LogError(exception, "Erro não tratado");
            else
                _logger.LogWarning("{Tipo}: {Mensagem}", exception.GetType().Name, exception.Message);

            var problem = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = status == 500 ? "Ocorreu um erro inesperado." : exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            return true;
        }
    }
}