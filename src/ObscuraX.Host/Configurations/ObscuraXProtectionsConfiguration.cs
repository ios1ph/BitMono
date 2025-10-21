namespace ObscuraX.Host.Configurations;

public class ObscuraXProtectionsConfiguration : JsonConfigurationAccessor, IObscuraXProtectionsConfiguration
{
    public ObscuraXProtectionsConfiguration(string? file = null) : base(file ?? "protections.json")
    {
    }
}