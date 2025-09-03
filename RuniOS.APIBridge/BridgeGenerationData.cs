using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace RuniOS.APIBridge
{
    public record struct BridgeGenerationData
    (
        ImmutableArray<string> targetAssemblies,
        INamedTypeSymbol targetSymbol,
        ImmutableArray<string> includeMembers,
        ImmutableArray<string> excludeMembers,
        bool forceStatic,
        bool skipCreateInstance,
        bool includePublicMember
    );
}