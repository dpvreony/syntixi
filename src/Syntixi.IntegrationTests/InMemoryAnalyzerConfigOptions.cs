using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Syntixi.IntegrationTests
{
    /// <summary>
    /// In memory implementation of <see cref="AnalyzerConfigOptions"/> to support unit testing.
    /// </summary>
    public sealed class InMemoryAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly Dictionary<string, string> _options = [];

        public void Add(string key, string value)
        {
            _options.Add(key, value);
        }

        /// <inheritdoc />
        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        {
            return _options.TryGetValue(key, out value);
        }
    }
}
