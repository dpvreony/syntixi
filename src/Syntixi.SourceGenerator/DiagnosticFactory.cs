using Microsoft.CodeAnalysis;

namespace Syntixi.SourceGenerator
{
    public static class DiagnosticFactory
    {
        public static Diagnostic SyncfusionLicenseArgumentNotPresent(Location? location)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.SyncfusionLicenseArgumentNotPresent,
                "The syncfusion license key is not present in the environment variables.",
                location);
        }

        public static Diagnostic SyncfusionLicenseArgumentEmpty(Location? location)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.SyncfusionLicenseArgumentEmpty,
                "The syncfusion license key is empty in the environment variables.",
                location);
        }

        public static Diagnostic SyntaxNodeIsNull(Location? location)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.SyntaxNodeIsNull,
                "The syncfusion license key is not present in the environment variables.",
                location);
        }

        public static Diagnostic ClassNotPartial(Location? location)
        {
            return ErrorDiagnostic(
                ReportDiagnosticIds.ClassNotPartial,
                "The class the syncfusion license key generation is attached to is not a partial class.",
                location);
        }

        private static Diagnostic InfoDiagnostic(
            string id,
            string message,
            Location? location)
        {
            return GetDiagnostic(
                id,
                message,
                DiagnosticSeverity.Info,
                1,
                location);
        }

        private static Diagnostic ErrorDiagnostic(
            string id,
            string message,
            Location? location)
        {
            return GetDiagnostic(
                id,
                message,
                DiagnosticSeverity.Error,
                0,
                location);
        }

        private static Diagnostic GetDiagnostic(string id,
            string message,
            DiagnosticSeverity severity,
            int warningLevel,
            Location? location)
        {
            return Diagnostic.Create(
                id,
                "Syntixi Generation",
                message,
                severity,
                severity,
                true,
                warningLevel,
                "Syntixi Generation",
                location: location);
        }
    }
}
