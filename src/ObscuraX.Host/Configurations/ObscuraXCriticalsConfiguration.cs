namespace ObscuraX.Host.Configurations;

public class ObscuraXCriticalsConfiguration : JsonConfigurationAccessor, IObscuraXCriticalsConfiguration
{
    public ObscuraXCriticalsConfiguration(string? file = null) : base(file ?? "criticals.json")
    {
    }
}