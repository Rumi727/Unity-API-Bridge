using RuniOS.APIBridge;
using UnityEngine.UIElements;
using UnityEngine.UIElements.UIR;

[assembly: APIBridgeNamespace("RuniOS.Editor.APIBridge")]
[assembly: GenerateAPIBridgeForAssembly("UnityEngine.UIElementsModule")]
[assembly: GenerateAPIBridgeForType(typeof(DataBindingManager.ChangesFromUI))]
[assembly: GenerateAPIBridgeForType(typeof(VisualElement), excludeMember = ["styleAnimation", "renderData", "nestedRenderData", "m_LayoutNode", "layoutNode", "Measure", "inlineStyleAccess", "computedStyle", "m_Style", "SetComputedStyle", "GetStylePropertyAnimationSystem"])]
[assembly: GenerateAPIBridgeForType(typeof(TextureRegistry.TextureInfo))]
[assembly: GenerateAPIBridgeForType(typeof(BaseField<>), includeMember = ["visualInput"])]
[assembly: GenerateAPIBridgeForType(typeof(IPrefixLabel))]
[assembly: GenerateAPIBridgeForType(typeof(BaseVerticalCollectionView), excludeMember = ["CreateVirtualizationController"])]
[assembly: GenerateAPIBridgeForType(typeof(ReusableCollectionItem), excludeMember = ["onGeometryChanged"])]
[assembly: GenerateAPIBridgeForType(typeof(StyleComplexSelector.PseudoStateData))]
[assembly: GenerateAPIBridgeForType(typeof(BaseVisualElementPanel), excludeMember = ["dataBindingManager", "layoutConfig", "styleAnimationSystem"])]
[assembly: GenerateAPIBridgeForType(typeof(TextureEntry))]
[assembly: GenerateAPIBridgeForType(typeof(StyleProperty), excludeMember = ["SetSize"])]
[assembly: GenerateAPIBridgeForType(typeof(TextInputBaseField<>))]
[assembly: GenerateAPIBridgeForType(typeof(Panel), excludeConstructors = [1])]
//[assembly: GenerateAPIBridgeForType(typeof(FilterFunction), excludeMember = ["m_Parameters", "parameters"], excludeConstructors = [2, 3])]
[assembly: GenerateAPIBridgeForType(typeof(VisualTreeUpdater), excludeMember = ["SetUpdater"])]
[assembly: GenerateAPIBridgeForType(typeof(Utility.GPUBuffer<>), excludeMember = ["UpdateRanges"])]
[assembly: GenerateAPIBridgeForType(typeof(Page.DataSet<>), excludeMember = ["cpuData", "updateRanges"])]