using Cadastramento.Infrastructure;
using StackExchange.Redis;
using Cadastramento.WebApi.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog (basic console sink)
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Read configuration
var redisConnectionString = builder.Configuration["Redis:Connection"] ?? "10.255.200.7:47846";
var sqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=localhost\\SQLEXPRESS;Initial Catalog=Entrevista;User ID=entrevista;Password=softpark@2025;TrustServerCertificate=True";

builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddScoped<IRedisSessionService, RedisSessionService>();

builder.Services.AddScoped<IUserRepository>(_ => new UserRepository(sqlConnectionString));

// Add services to the container - Swagger for .NET 8
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware global de autenticação redis
app.UseMiddleware<RedisAuthenticationMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
