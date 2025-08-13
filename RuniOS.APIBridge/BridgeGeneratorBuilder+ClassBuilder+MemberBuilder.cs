using Microsoft.CodeAnalysis;
using System.Linq;

namespace RuniOS.APIBridge
{
    public partial class BridgeGeneratorBuilder
    {
        partial struct ClassBuilder
        {
            public readonly struct MemberBuilder(BridgeGeneratorBuilder builder, INamedTypeSymbol targetSymbol)
            {
                public static void Build(BridgeGeneratorBuilder builder, INamedTypeSymbol targetSymbol) => new MemberBuilder(builder, targetSymbol).Build();

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
                    bool targetIsNonPublic = this.targetIsNonPublic;
                    var members = targetSymbol.GetMembers().Where(x => x is IFieldSymbol or IPropertySymbol or IEventSymbol or IMethodSymbol && (targetIsNonPublic || x.IsNonPublicMember()) && !x.IsImplicitlyDeclared && !x.IsInternalCall() && !x.IsExplicitInterfaceImplementations());
                    if (members.Any())
                    {
                        AppendLine();
                        AppendLine();
                    }
                    foreach (var member in members)
                    {
                        AppendLine();
                        
                        bool isStaticMember = member.IsStatic;
                        string memberName = member.Name;

                        string? instanceAccessPrefix;
                        if (isStaticMember)
                            instanceAccessPrefix = targetTypeName; // 정적 멤버는 직접 접근
                        else // __instance가 object 타입일 경우 캐스팅을 추가합니다.
                            instanceAccessPrefix = targetIsNonPublic ? $"(({targetTypeName})__instance)" : "__instance";
                        
                        if (member.IsObsolete(out string message))
                            AppendLine($"[global::System.ObsoleteAttribute(\"{message}\")]");

                        switch (member)
                        {
                            case IFieldSymbol field:
                            {
                                string fieldTypeName = field.Type.GetTypeNameOrBridgeName();
                                bool fieldTypeIsNonPublic = false;
                                bool fieldTypeIsDelegate = false;
                                {
                                    INamedTypeSymbol? namedTypeSymbol = field.Type.GetNamedTypeSymbol();
                                    if (namedTypeSymbol != null)
                                    {
                                        if (namedTypeSymbol.TypeKind == TypeKind.Delegate)
                                            fieldTypeIsDelegate = true;

                                        if (namedTypeSymbol.IsNonPublicMember())
                                        {
                                            fieldTypeIsNonPublic = true;
                                            builder.nonPublicTypeSymbols.Add(namedTypeSymbol);
                                        }
                                    }
                                }

                                if (fieldTypeIsDelegate && fieldTypeIsNonPublic)
                                    StartComment();

                                Append("public ");
                                if (field.IsConst)
                                {
                                    if (field.Type.TypeKind == TypeKind.Enum)
                                        AppendLine($"const {fieldTypeName} {memberName} = ({fieldTypeName})({((INamedTypeSymbol)field.Type).EnumUnderlyingType?.GetFullTypeName()}){targetTypeName}.{memberName};");
                                    else
                                        AppendLine($"const {fieldTypeName} {memberName} = {targetTypeName}.{memberName};");
                                }
                                else
                                {
                                    if (isStaticMember)
                                        Append("static ");
                                    if (field.IsUnsafe())
                                        Append("unsafe ");
                                    Append($"{fieldTypeName} {memberName}");

                                    if (field.IsReadOnly)
                                    {
                                        if (fieldTypeIsNonPublic && field.Type.TypeKind != TypeKind.Enum)
                                            AppendLine($" => {field.Type.GetBridgeTypeFullName()}.__GetInstanceFrom({instanceAccessPrefix}.{memberName});");
                                        else
                                        {
                                            if (field.Type.TypeKind == TypeKind.Enum)
                                                AppendLine($" => ({fieldTypeName})({((INamedTypeSymbol)field.Type).EnumUnderlyingType?.GetFullTypeName()}){instanceAccessPrefix}.{memberName};");
                                            else
                                                AppendLine($" => {instanceAccessPrefix}.{memberName};");
                                        }
                                    }
                                    else
                                    {
                                        AppendLine();
                                        StartBlock();
                                        {
                                            if (fieldTypeIsNonPublic && field.Type.TypeKind != TypeKind.Enum)
                                            {
                                                AppendLine($"get => {field.Type.GetBridgeTypeFullName()}.__GetInstanceFrom({instanceAccessPrefix}.{memberName});");
                                                AppendLine($"set => {instanceAccessPrefix}.{memberName} = ({field.Type.GetFullTypeName()})value.__instance;");
                                            }
                                            else
                                            {
                                                if (field.Type.TypeKind == TypeKind.Enum)
                                                {
                                                    AppendLine($"get => ({fieldTypeName})({((INamedTypeSymbol)field.Type).EnumUnderlyingType?.GetFullTypeName()}){instanceAccessPrefix}.{memberName};");
                                                    AppendLine($"set => {instanceAccessPrefix}.{memberName} = ({field.Type.GetFullTypeName()})({((INamedTypeSymbol)field.Type).EnumUnderlyingType?.GetFullTypeName()})value;");
                                                }
                                                else
                                                {
                                                    AppendLine($"get => {instanceAccessPrefix}.{memberName};");
                                                    AppendLine($"set => {instanceAccessPrefix}.{memberName} = value;");
                                                }
                                            }
                                        }
                                        EndBlock();
                                    }
                                }
                                
                                if (fieldTypeIsDelegate && fieldTypeIsNonPublic)
                                    EndComment();

                                break;
                            }
                            case IPropertySymbol property:
                            {
                                // 인덱서는 아직 구현되지 않음.
                                if (property.IsIndexer)
                                    break;
                                
                                var propertyTypeName = property.Type.GetTypeNameOrBridgeName();
                                bool propertyTypeIsNonPublic = false;
                                bool propertyTypeIsDelegate = false;
                                {
                                    INamedTypeSymbol? namedTypeSymbol = property.Type.GetNamedTypeSymbol();
                                    if (namedTypeSymbol != null)
                                    {
                                        if (namedTypeSymbol.TypeKind == TypeKind.Delegate)
                                            propertyTypeIsDelegate = true;

                                        if (namedTypeSymbol.IsNonPublicMember())
                                        {
                                            propertyTypeIsNonPublic = true;
                                            builder.nonPublicTypeSymbols.Add(namedTypeSymbol);
                                        }
                                    }
                                }

                                if (propertyTypeIsDelegate && propertyTypeIsNonPublic)
                                    StartComment();

                                Append("public ");
                                if (isStaticMember)
                                    Append("static ");
                                if (property.IsUnsafe())
                                    Append("unsafe ");
                                
                                Append($"{propertyTypeName} {memberName}");

                                if (property.GetMethod != null && property.SetMethod == null)
                                {
                                    if (propertyTypeIsNonPublic && property.Type.TypeKind != TypeKind.Enum)
                                        AppendLine($" => {property.Type.GetBridgeTypeFullName()}.__GetInstanceFrom({instanceAccessPrefix}.{memberName});");
                                    else
                                    {
                                        if (property.Type.TypeKind == TypeKind.Enum)
                                            AppendLine($" => ({propertyTypeName})({((INamedTypeSymbol)property.Type).EnumUnderlyingType?.GetFullTypeName()}){instanceAccessPrefix}.{memberName};");
                                        else
                                            AppendLine($" => {instanceAccessPrefix}.{memberName};");
                                    }
                                }
                                else
                                {
                                    AppendLine();
                                    StartBlock();

                                    if (propertyTypeIsNonPublic && property.Type.TypeKind != TypeKind.Enum)
                                    {
                                        if (property.GetMethod != null)
                                            AppendLine($"get => {property.Type.GetBridgeTypeFullName()}.__GetInstanceFrom({instanceAccessPrefix}.{memberName});");
                                        if (property.SetMethod != null)
                                            AppendLine($"set => {instanceAccessPrefix}.{memberName} = ({property.Type.GetFullTypeName()})value.__instance;");
                                    }
                                    else
                                    {
                                        if (property.Type.TypeKind == TypeKind.Enum)
                                        {
                                            if (property.GetMethod != null)
                                                AppendLine($"get => ({propertyTypeName})({((INamedTypeSymbol)property.Type).EnumUnderlyingType?.GetFullTypeName()}){instanceAccessPrefix}.{memberName};");
                                            if (property.SetMethod != null)
                                                AppendLine($"set => {instanceAccessPrefix}.{memberName} = ({property.Type.GetFullTypeName()})({((INamedTypeSymbol)property.Type).EnumUnderlyingType?.GetFullTypeName()})value;");
                                        }
                                        else
                                        {
                                            if (property.GetMethod != null)
                                                AppendLine($"get => {instanceAccessPrefix}.{memberName};");
                                            if (property.SetMethod != null)
                                                AppendLine($"set => {instanceAccessPrefix}.{memberName} = value;");
                                        }
                                    }

                                    EndBlock();
                                }

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

                                Append("public ");
                                if (isStaticMember)
                                    Append("static ");
                                AppendLine($"event {eventTypeName} {memberName}");

                                StartBlock();
                                {
                                    if (eventSymbol.Type.IsNonPublicMember())
                                    {
                                        if (eventSymbol.AddMethod != null)
                                            AppendLine($"add => {instanceAccessPrefix}.{memberName} += value.__instance;");
                                        if (eventSymbol.RemoveMethod != null)
                                            AppendLine($"remove => {instanceAccessPrefix}.{memberName} -= value.__instance;");
                                    }
                                    else
                                    {
                                        if (eventSymbol.AddMethod != null)
                                            AppendLine($"add => {instanceAccessPrefix}.{memberName} += value;");
                                        if (eventSymbol.RemoveMethod != null)
                                            AppendLine($"remove => {instanceAccessPrefix}.{memberName} -= value;");
                                    }
                                }
                                EndBlock();

                                if (eventTypeIsNonPublic)
                                    EndComment();

                                break;
                            }
                            case IMethodSymbol method when method.MethodKind == MethodKind.Ordinary:
                            {
                                string returnType = method.GetMethodReturnTypeName();
                                string parameters = string.Join(", ", method.Parameters.GetParameterText());
                                string callParameters = string.Join(", ", method.Parameters.GetCallParameterText());

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
                                            builder.nonPublicTypeSymbols.Add(namedReturnType);
                                            returnTypeIsNonPublic = true;
                                        }
                                    }
                                }

                                builder.nonPublicTypeSymbols.AddRange(method.Parameters.Select(static x => x.Type.GetNamedTypeSymbol()).OfType<INamedTypeSymbol>().Where(static x => x.IsNonPublicMember()));

                                if (returnTypeIsDelegate && returnTypeIsNonPublic)
                                    StartComment();

                                Append("public ");
                                if (isStaticMember)
                                    Append("static ");
                                if (method.IsUnsafe())
                                    Append("unsafe ");
                                Append($"{returnType} {memberName}{method.GetTypeParametersText()}");
                                Append($"({parameters})");
                                {
                                    string constraintsText = method.GetConstraintsText();
                                    if (!string.IsNullOrEmpty(constraintsText))
                                        Append($" {constraintsText}");
                                }
                                if (!method.Parameters.Where(static x => x.Type.IsNonPublicMember() && x.RefKind != RefKind.None).Any())
                                {
                                    Append(" => ");
                                    if (!method.ReturnsVoid && returnTypeIsNonPublic && method.ReturnType.TypeKind != TypeKind.Enum)
                                        AppendLine($"{method.ReturnType.GetBridgeTypeFullName()}.__GetInstanceFrom({GetMethodCallText()});");
                                    else
                                        AppendLine($"{GetMethodCallText()};");
                                }
                                else
                                {
                                    AppendLine();
                                    StartBlock();

                                    foreach (IParameterSymbol parameterSymbol in method.Parameters.Where(static x => x.Type.IsNonPublicMember()))
                                    {
                                        string parameterTypeName = parameterSymbol.Type.GetFullTypeName();
                                        switch (parameterSymbol.RefKind)
                                        {
                                            case RefKind.Ref:
                                            case RefKind.In:
                                            case RefKind.RefReadOnlyParameter:
                                            {
                                                Append($"{parameterTypeName} __{parameterSymbol.Name} = ");
                                                AppendLine($"({parameterTypeName}){parameterSymbol.Name}.__instance;");
                                                break;
                                            }
                                            case RefKind.Out:
                                            {
                                                AppendLine($"{parameterTypeName} __{parameterSymbol.Name};");
                                                break;
                                            }
                                        }
                                    }

                                    if (!method.ReturnsVoid)
                                        Append($"{returnType} __result = ");
                                    AppendLine($"{GetMethodCallText()};");

                                    foreach (IParameterSymbol parameterSymbol in method.Parameters.Where(static x => x.Type.IsNonPublicMember()))
                                    {
                                        if (parameterSymbol.RefKind is RefKind.Ref or RefKind.Out or RefKind.In or RefKind.RefReadOnlyParameter)
                                            AppendLine($"{parameterSymbol.Name} = {parameterSymbol.Type.GetBridgeTypeFullName()}.__GetInstanceFrom(__{parameterSymbol.Name});");
                                    }

                                    if (!method.ReturnsVoid)
                                    {
                                        if (returnTypeIsNonPublic && method.ReturnType.TypeKind != TypeKind.Enum)
                                            AppendLine($"return {method.ReturnType.GetBridgeTypeFullName()}.__GetInstanceFrom(__result);");
                                        else
                                            AppendLine("return __result;");
                                    }

                                    EndBlock();
                                }
                                
                                break;

                                string GetMethodCallText()
                                {
                                    string result = string.Empty;
                                    if (!method.ReturnsVoid && returnTypeIsNonPublic && method.ReturnType.TypeKind == TypeKind.Enum)
                                        result += $"({returnType})(int)";
                                    
                                    return result + $"{instanceAccessPrefix}.{memberName}{method.GetTypeArgumentsText()}({callParameters})";
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}