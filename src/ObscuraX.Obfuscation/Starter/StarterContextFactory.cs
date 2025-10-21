namespace ObscuraX.Obfuscation.Starter;

public class StarterContextFactory
{
    private readonly ModuleFactoryResult _moduleFactoryResult;
    private readonly ModuleDefinition _runtimeModule;
    private readonly ObscuraXContext _context;
    private readonly CancellationToken _cancellationToken;

    public StarterContextFactory(ModuleFactoryResult moduleFactoryResult, ModuleDefinition runtimeModule,
        ObscuraXContext context, CancellationToken cancellationToken)
    {
        _moduleFactoryResult = moduleFactoryResult;
        _runtimeModule = runtimeModule;
        _context = context;
        _cancellationToken = cancellationToken;
    }

    public StarterContext Create()
    {
        return new StarterContext
        {
            Module = _moduleFactoryResult.Module,
            RuntimeModule = _runtimeModule,
            ModuleReaderParameters = _moduleFactoryResult.ModuleReaderParameters,
            PEImageBuilder = _moduleFactoryResult.PEImageBuilder,
            RuntimeImporter = _runtimeModule.DefaultImporter,
            ObscuraXContext = _context,
            CancellationToken = _cancellationToken,
        };
    }
}