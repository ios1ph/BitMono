namespace ObscuraX.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RuntimeMonikerNETFramework : RuntimeMonikerAttribute
{
    public RuntimeMonikerNETFramework() : base(KnownRuntimeMonikers.NETFramework)
    {
    }
}