using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StockTradePro.BlazorUI.Services;
using StockTradePro.BlazorUI.Constants;
using StockTradePro.BlazorUI.Services.Mocks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configure Gateway URL
var gatewayUrl = builder.Configuration["ApiGatewayUrl"] ?? ServiceConstants.ApiGatewayUrl;

// Register Configured Typed Clients or Mocks
var useMockServices = builder.Configuration.GetValue<bool>("UseMockServices");

if (useMockServices)
{
    builder.Services.AddScoped<IAuthService, MockAuthService>();
    builder.Services.AddScoped<IStockDataService, MockStockDataService>();
    builder.Services.AddScoped<IPortfolioService, MockPortfolioService>();
    builder.Services.AddScoped<IWatchlistService, MockWatchlistService>();
    builder.Services.AddScoped<INotificationService, MockNotificationService>();
}
else
{
    builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
    {
        client.BaseAddress = new Uri(gatewayUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHttpClient<IStockDataService, StockDataService>(client =>
    {
        client.BaseAddress = new Uri(gatewayUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHttpClient<IPortfolioService, PortfolioService>(client =>
    {
        client.BaseAddress = new Uri(gatewayUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHttpClient<IWatchlistService, WatchlistService>(client =>
    {
        client.BaseAddress = new Uri(gatewayUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });

    builder.Services.AddHttpClient<INotificationService, NotificationService>(client =>
    {
        client.BaseAddress = new Uri(gatewayUrl);
        client.Timeout = TimeSpan.FromSeconds(30);
    });
}

// SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };

        // Allow SignalR to use JWT tokens
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// CORS for SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSignalR", policy =>
    {
        policy.WithOrigins(
                ServiceConstants.ApiGatewayUrl,
                ServiceConstants.StockPriceHubUrl.Replace("/stockPriceHub", ""),
                ServiceConstants.NotificationHubUrl.Replace("/notificationHub", ""),
                "http://localhost:3000",
                "http://localhost:8080"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowSignalR");
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();