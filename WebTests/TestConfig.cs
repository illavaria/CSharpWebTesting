using Microsoft.Extensions.Configuration;

namespace WebTests;

public class TestConfig
{
    public static IConfiguration Config { get; } =
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
}