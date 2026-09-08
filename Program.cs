using ChallengerPetCenter.Data;
using ChallengerPetCenter.HealthChecks;
using ChallengerPetCenter.Middleware;
using ChallengerPetCenter.Observability;
using ChallengerPetCenter.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using System.Reflection;
using System.Text.Json;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("Application", "ChallengerPetCenter")
        .WriteTo.Console()
        .WriteTo.File(
            path: "logs/petcenter-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 14,
            shared: true));

    builder.Services.AddControllers();
    builder.Services.AddHttpClient("ExternalHealthCheck", client =>
    {
        client.Timeout = TimeSpan.FromSeconds(5);
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    var useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
    if (useInMemory)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("PetCenterTests"));
    }
    else
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));
    }

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IPetService, PetService>();
    builder.Services.AddScoped<IVeterinarioService, VeterinarioService>();
    builder.Services.AddScoped<IDiarioEntradaService, DiarioEntradaService>();
    builder.Services.AddScoped<IRegistroService, RegistroService>();
    builder.Services.AddScoped<IComentarioService, ComentarioService>();
    builder.Services.AddScoped<ISolicitacaoService, SolicitacaoService>();
    builder.Services.AddScoped<IPetVeterinarioService, PetVeterinarioService>();

    builder.Services.AddSingleton<AppMetrics>();

    var externalHealthUrl = builder.Configuration["ExternalServices:HealthCheckUrl"];

    var healthChecks = builder.Services.AddHealthChecks()
        .AddCheck("api", () => HealthCheckResult.Healthy("API operacional."), tags: new[] { "ready", "live" })
        .AddDbContextCheck<AppDbContext>(
            name: "oracle-database",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "ready", "db" })
        .AddCheck<ExternalServiceHealthCheck>(
            name: "external-service",
            failureStatus: HealthStatus.Degraded,
            tags: new[] { "ready", "external" });

    if (!string.IsNullOrWhiteSpace(externalHealthUrl))
    {
        healthChecks.AddUrlGroup(
            new Uri(externalHealthUrl),
            name: "external-uri",
            failureStatus: HealthStatus.Degraded,
            tags: new[] { "ready", "external" });
    }

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
            .AddService(
                serviceName: "ChallengerPetCenter",
                serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0"))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.Filter = context =>
                    !context.Request.Path.StartsWithSegments("/health") &&
                    !context.Request.Path.StartsWithSegments("/metrics");
            })
            .AddHttpClientInstrumentation()
            .AddSource("ChallengerPetCenter")
            .AddConsoleExporter())
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMeter(AppMetrics.MeterName)
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter());

    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        };
    });

    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ApiKeyMiddleware>();
    app.UseMiddleware<RequestMetricsMiddleware>();
    app.UseAuthorization();
    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = WriteHealthResponse
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live"),
        ResponseWriter = WriteHealthResponse
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = WriteHealthResponse
    });

    app.MapPrometheusScrapingEndpoint("/metrics");

    Log.Information("ChallengerPetCenter iniciando...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada inesperadamente");
}
finally
{
    Log.CloseAndFlush();
}

static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var payload = new
    {
        status = report.Status.ToString(),
        totalDuration = report.TotalDuration.ToString(),
        entries = report.Entries.Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            description = entry.Value.Description,
            duration = entry.Value.Duration.ToString(),
            tags = entry.Value.Tags
        })
    };

    return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
}

public partial class Program { }
