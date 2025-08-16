using RuniOS.APIBridge;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;

[assembly: GenerateAPIBridgeForAssembly("UnityEditor.CoreModule")]

[assembly: GenerateAPIBridgeForType(typeof(AdvancedDropdown), includeMember = ["m_State", "SetFilter", "minimumSize", "maximumSize"])]

[assembly: GenerateAPIBridgeForType(typeof(AudioFilterGUI))]
[assembly: GenerateAPIBridgeForType(typeof(AudioUtil), forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(EditorGUI), includeMember = ["s_FoldoutHash", "HasKeyboardFocus", "MultiFieldPrefixLabel"])]
[assembly: GenerateAPIBridgeForType(typeof(EditorGUI.VUMeter), forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(EditorGUIUtility), includeMember = ["s_LastControlID", "s_LabelWidth", "s_FieldWidth", "contextWidth"], forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(EditorSettings))]
[assembly: GenerateAPIBridgeForType(typeof(GUIView))]
[assembly: GenerateAPIBridgeForType(typeof(HostView), includeMember = ["actualView"])]
[assembly: GenerateAPIBridgeForType(typeof(InspectorWindow), includeMember = ["RepaintAllInspectors", "GetInspectors", "GetAllInspectorWindows", "ShowWindow", "RemoveInspectorWindow", "GetInspectedObject", "GetInspectedObjects", "isLocked", "isVisible", "sharedTrackerInUse"])]
[assembly: GenerateAPIBridgeForType(typeof(PropertyEditor), includeMember = ["RebuildContentsContainers"])]
[assembly: GenerateAPIBridgeForType(typeof(PropertyHandler), excludeMember = ["s_DefaultObjectReferenceCache", "s_reorderableLists"])]
[assembly: GenerateAPIBridgeForType(typeof(PropertyHandlerCache), excludeMember = ["m_PropertyHandlers"])]
[assembly: GenerateAPIBridgeForType(typeof(ScriptAttributeUtility), excludeMember = ["k_DrawerTypeForType", "k_RenderPipelineTypeComparer", "s_FieldInfoFromPropertyPathCache", "BuildDrawerTypeForTypeDictionary"])]
[assembly: GenerateAPIBridgeForType(typeof(SerializedObject))]

[assembly: GenerateAPIBridgeForType(typeof(ReorderableListWrapper))]
