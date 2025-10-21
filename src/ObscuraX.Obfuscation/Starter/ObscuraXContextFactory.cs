namespace ObscuraX.Obfuscation.Starter;

public class ObscuraXContextFactory
{
    private readonly ModuleDefinition _module;
    private readonly IReferencesDataResolver _referencesDataResolver;
    private readonly ObfuscationSettings _obfuscationSettings;

    public ObscuraXContextFactory(ModuleDefinition module, IReferencesDataResolver referencesDataResolver,
        ObfuscationSettings obfuscationSettings)
    {
        _module = module;
        _referencesDataResolver = referencesDataResolver;
        _obfuscationSettings = obfuscationSettings;
    }

    public ObscuraXContext Create(string filePath, string outputDirectoryName, CancellationToken cancellationToken)
    {
        var referencesData = _referencesDataResolver.Resolve(_module, cancellationToken);
        var fileName = Path.GetFileName(filePath);
        return new ObscuraXContext
        {
            OutputDirectoryName = outputDirectoryName,
            ReferencesData = referencesData,
            Watermark = _obfuscationSettings.Watermark,
            FileName = fileName
        };
    }
}