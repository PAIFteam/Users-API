using Users.Core.Domain.Interfaces;
using Users.Core.Domain.Entities.RabbitMQ;
using Users.Core.Entities.RabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Core.Domain.Interfaces.Publishers;

namespace Users.Core.Application.Services
{
    public class WelcomeCustomerOnQueueAppService:IWelcomeCustomerOnQueueAppService
    {
        private readonly IPublisher _publisher;
        private readonly RabbitMqConfigurationSettings _rabbitMqConfigurationSettings;
        public async Task<bool> SendWelcomeCustomerToQueueAsync(Domain.Entities.User user)
        {
            //Prepara Mensagem de Boas Vindas ENVIO via RabbitMQ

            var message = new WelcomeCustomerMessage(user.Name, user.Login, user.Email);

            _ = _publisher.Publish(message, _rabbitMqConfigurationSettings.GetQueueAdress());

            return true;
        }
    }
}
