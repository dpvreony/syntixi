using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Syntixi.SourceGenerator.EmbedSyncfusionLicenseKey;
using Xunit;

namespace Syntixi.IntegrationTests.SourceGenerator.EmbedSyncfusionLicenseKey
{
    public sealed class EmbedSyncfusionLicenseKeyGeneratorTests
    {
        [InlineData(@"#define SYNTIXI_SYNCFUSION_LICENSE_KEY 1
namespace SomeNamespace{
    /// <summary>
    /// Blazor application class.
    /// </summary>
#if SYNTIXI_SYNCFUSION_LICENSE_KEY
    [Syntixi.Attributes.EmbedSyncfusionLicenseKeyAttribute]
#endif
    public partial class App
    {
        /// <summary>
        /// Initializes a new instance of the <see cref=""App""/> class.
        /// </summary>
        public App()
        {
#if SYNTIXI_SYNCFUSION_LICENSE_KEY
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_LICENSE_KEY);
#endif
        }
    }
}",
            "namespace SomeNamespace\r\n{\r\n    public partial class App\r\n    {\r\n        private const string SYNCFUSION_LICENSE_KEY = \"SOMELICENSEKEY\";\r\n    }\r\n}")]
        [InlineData(
            "namespace SomeNamespace{[Syntixi.Attributes.EmbedSyncfusionLicenseKeyAttribute]public partial class TestClass { }}",
            "namespace SomeNamespace\r\n{\r\n    public partial class TestClass\r\n    {\r\n        private const string SYNCFUSION_LICENSE_KEY = \"SOMELICENSEKEY\";\r\n    }\r\n}")]
        [Theory]
        public void Test(string sourceCodeToTest, string expected)
        {
            var globalOptions = new InMemoryAnalyzerConfigOptions();
            globalOptions.Add(
                "build_property.SyncfusionLicense",
                "SOMELICENSEKEY");

            var inMemoryAnalyzerConfigOptionsProvider =  new InMemoryAnalyzerConfigOptionsProvider(globalOptions);
            var references = new List<MetadataReference>();
            var cSharpCompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create("TestProject",
                [
                    CSharpSyntaxTree.ParseText(
                        """
                        namespace Syntixi.Attributes
                        {
                            [AttributeUsage(AttributeTargets.Class, Inherited = false)]
                            public sealed class EmbedSyncfusionLicenseKeyAttribute : Attribute;
                        }
                        """,
                        cancellationToken: TestContext.Current.CancellationToken),
                    CSharpSyntaxTree.ParseText(
                        sourceCodeToTest,
                        cancellationToken: TestContext.Current.CancellationToken)
                ],
                references,
                cSharpCompilationOptions);

            var generator = new EmbedSyncfusionLicenseKeyGenerator();
            var sourceGenerator = generator.AsSourceGenerator();

            // trackIncrementalGeneratorSteps allows to report info about each step of the generator
            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                generators: [sourceGenerator],
                optionsProvider: inMemoryAnalyzerConfigOptionsProvider,
                driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));

            // Run the generator
            driver = driver.RunGenerators(compilation);

            // Update the compilation and rerun the generator
            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText("// dummy"));
            driver = driver.RunGenerators(compilation);

            var result = driver.GetRunResult().Results.Single();
            var allOutputs = result.TrackedOutputSteps.SelectMany(outputStep => outputStep.Value).SelectMany(output => output.Outputs);
            _ = Assert.Single(allOutputs, x => x.Reason == IncrementalStepRunReason.Cached);

            var forAttributeWithMetadataNameOutputs = result.TrackedSteps["result_ForAttributeWithMetadataName"].Single().Outputs;
            _ = Assert.Single(
                forAttributeWithMetadataNameOutputs,
                x => x.Reason == IncrementalStepRunReason.Unchanged);

            var generatedOutput = Assert.Single(result.GeneratedSources);
            var generatedCodeAsString = generatedOutput.SourceText.ToString();
            Assert.Equal(
                expected,
                generatedCodeAsString);
        }
    }
}
