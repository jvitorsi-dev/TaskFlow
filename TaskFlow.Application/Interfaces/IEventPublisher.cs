using System;
using System.Collections.Generic;
using System.Text;

namespace TaskFlow.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent evento, CancellationToken ct = default)
            where TEvent : class;
    }
}
