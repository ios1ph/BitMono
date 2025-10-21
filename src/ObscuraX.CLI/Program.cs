namespace ObscuraX.CLI;

internal class Program
{
    private static readonly CancellationTokenSource CancellationTokenSource = new();
    private static CancellationToken CancellationToken => CancellationTokenSource.Token;

    private static async Task<int> Main(string[] args)
    {
        Console.CancelKeyPress += OnCancelKeyPress;
        var statusCode = KnownReturnStatuses.Success;
        ObfuscationNeeds? needs = null;
        try
        {
            needs = new ObfuscationNeedsFactory(args).Create(CancellationToken);
            if (needs == null)
            {
                statusCode = KnownReturnStatuses.Failure;
                return statusCode;
            }

            var module = new ObscuraXModule(
                configureContainer => configureContainer.AddProtections(),
                configureServices => configureServices.AddConfigurations(
                    protectionSettings: needs.ProtectionSettings,
                    criticalsFile: needs.CriticalsFile,
                    obfuscationFile: needs.ObfuscationFile,
                    loggingFile: needs.LoggingFile,
                    protectionsFile: needs.ProtectionsFile,
                    obfuscationSettings: needs.ObfuscationSettings),
                configureLogger => configureLogger.WriteTo.AddConsoleLogger(),
                loggingFile: needs.LoggingFile);

            var app = new ObscuraXApplication().RegisterModule(module);
            await using var serviceProvider = await app.BuildAsync(CancellationToken);

            var obfuscation = serviceProvider.GetRequiredService<IOptions<ObfuscationSettings>>().Value;
            var logger = serviceProvider
                .GetRequiredService<ILogger>()
                .ForContext<Program>();

            CancellationToken.ThrowIfCancellationRequested();

            if (obfuscation.ClearCLI)
            {
                Console.Clear();
            }
            logger.Information("Target file: {0}", needs.FileName);
            logger.Information("References directory: {0}", needs.ReferencesDirectoryName);
            logger.Information("Starting protection pipeline...");

            var info = new IncompleteFileInfo(needs.FileName, needs.ReferencesDirectoryName, needs.OutputPath);
            var engine = new ObscuraXStarter(serviceProvider);
            var succeed = await engine.StartAsync(info, CancellationToken);
            if (!succeed)
            {
                logger.Fatal("Engine has fatal issues, unable to continue!");
                if (needs.Way == ObfuscationNeedsWay.Readline)
                {
                    Console.WriteLine("Enter anything to exit!");
                    Console.ReadLine();
                }
                statusCode = KnownReturnStatuses.Failure;
                return statusCode;
            }

            if (obfuscation.OpenFileDestinationInFileExplorer)
            {
                try
                {
                    Process.Start(needs.OutputPath);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "An error occured while opening the destination file in explorer!");
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Obfuscation Canceled!");
            statusCode = KnownReturnStatuses.Failure;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Something went wrong! " + ex);
            statusCode = KnownReturnStatuses.Failure;
        }

        Console.CancelKeyPress -= OnCancelKeyPress;

        if (needs?.Way == ObfuscationNeedsWay.Readline)
        {
            Console.WriteLine("Enter anything to exit!");
            Console.ReadLine();
        }
        return statusCode;
    }

    private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        CancellationTokenSource.Cancel();
        e.Cancel = true;
    }
}
