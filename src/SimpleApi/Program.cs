using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(cfg => {
    cfg.Configuration = builder.Configuration.GetConnectionString("MyRedis");
    cfg.InstanceName = "WeatherRedisInstance";
});

var app = builder.Build();

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

app.MapGet("/weatherforecast", async ([FromServices]IDistributedCache cache) =>
{
    var cachedJson = await cache.GetStringAsync(CacheKeys.WEATHER_FORECAST);

    if (cachedJson != null)
    {
        return JsonSerializer.Deserialize<WeatherForecast[]>(cachedJson);
    }

    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    var forecastJson = JsonSerializer.Serialize(forecast);

    var cacheOption = new DistributedCacheEntryOptions()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30),
    };

    await cache.SetStringAsync(CacheKeys.WEATHER_FORECAST, forecastJson, cacheOption);

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// For expose to Testing project
public partial class Program { }