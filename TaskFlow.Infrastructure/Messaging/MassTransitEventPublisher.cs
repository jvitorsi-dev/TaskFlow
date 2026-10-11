using MassTransit;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Infrastructure.Messaging
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task PublishAsync<TEvent>(TEvent evento, CancellationToken ct = default)
            where TEvent : class
            => _publishEndpoint.Publish(evento, ct);
    }
}