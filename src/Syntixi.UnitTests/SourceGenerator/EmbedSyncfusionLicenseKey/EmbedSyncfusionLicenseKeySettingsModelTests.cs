using NetTestRegimentation;
using Xunit;

namespace Syntixi.UnitTests.SourceGenerator.EmbedSyncfusionLicenseKey
{
    public static class EmbedSyncfusionLicenseKeySettingsModelTests
    {
        public sealed class ConstructorMethod : ITestConstructorMethod
        {
            [Fact]
            public void ReturnsInstance()
            {
                var instance = new Syntixi.SourceGenerator.EmbedSyncfusionLicenseKey.EmbedSyncfusionLicenseKeySettingsModel("TestLicenseKey");
                Assert.NotNull(instance);
            }
        }
    }
}
