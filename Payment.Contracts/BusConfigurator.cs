using MassTransit;

namespace Payment.Contracts;

public static class BusConfigurator
{
    public static IBusControl ConfigureBus(Action<IRabbitMqBusFactoryConfigurator>? registrationAction = null)
    {
        return Bus.Factory.CreateUsingRabbitMq(configuration =>
        {
            configuration.Host(RabbitMqConstants.Uri, hostConfiguration =>
            {
                hostConfiguration.Username(RabbitMqConstants.Username);
                hostConfiguration.Password(RabbitMqConstants.Password);
            });

            registrationAction?.Invoke(configuration);
        });
    }
}