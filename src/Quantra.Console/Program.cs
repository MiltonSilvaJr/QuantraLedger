using Quantra.Security;
using Microsoft.EntityFrameworkCore;
using Quantra.Persistence;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Prometheus;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("midaz.console"))
    .WithTracing(t => t
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(m => m
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter());

builder.Services.AddRazorComponents();
builder.Services.AddServerSideBlazor();

string conn = builder.Configuration.GetConnectionString("LedgerDb") 
              ?? Environment.GetEnvironmentVariable("LEDGER_CONNECTION") 
              ?? "Host=localhost;Database=midaz;Username=midaz;Password=midaz";
builder.Services.AddDbContext<LedgerDbContext>(opt => opt.UseNpgsql(conn));


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
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
