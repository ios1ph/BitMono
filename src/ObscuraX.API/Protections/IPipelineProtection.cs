namespace ObscuraX.API.Protections;

public interface IPipelineProtection : IProtection
{
    IEnumerable<IPhaseProtection> PopulatePipeline();
}