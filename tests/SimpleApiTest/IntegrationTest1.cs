using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleApiTest;

public class IntegrationTest1: IntegrationTestBase
{
    public IntegrationTest1(IntegrationTestWebAppFactory factory) : base(factory)
    {

    }

    [Fact]
    public async Task Test_Cache_Should()
    {
        //Arrange
        var cache = _scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var before = await cache.GetStringAsync(CacheKeys.WEATHER_FORECAST);

        //Act
        var firstResponse = await _client.GetAsync("/weatherforecast");
      
        //Assert
        before.Should().BeNull();

        firstResponse.IsSuccessStatusCode.Should().BeTrue();

        var after = await cache.GetStringAsync(CacheKeys.WEATHER_FORECAST);

        after.Should().NotBeNull();

        var firstForecast = await firstResponse.Content.ReadFromJsonAsync<WeatherForecast[]>();

        firstForecast.Should().NotBeNullOrEmpty();

    }
}