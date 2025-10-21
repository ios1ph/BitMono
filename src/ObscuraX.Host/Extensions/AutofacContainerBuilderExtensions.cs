namespace ObscuraX.Host.Extensions;

[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public static class AutofacContainerBuilderExtensions
{
    private const string ProtectionsFileName = "ObscuraX.Protections.dll";
    private const string UnityFileName = "ObscuraX.Unity.dll";

    public static ContainerBuilder AddProtections(this ContainerBuilder source, string? file = null)
    {
        var protectionsFilePath = file ?? Path.Combine(AppContext.BaseDirectory, ProtectionsFileName);
        var rawData = File.ReadAllBytes(file ?? protectionsFilePath);
        Assembly.Load(rawData);
        
        var unityFilePath = Path.Combine(AppContext.BaseDirectory, UnityFileName);
        if (File.Exists(unityFilePath))
        {
            var unityRawData = File.ReadAllBytes(unityFilePath);
            Assembly.Load(unityRawData);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        source.RegisterAssemblyTypes(assemblies)
            .PublicOnly()
            .Where(x => x.GetInterface(nameof(IPhaseProtection)) == null && x.GetInterface(nameof(IProtection)) != null)
            .OwnedByLifetimeScope()
            .AsImplementedInterfaces()
            .SingleInstance();
        return source;
    }
    public static ServiceCollection AddConfigurations(this ServiceCollection source,
        ProtectionSettings? protectionSettings = null, string? criticalsFile = null, string? obfuscationFile = null, string? loggingFile = null, string? protectionsFile = null, ObfuscationSettings? obfuscationSettings = null)
    {
        var criticals = new ObscuraXCriticalsConfiguration(criticalsFile);
        source.AddOptions()
            .Configure<CriticalsSettings>(criticals.Configuration);

        if (obfuscationSettings != null)
        {
            source.Configure<ObfuscationSettings>(configure =>
            {
                configure.Watermark = obfuscationSettings.Watermark;
                configure.ClearCLI = obfuscationSettings.ClearCLI;
                configure.ForceObfuscation = obfuscationSettings.ForceObfuscation;
                configure.ReferencesDirectoryName = obfuscationSettings.ReferencesDirectoryName;
                configure.OutputDirectoryName = obfuscationSettings.OutputDirectoryName;
                configure.OpenFileDestinationInFileExplorer = obfuscationSettings.OpenFileDestinationInFileExplorer;
            });
        }
        else
        {
            var obfuscation = new ObscuraXObfuscationConfiguration(obfuscationFile);
            source.Configure<ObfuscationSettings>(obfuscation.Configuration);
        }

        if (protectionSettings != null)
        {
            source.Configure<ProtectionSettings>(configure =>
            {
                configure.Protections = protectionSettings.Protections;
            });
        }
        else
        {
            var protections = new ObscuraXProtectionsConfiguration(protectionsFile);
            source.Configure<ProtectionSettings>(protections.Configuration);
        }

        return source;
    }
}