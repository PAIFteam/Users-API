using Users.Core.Domain.Interfaces;
using MassTransit;


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
            //var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(queueAddress);
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("rabbitmq://localhost/welcome_customer_queue"));

            await sendEndpoint.Send(content);
        }
    }
}
