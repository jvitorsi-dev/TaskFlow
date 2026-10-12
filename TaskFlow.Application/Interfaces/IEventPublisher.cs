using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Interfaces
{
    public interface IEventPublisher
    {
        /// <summary>
        /// Registra um evento para publicação. Com o Outbox ativo, o evento só é
        /// efetivamente gravado (e depois entregue) no próximo IUnitOfWork.CommitAsync().
        /// </summary>
        Task PublishAsync<TEvent>(TEvent evento, CancellationToken ct = default)
            where TEvent : class;
    }
}
