using SupportPortal.Api.Repositories;
using SupportPortal.Api.Services;
using MySqlConnector;
using SupportPortal.Api.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<CustomerAccountRepository>();
builder.Services.AddScoped<CustomerAccountService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};





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

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
