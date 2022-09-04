using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.Api;
using Payment.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext");
    options.UseNpgsql(connectionString);
});

builder.Services.AddSingleton<IntegrationEventSenderService>();
builder.Services.AddHostedService<IntegrationEventSenderService>(provider =>
    provider.GetService<IntegrationEventSenderService>());

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(RabbitMqConstants.Uri, hostCongiuration =>
        {
            hostCongiuration.Username(RabbitMqConstants.Username);
            hostCongiuration.Password(RabbitMqConstants.Password);
        });
    });
});
builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();