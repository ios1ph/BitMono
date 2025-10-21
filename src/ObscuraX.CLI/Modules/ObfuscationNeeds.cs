﻿namespace ObscuraX.CLI.Modules;

internal class ObfuscationNeeds
{
#pragma warning disable CS8618
    public string FileName { get; set; }
    public string FileBaseDirectory { get; set; }
    public string ReferencesDirectoryName { get; set; }
    public string OutputPath { get; set; }
    public ObfuscationNeedsWay Way { get; set; }
    public List<string> Protections { get; set; }
    public ProtectionSettings? ProtectionSettings { get; set; }
    public string? CriticalsFile { get; set; }
    public string? LoggingFile { get; set; }
    public string? ObfuscationFile { get; set; }
    public string? ProtectionsFile { get; set; }
    public ObfuscationSettings? ObfuscationSettings { get; set; }
#pragma warning restore CS8618
}

/// <summary>
/// The way <see cref="ObfuscationNeeds"/> was created.
/// </summary>
public enum ObfuscationNeedsWay
{
    Unknown,
    Readline,
    Options,
    Other,
}