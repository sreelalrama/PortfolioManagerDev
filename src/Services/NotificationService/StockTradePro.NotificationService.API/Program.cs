using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StockTradePro.NotificationService.API.Data;
using StockTradePro.NotificationService.API.Hubs;
using StockTradePro.NotificationService.API.Services;
using StockTradePro.NotificationService.API.BackgroundServices;
using StockTradePro.Shared.Extensions;
using StockTradePro.Shared.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// SignalR
builder.Services.AddSignalR();

// RabbitMQ Configuration
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// HTTP Clients
builder.Services.AddHttpClient<IUserService, UserService>();

// Services
builder.Services.AddScoped<INotificationService, NotificationServiceImpl>();
builder.Services.AddScoped<IUserNotificationPreferenceService, UserNotificationPreferenceService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();

// RabbitMQ Consumer
builder.Services.AddSingleton<IMessageConsumer, RabbitMQConsumer>();

// Background Services
builder.Services.AddHostedService<MessageConsumerService>();

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
        Title = "Notification Service API",
        Version = "v1",
        Description = "Real-time notification management API with SignalR and RabbitMQ"
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
              .AllowCredentials(); // Required for SignalR
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Service API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");

app.MapControllers();
app.MapHealthChecks("/health");

// Map SignalR Hub
app.MapHub<NotificationHub>("/notificationHub");

// Database migration on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    try
    {
        context.Database.Migrate();
        Console.WriteLine("Notification Service: Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Notification Service: Database migration failed: {ex.Message}");
    }
}

Console.WriteLine("Notification Service starting...");
Console.WriteLine("RabbitMQ Consumer started for incoming messages");
Console.WriteLine("SignalR Hub available at: /notificationHub");
Console.WriteLine("Health check available at: /health");
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Swagger UI available at: /swagger");
}

app.Run();