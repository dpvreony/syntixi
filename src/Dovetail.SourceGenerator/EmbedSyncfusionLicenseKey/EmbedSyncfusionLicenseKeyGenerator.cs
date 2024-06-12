using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Dovetail.SourceGenerator.EmbedSyncfusionLicenseKey
{
    /// <summary>
    /// Source generator that embeds the Syncfusion license key in a partial class.
    /// </summary>
    [Generator]
    public sealed class EmbedSyncfusionLicenseKeyGenerator : IIncrementalGenerator
    {
        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classesToEmbedLicenseKeysIn = context.SyntaxProvider
                .ForAttributeWithMetadataName( // 👈 use the new API
                    typeof(EmbedSyncfusionLicenseKeyAttribute).FullName!,
                    predicate: (node, _) => node is ClassDeclarationSyntax,
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx));

            var trigger = classesToEmbedLicenseKeysIn.Combine(context.AnalyzerConfigOptionsProvider);

            context.RegisterImplementationSourceOutput(
                trigger,
                static (productionContext, triggerValue) => GeneratePartialClass(productionContext, triggerValue.Left, triggerValue.Right));
        }

        private static void GeneratePartialClass(
            SourceProductionContext productionContext,
            ClassDeclarationSyntax? syntax,
            AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider)
        {
            var globalOptions = analyzerConfigOptionsProvider.GlobalOptions;
            if (!globalOptions.TryGetBuildPropertyValue("SyncfusionLicense", out var syncfusionLicenseKey))
            {
                productionContext.ReportDiagnostic(DiagnosticFactory.SyncfusionLicenseArgumentNotPresent());
                return;
            }

            if (string.IsNullOrWhiteSpace(syncfusionLicenseKey))
            {
                productionContext.ReportDiagnostic(DiagnosticFactory.SyncfusionLicenseArgumentEmpty());
                return;
            }

            if (syntax is null)
            {
                productionContext.ReportDiagnostic(DiagnosticFactory.SyntaxNodeIsNull());
                return;
            }

            if (syntax.Modifiers.All(x => !x.IsKind(SyntaxKind.PartialKeyword)))
            {
                productionContext.ReportDiagnostic(DiagnosticFactory.ClassNotPartial());
                return;
            }

            var hintName = $"{syntax.Identifier.ValueText}.SyncfusionLicense.cs";
            var sourceText = GetSourceText(
                syntax,
                syncfusionLicenseKey!);

            productionContext.AddSource(
                hintName,
                sourceText);
        }

        private static SourceText GetSourceText(ClassDeclarationSyntax classDeclarationSyntax, string syncfusionLicenseKey)
        {
            var memberDeclarationSyntax = GetMemberDeclarationSyntax(
                classDeclarationSyntax,
                syncfusionLicenseKey);

            var triviaList = GetTriviaList();

            var cu = SyntaxFactory.CompilationUnit()
                .AddMembers(memberDeclarationSyntax)
                .WithLeadingTrivia(triviaList)
                .NormalizeWhitespace();

            var syntaxTree = classDeclarationSyntax.SyntaxTree;
            var parseOptions = syntaxTree.Options;

            var sourceText = SyntaxFactory.SyntaxTree(
                    cu,
                    parseOptions,
                    encoding: Encoding.UTF8)
                .GetText();

            return sourceText;
        }

        private static MemberDeclarationSyntax[] GetMemberDeclarationSyntax(
            ClassDeclarationSyntax classDeclarationSyntax,
            string syncfusionLicenseKey)
        {
            throw new System.NotImplementedException();
        }

        private static SyntaxTrivia[]? GetTriviaList()
        {
            throw new System.NotImplementedException();
        }

        private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext ctx)
        {
            return ctx.TargetNode as ClassDeclarationSyntax;
        }

        private static void GeneratePartialClass(
            SourceProductionContext productionContext,
            (ClassDeclarationSyntax? Left, AnalyzerConfigOptionsProvider Right) syntax)
        {
        }
    }
}
