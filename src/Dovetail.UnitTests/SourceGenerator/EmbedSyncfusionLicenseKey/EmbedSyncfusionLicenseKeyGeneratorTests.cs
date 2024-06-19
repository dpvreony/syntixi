﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dovetail.SourceGenerator.EmbedSyncfusionLicenseKey;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Dovetail.UnitTests.SourceGenerator.EmbedSyncfusionLicenseKey
{
    public sealed class EmbedSyncfusionLicenseKeyGeneratorTests
    {
        [Fact]
        public void Test()
        {
            var references = new List<MetadataReference>();

            var compilation = CSharpCompilation.Create("TestProject",
                new[] { CSharpSyntaxTree.ParseText("[EmbedSyncfusionLicenseKeyAttribute]public partial class TestClass { }") },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new EmbedSyncfusionLicenseKeyGenerator();
            var sourceGenerator = generator.AsSourceGenerator();

            // trackIncrementalGeneratorSteps allows to report info about each step of the generator
            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                generators: new ISourceGenerator[] { sourceGenerator },
                driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));

            // Run the generator
            driver = driver.RunGenerators(compilation);

            // Update the compilation and rerun the generator
            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText("// dummy"));
            driver = driver.RunGenerators(compilation);

            // Assert the driver doesn't recompute the output
            var result = driver.GetRunResult().Results.Single();
            var allOutputs = result.TrackedOutputSteps.SelectMany(outputStep => outputStep.Value).SelectMany(output => output.Outputs);
            Assert.Collection(allOutputs, output => Assert.Equal(IncrementalStepRunReason.Cached, output.Reason));

            // Assert the driver use the cached result from AssemblyName and Syntax
            var assemblyNameOutputs = result.TrackedSteps["AssemblyName"].Single().Outputs;
            Assert.Collection(assemblyNameOutputs, output => Assert.Equal(IncrementalStepRunReason.Unchanged, output.Reason));

            var syntaxOutputs = result.TrackedSteps["Syntax"].Single().Outputs;
            Assert.Collection(syntaxOutputs, output => Assert.Equal(IncrementalStepRunReason.Unchanged, output.Reason));
        }
    }
}
