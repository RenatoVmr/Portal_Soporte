using SupportPortal.Api.Repositories;
using SupportPortal.Api.Services;
using SupportPortal.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<CustomerAccountRepository>();
builder.Services.AddScoped<CustomerAccountService>();

var frontendOrigin = builder.Configuration["FrontendOrigin"]
    ?? (builder.Environment.IsDevelopment() ? "http://localhost:5173" : null);

if (string.IsNullOrWhiteSpace(frontendOrigin))
{
    throw new InvalidOperationException(
        "FrontendOrigin must be configured outside the Development environment.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(frontendOrigin.TrimEnd('/'))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Frontend");

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/customers/search", async Task<IResult> (
    string? documentNumber,
    CustomerAccountService service) =>
{
    try
    {
        var customer = await service.SearchByDocumentAsync(documentNumber ?? "");

        if (customer is null)
            return Results.NotFound(new { message = "Customer not found" });

        return Results.Ok(customer);
    }
    catch (ArgumentException exception)
    {
        return Results.BadRequest(new { message = exception.Message });
    }
});

app.MapPut("/api/accounts/{accountId:int}/status", async Task<IResult> (
    int accountId,
    UpdateAccountStatusRequest request,
    CustomerAccountService service) =>
{
    try
    {
        await service.UpdateStatusAsync(accountId, request);

        return Results.Ok(new
        {
            message = "Account status updated successfully"
        });
    }
    catch (ArgumentException exception)
    {
        return Results.BadRequest(new
        {
            message = exception.Message
        });
    }
});

app.MapGet("/api/customers", async (CustomerAccountService service) =>
{
    var customers = await service.ListAllAsync();
    return Results.Ok(customers);
});

app.Run();
