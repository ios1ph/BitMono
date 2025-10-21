namespace ObscuraX.API.Configuration;

public interface IConfigurationAccessor
{
    IConfiguration Configuration { get; }
}