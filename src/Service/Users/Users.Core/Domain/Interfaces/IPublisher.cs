using System;
using System.Threading.Tasks;

namespace Users.Core.Domain.Interfaces
{
    public interface IPublisher
    {
            Task Publish<T>(T content, Uri queueAddress);
    }
}
