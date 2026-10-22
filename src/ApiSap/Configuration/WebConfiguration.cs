using Microsoft.Extensions.Configuration;

namespace WS_SAP;

public static class WebConfiguration
{
    private static IConfiguration? configuration;

    public static void Initialize(IConfiguration applicationConfiguration)
    {
        configuration = applicationConfiguration;
    }

    public static string GetRequiredValue(string key)
    {
        return configuration?[key]
            ?? throw new InvalidOperationException($"Missing required configuration value: {key}");
    }
}
