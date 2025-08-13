using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RuniOS.APIBridge
{
    public static class Utility
    {
        /// <summary>
        /// 주어진 심볼이 OriginalAttributesAttribute를 통해 원래 비공개였는지 확인합니다.
        /// </summary>
        /// <param name="symbol">확인할 심볼입니다.</param>
        /// <returns>원래 비공개였으면 <see langword="true"/>, 아니면 <see langword="false"/>를 반환합니다.</returns>
        public static bool IsNonPublicMember(this ISymbol symbol)
        {
            if (symbol.DeclaredAccessibility != Accessibility.Public)
                return true;
        
            var originalAttributesAttribute = symbol.GetAttributes()
                .FirstOrDefault(static ad => ad.AttributeClass?.ToDisplayString() == "BepInEx.AssemblyPublicizer.OriginalAttributesAttribute");

            if (originalAttributesAttribute != null && originalAttributesAttribute.ConstructorArguments.Length > 0)
            {
                var arg = originalAttributesAttribute.ConstructorArguments[0];
                if (arg.Kind == TypedConstantKind.Enum)
                {
                    if (arg.Type?.ToDisplayString() == "System.Reflection.MethodAttributes")
                    {
                        if (arg.Value is int enumValue)
                            return (enumValue & (int)MethodAttributes.MemberAccessMask) != (int)MethodAttributes.Public;
                    }
                    else if (arg.Type?.ToDisplayString() == "System.Reflection.FieldAttributes")
                    {
                        if (arg.Value is int enumValue)
                            return (enumValue & (int)FieldAttributes.FieldAccessMask) != (int)FieldAttributes.Public;
                    }
                    else if (arg.Type?.ToDisplayString() == "System.Reflection.TypeAttributes")
                    {
                        if (arg.Value is int enumValue)
                            return (enumValue & (int)TypeAttributes.VisibilityMask) != (int)TypeAttributes.Public;
                    }
                }
            }
            
            return false; // 어트리뷰트가 없거나 비공개를 나타내지 않음
        }
        
        /// <summary>
        /// 주어진 심볼이 MethodImpl(MethodImplOptions.InternalCall)인지 확인합니다.
        /// </summary>
        /// <param name="symbol">확인할 심볼입니다.</param>
        /// <returns>심볼이 InternalCall이면 <see langword="true"/>, 아니면 <see langword="false"/>를 반환합니다.</returns>
        public static bool IsInternalCall(this ISymbol symbol)
        {
            var originalAttributesAttribute = symbol.GetAttributes()
                .FirstOrDefault(static ad => ad.AttributeClass?.ToDisplayString() == "System.Runtime.CompilerServices.MethodImplAttribute");

            if (originalAttributesAttribute != null && originalAttributesAttribute.ConstructorArguments.Length > 0)
            {
                var arg = originalAttributesAttribute.ConstructorArguments[0];
                if (arg.Kind == TypedConstantKind.Enum && arg.Type?.ToDisplayString() == "System.Runtime.CompilerServices.MethodImplOptions")
                {
                    if (arg.Value is int enumValue)
                        return (enumValue & (int)MethodImplOptions.InternalCall) != 0;
                }
                else if (arg.Kind == TypedConstantKind.Primitive && arg.Type?.ToDisplayString() == "short")
                {
                    if (arg.Value is short enumValue)
                        return (enumValue & (int)MethodImplOptions.InternalCall) != 0;
                }
            }
            return false;
        }

        /// <summary>
        /// 주어진 심볼이 unsafe인지 확인합니다.
        /// </summary>
        /// <param name="symbol">확인할 심볼입니다.</param>
        /// <returns>심볼이 unsafe이면 <see langword="true"/>, 아니면 <see langword="false"/>를 반환합니다.</returns>
        public static bool IsUnsafe(this ISymbol symbol)
        {
            return false;
            /*return symbol switch
            {
                IPointerTypeSymbol => true,
                IArrayTypeSymbol arrayTypeSymbol => arrayTypeSymbol.ElementType.IsUnsafe(),
                INamedTypeSymbol namedTypeSymbol => namedTypeSymbol.GetMembers().Any(static x => x.IsUnsafe()),
                IFieldSymbol fieldSymbol => fieldSymbol.Type.IsUnsafe(),
                IPropertySymbol propertySymbol => propertySymbol.Type.IsUnsafe() || propertySymbol.Parameters.Any(static x => x.IsUnsafe()),
                IMethodSymbol methodSymbol => methodSymbol.ReturnType.IsUnsafe() || methodSymbol.Parameters.Any(static x => x.IsUnsafe()) || methodSymbol.TypeParameters.Any(static x => x.IsUnsafe()),
                IParameterSymbol parameterSymbol => parameterSymbol.Type.IsUnsafe(),
                IEventSymbol eventSymbol => eventSymbol.Type.IsUnsafe(),
                IAliasSymbol aliasSymbol => aliasSymbol.Target.IsUnsafe(),
                _ => false
            };*/
        }

        /// <summary>
        /// 지정된 멤버 심볼이 명시적 인터페이스 구현인지 확인합니다.
        /// </summary>
        /// <param name="symbol">확인할 멤버 심볼입니다.</param>
        /// <returns>
        /// 멤버가 명시적 인터페이스 구현이면 <see langword="true"/>를, 그렇지 않으면 <see langword="false"/>를 반환합니다.
        /// </returns>
        public static bool IsExplicitInterfaceImplementations(this ISymbol symbol)
        {
            return symbol switch
            {
                IMethodSymbol methodSymbol => !methodSymbol.ExplicitInterfaceImplementations.IsEmpty,
                IPropertySymbol propertySymbol => !propertySymbol.ExplicitInterfaceImplementations.IsEmpty,
                IEventSymbol eventSymbol => !eventSymbol.ExplicitInterfaceImplementations.IsEmpty,
                _ => false
            };
        }

        public static bool IsNullable(this ITypeSymbol symbol)
        {
            if (symbol.TypeKind == TypeKind.Struct)
                return symbol.SpecialType == SpecialType.System_Nullable_T;
            else if (symbol.IsReferenceType)
                return true;

            return false;
        }

        /// <summary>
        /// 대상 심볼의 네임스페이스를 기반으로 브릿지 네임스페이스를 생성합니다.
        /// </summary>
        /// <param name="symbol">대상 심볼입니다.</param>
        /// <returns>생성된 브릿지 네임스페이스 문자열입니다.</returns>
        public static string GetBridgeNamespace(this ITypeSymbol symbol)
        {
            if (symbol.ContainingNamespace == null || string.IsNullOrEmpty(symbol.ContainingNamespace.Name))
                return "RuniOS.APIBridge";
            else
                return $"RuniOS.APIBridge.{symbol.ContainingNamespace.Name}";
        }

        /// <summary>
        /// 대상 심볼의 이름을 기반으로 브릿지 타입 이름을 생성합니다.
        /// 중첩 타입의 경우 'OuterBridge.InnerBridge' 형태로 생성합니다.
        /// </summary>
        /// <param name="symbol">대상 심볼입니다.</param>
        /// <returns>생성된 브릿지 타입 이름 문자열입니다.</returns>
        public static string GetBridgeTypeNameIncludeContaining(this ITypeSymbol symbol)
        {
            string result = string.Empty;
            if (symbol.ContainingType != null)
                result += symbol.ContainingType.GetBridgeTypeNameIncludeContaining() + '.';

            return result + symbol.GetBridgeTypeName() + symbol.GetTypeArgumentsText();
        }

        public static string GetBridgeTypeFullName(this ITypeSymbol symbol) => $"global::{symbol.GetBridgeNamespace()}.{symbol.GetBridgeTypeNameIncludeContaining()}"; 
        
        public static string GetBridgeTypeName(this ITypeSymbol symbol) => symbol.Name + "Bridge";

        /// <summary>
        /// 주어진 타입 심볼에 대한 브릿지 타입의 완전한 이름을 반환합니다.<br/>
        /// 비공개 타입이 아닌경우, 주어진 타입의 전체 이름을 반환합니다.
        /// </summary>
        /// <param name="symbol">변환할 타입 심볼입니다.</param>
        /// <returns>브릿지 타입의 완전한 이름 또는 원본 타입의 이름입니다.</returns>
        public static string GetTypeNameOrBridgeName(this ITypeSymbol symbol)
        {
            if (symbol.IsNonPublicMember() && symbol is INamedTypeSymbol namedTypeSymbol and not { TypeKind: TypeKind.Delegate })
                return symbol.GetBridgeTypeFullName();

            return symbol switch
            {
                IArrayTypeSymbol arrayTypeSymbol => $"{arrayTypeSymbol.ElementType.GetTypeNameOrBridgeName()}[]", // 배열 타입 처리 (예: MyClass[] -> MyClassBridge[])
                IPointerTypeSymbol pointerTypeSymbol => $"{pointerTypeSymbol.PointedAtType.GetTypeNameOrBridgeName()}*", // 포인터 타입 처리 (예: MyClass* -> MyClassBridge*)
                _ => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) // 그 외 기본 타입이나 다른 어셈블리 타입은 그대로 사용
            };
        }

        public static ITypeSymbol GetElementType(this ITypeSymbol symbol)
        {
            while (symbol is IArrayTypeSymbol arrayTypeSymbol)
                symbol = arrayTypeSymbol.ElementType;
            
            return symbol;
        }
        
        public static ITypeSymbol GetPointedAtType(this ITypeSymbol symbol)
        {
            while (symbol is IPointerTypeSymbol pointerTypeSymbol)
                symbol = pointerTypeSymbol.PointedAtType;
            
            return symbol;
        }

        public static INamedTypeSymbol? GetNamedTypeSymbol(this ITypeSymbol symbol)
        {
            symbol = symbol.GetElementType();
            symbol = symbol.GetPointedAtType();
            
            if (symbol is INamedTypeSymbol namedTypeSymbol)
                return namedTypeSymbol;

            return null;
        }

        public static IEnumerable<INamedTypeSymbol> GetContainingTypes(this INamedTypeSymbol? symbol)
        {
            while (symbol != null)
            {
                yield return symbol;
                symbol = symbol.ContainingType;
            }
        }
        
        public static string GetFullTypeName(this ITypeSymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        public static string GetFullTypeNameIncludeNullable(this ITypeSymbol symbol)
        { 
            string result = symbol.GetFullTypeName();
            if (symbol.NullableAnnotation == NullableAnnotation.Annotated || symbol.SpecialType == SpecialType.System_Nullable_T)
                result += "?";
            
            return result;
        }
        
        /// <summary>
        /// 지정된 심볼이 Obsolete 특성을 가지고 있는지 확인하고, 메시지를 반환합니다.
        /// </summary>
        /// <param name="symbol">확인할 <see cref="ISymbol"/>입니다.</param>
        /// <param name="message">
        /// 심볼이 obsolete인 경우 메시지가 포함되고, 그렇지 않으면 <see langword="null"/>이 됩니다.
        /// </param>
        /// <returns>심볼이 obsolete이면 <see langword="true"/>를, 그렇지 않으면 <see langword="false"/>를 반환합니다.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="symbol"/>이 <see langword="null"/>인 경우 발생합니다.
        /// </exception>
        public static bool IsObsolete(this ISymbol symbol, out string message)
        {
            // 심볼에 적용된 모든 특성을 가져옵니다.
            foreach (AttributeData attributeData in symbol.GetAttributes().Where(static x => x.AttributeClass?.ToDisplayString() == "System.ObsoleteAttribute"))
            {
                // Obsolete 특성의 생성자 인수를 확인합니다.
                // 첫 번째 인수는 보통 경고 메시지입니다.
                if (attributeData.ConstructorArguments.Length > 0 && attributeData.ConstructorArguments[0].Value is string obsoleteMessage)
                {
                    message = obsoleteMessage;
                    return true;
                }
            }

            message = string.Empty;
            return false;
        }

        public static string GetTypeParametersText(this ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeParameters.Any())
                return "<" + string.Join(", ", namedTypeSymbol.TypeParameters.Select(static x => x.Name)) + ">";

            return string.Empty;
        }
        
        public static string GetTypeArgumentsText(this ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeArguments.Any())
                return "<" + string.Join(", ", namedTypeSymbol.TypeArguments.Select(static x => x.GetTypeNameOrBridgeName())) + ">";

            return string.Empty;
        }
        
        public static string GetTypeParametersText(this IMethodSymbol symbol)
        {
            if (symbol.TypeParameters.Any())
                return "<" + string.Join(", ", symbol.TypeParameters.Select(static x => x.Name)) + ">";
            else
                return string.Empty;
        }
        
        public static string GetTypeArgumentsText(this IMethodSymbol symbol)
        {
            if (symbol.TypeArguments.Any())
                return "<" + string.Join(", ", symbol.TypeArguments.Select(static x => x.GetTypeNameOrBridgeName())) + ">";
            else
                return string.Empty;
        }

        public static string GetTypeDeclarationKindName(this INamedTypeSymbol symbol) => symbol.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Interface => "interface",
            TypeKind.Delegate => "delegate",
            TypeKind.Enum => "enum",
            TypeKind.Struct => "struct",
            _ => string.Empty
        };

        public static string GetTypeDeclarationText(this INamedTypeSymbol symbol)
        {
            string result = "public ";
            if (symbol.TypeKind == TypeKind.Class)
            {
                if (symbol.IsStatic)
                    result += "static ";
                if (symbol.IsSealed)
                    result += "sealed ";
            }
            else if (symbol.TypeKind == TypeKind.Struct)
            {
                result += "readonly ";
                if (symbol.IsRefLikeType)
                    result += "ref ";
            }

            if (symbol.IsUnsafe())
                result += "unsafe ";
            
            if (symbol.TypeKind != TypeKind.Enum)
                result += "partial ";
            
            result += $"{symbol.GetTypeDeclarationKindName()} {symbol.GetBridgeTypeName()}{symbol.GetTypeParametersText()}";
            if (symbol.EnumUnderlyingType != null)
            {
                string enumUnderlyingTypeName = symbol.EnumUnderlyingType.GetFullTypeName();
                if (enumUnderlyingTypeName != "int")
                    result += $" : {symbol.EnumUnderlyingType.GetFullTypeName()}";
            }
            
            return $"{result} {symbol.GetConstraintsText()}";
        }

        /// <summary>
        /// 메소드의 반환 타입을 브릿지 타입으로 변환합니다.
        /// </summary>
        /// <param name="methodSymbol">변환할 메소드 심볼입니다.</param>
        /// <returns>브릿지 타입의 완전한 이름 또는 원본 타입의 이름입니다.</returns>
        public static string GetMethodReturnTypeName(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol.ReturnsVoid)
                return "void";
        
            return methodSymbol.ReturnType.GetTypeNameOrBridgeName();
        }

        public static INamedTypeSymbol? GetPublicBaseType(this INamedTypeSymbol? typeSymbol)
        {
            while (typeSymbol != null)
            {
                if (!typeSymbol.IsNonPublicMember())
                    return typeSymbol;
                
                typeSymbol = typeSymbol.BaseType;
            }
            
            return null;
        }

        public static string GetConstraintsText(this ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol namedTypeSymbol)
                return string.Join(", ", namedTypeSymbol.TypeParameters.Select(static x => x.GetConstraintsText()).Where(static x => !string.IsNullOrEmpty(x)));
            
            return string.Empty;
        }
        
        public static string GetConstraintsText(this IMethodSymbol symbol) => string.Join(", ", symbol.TypeParameters.Select(static x => x.GetConstraintsText()).Where(static x => !string.IsNullOrEmpty(x)));

        /// <summary>
        /// 지정된 제네릭 타입 매개변수의 모든 제약 조건을 문자열로 반환합니다.
        /// </summary>
        /// <param name="symbol">확인할 <see cref="ITypeParameterSymbol"/>입니다.</param>
        /// <returns>
        /// 모든 제약 조건을 포함하는 문자열을 반환합니다. 제약 조건이 없으면 빈 문자열을 반환합니다.
        /// </returns>
        public static string GetConstraintsText(this ITypeParameterSymbol symbol)
        {
            string result = string.Empty;
            bool any = false;
            if (symbol.HasReferenceTypeConstraint)
            {
                result += "class";
                if (any)
                    result += ", ";
                any = true;
            }
            else if (symbol.HasUnmanagedTypeConstraint)
            {
                if (any)
                    result += ", ";
                result += "unmanaged";
                any = true;
            }
            else if (symbol.HasValueTypeConstraint)
            {
                if (any)
                    result += ", ";
                result += "struct";
                any = true;
            }
            if (symbol.HasNotNullConstraint)
            {
                if (any)
                    result += ", ";
                result += "notnull";
                any = true;
            }
            if (symbol.HasConstructorConstraint)
            {
                if (any)
                    result += ", ";
                result += "new()";
                any = true;
            }
            if (symbol.ConstraintTypes.Any())
            {
                if (any)
                    result += ", ";
                result += string.Join(", ", symbol.ConstraintTypes.Select(static x => x.GetTypeNameOrBridgeName()));
                any = true;
            }

            if (!any)
                return string.Empty;

            return $"where {symbol.Name} : {result}";
        }
        
        public static string GetParameterText(this IEnumerable<IParameterSymbol> parameterSymbols) => string.Join(", ", parameterSymbols.Select(static x => x.GetParameterText()));

        /// <summary>
        /// 파라미터를 코드 텍스트로 변환합니다.
        /// </summary>
        /// <param name="parameterSymbol">변환할 파라미터 심볼입니다.</param>
        /// <returns>this, params, ref, out, in 키워드 및 선택형을 적절하게 붙여줍니다</returns>
        public static string GetParameterText(this IParameterSymbol parameterSymbol)
        {
            string keyword = string.Empty;
            if (parameterSymbol.IsOptional && !parameterSymbol.HasExplicitDefaultValue)
                keyword += "[global::System.Runtime.InteropServices.OptionalAttribute] ";
            if (parameterSymbol.IsThis)
                keyword += "this ";
            if (parameterSymbol.IsParams)
                keyword += "params ";
            switch (parameterSymbol.RefKind)
            {
                case RefKind.Ref:
                {
                    keyword += "ref ";
                    break;
                }
                case RefKind.Out:
                {
                    keyword += "out ";
                    break;
                }
                case RefKind.In:
                {
                    keyword += "in ";
                    break;
                }
                case RefKind.RefReadOnlyParameter:
                {
                    keyword += "ref readonly ";
                    break;
                }
            }

            keyword += $"{parameterSymbol.Type.GetTypeNameOrBridgeName()} {parameterSymbol.Name}";
            if (parameterSymbol.HasExplicitDefaultValue)
            {
                string value = parameterSymbol.ExplicitDefaultValue switch
                {
                    bool boolValue => boolValue ? "true" : "false",
                    uint uintValue => $"{uintValue}u",
                    long longValue => $"{longValue}L",
                    ulong ulongValue => $"{ulongValue}ul",
                    float floatValue => $"{floatValue}f",
                    double doubleValue => $"{doubleValue}d",
                    decimal decimalValue => $"{decimalValue}m",
                    char charValue => $"'{charValue}'",
                    string stringValue => $"\"{stringValue}\"",
                    _ => parameterSymbol.ExplicitDefaultValue?.ToString() ?? "null"
                };
                
                keyword += $" = {value}";
            }

            return keyword;
        }
        
        public static string GetCallParameterText(this IEnumerable<IParameterSymbol> parameterSymbols) => string.Join(", ", parameterSymbols.Select(static x => x.GetCallParameterText()));
        
        /// <summary>
        /// 콜 파라미터를 코드 텍스트로 변환합니다.
        /// </summary>
        /// <param name="parameterSymbol">변환할 파라미터 심볼입니다.</param>
        /// <returns>비공개 타입에 ref, out, in 키워드가 붙을 경우, 파라미터 이름은 __로 시작하고, 키워드가 없을 경우엔 .__instance가 접미사로 붙습니다.</returns>
        public static string GetCallParameterText(this IParameterSymbol parameterSymbol)
        {
            bool isNonPublic = parameterSymbol.Type.IsNonPublicMember();
            string result = string.Empty;
            
            switch (parameterSymbol.RefKind)
            {
                case RefKind.Ref:
                {
                    result += "ref ";
                    if (isNonPublic)
                        result += "__";
                    break;
                }
                case RefKind.Out:
                {
                    result += "out ";
                    if (isNonPublic)
                        result += "__";
                    break;
                }
                case RefKind.In:
                {
                    result += "in ";
                    if (isNonPublic)
                        result += "__";
                    break;
                }
                case RefKind.RefReadOnlyParameter:
                {
                    result += "ref readonly ";
                    if (isNonPublic)
                        result += "__";
                    break;
                }
                case RefKind.None:
                {
                    if (isNonPublic)
                    {
                        result += $"({parameterSymbol.Type.GetFullTypeName()})";
                        if (parameterSymbol.Type.TypeKind == TypeKind.Enum)
                            result += "(int)";
                    }
                    break;
                }
            }
            result += parameterSymbol.Name;
            
            if (isNonPublic && parameterSymbol.Type.TypeKind != TypeKind.Enum && parameterSymbol.RefKind == RefKind.None && parameterSymbol.Type.TypeKind != TypeKind.TypeParameter)
                result += ".__instance";
            
            return result;
        }
    }
}