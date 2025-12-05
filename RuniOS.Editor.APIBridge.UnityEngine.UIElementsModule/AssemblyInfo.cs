using RuniOS.APIBridge;
using UnityEngine.UIElements;

[assembly: APIBridgeNamespace("RuniOS.Editor.APIBridge")]
[assembly: GenerateAPIBridgeForAssembly("UnityEngine.UIElementsModule")]
[assembly: GenerateAPIBridgeForType(typeof(VisualElement), excludeMember = ["m_RunningAnimations", "s_TypeData"], onlyByMyself = true)]
[assembly: GenerateAPIBridgeForType(typeof(PseudoStates))]
[assembly: GenerateAPIBridgeForType(typeof(BaseField<>), excludeMember = ["expressionEvaluated"], onlyByMyself = true)]
[assembly: GenerateAPIBridgeForType(typeof(IPrefixLabel))]
[assembly: GenerateAPIBridgeForType(typeof(TextInputBaseField<>))]
[assembly: GenerateAPIBridgeForType(typeof(Panel), skipConstructors = true, onlyByMyself = true)]