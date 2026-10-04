using System;
using System.Collections.Generic;
using System.Text;
using TaskFlow.Application.Projetos.Commands;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Application.Projetos.UseCases
{
    public class ExcluirProjetoUseCase
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IUnitOfWork _unityOfWork;

        public ExcluirProjetoUseCase(IProjetoRepository projetoRepository,
            IUnitOfWork unitOfWork)
        {
            _projetoRepository = projetoRepository;
            _unityOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(BuscarProjetoCommand command)
        {
            if (command.ProjetoId <= 0)
                throw new DomainException("O id do projeto deve ser maior que zero.");

            var projeto = await _projetoRepository
                .ObterPorIdAsync(command.ProjetoId, command.UsuarioId) ??
                throw new NotFoundException(
                    $"Projeto com Id {command.ProjetoId} não encontrado.");

            await _projetoRepository.ExcluirAsync(projeto);
            await _unityOfWork.CommitAsync();

        }
    }
}
