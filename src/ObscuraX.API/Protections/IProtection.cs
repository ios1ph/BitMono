namespace ObscuraX.API.Protections;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IProtection
{
    Task ExecuteAsync();
}