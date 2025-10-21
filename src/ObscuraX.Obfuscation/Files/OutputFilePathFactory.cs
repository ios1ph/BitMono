﻿namespace ObscuraX.Obfuscation.Files;

public static class OutputFilePathFactory
{
    private const string WatermarkText = "_secured";

    public static string Create(ObscuraXContext context)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(Path.GetFileNameWithoutExtension(context.FileName));
        if (context.Watermark)
        {
            stringBuilder.Append(WatermarkText);
        }
        stringBuilder.Append(Path.GetExtension(context.FileName));
        return Path.Combine(context.OutputDirectoryName, stringBuilder.ToString());
    }
}
