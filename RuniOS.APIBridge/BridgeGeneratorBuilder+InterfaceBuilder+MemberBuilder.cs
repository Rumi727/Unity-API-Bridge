using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace RuniOS.APIBridge
{
    public partial class BridgeGeneratorBuilder
    {
        partial struct InterfaceBuilder
        {
            public readonly struct MemberBuilder(BridgeGeneratorBuilder builder, INamedTypeSymbol targetSymbol)
            {
                public static void Build(BridgeGeneratorBuilder builder, INamedTypeSymbol targetSymbol) => new MemberBuilder(builder, targetSymbol).Build();

                readonly BridgeGeneratorBuilder builder = builder;
                
                readonly string targetTypeName = targetSymbol.GetFullTypeName();
                readonly bool targetIsNonPublic = targetSymbol.IsNonPublicMember();

                void Append(string text = "") => builder.Append(text);

                void AppendLine(string text = "") => builder.AppendLine(text);

                void StartBlock() => builder.StartBlock();

                void EndBlock() => builder.EndBlock();

                void StartComment() => builder.StartComment();

                void EndComment() => builder.EndComment();

                void AppendLineDocumentation(ISymbol symbol) => builder.AppendLineDocumentation(symbol);

                public void Build()
                {
                    BridgeGeneratorBuilder? builder = this.builder;
                    var members = targetSymbol.GetMembers()
                        .Where(static x => x is IFieldSymbol or IPropertySymbol or IEventSymbol or IMethodSymbol)
                        .Where(static x => !x.IsStatic)
                        .Where(static x => !x.IsImplicitlyDeclared)
                        .Where(static x => x.IsNonPublicMember())
                        .Where(static x => !x.IsInternalCall())
                        .Where(static x => !x.IsExplicitInterfaceImplementations())
                        .Where(static x => !x.IsCompilerGenerated())
                        .Where(x => !builder.includeMembers.Any() || builder.includeMembers.Contains(x.Name))
                        .Where(x => !builder.excludeMembers.Contains(x.Name));
                    if (members.Any())
                    {
                        AppendLine();
                        AppendLine();
                    }
                    foreach (var member in members)
                    {
                        AppendLine();
                        
                        string memberName = member.Name;
                        if (member.IsObsolete(out string message))
                            AppendLine($"[global::System.ObsoleteAttribute(\"{message}\")]");

                        switch (member)
                        {
                            case IFieldSymbol field:
                            {
                                string fieldTypeName = field.Type.GetTypeNameOrBridgeName();
                                bool fieldTypeIsNonPublic = false;
                                bool fieldTypeIsDelegate = false;
                                INamedTypeSymbol? namedTypeSymbol = field.Type.GetNamedTypeSymbol();
                                if (namedTypeSymbol != null)
                                {
                                    if (namedTypeSymbol.TypeKind == TypeKind.Delegate)
                                        fieldTypeIsDelegate = true;

                                    if (namedTypeSymbol.IsNonPublicMember())
                                    {
                                        fieldTypeIsNonPublic = true;
                                        builder.nonPublicTypeSymbols.Add(new BridgeGenerationData(builder.targetAssemblies, namedTypeSymbol.OriginalDefinition, ImmutableArray<string>.Empty, ImmutableArray<string>.Empty, false, false));
                                    }
                                }

                                if (fieldTypeIsDelegate && fieldTypeIsNonPublic)
                                    StartComment();
                                
                                Append($"public {fieldTypeName} {memberName} {{ get; {(!field.IsReadOnly ? "set; " : string.Empty)}}}");
                                
                                if (fieldTypeIsDelegate && fieldTypeIsNonPublic)
                                    EndComment();

                                break;
                            }
                            case IPropertySymbol property:
                            {
                                // 인덱서는 아직 구현되지 않음.
                                if (property.IsIndexer)
                                    break;
                                
                                string propertyTypeName = property.Type.GetTypeNameOrBridgeName();
                                bool propertyTypeIsNonPublic = false;
                                bool propertyTypeIsDelegate = false;
                                INamedTypeSymbol? namedTypeSymbol = property.Type.GetNamedTypeSymbol();
                                if (namedTypeSymbol != null)
                                {
                                    if (namedTypeSymbol.TypeKind == TypeKind.Delegate)
                                        propertyTypeIsDelegate = true;

                                    if (namedTypeSymbol.IsNonPublicMember())
                                    {
                                        propertyTypeIsNonPublic = true;
                                        builder.nonPublicTypeSymbols.Add(new BridgeGenerationData(builder.targetAssemblies, namedTypeSymbol.OriginalDefinition, ImmutableArray<string>.Empty, ImmutableArray<string>.Empty, false, false));
                                    }
                                }

                                if (propertyTypeIsDelegate && propertyTypeIsNonPublic)
                                    StartComment();

                                Append($"public {propertyTypeName} {memberName} {{ {(property.GetMethod != null ? "get; " : string.Empty)}{(property.SetMethod != null ? "set; " : string.Empty)}}}");

                                if (propertyTypeIsDelegate && propertyTypeIsNonPublic)
                                    EndComment();

                                break;
                            }
                            case IEventSymbol eventSymbol:
                            {
                                string eventTypeName = eventSymbol.Type.GetTypeNameOrBridgeName();
                                bool eventTypeIsNonPublic = eventSymbol.Type.IsNonPublicMember();

                                // 딜리게이트가 Public이 아닐때 어떻게 브릿지를 지을지 생각하지 못했습니다.
                                if (eventTypeIsNonPublic)
                                    StartComment();

                                Append($"public event {eventTypeName} {memberName} {{ add; remove; }}");

                                if (eventTypeIsNonPublic)
                                    EndComment();

                                break;
                            }
                            case IMethodSymbol method when method.MethodKind == MethodKind.Ordinary:
                            {
                                string returnType = method.GetMethodReturnTypeName();
                                string parameters = string.Join(", ", method.Parameters.GetParameterText());

                                bool returnTypeIsNonPublic = false;
                                bool returnTypeIsDelegate = false;
                                if (!method.ReturnsVoid)
                                {
                                    INamedTypeSymbol? namedReturnType = method.ReturnType.GetNamedTypeSymbol();
                                    if (namedReturnType != null)
                                    {
                                        if (namedReturnType.TypeKind == TypeKind.Delegate)
                                            returnTypeIsDelegate = true;

                                        if (namedReturnType.IsNonPublicMember())
                                        {
                                            builder.nonPublicTypeSymbols.Add(new BridgeGenerationData(builder.targetAssemblies, namedReturnType.OriginalDefinition, ImmutableArray<string>.Empty, ImmutableArray<string>.Empty, false, false));
                                            returnTypeIsNonPublic = true;
                                        }
                                    }
                                }

                                builder.nonPublicTypeSymbols.AddRange(method.Parameters
                                    .Select(static x => x.Type.GetNamedTypeSymbol())
                                    .OfType<INamedTypeSymbol>()
                                    .Where(static x => x.IsNonPublicMember())
                                    .Select(x => new BridgeGenerationData(builder.targetAssemblies, x.OriginalDefinition, ImmutableArray<string>.Empty, ImmutableArray<string>.Empty, false, false)));

                                if (returnTypeIsDelegate && returnTypeIsNonPublic)
                                    StartComment();

                                Append("public ");
                                if (method.IsUnsafe())
                                    Append("unsafe ");
                                Append($"{returnType} {memberName}{method.GetTypeArgumentsText()}({parameters}) {method.GetConstraintsText()};");
                                
                                if (returnTypeIsDelegate && returnTypeIsNonPublic)
                                    EndComment();

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}