using RuniOS.APIBridge;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;

[assembly: APIBridgeNamespace("RuniOS.Editor.APIBridge")]

[assembly: GenerateAPIBridgeForAssembly("UnityEditor.CoreModule")]

[assembly: GenerateAPIBridgeForType(typeof(AdvancedDropdown), includeMember = ["m_State", "SetFilter", "minimumSize", "maximumSize"])]

[assembly: GenerateAPIBridgeForType(typeof(AudioFilterGUI))]
[assembly: GenerateAPIBridgeForType(typeof(AudioUtil), forceStatic = true)]

[assembly: GenerateAPIBridgeForType(typeof(EditorGUI), excludeMember = ["s_PropertyStack", "AdvancedLazyPopup"], onlyByMyself = true, forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(EditorGUI.VUMeter), forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(EditorGUIUtility), onlyByMyself = true, forceStatic = true)]
[assembly: GenerateAPIBridgeForType(typeof(GUIView), onlyByMyself = true)]
[assembly: GenerateAPIBridgeForType(typeof(InspectorWindow), onlyByMyself = true, excludeMember = ["CreatePreviewEllipsisMenu"])]
[assembly: GenerateAPIBridgeForType(typeof(ScriptAttributeUtility), onlyByMyself = true, excludeMember = ["k_DrawerTypeForType", "k_RenderPipelineTypeComparer", "s_FieldInfoFromPropertyPathCache", "BuildDrawerTypeForTypeDictionary"])]
[assembly: GenerateAPIBridgeForType(typeof(SerializedObject))]
[assembly: GenerateAPIBridgeForType(typeof(EditorStyles))]
[assembly: GenerateAPIBridgeForType(typeof(EditorWindow), onlyByMyself = true)]
[assembly: GenerateAPIBridgeForType(typeof(PlayModeView), onlyByMyself = true)]
[assembly: GenerateAPIBridgeForType(typeof(GameView), onlyByMyself = true, excludeMember = ["m_DisplaySubsystems"])]

[assembly: GenerateAPIBridgeForType(typeof(ReorderableListWrapper))]