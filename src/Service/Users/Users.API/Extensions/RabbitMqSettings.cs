using Users.Core.Domain.Interfaces.Publishers;
using Users.Infra.RabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Users.Core.Application.Settings
{
    public static class RabbitMqSettings
    {
        public static IServiceCollection AddRabbitMqSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConfigurationsSettings = new Domain.Entities.RabbitMQ.RabbitMqConfigurationSettings();

            configuration
                .GetSection(Domain.Entities.RabbitMQ.RabbitMqConfigurationSettings.OPTION_KEY)
                .Bind(rabbitMqConfigurationsSettings);
            services.AddScoped<IPublisher, Publisher>();


            services.Configure<Domain.Entities.RabbitMQ.RabbitMqConfigurationSettings>(configuration.GetSection(Domain.Entities.RabbitMQ.RabbitMqConfigurationSettings.OPTION_KEY));
            return services;
        }
    }
}
