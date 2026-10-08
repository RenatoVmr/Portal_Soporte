namespace SupportPortal.Api.Models;

public class CustomerAccountDto
{
    public int CustomerId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public int AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? StatusReason { get; set; }
}