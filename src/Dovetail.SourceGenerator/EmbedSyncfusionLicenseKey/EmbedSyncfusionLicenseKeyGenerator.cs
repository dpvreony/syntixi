using System.Linq;
using System.Text;
using Dovetail.Attributes;
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
                .ForAttributeWithMetadataName(
                    "Dovetail.Attributes.EmbedSyncfusionLicenseKeyAttribute",
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

            if (sourceText == null)
            {
                return;
            }

            productionContext.AddSource(
                hintName,
                sourceText);
        }

        private static SourceText? GetSourceText(ClassDeclarationSyntax classDeclarationSyntax, string syncfusionLicenseKey)
        {
            var memberDeclarationSyntax = GetMemberDeclarationSyntax(
                classDeclarationSyntax,
                syncfusionLicenseKey);

            if (memberDeclarationSyntax == null)
            {
                return null;
            }


            var cu = SyntaxFactory.CompilationUnit()
                .AddMembers(memberDeclarationSyntax);

            var triviaList = GetTriviaList();
            if (triviaList != null)
            {
                cu = cu
                    .WithLeadingTrivia(triviaList);
            }

            cu = cu.NormalizeWhitespace();

            var syntaxTree = classDeclarationSyntax.SyntaxTree;
            var parseOptions = syntaxTree.Options;

            var sourceText = SyntaxFactory.SyntaxTree(
                    cu,
                    parseOptions,
                    encoding: Encoding.UTF8)
                .GetText();

            return sourceText;
        }

        private static MemberDeclarationSyntax[]? GetMemberDeclarationSyntax(
            ClassDeclarationSyntax classDeclarationSyntax,
            string syncfusionLicenseKey)
        {
            var namespaceDeclarationSyntax = GetNamespaceDeclarationSyntax(classDeclarationSyntax);
            if (namespaceDeclarationSyntax == null)
            {
                return null;
            }

            var newClassDeclarationSyntax = GetClassDeclarationSyntax(
                classDeclarationSyntax,
                syncfusionLicenseKey);
            if (newClassDeclarationSyntax == null)
            {
                return null;
            }

            return [namespaceDeclarationSyntax.AddMembers(newClassDeclarationSyntax)];
        }

        private static ClassDeclarationSyntax? GetClassDeclarationSyntax(ClassDeclarationSyntax classDeclarationSyntax, string syncfusionLicenseKey)
        {
            var className = classDeclarationSyntax.Identifier;
            var classMembers = GetClassMembers(syncfusionLicenseKey);
            var modifiers = classDeclarationSyntax.Modifiers;
            return SyntaxFactory.ClassDeclaration(className)
                .WithModifiers(modifiers)
                .AddMembers(classMembers);
        }

        private static MemberDeclarationSyntax[] GetClassMembers(string syncfusionLicenseKey)
        {

            var attributeLists = SyntaxFactory.List<AttributeListSyntax>();

            var variableType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword));

            var variableIdentifier = SyntaxFactory.Identifier("SYNCFUSION_LICENSE_KEY");
            var initializer = SyntaxFactory.EqualsValueClause(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal(syncfusionLicenseKey)));
            var variableDeclaration = SyntaxFactory.VariableDeclarator(variableIdentifier, null, initializer);
            var variables = SyntaxFactory.SingletonSeparatedList(variableDeclaration);

            var declaration = SyntaxFactory.VariableDeclaration(variableType, variables);

            var modifiers = new SyntaxTokenList(
                SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                SyntaxFactory.Token(SyntaxKind.ConstKeyword));

            var semicolonToken = SyntaxFactory.Token(SyntaxKind.SemicolonToken);

            return [SyntaxFactory.FieldDeclaration(attributeLists, modifiers, declaration, semicolonToken)];
        }

        private static NamespaceDeclarationSyntax? GetNamespaceDeclarationSyntax(ClassDeclarationSyntax classDeclarationSyntax)
        {
            var parent = classDeclarationSyntax.Parent;
            while (parent != null)
            {
                if (parent is NamespaceDeclarationSyntax namespaceDeclarationSyntax)
                {
                    return namespaceDeclarationSyntax;
                }
                parent = parent.Parent;
            }

            return null;
        }

        private static SyntaxTrivia[]? GetTriviaList()
        {
            return null;
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
