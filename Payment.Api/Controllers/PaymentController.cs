using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Payment.Api.Entities;
using Payment.Api.Models;
using Payment.Contracts.Messages;

namespace Payment.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase
{
    private readonly ILogger<PaymentController> _logger;
    private readonly ApplicationDbContext _context;

    public PaymentController(ILogger<PaymentController> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Post(PaymentViewModel model)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        var paymentEntity = new Entities.Payment()
        {
            Username = model.Username,
            StockCount = model.StockCount
        };
        _context.Payments.Add(paymentEntity);
        await _context.SaveChangesAsync();

        var integrationEventData = JsonConvert.SerializeObject(new BillingMessage()
        {
            PaymentId = paymentEntity.Id,
            Username = paymentEntity.Username,
            StockCount = paymentEntity.StockCount,
            Date = DateTime.Now
        });

        _context.IntegrationEvents.Add(
            new IntegrationEvent()
            {
                Type = typeof(BillingMessage).AssemblyQualifiedName,
                Data = integrationEventData
            });

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return CreatedAtAction(nameof(Get), new {id = paymentEntity.Id}, paymentEntity);
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(x => x.Id == id);
        if (payment == null)
        {
            return NotFound();
        }

        return Ok(payment);
    }
}