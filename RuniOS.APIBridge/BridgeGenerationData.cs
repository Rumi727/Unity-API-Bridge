using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RuniOS.APIBridge
{
    public record struct BridgeGenerationData
    (
        INamedTypeSymbol targetSymbol,
        ImmutableArray<string> includeMembers,
        ImmutableArray<string> excludeMembers,
        bool forceStatic,
        bool skipCreateInstance
    );
}