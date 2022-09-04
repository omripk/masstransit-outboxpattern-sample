namespace Payment.Contracts.Messages;

public class BillingMessage
{
    public int PaymentId { get; set; }
    public string Username { get; set; }
    public int StockCount { get; set; }
    public DateTime Date { get; set; }
}