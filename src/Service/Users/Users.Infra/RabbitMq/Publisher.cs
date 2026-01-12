using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MassTransit;
using Users.Core.Domain.Interfaces.Publishers;

namespace Users.Infra.RabbitMq
{
    public class Publisher : IPublisher
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public Publisher(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }
        public async Task Publish<T>(T content, Uri queueAddress)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(queueAddress);
            sendEndpoint.Send(content);
        }
    }
}
