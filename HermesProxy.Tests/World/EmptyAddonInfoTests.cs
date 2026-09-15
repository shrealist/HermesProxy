using System.IO;
using System.IO.Compression;
using HermesProxy.World.Client;
using Xunit;

namespace HermesProxy.Tests.World;

public class EmptyAddonInfoTests
{
    [Fact]
    public void EmptyList_CanBeReadWithAzerothCoreLayout()
    {
        byte[] blob = WorldClient.BuildEmptyAddonInfoBlob();
        using var input = new MemoryStream(blob);
        using var header = new BinaryReader(input);
        uint declaredSize = header.ReadUInt32();
        using var zlib = new ZLibStream(input, CompressionMode.Decompress);
        using var decoded = new MemoryStream();
        zlib.CopyTo(decoded);
        Assert.Equal((long)declaredSize, decoded.Length);
        decoded.Position = 0;
        using var reader = new BinaryReader(decoded);
        Assert.Equal(0u, reader.ReadUInt32()); // No addon records follow.
        Assert.Equal(0u, reader.ReadUInt32()); // Core reads this even for an empty list.
        Assert.Equal(decoded.Length, decoded.Position);
    }
}
