using Quantra.Security;
using MediatR;
using Quantra.Persistence;
using Quantra.Onboarding.Commands;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Prometheus;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("midaz.onboarding"))
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
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateOrganization>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/organizations", async (IMediator mediator, CreateOrganization cmd) =>
{
    var id = await mediator.Send(cmd);
    return Results.Created($"/organizations/{id}", new { id });
});

app.MapPost("/accounts", async (IMediator mediator, CreateAccount cmd) =>
{
    var id = await mediator.Send(cmd);
    return Results.Created($"/accounts/{id}", new { id });
});

app.MapGet("/organizations", async (LedgerDbContext db) =>
{
    var list = await db.Organizations
                       .Include(o=>o.Accounts)
                       .AsNoTracking()
                       .ToListAsync();
    return Results.Ok(list);
});

app.Run();


app.MapPost("/login", async (JwtService jwt, LedgerDbContext db, LoginRequest req) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
    if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
        return Results.Unauthorized();
    var token = jwt.GenerateToken(user.Id, user.Username, user.Role);
    return Results.Ok(new { token });
});
record LoginRequest(string Username, string Password);
