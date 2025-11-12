using RuniOS.APIBridge;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;

[assembly: APIBridgeNamespace("RuniOS.Editor.APIBridge")]

[assembly: GenerateAPIBridgeForAssembly("UnityEditor.CoreModule")]

[assembly: GenerateAPIBridgeForType(typeof(AdvancedDropdown), includeMember = ["m_State", "SetFilter", "minimumSize", "maximumSize"])]

[assembly: GenerateAPIBridgeForType(typeof(AudioFilterGUI))]
[assembly: GenerateAPIBridgeForType(typeof(AudioUtil), forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(EditorGUI), excludeMember = ["AdvancedLazyPopup"], forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(EditorGUI.VUMeter), forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(PropertyGUIData))]
[assembly: GenerateAPIBridgeForType(typeof(EditorGUIUtility), forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(EditorSettings))]
[assembly: GenerateAPIBridgeForType(typeof(GUIView))]
[assembly: GenerateAPIBridgeForType(typeof(HostView))]
[assembly: GenerateAPIBridgeForType(typeof(InspectorWindow))]
[assembly: GenerateAPIBridgeForType(typeof(PropertyEditor))]
[assembly: GenerateAPIBridgeForType(typeof(PropertyHandler), excludeMember = ["s_DefaultObjectReferenceCache", "s_reorderableLists"])]
[assembly: GenerateAPIBridgeForType(typeof(PropertyHandlerCache), excludeMember = ["m_PropertyHandlers"])]
[assembly: GenerateAPIBridgeForType(typeof(ScriptAttributeUtility), excludeMember = ["k_DrawerTypeForType", "k_RenderPipelineTypeComparer", "s_FieldInfoFromPropertyPathCache", "BuildDrawerTypeForTypeDictionary"])]
[assembly: GenerateAPIBridgeForType(typeof(SerializedObject))]
[assembly: GenerateAPIBridgeForType(typeof(ScalableGUIContent.TextureResource))]
[assembly: GenerateAPIBridgeForType(typeof(TypeSelection))]
[assembly: GenerateAPIBridgeForType(typeof(Delayer), excludeConstructors = [1])]

[assembly: GenerateAPIBridgeForType(typeof(ReorderableListWrapper))]