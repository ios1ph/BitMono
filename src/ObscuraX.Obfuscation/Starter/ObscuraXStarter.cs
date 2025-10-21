﻿namespace ObscuraX.Obfuscation.Starter;

public class ObscuraXStarter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ObfuscationSettings _obfuscationSettings;
    private readonly IEngineContextAccessor _engineContextAccessor;
    private readonly ILogger _logger;

    public ObscuraXStarter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _obfuscationSettings = serviceProvider.GetRequiredService<IOptions<ObfuscationSettings>>().Value;
        _engineContextAccessor = serviceProvider.GetRequiredService<IEngineContextAccessor>();
        _logger = serviceProvider
            .GetRequiredService<ILogger>()
            .ForContext<ObscuraXStarter>();
    }

    private async Task<bool> StartAsync(StarterContext context, IDataWriter dataWriter)
    {
        context.ThrowIfCancellationRequested();

        var obfuscator = new ObscuraXObfuscator(_serviceProvider, context, dataWriter, _obfuscationSettings, _logger);
        await obfuscator.ProtectAsync();
        return true;
    }
    public Task<bool> StartAsync(FinalFileInfo info, IModuleFactory moduleFactory, IDataWriter dataWriter,
        IReferencesDataResolver referencesDataResolver,
        CancellationToken cancellationToken)
    {
        var runtimeModule = ModuleDefinition.FromFile(typeof(Runtime.Data).Assembly.Location);
        var moduleFactoryResult = moduleFactory.Create();
        var bitMonoContextFactory = new ObscuraXContextFactory(moduleFactoryResult.Module, referencesDataResolver, _obfuscationSettings);
        var bitMonoContext = bitMonoContextFactory.Create(info.FilePath, info.OutputDirectoryPath, cancellationToken);
        var engineContextFactory = new StarterContextFactory(moduleFactoryResult, runtimeModule, bitMonoContext, cancellationToken);
        var engineContext = engineContextFactory.Create();
        _engineContextAccessor.Instance = engineContext;
        bitMonoContext.OutputFile = OutputFilePathFactory.Create(bitMonoContext);
        return StartAsync(engineContext, dataWriter);
    }
    public Task<bool> StartAsync(CompleteFileInfo info, CancellationToken cancellationToken)
    {
        return StartAsync(new FinalFileInfo(info.FileName, info.OutputDirectoryPath),
            new ModuleFactory(info.FileData, _obfuscationSettings, new LogErrorListener(_logger, _obfuscationSettings), _logger),
            new FileDataWriter(), new AutomaticReferencesDataResolver(info.FileReferences), cancellationToken);
    }
    public Task<bool> StartAsync(IncompleteFileInfo info, CancellationToken cancellationToken)
    {
        return StartAsync(new FinalFileInfo(info.FilePath, info.OutputDirectoryPath),
            new ModuleFactory(File.ReadAllBytes(info.FilePath), _obfuscationSettings, new LogErrorListener(_logger, _obfuscationSettings), _logger),
            new FileDataWriter(), new AutomaticPathReferencesDataResolver(info.ReferencesDirectoryPath), cancellationToken);
    }
}