using MassTransit;
using Payment.Contracts.Messages;

namespace Payment.Contracts.Consumers;

public class BillingConsumer : IConsumer<BillingMessage>
{
    public async Task Consume(ConsumeContext<BillingMessage> context)
    {
        var message = context.Message;
        await Console.Out.WriteLineAsync($"PaymentId: {message.PaymentId}, Username: {message.Username} | " +
                                         $"StockCount: {message.StockCount} | Date: {message.Date}");
    }
}