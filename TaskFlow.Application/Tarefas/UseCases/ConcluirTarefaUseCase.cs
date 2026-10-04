using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tarefas.UseCases
{
    public class ConcluirTarefaUseCase
    {
        private readonly IUnitOfWork _unityOfWork;
        private readonly IProjetoRepository _projetoRepository;

        public ConcluirTarefaUseCase(IUnitOfWork unityOfWork, IProjetoRepository projetoRepository)
        {
            _unityOfWork = unityOfWork;
            _projetoRepository = projetoRepository;
        }

        public async Task ExecuteAsync(BuscarTarefaCommand command)
        {
            var tarefa = await _projetoRepository.ObterTarefaProjeto(command.ProjetoId, command.TarefaId)
                ?? throw new NotFoundException($"Tarefa com Id {command.TarefaId} não encontrada no projeto {command.ProjetoId}.");

            tarefa.Concluir();
            await _unityOfWork.CommitAsync();
        }
    }
}
