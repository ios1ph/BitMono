﻿namespace ObscuraX.Obfuscation.Tests.Referencing;

public class CosturaReferencesDataResolverTest
{
    [Fact]
    public void CheckSameAmountOfCostura()
    {
        var resolver = new CosturaReferencesDataResolver();
        var module = ModuleDefinition.FromFile(typeof(TestCases.CosturaDecompressor.Program).Assembly.Location);
        var countOfEmbeddedCosturaResources = module.Resources.Count(x => x.IsEmbeddedCosturaResource());

        var result = resolver.Resolve(module, CancellationToken.None);

        result
            .Should()
            .NotBeEmpty().And
            .HaveCount(countOfEmbeddedCosturaResources);
    }
    [Theory]
    [InlineData("costura.asmresolver.dll.compressed")]
    [InlineData("costura.asmresolver.pe.dll.compressed")]
    [InlineData("costura.asmresolver.pe.file.dll.compressed")]
    [InlineData("costura.microsoft.entityframeworkcore.dll.compressed")]
    [InlineData("costura.microsoft.entityframeworkcore.abstractions.dll.compressed")]
    [InlineData("costura.microsoft.extensions.caching.abstractions.dll.compressed")]
    [InlineData("costura.microsoft.extensions.caching.memory.dll.compressed")]
    [InlineData("costura.microsoft.extensions.dependencyinjection.abstractions.dll.compressed")]
    [InlineData("costura.microsoft.extensions.dependencyinjection.dll.compressed")]
    [InlineData("costura.microsoft.extensions.logging.abstractions.dll.compressed")]
    [InlineData("costura.microsoft.extensions.logging.dll.compressed")]
    [InlineData("costura.microsoft.extensions.options.dll.compressed")]
    [InlineData("costura.microsoft.extensions.primitives.dll.compressed")]
    public void FindCosturaResource(string costuraResourceName)
    {
        var module = ModuleDefinition.FromFile(typeof(TestCases.CosturaDecompressor.Program).Assembly.Location);

        var result = module.Resources.First(x => x.Name.Value.Equals(costuraResourceName));

        result
            .IsEmbeddedCosturaResource()
            .Should()
            .BeTrue();
    }
}