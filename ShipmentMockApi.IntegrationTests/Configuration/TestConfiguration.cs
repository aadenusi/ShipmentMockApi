using Microsoft.Extensions.Configuration;

namespace ShipmentMockApi.IntegrationTests.Configuration;

public class TestConfiguration
{
    private static TestConfiguration? _instance;
    private readonly IConfiguration _configuration;

    private TestConfiguration()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static TestConfiguration Instance => _instance ??= new TestConfiguration();

    public string BaseUrl => _configuration["ApiSettings:BaseUrl"] 
        ?? throw new InvalidOperationException("BaseUrl not configured in appsettings.json");

    public int DefaultTimeout => int.Parse(_configuration["TestSettings:DefaultTimeout"] ?? "30");

    public int RetryAttempts => int.Parse(_configuration["TestSettings:RetryAttempts"] ?? "3");
}
