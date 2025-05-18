using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation.AspNetCore;
using MediatR;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Quantra.Persistence; // traz LedgerDbContext

var builder = WebApplication.CreateBuilder(args);

// → API & Swagger
builder.Services
    .AddControllers()
    .AddFluentValidation(cfg =>
        cfg.RegisterValidatorsFromAssemblyContaining<Program>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// → MediatR (CQRS / handlers)
builder.Services.AddMediatR(typeof(Program).Assembly);

// → OpenTelemetry Tracing
builder.Services.AddOpenTelemetryTracing(tracerProvider =>
    tracerProvider
        .SetResourceBuilder(ResourceBuilder
            .CreateDefault()
            .AddService(builder.Environment.ApplicationName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
);

// → OpenTelemetry Metrics
builder.Services.AddOpenTelemetryMetrics(meterProvider =>
    meterProvider
        .SetResourceBuilder(ResourceBuilder
            .CreateDefault()
            .AddService(builder.Environment.ApplicationName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
);

// → DbContext em memória para dev
builder.Services.AddDbContext<LedgerDbContext>(opt =>
    opt.UseInMemoryDatabase("QuantraOnboardingDev"));

var app = builder.Build();

// → Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
