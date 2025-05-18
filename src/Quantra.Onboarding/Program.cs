using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quantra.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1) Controllers (sem chaining de FluentValidation aqui)
builder.Services.AddControllers();

// 2) FluentValidation em separado
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters(); // opcional, se quiser client-side
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 3) Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4) MediatR
builder.Services.AddMediatR(typeof(Program).Assembly);

// 5) OpenTelemetry (Tracing + Metrics)
builder.Services.AddOpenTelemetry()
    .WithTracing(tb => tb
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
    )
    .WithMetrics(mb => mb
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
    );

// 6) DbContext em memória para dev
builder.Services.AddDbContext<LedgerDbContext>(opt =>
    opt.UseInMemoryDatabase("QuantraOnboardingDev"));

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
