using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RuniOS.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AssetRefSerializationSuppressor : DiagnosticSuppressor
    {
        const string assetRefMetadataName = "RuniOS.Resource.AssetRef`1";

        static readonly SuppressionDescriptor descriptor = new
        (
            "ROSSUP0001",
            "UAC1001",
            "AssetRef<TAsset> intentionally allows TAsset to be skipped by Unity serialization when unsupported."
        );

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(descriptor);

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            INamedTypeSymbol? assetRefType = context.Compilation.GetTypeByMetadataName(assetRefMetadataName);
            if (assetRefType == null)
                return;

            foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
            {
                if (diagnostic.Id != "UAC1001" || !diagnostic.Location.IsInSource)
                    continue;

                SyntaxTree? tree = diagnostic.Location.SourceTree;
                if (tree == null)
                    continue;

                SyntaxNode root = tree.GetRoot(context.CancellationToken);
                SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

                FieldDeclarationSyntax? declaration = node.FirstAncestorOrSelf<FieldDeclarationSyntax>();
                if (declaration == null)
                    continue;

                SemanticModel semanticModel = context.GetSemanticModel(tree);
                foreach (VariableDeclaratorSyntax variable in declaration.Declaration.Variables)
                {
                    if (semanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is not IFieldSymbol field)
                        continue;

                    if (field.Type is not INamedTypeSymbol fieldType)
                        continue;

                    if (!SymbolEqualityComparer.Default.Equals(fieldType.OriginalDefinition, assetRefType))
                        continue;

                    context.ReportSuppression(Suppression.Create(descriptor, diagnostic));
                    break;
                }
            }
        }
    }
}