namespace ObscuraX.Obfuscation.Referencing;

public interface IReferencesDataResolver
{
    List<byte[]> Resolve(ModuleDefinition module, CancellationToken cancellationToken);
}