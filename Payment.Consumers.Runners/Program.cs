using MassTransit;
using Payment.Contracts;
using Payment.Contracts.Consumers;

var bus = BusConfigurator.ConfigureBus(configuration =>
{
    configuration.ReceiveEndpoint(RabbitMqConstants.BillingQueueName, e =>
    {
        e.Consumer<BillingConsumer>();
    });
});
 
await bus.StartAsync();
 
Console.WriteLine($"Listening {nameof(BillingConsumer)}... Press any key to exit.");
Console.ReadKey();
 
await bus.StopAsync();