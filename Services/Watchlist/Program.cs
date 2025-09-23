using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StockTradePro.WatchlistService.API.Data;
using StockTradePro.WatchlistService.API.Services;
using StockTradePro.WatchlistService.API.BackgroundServices;
using StockTradePro.Shared.Extensions;
using StockTradePro.Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<WatchlistDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// RabbitMQ Configuration
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// HTTP Clients (Only for Stock Data Service now)
builder.Services.AddHttpClient<IStockDataService, StockDataService>();

// Services
builder.Services.AddScoped<IWatchlistService, WatchlistServiceImpl>();
builder.Services.AddScoped<IPriceAlertService, PriceAlertService>();

// RabbitMQ Publisher (Replaces HTTP notification service)
builder.Services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();

// Background Services
builder.Services.AddHostedService<AlertMonitoringService>();

// Controllers
builder.Services.AddControllers();

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"));

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Watchlist Service API",
        Version = "v1",
        Description = "Stock Portfolio Watchlist Management API with RabbitMQ messaging"
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Watchlist Service API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");

app.MapControllers();
app.MapHealthChecks("/health");

// Database migration on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WatchlistDbContext>();
    try
    {
        context.Database.Migrate();
        Console.WriteLine("Watchlist Service: Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Watchlist Service: Database migration failed: {ex.Message}");
    }
}

Console.WriteLine("Watchlist Service starting...");
Console.WriteLine("RabbitMQ Publisher configured for notifications");
Console.WriteLine("Health check available at: /health");
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Swagger UI available at: /swagger");
}

app.Run();