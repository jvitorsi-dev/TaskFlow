using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tarefas.UseCases
{
    public class AlterarPrioridadeTarefaUseCase
    {
        private readonly IUnitOfWork _unityOfWork;
        private readonly IProjetoRepository _projetoRepository;

        public AlterarPrioridadeTarefaUseCase(IUnitOfWork unityOfWork, IProjetoRepository projetoRepository)
        {
            _unityOfWork = unityOfWork;
            _projetoRepository = projetoRepository;
        }

        public async Task AlterarPrioridadeTarefa(AlterarPrioridadeTarefaCommand command)
        {
            var tarefa = await _projetoRepository.ObterTarefaProjeto(command.ProjetoId, command.TarefaId)
                ?? throw new NotFoundException($"Tarefa com Id {command.TarefaId} não encontrada no projeto {command.ProjetoId}.");

            tarefa.AlterarPrioridade(command.Prioridade);
            await _unityOfWork.CommitAsync();
        }
    }
}
