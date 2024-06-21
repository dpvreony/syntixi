using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Syntixi.SourceGenerator.EmbedSyncfusionLicenseKey;
using Xunit;

namespace Syntixi.UnitTests.SourceGenerator.EmbedSyncfusionLicenseKey
{
    public sealed class EmbedSyncfusionLicenseKeyGeneratorTests
    {
        [Fact]
        public void Test()
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
                        """),
                    CSharpSyntaxTree.ParseText("namespace SomeNamespace{[Syntixi.Attributes.EmbedSyncfusionLicenseKeyAttribute]public partial class TestClass { }}")
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
                "namespace SomeNamespace\r\n{\r\n    public partial class TestClass\r\n    {\r\n        private const string SYNCFUSION_LICENSE_KEY = \"SOMELICENSEKEY\";\r\n    }\r\n}",
                generatedCodeAsString);
        }
    }
}
