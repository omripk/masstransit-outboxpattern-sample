namespace Payment.Contracts;

public static class RabbitMqConstants
{
    public const string Uri = "amqp://localhost";
    public const string Username = "guest";
    public const string Password = "guest";
    
    public const string BillingQueueName = "Payment.Billing";
}