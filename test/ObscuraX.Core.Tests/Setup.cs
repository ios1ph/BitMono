namespace ObscuraX.Core.Tests;

public static class Setup
{
    public static ModelAttributeCriticalAnalyzer ModelAttributeCriticalAnalyzer(IOptions<CriticalsSettings> criticals)
    {
        return new ModelAttributeCriticalAnalyzer(criticals);
    }
    public static CriticalMethodsCriticalAnalyzer CriticalMethodsCriticalAnalyzer(IOptions<CriticalsSettings> criticals)
    {
        return new CriticalMethodsCriticalAnalyzer(criticals);
    }
    public static CriticalMethodsStartsWithAnalyzer CriticalMethodsStartsWithCriticalAnalyzer(IOptions<CriticalsSettings> criticals)
    {
        return new CriticalMethodsStartsWithAnalyzer(criticals);
    }
    public static NoInliningMethodMemberResolver NoInliningMethodMemberResolver(IOptions<ObfuscationSettings> obfuscation)
    {
        return new NoInliningMethodMemberResolver(obfuscation);
    }
    public static ObfuscationAttributeResolver ObfuscationAttributeResolver(IOptions<ObfuscationSettings> obfuscation)
    {
        return new ObfuscationAttributeResolver(obfuscation);
    }
    public static ObfuscateAssemblyAttributeResolver ObfuscateAssemblyAttributeResolver(
        IOptions<ObfuscationSettings> obfuscation)
    {
        return new ObfuscateAssemblyAttributeResolver(obfuscation);
    }
    public static SerializableBitCriticalAnalyzer SerializableBitCriticalAnalyzer(IOptions<ObfuscationSettings> obfuscation)
    {
        return new SerializableBitCriticalAnalyzer(obfuscation);
    }
    public static ReflectionCriticalAnalyzer ReflectionCriticalAnalyzer(IOptions<ObfuscationSettings> obfuscation)
    {
        return new ReflectionCriticalAnalyzer(obfuscation);
    }
}