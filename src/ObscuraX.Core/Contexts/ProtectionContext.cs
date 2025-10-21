namespace ObscuraX.Core.Contexts;

public class ProtectionContext
{
    public ProtectionContext(ModuleDefinition module, ModuleDefinition runtimeModule, ObscuraXContext bitMonoContext,
        ProtectionParameters parameters, CancellationToken cancellationToken)
    {
        Module = module;
        RuntimeModule = runtimeModule;
        ObscuraXContext = bitMonoContext;
        Parameters = parameters;
        CancellationToken = cancellationToken;
    }

    public ModuleDefinition Module { get; }
    public ModuleDefinition RuntimeModule { get; }
    public ObscuraXContext ObscuraXContext { get; }
    public ProtectionParameters Parameters { get; }
    public CancellationToken CancellationToken { get; }

    public ReferenceImporter ModuleImporter => Module.DefaultImporter;
    public ReferenceImporter RuntimeImporter => Module.DefaultImporter;
    public bool X86 => Module.MachineType == MachineType.I386;

    public void ThrowIfCancellationTokenRequested()
    {
        CancellationToken.ThrowIfCancellationRequested();
    }
}