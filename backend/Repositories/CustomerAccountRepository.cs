using System.Data;
using MySqlConnector;
using SupportPortal.Api.Models;


namespace SupportPortal.Api.Repositories;

public class CustomerAccountRepository
{
    private readonly string _connectionString;

    public CustomerAccountRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<CustomerAccountDto?> SearchByDocumentAsync(string documentNumber)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new MySqlCommand(
            "sp_search_customer_by_document",
            connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        command.Parameters.AddWithValue("p_document_number", documentNumber);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        var reasonIndex = reader.GetOrdinal("status_reason");

        return new CustomerAccountDto
        {
            CustomerId = reader.GetInt32("customer_id"),
            DocumentNumber = reader.GetString("document_number"),
            BusinessName = reader.GetString("business_name"),
            AccountId = reader.GetInt32("account_id"),
            AccountNumber = reader.GetString("account_number"),
            Status = reader.GetString("status"),
            StatusReason = reader.IsDBNull(reasonIndex)
                ? null
                : reader.GetString(reasonIndex)
        };
    }
    public async Task UpdateStatusAsync(
    int accountId,
    string status,
    string? statusReason)
{
    await using var connection = new MySqlConnection(_connectionString);
    await connection.OpenAsync();

    await using var command = new MySqlCommand(
        "sp_update_account_status",
        connection)
    {
        CommandType = CommandType.StoredProcedure
    };

    command.Parameters.AddWithValue("p_account_id", accountId);
    command.Parameters.AddWithValue("p_status", status);
    command.Parameters.AddWithValue(
        "p_status_reason",
        (object?)statusReason ?? DBNull.Value);

    await command.ExecuteNonQueryAsync();
}

public async Task<List<CustomerAccountDto>> ListAllAsync()
{
    var customers = new List<CustomerAccountDto>();

    await using var connection = new MySqlConnection(_connectionString);
    await connection.OpenAsync();

    await using var command = new MySqlCommand(
        "sp_list_customers",
        connection)
    {
        CommandType = CommandType.StoredProcedure
    };

    await using var reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        var reasonIndex = reader.GetOrdinal("status_reason");

        customers.Add(new CustomerAccountDto
        {
            CustomerId = reader.GetInt32("customer_id"),
            DocumentNumber = reader.GetString("document_number"),
            BusinessName = reader.GetString("business_name"),
            AccountId = reader.GetInt32("account_id"),
            AccountNumber = reader.GetString("account_number"),
            Status = reader.GetString("status"),
            StatusReason = reader.IsDBNull(reasonIndex)
                ? null
                : reader.GetString("status_reason")
        });
    }

    return customers;
}
}