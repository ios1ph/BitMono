namespace ObscuraX.Core.Contexts;

public class ObscuraXContext
{
#pragma warning disable CS8618
    public string FileName { get; set; }
    public string OutputDirectoryName { get; set; }
    public string OutputFile { get; set; }
    public List<byte[]> ReferencesData { get; set; }
    public bool Watermark { get; set; }
#pragma warning restore CS8618
}
