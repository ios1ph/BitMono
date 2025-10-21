namespace ObscuraX.Host.Configurations;

public class ObscuraXObfuscationConfiguration : JsonConfigurationAccessor, IObscuraXObfuscationConfiguration
{
    public ObscuraXObfuscationConfiguration(string? file = null) : base(file ?? "obfuscation.json")
    {
    }
}