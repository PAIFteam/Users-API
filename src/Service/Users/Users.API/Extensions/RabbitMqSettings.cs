using Users.Core.Domain.Interfaces;
using Users.Infra.RabbitMq;
using MassTransit;


namespace Users.Core.Application.Settings
{
    public static class RabbitMqSettings
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConfigurationsSettings = new Domain.Entities.RabbitMQ.RabbitMqConfigurationSettings();

            configuration
                .GetSection(Domain.Entities.RabbitMQ.RabbitMqConfigurationSettings.OPTION_KEY)
                .Bind(rabbitMqConfigurationsSettings);
            services.AddScoped<IPublisher, Publisher>();
            services.AddScoped(_ => rabbitMqConfigurationsSettings);
            

            return services;
        }
    }
}
