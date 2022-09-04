namespace Payment.Api.Entities;

public class IntegrationEvent
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Data { get; set; }
}