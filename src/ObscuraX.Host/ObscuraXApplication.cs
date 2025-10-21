namespace ObscuraX.Host;

public class ObscuraXApplication : IApplication
{
    private readonly ContainerBuilder _containerBuilder;
    private readonly List<IModule> _modules;

    public ObscuraXApplication()
    {
        _containerBuilder = new ContainerBuilder();
        _modules = [];
    }

    public IApplication Populate(IEnumerable<ServiceDescriptor> descriptors)
    {
        _containerBuilder.Populate(descriptors);
        return this;
    }
    public IApplication RegisterModule(IModule module)
    {
        _modules.Add(module);
        return this;
    }
    public Task<AutofacServiceProvider> BuildAsync(CancellationToken cancellationToken)
    {
        foreach (var module in _modules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _containerBuilder.RegisterModule(module);
        }
        var container = _containerBuilder.Build();
        var provider = new AutofacServiceProvider(container.Resolve<ILifetimeScope>());
        return Task.FromResult(provider);
    }
}