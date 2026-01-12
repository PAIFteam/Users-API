using System;
using System.Threading.Tasks;

namespace Users.Infra.RabbitMq
{
    public interface IPublisher
    {
            Task Publish<T>(T content, Uri queueAddress);
    }
}
