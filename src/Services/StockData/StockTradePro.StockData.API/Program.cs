using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using StockTradePro.StockData.API.Data;
using StockTradePro.StockData.API.Services;
using StockTradePro.StockData.API.Hubs;                    // ADD THIS
using StockTradePro.StockData.API.BackgroundServices;       // ADD THIS

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Database Configuration
builder.Services.AddDbContext<StockDataDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Application Services
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IPriceSimulationService, PriceSimulationService>(); // ADD THIS

// Background Services
builder.Services.AddHostedService<PriceUpdateService>(); // ADD THIS

// SignalR for real-time updates
builder.Services.AddSignalR();

// CORS Configuration - UPDATED for SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:8080", "file://")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()
               .SetIsOriginAllowed(_ => true); // Allow any origin for development
    });
});

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StockTradePro Stock Data API",
        Version = "v1",
        Description = "Stock Data Service - Provides stock information, prices, and market data",
        Contact = new OpenApiContact
        {
            Name = "StockTradePro Team",
            Email = "support@stocktradepro.com"
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? "");

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockTradePro Stock Data API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// SignalR hub mapping - ADD THIS
app.MapHub<StockPriceHub>("/stockPriceHub");

// Database setup and seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StockDataDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    int maxRetries = 10;
    int retryCount = 0;

    while (retryCount < maxRetries)
    {
        try
        {
            // Apply migrations
            context.Database.Migrate();
            Log.Information("Stock Data database migrations applied successfully");

            // Seed data
            await StockSeeder.SeedAsync(context, logger);
            break;
        }
        catch (Exception ex)
        {
            retryCount++;
            if (retryCount >= maxRetries)
            {
                logger.LogError(ex, "Failed to migrate StockData database after {MaxRetries} attempts.", maxRetries);
                throw;
            }
            
            var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
            logger.LogWarning(ex, "Failed to migrate StockData database. Attempt {RetryCount} of {MaxRetries}. Retrying in {Delay} seconds...", retryCount, maxRetries, delay.TotalSeconds);
            Thread.Sleep(delay);
        }
    }
}

Log.Information("StockTradePro Stock Data API started successfully");

app.Run();