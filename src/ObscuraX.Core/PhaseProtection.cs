namespace ObscuraX.Core;

public abstract class PhaseProtection : ProtectionBase, IPhaseProtection
{
    protected PhaseProtection(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}