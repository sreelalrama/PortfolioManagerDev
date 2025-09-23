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

// Single HTTP Client pointing to API Gateway
builder.Services.AddHttpClient("ApiGateway", client =>
{
    client.BaseAddress = new Uri(ServiceConstants.ApiGatewayUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

//mock services are in use
builder.Services.AddScoped<IAuthService, MockAuthService>();
builder.Services.AddScoped<IStockDataService, MockStockDataService>();
builder.Services.AddScoped<IPortfolioService, MockPortfolioService>();
builder.Services.AddScoped<IWatchlistService, MockWatchlistService>();
builder.Services.AddScoped<INotificationService, MockNotificationService>();

// Register services - they'll all use the same gateway client
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IStockDataService, StockDataService>();
//builder.Services.AddScoped<IPortfolioService, PortfolioService>();
//builder.Services.AddScoped<IWatchlistService, WatchlistService>();
//builder.Services.AddScoped<INotificationService, NotificationService>();

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