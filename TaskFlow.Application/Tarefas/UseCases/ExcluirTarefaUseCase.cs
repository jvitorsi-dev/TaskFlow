using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Tarefas.Commands;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Tarefas.UseCases
{
    public class ExcluirTarefaUseCase
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExcluirTarefaUseCase(IProjetoRepository projetoRepository, IUnitOfWork unitOfWork)
        {
            _projetoRepository = projetoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(BuscarTarefaCommand command)
        {
            var projeto = await _projetoRepository.ObterPorIdAsync(command.ProjetoId, command.UsuarioId);
            if (projeto == null)
            {
                throw new NotFoundException("Projeto não encontrado.");
            }

            var tarefa = projeto.Tarefas.FirstOrDefault(t => t.Id == command.TarefaId);
            if (tarefa == null)
            {
                throw new NotFoundException("Tarefa não encontrada.");
            }

            projeto.ExcluirTarefa(command.TarefaId);
            await _unitOfWork.CommitAsync();
        }
    }
}
