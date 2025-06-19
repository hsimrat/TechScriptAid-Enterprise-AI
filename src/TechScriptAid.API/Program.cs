using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using TechScriptAid.API.Middleware;
using TechScriptAid.API.Services;
using TechScriptAid.Core.Interfaces;
using TechScriptAid.Infrastructure.Data;
using TechScriptAid.Infrastructure.Data.Seeding;
using TechScriptAid.Infrastructure.Repositories;
using TechScriptAid.Infrastructure.Repositories.Decorators;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(Program));
// Configure Swagger with better documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TechScriptAid Enterprise AI API",
        Version = "v1",
        Description = "Enterprise-grade AI document processing API",
        Contact = new OpenApiContact
        {
            Name = "TechScriptAid Team",
            Email = "support@techscriptaid.com"
        }
    });

    // Add XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("TechScriptAid.Infrastructure");
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });

    // Enable sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Add Memory Cache
builder.Services.AddMemoryCache();

// Configure repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add caching decorator for DocumentRepository
//builder.Services.Decorate<IDocumentRepository>((inner, provider) =>
//{
//    var cache = provider.GetRequiredService<IMemoryCache>();
//    return new CachedRepository<Document>(inner as IGenericRepository<Document>, cache);
//});

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// Add services
builder.Services.AddScoped<IDocumentService, DocumentService>();

// Add Data Seeder
builder.Services.AddDataSeeder();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database");

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",
                    "https://localhost:3000",
                    "http://localhost:5173",
                    "https://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add Response Caching
builder.Services.AddResponseCaching();

// Add Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (app.Environment.IsDevelopment())
        {
            // Using EnsureCreatedAsync instead of migrations
            await context.Database.EnsureCreatedAsync();

            // Temporarily skip seeding
            // await scope.ServiceProvider.SeedDatabaseAsync();

            Log.Information("Database created/verified successfully");
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred during application startup");
        throw;
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechScriptAid Enterprise AI API V1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}
else
{
    // Production error handling
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseResponseCaching();
app.UseSerilogRequestLogging();
app.UseCors("AllowSpecificOrigins");

// Custom middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// Health checks endpoint
app.MapHealthChecks("/health");

app.UseAuthorization();
app.MapControllers();

// Add a welcome endpoint
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

try
{
    Log.Information("Starting TechScriptAid Enterprise AI API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make the implicit Program class public so test projects can access it
public partial class Program { }