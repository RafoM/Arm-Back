using ContentService.Models.ConfigModels;
using MassTransit;

namespace ContentService
{
    public static class MessagingSetup
    {
        public static IServiceCollection AddMessaging(
            this IServiceCollection services,
            IConfiguration configuration,
            params Type[] consumers)
        {
            var rabbitMqConfigs = configuration
                .GetSection("RabbitMQ")
                .Get<RabbitMqConfigModel>() ?? new RabbitMqConfigModel();

            services.AddMassTransit(x =>
            {
                foreach (var consumer in consumers)
                    x.AddConsumer(consumer);

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(rabbitMqConfigs.Host), "/", h =>
                    {
                        h.Username(rabbitMqConfigs.Username);
                        h.Password(rabbitMqConfigs.Password);
                    });

                    foreach (var consumer in consumers)
                    {
                        cfg.ReceiveEndpoint(consumer.FullName, e =>
                        {
                            e.ConfigureConsumer(context, consumer);
                        });
                    }
                });
            });

            return services;
        }
    }
}
