using Framework.Realm;
using HermesProxy.Configuration.Options;
using HermesProxy.Enums;
using Xunit;

namespace HermesProxy.Tests.BnetServer;

public class ClientBuildCompatibilityTests
{
    [Theory]
    [InlineData(ClientVersionBuild.V3_4_3_52237)]
    [InlineData(ClientVersionBuild.V3_4_3_54261)]
    public void Wrath343_UsesExistingProtocolAndLegacyBackend(ClientVersionBuild build)
    {
        Assert.True(VersionChecker.IsSupportedModernVersion(build));
        Assert.Equal(ClientVersionBuild.V3_4_3_54261, VersionChecker.GetProtocolBuild(build));
        Assert.Equal(ClientVersionBuild.V3_3_5a_12340, VersionChecker.GetBestLegacyVersion(build));
    }

    [Theory]
    [InlineData(ClientVersionBuild.V1_14_2_42597)]
    [InlineData(ClientVersionBuild.V2_5_2_40892)]
    public void ExistingBuilds_KeepTheirProtocolIdentity(ClientVersionBuild build)
    {
        Assert.Equal(build, VersionChecker.GetProtocolBuild(build));
    }

    [Theory]
    [InlineData(52237u)]
    [InlineData(54261u)]
    public void RealmBuildInfo_PreservesExactClientBuild(uint build)
    {
        var manager = new RealmManager(new ClientOptions
        {
            ClientBuild = (ClientVersionBuild)build,
            ClientSeed = new byte[16]
        }, new ProxyNetworkOptions());
        Assert.Equal(build, manager.GetBuildInfo(build)!.Build);
        Assert.Null(manager.GetBuildInfo(build == 52237u ? 54261u : 52237u));
    }

    [Fact]
    public void UnknownBuild_IsStillRejected()
    {
        Assert.False(VersionChecker.IsSupportedModernVersion((ClientVersionBuild)52238));
    }
}
