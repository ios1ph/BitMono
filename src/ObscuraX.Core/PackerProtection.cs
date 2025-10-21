namespace ObscuraX.Core;

public abstract class PackerProtection : ProtectionBase, IPacker
{
    protected PackerProtection(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}