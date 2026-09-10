using ChallengerPetCenter.Data;
using ChallengerPetCenter.HealthChecks;
using ChallengerPetCenter.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IVeterinarioService, VeterinarioService>();
builder.Services.AddScoped<IPetVeterinarioService, PetVeterinarioService>();
builder.Services.AddScoped<IDiarioEntradaService, DiarioEntradaService>();
builder.Services.AddScoped<IRegistroService, RegistroService>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<ISolicitacaoService, SolicitacaoService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    options.IncludeXmlComments(
        Path.Combine(AppContext.BaseDirectory, xmlFilename)
    );
});

// DbContext Oracle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(
        builder.Configuration.GetConnectionString("OracleConnection")));

// Health Checks
builder.Services.Configure<ExternalServiceHealthCheckOptions>(
    builder.Configuration.GetSection(ExternalServiceHealthCheckOptions.SectionName));

builder.Services.AddHttpClient(nameof(ExternalServiceHealthCheck), client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHealthChecks()
    .AddCheck<SelfHealthCheck>(
        name: "self",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "self", "live" })
    .AddDbContextCheck<AppDbContext>(
        name: "oracle",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "database", "ready" })
    .AddCheck<ExternalServiceHealthCheck>(
        name: "external_service",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "external", "ready" });

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var healthJsonOptions = new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
};

// Visão geral: todos os checks
app.MapHealthChecks("/health", healthJsonOptions);
app.MapHealthChecks("/healthz", healthJsonOptions);

// Liveness: processo da API
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Readiness: Oracle + serviço externo
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

// Necessário para WebApplicationFactory nos testes de integração
public partial class Program { }
