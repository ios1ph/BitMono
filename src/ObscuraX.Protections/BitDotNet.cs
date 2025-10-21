﻿namespace ObscuraX.Protections;

[RuntimeMonikerMono]
public class BitDotNet : PackerProtection
{
    private const int PEHeaderWithExtraByteHex = 0x00014550;

    public BitDotNet(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override Task ExecuteAsync()
    {
        using (var stream = File.Open(Context.ObscuraXContext.OutputFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
        using (var reader = new BinaryReader(stream))
        using (var writer = new BinaryWriter(stream))
        {
            stream.Position = 0x3C;
            var peHeader = reader.ReadUInt32();
            stream.Position = peHeader;
            writer.Write(PEHeaderWithExtraByteHex);

            stream.Position += 0x2;
            var numberOfSections = reader.ReadUInt16();

            stream.Position += 0x10;
            var x64PEOptionsHeader = reader.ReadUInt16() == 0x20B;

            stream.Position += x64PEOptionsHeader ? 0x38 : 0x28 + 0xA6;
            var dotNetVirtualAddress = reader.ReadUInt32();

            uint dotNetPointerRaw = 0;
            stream.Position += 0xC;
            for (var i = 0; i < numberOfSections; i++)
            {
                stream.Position += 0xC;
                var virtualAddress = reader.ReadUInt32();
                var sizeOfRawData = reader.ReadUInt32();
                var pointerToRawData = reader.ReadUInt32();
                stream.Position += 0x10;

                if (dotNetVirtualAddress >= virtualAddress && dotNetVirtualAddress < virtualAddress + sizeOfRawData && dotNetPointerRaw == 0)
                {
                    dotNetPointerRaw = dotNetVirtualAddress + pointerToRawData - virtualAddress;
                }
            }

            stream.Position = dotNetPointerRaw;
            writer.Write(0);
            writer.Write(0);
            stream.Position += 0x4;
            writer.Write(0);
        }
        return Task.CompletedTask;
    }
}