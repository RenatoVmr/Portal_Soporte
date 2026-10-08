namespace SupportPortal.Api.Models;

public class UpdateAccountStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? StatusReason { get; set; }
}