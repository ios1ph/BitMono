﻿namespace ObscuraX.Core.Contexts;

public class StarterContext
{
#pragma warning disable CS8618
    public ModuleDefinition Module { get; set; }
    public ModuleReaderParameters ModuleReaderParameters { get; set; }
    public IPEImageBuilder PEImageBuilder { get; set; }
    public ModuleDefinition RuntimeModule { get; set; }
    public ReferenceImporter RuntimeImporter { get; set; }
    public ObscuraXContext ObscuraXContext { get; set; }
    public CancellationToken CancellationToken { get; set; }

    public IAssemblyResolver AssemblyResolver => Module.MetadataResolver.AssemblyResolver;
#pragma warning restore CS8618

    public void ThrowIfCancellationRequested()
    {
        CancellationToken.ThrowIfCancellationRequested();
    }
}