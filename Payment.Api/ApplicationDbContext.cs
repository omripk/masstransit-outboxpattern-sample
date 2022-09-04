using Microsoft.EntityFrameworkCore;
using Payment.Api.Entities;

namespace Payment.Api;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    {
    }
 
    public DbSet<Entities.Payment> Payments { get; set; }
    public DbSet<IntegrationEvent> IntegrationEvents { get; set; }
}