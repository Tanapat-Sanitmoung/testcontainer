

using Microsoft.Extensions.DependencyInjection;

public class IntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly IServiceScope _scope;
    protected readonly HttpClient _client;

    public IntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _client = factory.CreateDefaultClient();
    }
}