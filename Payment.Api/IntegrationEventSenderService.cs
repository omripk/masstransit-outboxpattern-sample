using MassTransit;
using Newtonsoft.Json;

namespace Payment.Api;

public class IntegrationEventSenderService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public IntegrationEventSenderService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        using var scope = _scopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await PublishOutstandingIntegrationEvents(stoppingToken);
        }
    }

    private async Task PublishOutstandingIntegrationEvents(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var bus = scope.ServiceProvider.GetService<IBus>();
                if (bus == null)
                    return;
                
                using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var events = dbContext.IntegrationEvents.OrderBy(o => o.Id).ToList();
                
                foreach (var item in events)
                {
                    if (string.IsNullOrEmpty(item.Type))
                        return;

                    var type = Type.GetType(item.Type);
                    if (type == null)
                        return;

                    var message = JsonConvert.DeserializeObject(item.Data, type);
                    if (message == null)
                        return;

                    await bus.Publish(message, stoppingToken);

                    Console.WriteLine("Published: " + item.Type + " " + item.Data);
                    dbContext.Remove(item);
                    await dbContext.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(15 * 1000, stoppingToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            await Task.Delay(15 * 1000, stoppingToken);
        }
    }
}