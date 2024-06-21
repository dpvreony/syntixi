using Syntixi.Attributes;

namespace Syntixi.Sample.Cmd
{
    [EmbedSyncfusionLicenseKey]
    public partial class SomeAppClass
    {
#if SYNTIXI_SYNCFUSION_LICENSE_KEY
        public string GetSampleKey() => SYNCFUSION_LICENSE_KEY;
#endif
    }
}
