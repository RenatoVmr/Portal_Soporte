using SupportPortal.Api.Models;
using SupportPortal.Api.Repositories;

namespace SupportPortal.Api.Services;

public class CustomerAccountService
{
    private readonly CustomerAccountRepository _repository;

    public CustomerAccountService(CustomerAccountRepository repository)
    {
        _repository = repository;
    }

    public Task<CustomerAccountDto?> SearchByDocumentAsync(string documentNumber)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException("Document number is required.");

        return _repository.SearchByDocumentAsync(documentNumber.Trim());
    }

    public async Task UpdateStatusAsync(
        int accountId,
        UpdateAccountStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
            throw new ArgumentException("Status is required.");

        var status = request.Status.Trim().ToUpperInvariant();
        var reason = request.StatusReason?.Trim();

        if (status != "ACTIVE" && status != "BLOCKED")
            throw new ArgumentException("Status must be ACTIVE or BLOCKED.");

        if (status == "BLOCKED" && string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A reason is required to block an account.");

        if (status == "ACTIVE")
            reason = null;

        await _repository.UpdateStatusAsync(accountId, status, reason);
    }
    public Task<List<CustomerAccountDto>> ListAllAsync()
{
    return _repository.ListAllAsync();
}
}