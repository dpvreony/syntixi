namespace Syntixi.SourceGenerator
{
    /// <summary>
    /// Diagnostic ID string constants.
    /// </summary>
    public static class ReportDiagnosticIds
    {
        /// <summary>
        /// Diagnostic ID for when the Syncfusion license key is not present in the environment variables.
        /// </summary>
        public const string SyncfusionLicenseArgumentNotPresent = "STX001";

        /// <summary>
        /// Diagnostic ID for when the Syncfusion license key is empty in the environment variables.
        /// </summary>
        public const string SyncfusionLicenseArgumentEmpty = "STX002";

        /// <summary>
        /// Diagnostic ID for when the syntax node is null.
        /// </summary>
        public const string SyntaxNodeIsNull = "STX003";

        /// <summary>
        /// Diagnostic ID for when the class the syncfusion license key generation attribute is attached to is not a partial class.
        /// </summary>
        public const string ClassNotPartial = "STX004";
    }
}
