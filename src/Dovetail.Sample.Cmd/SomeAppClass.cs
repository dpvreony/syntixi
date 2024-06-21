using Dovetail.Attributes;

namespace Dovetail.Sample.Cmd
{
    [EmbedSyncfusionLicenseKey]
    public partial class SomeAppClass
    {
#if DOVETAIL_SYNCFUSION_LICENSE_KEY
        public string GetSampleKey() => SYNCFUSION_LICENSE_KEY;
#endif
    }
}
