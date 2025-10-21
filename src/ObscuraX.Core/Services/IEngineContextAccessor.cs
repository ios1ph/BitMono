namespace ObscuraX.Core.Services;

public interface IEngineContextAccessor
{
    StarterContext Instance { get; set; }
}