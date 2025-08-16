using RuniOS.APIBridge;
using UnityEngine.UIElements;

[assembly: GenerateAPIBridgeForAssembly("UnityEngine.UIElementsModule")]
[assembly: GenerateAPIBridgeForType(typeof(VisualElement), includeMember = ["IncrementVersion"])]
[assembly: GenerateAPIBridgeForType(typeof(BaseField<>), includeMember = ["visualInput"])]
[assembly: GenerateAPIBridgeForType(typeof(IPrefixLabel))]
