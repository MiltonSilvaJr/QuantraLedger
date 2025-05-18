using Quantra.Security;
using Quantra.Messaging;
using Quantra.Domain;
using Quantra.Domain.Models;
using Quantra.Persistence;
using Quantra.Transaction;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Prometheus;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("midaz.transaction"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter());


string conn = builder.Configuration.GetConnectionString("LedgerDb") 
              ?? Environment.GetEnvironmentVariable("LEDGER_CONNECTION") 
              ?? "Host=localhost;Database=midaz;Username=midaz;Password=midaz";

builder.Services.AddDbContext<LedgerDbContext>(opt => opt.UseNpgsql(conn));
builder.Services.AddScoped<ILedgerService, EfCoreLedgerService>();
builder.Services.AddMessageBus(builder.Configuration);


var secret = builder.Configuration["JwtSecret"] ?? "insecure_test_secret_please_replace";
var jwt = new JwtService(secret, TimeSpan.FromHours(8));
jwt.ConfigureAuthentication(builder.Services);
builder.Services.AddSingleton(jwt);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/transactions", async (HttpRequest request, ILedgerService ledger, TransactionRequest dto) =>
{
    string key = request.Headers["Idempotency-Key"].FirstOrDefault() ?? dto.ExternalId ?? Guid.NewGuid().ToString("N");
    var tx = await ledger.PostAsync(dto.DebitAccount, dto.CreditAccount, dto.Amount, dto.Currency, key);
    return Results.Ok(tx);
});


app.MapPost("/transactions/dsl", async (HttpRequest request, ILedgerService ledger, string script) =>
{
    var key = request.Headers["Idempotency-Key"].FirstOrDefault();
    var instructions = TxDslParser.ParseScript(script);
    var tx = await ledger.PostAsync(instructions, key);
    return Results.Ok(tx);
});

app.MapGet("/balance/{account}", async (ILedgerService ledger, string account) =>
{
    var bal = await ledger.GetBalanceAsync(account);
    return Results.Ok(new { account, balance = bal });
});

app.Run();

record TransactionRequest(string DebitAccount, string CreditAccount, decimal Amount, string Currency = "BRL", string? ExternalId = null);
